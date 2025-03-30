using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components.Views
{
    public class CreateModelViewComponent : CreateViewBaseComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.ModelView;

        public CreateModelViewComponent() : base(ComponentInfos.CreateModelViewComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.item, optional: true);
            pManager.AddParameter(new TeklaViewParam(ParamInfos.ModelView, GH_ParamAccess.list));
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.list, true);
            AddIntegerParameter(pManager, ParamInfos.Scale, GH_ParamAccess.list, true);
            AddPointParameter(pManager, ParamInfos.ViewInsertionPoint, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.list);
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<DatabaseObject>(ParamInfos.Drawing) as Drawing;
            if (drawing == null)
            {
                HandleMissingInput();
                return null;
            }

            if (!DrawingInteractor.IsTheActiveDrawing(drawing))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The supplied drawing is not currently opened");
                return null;
            }

            var modelViews = DA.GetGooListValue<TeklaView>(ParamInfos.ModelView);
            if (modelViews == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Tekla model views not specified");
                return null;
            }

            var attributesFileNames = new List<string>();
            DA.GetDataList(ParamInfos.Attributes.Name, attributesFileNames);

            var scales = new List<int>();
            DA.GetDataList(ParamInfos.Scale.Name, scales);

            var insertionPoints = new List<Rhino.Geometry.Point3d>();
            DA.GetDataList(ParamInfos.ViewInsertionPoint.Name, insertionPoints);

            var viewsNumber = new int[] { modelViews.Count, attributesFileNames.Count, scales.Count, insertionPoints.Count }.Max();
            var createdViews = new View[viewsNumber];
            for (int i = 0; i < viewsNumber; i++)
            {
                var createdView = InsertView(
                    drawing,
                    modelViews.ElementAtOrLast(i),
                    attributesFileNames.Count > 0 ? attributesFileNames.ElementAtOrLast(i) : null,
                    scales.Count > 0 ? scales.ElementAtOrLast(i) : new int?(),
                    insertionPoints.Count > 0 ? insertionPoints.ElementAtOrLast(i) : new Rhino.Geometry.Point3d?());

                createdViews[i] = createdView;
            }

            DrawingInteractor.CommitChanges();
            DA.SetDataList(ParamInfos.View.Name, createdViews.Select(v => new TeklaDatabaseObjectGoo(v)));

            return createdViews;
        }

        private View InsertView(Drawing drawing, TeklaView teklaView, string attributesFileName, int? scale, Rhino.Geometry.Point3d? insertionPoint)
        {
            var aabb = new TSG.AABB(
                new TSG.Point(teklaView.RestrictionBox.X.Min.ToTekla(), teklaView.RestrictionBox.Y.Min.ToTekla(), teklaView.RestrictionBox.Z.Min.ToTekla()),
                new TSG.Point(teklaView.RestrictionBox.X.Max.ToTekla(), teklaView.RestrictionBox.Y.Max.ToTekla(), teklaView.RestrictionBox.Z.Max.ToTekla()));

            var attributesToUse = attributesFileName ?? "standard";
            var view = new View(
                drawing.GetSheet(),
                teklaView.ViewCoordinateSystem.ToTekla(),
                teklaView.DisplayCoordinateSystem.ToTekla(),
                aabb,
                attributesToUse)
            {
                Name = teklaView.Name
            };

            if (scale.HasValue)
                view.Attributes.Scale = scale.Value;

            view.Insert();

            LoadAttributesWithMacroIfNecessary(view, attributesFileName);

            if (!string.IsNullOrEmpty(teklaView.Name))
            {
                view.Name = teklaView.Name;
                view.Modify();
            }

            if (insertionPoint.HasValue)
            {
                var lowerLeft = view.Origin + view.FrameOrigin;
                var movementVector = insertionPoint.Value.ToTekla() - lowerLeft;
                view.Select();
                view.Origin += movementVector;
                view.Modify();
            }

            return view;
        }
    }
}
