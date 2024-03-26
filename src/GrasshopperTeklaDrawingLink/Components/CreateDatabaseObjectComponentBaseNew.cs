using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public abstract class CreateDatabaseObjectComponentBaseNew<T> : TeklaComponentBaseNew<T>, IBakeable where T : CommandBase, new()
    {
        private readonly List<DatabaseObject> _insertedObjects = new List<DatabaseObject>();

        protected CreateDatabaseObjectComponentBaseNew(GH_InstanceDescription info)
            : base(info)
        {

        }

        protected abstract IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess dA);

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            if (DA.Iteration == 0)
                RemoveInsertedObjects();

            var insertedObjects = InsertObjects(DA);

            if (insertedObjects != null)
                AddInsertedObjects(insertedObjects);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.RecomputeObjects.Name, RecomputeObjectsMenuItem_Clicked).ToolTipText = ParamInfos.RecomputeObjects.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.BakeToTekla.Name, BakeMenuItem_Clicked).ToolTipText = ParamInfos.BakeToTekla.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.DeleteTeklaObjects.Name, DeleteMenuItem_Clicked).ToolTipText = ParamInfos.DeleteTeklaObjects.Description;
            GH_DocumentObject.Menu_AppendSeparator(menu);
        }

        private void RecomputeObjectsMenuItem_Clicked(object sender, EventArgs e)
        {
            base.ExpireSolution(recompute: true);
        }

        private void BakeMenuItem_Clicked(object sender, EventArgs e)
        {
            BakeToTekla();
        }

        private void DeleteMenuItem_Clicked(object sender, EventArgs e)
        {
            DeleteObjects();
        }

        public void BakeToTekla()
        {
            _insertedObjects.Clear();
        }

        public List<DatabaseObject> GetObjects() => _insertedObjects;

        public void DeleteObjects()
        {
            if (_insertedObjects.Any())
            {
                DrawingInteractor.DeleteObjects(_insertedObjects.OfType<DrawingObject>());
                _insertedObjects.Clear();
                DrawingInteractor.CommitChanges();
            }
            base.ExpireDownStreamObjects();
        }

        public override void CreateAttributes()
        {
            m_attributes = new CreateDatabaseObjectComponentAttributesNew<T>(this);
        }

        protected void RemoveInsertedObjects()
        {
            DrawingInteractor.DeleteObjects(_insertedObjects.OfType<DrawingObject>());
            _insertedObjects.Clear();
        }

        protected void AddInsertedObjects(IEnumerable<DatabaseObject> databaseObjects)
        {
            _insertedObjects.AddRange(databaseObjects);
        }

        private IEnumerable<DrawingObject> GetDrawingObjects()
        {
            foreach (var item in _insertedObjects)
            {
                if (item is DrawingObject drawingObject)
                {
                    yield return drawingObject;
                }
            }
        }
        public void HighlightObjects()
        {
            DrawingInteractor.Highlight(GetDrawingObjects());
        }

        public void UnHighlightObjects()
        {
            DrawingInteractor.UnHighlight();
        }

        protected SolverStrategy GetSolverStrategy(bool alwaysAsTree, params TreeData[] trees)
        {
            var pathCounts = trees.Select(t => t.PathCount).ToList();
            var mode = InputMode.TreeMode;
            if (!alwaysAsTree)
                mode = pathCounts.All(c => c == 1) ? InputMode.ListMode : InputMode.TreeMode;

            var highestCount = trees.Max(t => t.GetMaxCount(mode));
            var templateTree = trees.First(t => t.GetMaxCount(mode) == highestCount);

            return new SolverStrategy(mode, templateTree);
        }

        protected class SolverStrategy
        {
            public InputMode Mode { get; }
            public TreeData TemplateTree { get; }
            public int Iterations { get; }

            public SolverStrategy(InputMode mode, TreeData templateTree)
            {
                Mode = mode;
                TemplateTree = templateTree ?? throw new ArgumentNullException(nameof(templateTree));

                Iterations = templateTree.GetMaxCount(Mode);
            }

            public GH_Path GetPath(int iteration)
            {
                return Mode switch
                {
                    InputMode.ListMode => TemplateTree.Paths.First(),
                    InputMode.TreeMode => TemplateTree.Paths[iteration],
                    _ => throw new ArgumentOutOfRangeException(nameof(Mode)),
                };
            }
        }
    }

    public enum InputMode
    {
        ListMode,
        TreeMode
    }

    public class ViewCollection<T> where T : ViewBase
    {
        private readonly List<T> _views;

        public ViewCollection(List<T> views)
        {
            _views = views ?? throw new ArgumentNullException(nameof(views));
        }

        public T Get(GH_Path path)
        {
            var index = path.Indices.First();
            return Get(index);
        }

        private T Get(int index)
        {
            return _views.ElementAtOrLast(index);
        }
    }
}
