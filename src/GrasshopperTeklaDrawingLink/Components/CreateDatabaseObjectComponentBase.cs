using GH_IO.Serialization;
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
        private readonly List<DatabaseObject> _insertedObjects = new List<DatabaseObject>();
        private bool _deleteIfInputIsEmpty;

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
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.DeleteIfInputIsEmpty.Name, DeleteIfInputIsEmptyMenuItem_Clicked, true, _deleteIfInputIsEmpty).ToolTipText = ParamInfos.DeleteIfInputIsEmpty.Description;
            GH_DocumentObject.Menu_AppendSeparator(menu);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetBoolean(ParamInfos.DeleteIfInputIsEmpty.Name, _deleteIfInputIsEmpty);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            reader.TryGetBoolean(ParamInfos.DeleteIfInputIsEmpty.Name, ref _deleteIfInputIsEmpty);
            return base.Read(reader);
        }

        public bool DeleteIfInputIsEmpty
        {
            get { return _deleteIfInputIsEmpty; }
            protected set { _deleteIfInputIsEmpty = value; }
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

        private void DeleteIfInputIsEmptyMenuItem_Clicked(object sender, EventArgs e)
        {
            _deleteIfInputIsEmpty = !_deleteIfInputIsEmpty;
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
                if (DrawingInteractor.DeleteObjects(_insertedObjects.OfType<DrawingObject>()))
                    DrawingInteractor.CommitChanges();

                _insertedObjects.Clear();
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
