using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components
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
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Drawing, GH_ParamAccess.item));
            pManager.AddParameter(new TeklaViewParam(ParamInfos.ModelView, GH_ParamAccess.list));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.list));
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<DatabaseObject>(ParamInfos.Drawing) as Drawing;
            if (drawing == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input drawing not specified");
                return null;
            }

            var modelViews = DA.GetGooListValue<TeklaView>(ParamInfos.ModelView);
            if (modelViews == null)
                return null;

            var viewsNumber = new int[] { modelViews.Count }.Max();
            var createdViews = new View[viewsNumber];
            for (int i = 0; i < viewsNumber; i++)
            {
                var createdView = InsertView(
                    drawing,
                    modelViews.ElementAtOrLast(i));

                createdViews[i] = createdView;
            }

            DrawingInteractor.CommitChanges();
            DA.SetDataList(ParamInfos.View.Name, createdViews.Select(v => new TeklaDatabaseObjectGoo(v)));

            return createdViews;
        }

        private View InsertView(Drawing drawing, TeklaView teklaView)
        {
            var aabb = new TSG.AABB(
                new TSG.Point(teklaView.RestrictionBox.X.Min, teklaView.RestrictionBox.Y.Min, teklaView.RestrictionBox.Z.Min),
                new TSG.Point(teklaView.RestrictionBox.X.Max, teklaView.RestrictionBox.Y.Max, teklaView.RestrictionBox.Z.Max));

            var view = new View(
                drawing.GetSheet(),
                teklaView.ViewCoordinateSystem.ToTekla(),
                teklaView.DisplayCoordinateSystem.ToTekla(),
                aabb)
            {
                Name = teklaView.Name
            };
            view.Insert();

            //LoadAttributesWithMacroIfNecessary(createdView, attributesFileNames);

            //if (!string.IsNullOrEmpty(viewName))
            //{
            //    createdView.Name = viewName;
            //    createdView.Modify();
            //}

            return view;
        }
    }
}
