using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreatePartViewComponent : CreateViewBaseComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.PartView;

        public CreatePartViewComponent() : base(ComponentInfos.CreatePartViewComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Drawing, GH_ParamAccess.item));
            AddTextParameter(pManager, ParamInfos.ViewType, GH_ParamAccess.list);
            pManager.AddPointParameter("Point", "P", "View insertion point", GH_ParamAccess.list);

            AddIntegerParameter(pManager, ParamInfos.Scale, GH_ParamAccess.list, true);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.list, true);
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.list));
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var drawing = DA.GetGooValue<DatabaseObject>(ParamInfos.Drawing) as Drawing;
            if (drawing == null)
                return null;

            var viewTypes = new List<string>();
            var parameterSet = DA.GetDataList(ParamInfos.ViewType.Name, viewTypes);
            if (!parameterSet)
                return null;

            var insertionPoints = new List<Rhino.Geometry.Point3d>();
            parameterSet = DA.GetDataList("Point", insertionPoints);
            if (!parameterSet)
                return null;

            var attributesFileNames = new List<string>();
            DA.GetDataList(ParamInfos.Attributes.Name, attributesFileNames);

            var scales = new List<int>();
            DA.GetDataList(ParamInfos.Scale.Name, scales);

            var viewNames = new List<string>();
            DA.GetDataList(ParamInfos.Name.Name, viewNames);

            var viewsNumber = new int[] { viewTypes.Count, insertionPoints.Count, attributesFileNames.Count, scales.Count }.Max();
            var createdViews = new View[viewsNumber];
            for (int i = 0; i < viewsNumber; i++)
            {
                var createdView = InsertView(
                    drawing,
                    viewTypes.ElementAtOrLast(i),
                    insertionPoints.ElementAtOrLast(i),
                    attributesFileNames.Count > 0 ? attributesFileNames.ElementAtOrLast(i) : null,
                    scales.Count > 0 ? scales.ElementAtOrLast(i) : new int?(),
                    viewNames.Count > 0 ? viewNames.ElementAtOrLast(i) : null);

                createdViews[i] = createdView;
            }

            DrawingInteractor.CommitChanges();
            DA.SetDataList(ParamInfos.View.Name, createdViews.Select(v => new TeklaDatabaseObjectGoo(v)));

            return createdViews;
        }

        private View InsertView(
            Drawing drawing,
            string viewType,
            Rhino.Geometry.Point3d insertionPoint,
            string attributesFileNames,
            int? scale,
            string viewName)
        {
            var attributes = new View.ViewAttributes();
            if (!string.IsNullOrEmpty(attributesFileNames))
                attributes.LoadAttributes(attributesFileNames);

            if (scale.HasValue)
                attributes.Scale = scale.Value;

            var teklaPoint = insertionPoint.ToTekla();

            View createdView = null;
            switch (viewType.ToUpper())
            {
                case "FRONT":
                    View.CreateFrontView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "TOP":
                    View.CreateTopView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "BOTTOM":
                    View.CreateBottomView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "BACK":
                    View.CreateBackView(drawing, teklaPoint, attributes, out createdView);
                    break;
                case "3D":
                    View.Create3dView(drawing, teklaPoint, attributes, out createdView);
                    break;
                default:
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "You can only use Front | Top | Bottom | Back or 3D as input view type");
                    break;
            }

            if (createdView == null)
                return null;

            LoadAttributesWithMacroIfNecessary(createdView, attributesFileNames);

            if (!string.IsNullOrEmpty(viewName))
            {
                createdView.Name = viewName;
                createdView.Modify();
            }

            return createdView;
        }
    }
}
