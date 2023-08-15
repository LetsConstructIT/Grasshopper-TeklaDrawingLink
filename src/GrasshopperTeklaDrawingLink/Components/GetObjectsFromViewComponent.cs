using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetObjectsFromViewComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetObjectsFromView;

        public GetObjectsFromViewComponent() : base(ComponentInfos.GetObjectsFromViewComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.TeklaDatabaseObject, GH_ParamAccess.tree);
            AddTextParameter(pManager, ParamInfos.GroupingKeys, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var view = DA.GetGooValue<DatabaseObject>(ParamInfos.View) as ViewBase;
            if (view == null)
                return;

            var childObjectsGroupedByType = GetChildObjectsGroupedByType(view);
            
            DA.SetDataTree(0, GetOutputTree(childObjectsGroupedByType));
            DA.SetDataList(ParamInfos.GroupingKeys.Name, childObjectsGroupedByType.Select(c => c.Key));
        }

        private IEnumerable<IGrouping<string, DrawingObject>> GetChildObjectsGroupedByType(ViewBase viewBase)
        {
            var childObjects = new List<DrawingObject>();
            var doe = viewBase.GetObjects();
            while (doe.MoveNext())
            {
                var drawingObject = doe.Current;

                childObjects.Add(drawingObject);
            }

            return childObjects.GroupBy(o => o.GetType().ToShortString()).OrderBy(o=>o.Key);
        }

        private IGH_Structure GetOutputTree(IEnumerable<IGrouping<string, DrawingObject>> childObjectsGroupedByType)
        {
            var output = new GH_Structure<TeklaDatabaseObjectGoo>();

            var index = 0;
            foreach (var currentObjects in childObjectsGroupedByType)
            {
                var indicies = currentObjects.Select(o => new TeklaDatabaseObjectGoo(o));
                output.AppendRange(indicies, new GH_Path(0, index));

                index++;
            }

            return output;
        }
    }
}
