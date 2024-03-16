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
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using Grasshopper.Kernel.Types;

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
            Menu_AppendItem(menu, ParamInfos.AllObjects.Name, AllObjectsMenuItem_Clicked, true, _mode == QueryMode.AllObjects).ToolTipText = ParamInfos.AllObjects.Description;
            Menu_AppendItem(menu, ParamInfos.OnlyVisibleObjects.Name, OnlyVisibleMenuItem_Clicked, true, _mode == QueryMode.OnlyVisibleObjects).ToolTipText = ParamInfos.OnlyVisibleObjects.Description;
            Menu_AppendSeparator(menu);
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
            Message = _mode switch
            {
                QueryMode.AllObjects => "All objects",
                QueryMode.OnlyVisibleObjects => "Only visible objects",
                _ => "",
            };
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
            AddTextParameter(pManager, ParamInfos.GroupingKeys, GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            ViewBase viewBase = null;
            var input = DA.GetGooValue<DatabaseObject>(ParamInfos.View);
            if (input is ViewBase)
                viewBase = input as ViewBase;
            else if (input is Drawing drawing)
                viewBase = drawing.GetSheet();

            if (viewBase == null)
                return;

            var childObjectsGroupedByType = GetChildObjectsGroupedByType(viewBase);

            var trees = GetOutputTrees(DA.Iteration, childObjectsGroupedByType);
            DA.SetDataTree(0, trees.Objects);
            DA.SetDataTree(1, trees.Keys);
        }

        private IEnumerable<IGrouping<string, DrawingObject>> GetChildObjectsGroupedByType(ViewBase viewBase)
        {
            var viewAabb = GetViewAabb(viewBase);

            var childObjects = new List<DrawingObject>();
            var doe = viewBase.GetObjects();
            while (doe.MoveNext())
            {
                if (_mode == QueryMode.OnlyVisibleObjects && viewAabb != null && doe.Current is ModelObject modelObject)
                {
                    var objectAabb = ModelInteractor.GetAabb(modelObject.ModelIdentifier);
                    if (objectAabb is null)
                        continue;

                    var transformedObjectAabb = objectAabb.Transform(ModelInteractor.TransformationMatrixToGlobal);
                    if (!viewAabb.Collide(transformedObjectAabb))
                        continue;
                }

                childObjects.Add(doe.Current);
            }

            return childObjects.GroupBy(o => o.GetType().ToShortString()).OrderBy(o => o.Key);
        }

        private AABB? GetViewAabb(ViewBase viewBase)
        {
            if (!(viewBase is TSD.View view))
                return null;

            view.Select();

            var matrix = MatrixFactory.FromCoordinateSystem(view.ViewCoordinateSystem);
            var pts = new TSG.Point[] { view.RestrictionBox.MinPoint, view.RestrictionBox.MaxPoint }
                .Select(p => matrix.Transform(p));
            return AABBFactory.FromPoints(pts);
        }

        private (IGH_Structure Objects, IGH_Structure Keys) GetOutputTrees(int iteration, IEnumerable<IGrouping<string, DrawingObject>> childObjectsGroupedByType)
        {
            var objects = new GH_Structure<TeklaDatabaseObjectGoo>();
            var keys = new GH_Structure<GH_String>();

            var index = 0;
            foreach (var currentObjects in childObjectsGroupedByType)
            {
                var path = new GH_Path(iteration, index);
                var indicies = currentObjects.Select(o => new TeklaDatabaseObjectGoo(o));

                objects.AppendRange(indicies, path);
                keys.Append(new GH_String(currentObjects.Key), path);

                index++;
            }

            return (objects, keys);
        }

        enum QueryMode
        {
            AllObjects,
            OnlyVisibleObjects
        }
    }
}
