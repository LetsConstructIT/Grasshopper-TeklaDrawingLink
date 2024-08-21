using Grasshopper.Kernel;
using GTDrawingLink.Types;

namespace GTDrawingLink.Components
{
    public abstract class DeconstructDatabaseObjectComponentBase : TeklaComponentBase
	{
		private TeklaDatabaseObjectParam _databaseObjectInputParam;

		public DeconstructDatabaseObjectComponentBase(GH_InstanceDescription info)
			: base(info)
		{
		}

		protected virtual void RegisterDatabaseObjectInputParam(GH_InputParamManager pManager, GH_InstanceDescription inputDescription)
		{
			_databaseObjectInputParam = new TeklaDatabaseObjectParam(inputDescription, GH_ParamAccess.item);
			pManager.AddParameter(_databaseObjectInputParam);
		}

		public override void CreateAttributes()
		{
			m_attributes = new DeconstructDatabaseObjectComponentAttributes(this);
		}

		public virtual void HighlightObjects()
		{
			_databaseObjectInputParam.HighlightObjects();
		}

		public virtual void UnHighlightObjects()
		{
			_databaseObjectInputParam.UnHighlightObjects();
		}
	}
}
