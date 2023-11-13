using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components
{
    public class GetObjectsFromViewComponent : TeklaComponentBase
    {
        private QueryMode _mode = QueryMode.AllObjects;

        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetObjectsFromView;

        public GetObjectsFromViewComponent() : base(ComponentInfos.GetObjectsFromViewComponent)
        {
            SetCustomMessage();
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.AllObjects.Name, AllObjectsMenuItem_Clicked, true, _mode == QueryMode.AllObjects).ToolTipText = ParamInfos.AllObjects.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.OnlyVisibleObjects.Name, OnlyVisibleMenuItem_Clicked, true, _mode == QueryMode.OnlyVisibleObjects).ToolTipText = ParamInfos.OnlyVisibleObjects.Description;
            GH_DocumentObject.Menu_AppendSeparator(menu);
        }

        private void AllObjectsMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = QueryMode.AllObjects;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void OnlyVisibleMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = QueryMode.OnlyVisibleObjects;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            switch (_mode)
            {
                case QueryMode.AllObjects:
                    base.Message = "All objects";
                    break;
                case QueryMode.OnlyVisibleObjects:
                    base.Message = "Only visible objects";
                    break;
                default:
                    base.Message = "";
                    break;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.AllObjects.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.AllObjects.Name, ref serializedInt);
            _mode = (QueryMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
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
            var viewAabb = GetViewAabb(viewBase);

            var childObjects = new List<DrawingObject>();
            var doe = viewBase.GetObjects();
            while (doe.MoveNext())
            {
                var drawingObject = doe.Current;
                if (viewAabb != null &&
                    _mode == QueryMode.OnlyVisibleObjects &&
                    drawingObject is ModelObject modelObject)
                {
                    var objectAabb = ModelInteractor.GetAabb(modelObject.ModelIdentifier);
                    if (objectAabb is null ||
                        !viewAabb.Collide(objectAabb))
                        continue;
                }

                childObjects.Add(drawingObject);
            }

            return childObjects.GroupBy(o => o.GetType().ToShortString()).OrderBy(o => o.Key);
        }

        private AABB? GetViewAabb(ViewBase viewBase)
        {
            if (viewBase is Tekla.Structures.Drawing.View view)
            {
                view.Select();
                var matrix = MatrixFactory.FromCoordinateSystem(view.ViewCoordinateSystem);

                return new AABB(
                    new Tekla.Structures.Geometry3d.Point[]
                    {
                        matrix.Transform(view.RestrictionBox.MinPoint),
                        matrix.Transform(view.RestrictionBox.MaxPoint)
                    });
            }

            return null;
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

        enum QueryMode
        {
            AllObjects,
            OnlyVisibleObjects
        }
    }
}
