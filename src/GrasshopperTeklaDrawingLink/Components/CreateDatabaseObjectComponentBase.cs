using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public abstract class CreateDatabaseObjectComponentBase : TeklaComponentBase, IBakeable
    {
        private List<DatabaseObject> _insertedObjects = new List<DatabaseObject>();

        protected CreateDatabaseObjectComponentBase(GH_InstanceDescription info)
            : base(info)
        {

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
            m_attributes = new CreateDatabaseObjectComponentAttributes(this);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (DA.Iteration == 0)
                RemoveInsertedObjects();

            var insertedObjects = InsertObjects(DA);

            if (insertedObjects != null)
                AddInsertedObjects(insertedObjects);
        }

        protected abstract IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess dA);

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
    }
}
