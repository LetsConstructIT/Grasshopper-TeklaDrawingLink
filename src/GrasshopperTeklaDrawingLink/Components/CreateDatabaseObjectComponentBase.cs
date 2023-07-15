using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public abstract class CreateDatabaseObjectComponentBase : TeklaComponentBase
    {
        private List<DatabaseObject> _insertedObjects = new List<DatabaseObject>();

        protected CreateDatabaseObjectComponentBase(GH_InstanceDescription info)
            : base(info)
        {

        }

        public override void CreateAttributes()
        {
            m_attributes = new CreateDatabaseObjectComponentAttributes(this);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            RemoveInsertedObjects();

            var insertedObjects = InsertObjects(DA);

            if (insertedObjects != null)
                AddInsertedObjects(insertedObjects);
        }

        protected abstract IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess dA);

        protected void RemoveInsertedObjects()
        {
            foreach (var item in _insertedObjects)
            {
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
