using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateDetailViewComponent : CreateViewBaseComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.DetailView;
        private int _defaultRadius = 500;

        public CreateDetailViewComponent()
            : base(ComponentInfos.CreateDetailViewComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
            pManager.AddPointParameter("Center point", "CP", "Center point of detail", GH_ParamAccess.list);
            pManager.AddPointParameter("Label point", "LP", "Label location", GH_ParamAccess.list);
            pManager.AddPointParameter("Insertion point", "IP", "Detail view insertion point", GH_ParamAccess.list);

            pManager.AddIntegerParameter("Radius", "R", "Detail range", GH_ParamAccess.list, _defaultRadius);
            pManager.AddTextParameter("View attributes", "VA", "View attributes file name", GH_ParamAccess.list, "standard");
            pManager.AddTextParameter("Mark attributes", "MA", "Detail mark attributes file name", GH_ParamAccess.list, "standard");
            AddIntegerParameter(pManager, ParamInfos.Scale, GH_ParamAccess.list, true);
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.list);
            AddGenericParameter(pManager, ParamInfos.Mark, GH_ParamAccess.list);
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as View;
            if (view == null)
                return null;

            var centerPoints = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("Center point", centerPoints))
                return null;

            var radiuses = new List<int>();
            if (!DA.GetDataList("Radius", radiuses) || radiuses.Any(r => r == 0))
                return null;

            var labelPoints = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("Label point", labelPoints))
                return null;

            var insertionPoints = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("Insertion point", insertionPoints))
                return null;

            var viewAttributesFileNames = new List<string>();
            DA.GetDataList("View attributes", viewAttributesFileNames);

            var scales = new List<int>();
            DA.GetDataList(ParamInfos.Scale.Name, scales);

            var markAttributesFileNames = new List<string>();
            DA.GetDataList("Mark attributes", markAttributesFileNames);

            var viewNames = new List<string>();
            DA.GetDataList(ParamInfos.Name.Name, viewNames);

            var viewsNumber = new int[]
            {
                centerPoints.Count,
                radiuses.Count,
                labelPoints.Count,
                insertionPoints.Count,
                viewAttributesFileNames.Count,
                scales.Count,
                markAttributesFileNames.Count,
                viewNames.Count
            }.Max();

            var createdViews = new (View view, DetailMark mark)[viewsNumber];
            for (int i = 0; i < viewsNumber; i++)
            {
                var viewWithMark = InsertView(
                    view,
                    centerPoints.ElementAtOrLast(i),
                    radiuses.ElementAtOrLast(i),
                    labelPoints.ElementAtOrLast(i),
                    insertionPoints.ElementAtOrLast(i),
                    viewAttributesFileNames.Count > 0 ? viewAttributesFileNames.ElementAtOrLast(i) : null,
                    scales.Count > 0 ? scales.ElementAtOrLast(i) : new int?(),
                    markAttributesFileNames.Count > 0 ? markAttributesFileNames.ElementAtOrLast(i) : null,
                    viewNames.Count > 0 ? viewNames.ElementAtOrLast(i) : null);

                createdViews[i] = viewWithMark;
            }

            var views = createdViews.Select(v => v.view);
            var marks = createdViews.Select(v => v.mark);
            DA.SetDataList(ParamInfos.View.Name, views.Select(v => new TeklaDatabaseObjectGoo(v)));
            DA.SetDataList(ParamInfos.Mark.Name, marks.Select(m => new TeklaDatabaseObjectGoo(m)));

            DrawingInteractor.CommitChanges();

            var viewsWithMarks = new List<DatabaseObject>();
            viewsWithMarks.AddRange(views);
            viewsWithMarks.AddRange(marks);
            return viewsWithMarks;
        }

        private (View view, DetailMark mark) InsertView(
            View view,
            Rhino.Geometry.Point3d centerPoint,
            int radius,
            Rhino.Geometry.Point3d labelPoint,
            Rhino.Geometry.Point3d insertionPoint,
            string viewAttributesFileName,
            int? scale,
            string markAttributesFileName,
            string viewName)
        {
            var markAttributes = new DetailMark.DetailMarkAttributes();
            if (!string.IsNullOrEmpty(markAttributesFileName))
                markAttributes.LoadAttributes(markAttributesFileName);

            if (!string.IsNullOrEmpty(viewName))
                markAttributes.MarkName = viewName;

            var viewAttributes = new View.ViewAttributes();
            if (!string.IsNullOrEmpty(viewAttributesFileName))
                viewAttributes.LoadAttributes(viewAttributesFileName);
            if (scale.HasValue)
                viewAttributes.Scale = scale.Value;

            var boundaryPoint = centerPoint + new Rhino.Geometry.Point3d(radius, 0, 0);
            View.CreateDetailView(
                view,
                centerPoint.ToTekla(),
                boundaryPoint.ToTekla(),
                labelPoint.ToTekla(),
                insertionPoint.ToTekla(),
                viewAttributes,
                markAttributes,
                out View createdView,
                out DetailMark createdMark);

            LoadAttributesWithMacroIfNecessary(createdView, viewAttributesFileName);

            return (createdView, createdMark);
        }
    }
}
