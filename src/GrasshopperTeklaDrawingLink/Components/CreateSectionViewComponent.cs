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
    public class CreateSectionViewComponent : CreateViewBaseComponent
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.SectionView;

        private int _defaultDepth = 500;

        public CreateSectionViewComponent()
            : base(ComponentInfos.CreateSectionViewComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
            pManager.AddPointParameter("Start point", "P1", "Start point of section line", GH_ParamAccess.list);
            pManager.AddPointParameter("End point", "P2", "End point of section line", GH_ParamAccess.list);
            pManager.AddPointParameter("Insertion point", "IP", "Detail view insertion point", GH_ParamAccess.list);

            pManager.AddIntegerParameter("Depth up", "DU", "Section depth up", GH_ParamAccess.list, _defaultDepth);
            pManager.AddIntegerParameter("Depth down", "DD", "Sectino depth down", GH_ParamAccess.list, _defaultDepth);
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

            var startPoints = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("Start point", startPoints))
                return null;

            var endPoints = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("End point", endPoints))
                return null;

            var insertionPoint = new List<Rhino.Geometry.Point3d>();
            if (!DA.GetDataList("Insertion point", insertionPoint))
                return null;

            var depthUps = new List<int>();
            if (!DA.GetDataList("Depth up", depthUps) || depthUps.Count == 0)
                return null;

            var depthDowns = new List<int>();
            if (!DA.GetDataList("Depth down", depthDowns) || depthDowns.Count == 0)
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
                startPoints.Count,
                endPoints.Count,
                insertionPoint.Count,
                depthUps.Count,
                depthDowns.Count,
                viewAttributesFileNames.Count,
                scales.Count,
                markAttributesFileNames.Count,
                viewNames.Count
            }.Max();

            var createdViews = new (View view, SectionMark mark)[viewsNumber];
            for (int i = 0; i < viewsNumber; i++)
            {
                var viewWithMark = InsertView(
                    view,
                    startPoints.ElementAtOrLast(i),
                    endPoints.ElementAtOrLast(i),
                    insertionPoint.ElementAtOrLast(i),
                    depthUps.ElementAtOrLast(i),
                    depthDowns.ElementAtOrLast(i),
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

        private (View view, SectionMark mark) InsertView(
           View view,
           Rhino.Geometry.Point3d startPoint,
           Rhino.Geometry.Point3d endPoint,
           Rhino.Geometry.Point3d insertionPoint,
           int depthUp,
           int depthDown,
           string viewAttributesFileName,
           int? scale,
           string markAttributesFileName,
           string viewName)
        {
            var viewAttributes = new View.ViewAttributes();
            if (!string.IsNullOrEmpty(viewAttributesFileName))
                viewAttributes.LoadAttributes(viewAttributesFileName);

            if (scale.HasValue)
                viewAttributes.Scale = scale.Value;

            var markAttributes = new SectionMarkBase.SectionMarkAttributes();
            if (!string.IsNullOrEmpty(markAttributesFileName))
                markAttributes.LoadAttributes(markAttributesFileName);

            if (!string.IsNullOrEmpty(viewName))
                markAttributes.MarkName = viewName;

            View.CreateSectionView(
                view,
                startPoint.ToTekla(),
                endPoint.ToTekla(),
                insertionPoint.ToTekla(),
                depthUp,
                depthDown,
                viewAttributes,
                markAttributes,
                out View createdView,
                out SectionMark createdMark);

            LoadAttributesWithMacroIfNecessary(createdView, viewAttributesFileName);

            if (!string.IsNullOrEmpty(viewName))
            {
                createdMark.Attributes.MarkName = viewName;
                createdMark.Modify();
            }

            return (createdView, createdMark);
        }
    }
}
