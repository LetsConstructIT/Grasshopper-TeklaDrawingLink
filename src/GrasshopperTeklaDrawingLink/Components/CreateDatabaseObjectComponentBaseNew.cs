using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public abstract class CreateDatabaseObjectComponentBaseNew<T> : TeklaComponentBaseNew<T> where T : CommandBase, new()
    {
        private readonly List<DatabaseObject> _insertedObjects = new List<DatabaseObject>();

        protected CreateDatabaseObjectComponentBaseNew(GH_InstanceDescription info)
            : base(info)
        {

        }

        protected abstract IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess dA);

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            RemoveInsertedObjects();

            var insertedObjects = InsertObjects(DA);

            if (insertedObjects != null)
                AddInsertedObjects(insertedObjects);
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.RegenerateObjects.Name, RegenerateObjectsMenuItem_Clicked).ToolTipText = ParamInfos.RegenerateObjects.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.BakeToTekla.Name, BakeMenuItem_Clicked).ToolTipText = ParamInfos.BakeToTekla.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.DeleteTeklaObjects.Name, DeleteMenuItem_Clicked).ToolTipText = ParamInfos.DeleteTeklaObjects.Description;
            GH_DocumentObject.Menu_AppendSeparator(menu);
        }

        private void RegenerateObjectsMenuItem_Clicked(object sender, EventArgs e)
        {
            RegenerateObjects();
        }

        private void BakeMenuItem_Clicked(object sender, EventArgs e)
        {
            BakeToTekla();
        }

        private void DeleteMenuItem_Clicked(object sender, EventArgs e)
        {
            DeleteObjects();
        }

        public void RegenerateObjects()
        {
            base.ExpireSolution(recompute: true);
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
                _insertedObjects.ForEach(o => o.Delete());
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
            foreach (var item in _insertedObjects)
            {
                if (item != null)
                    item.Delete();
            }
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
