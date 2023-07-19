using System;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;

namespace GTDrawingLink.Components
{
    public abstract class TeklaComponentBase : GH_Component
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        protected TeklaComponentBase(GH_InstanceDescription info)
            : base(info.Name, info.NickName, info.Description, info.Category, info.SubCategory)
        {

        }

        protected void AddOptionalParameter(GH_InputParamManager pManager, IGH_Param param)
        {
            pManager.AddParameter(param);
            SetLastParameterAsOptional(pManager, true);
        }

        protected void AddGenericParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            pManager.AddGenericParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected void AddBooleanParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            pManager.AddBooleanParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
        }

        protected void AddTextParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            pManager.AddTextParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
        }

        protected void AddIntegerParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            pManager.AddIntegerParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
        }

        protected void AddTextParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            pManager.AddTextParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected void AddGenericParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            pManager.AddGenericParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected void AddPlaneParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            pManager.AddPlaneParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected void AddBoxParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            pManager.AddBoxParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected void AddPointParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            pManager.AddPointParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected void AddCurveParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            pManager.AddCurveParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected void SetLastParameterAsOptional(GH_InputParamManager pManager, bool optional = true)
        {
            if (optional)
                pManager[pManager.ParamCount - 1].Optional = true;
        }
    }
}
