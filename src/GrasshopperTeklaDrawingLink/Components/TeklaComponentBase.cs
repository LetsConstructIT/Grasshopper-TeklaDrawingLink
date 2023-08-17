﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

namespace GTDrawingLink.Components
{
    public abstract class TeklaComponentBase : GH_Component
    {
        public override Guid ComponentGuid => VersionSpecificConstants.GetGuid(GetType());

        protected TeklaComponentBase(GH_InstanceDescription info)
            : base(info.Name, info.NickName, info.Description, info.Category, info.SubCategory)
        {

        }

        protected int AddOptionalParameter(GH_InputParamManager pManager, IGH_Param param)
        {
            var index = pManager.AddParameter(param);
            SetLastParameterAsOptional(pManager, true);
            return index;
        }

        protected int AddGenericParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddGenericParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddBooleanParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddBooleanParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddTextParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddTextParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddTeklaDbObjectParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddParameter(new TeklaDatabaseObjectParam(paramInfo, access));
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddIntegerParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddIntegerParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddNumberParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddNumberParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddPointParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddPointParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddPlaneParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddPlaneParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddBoxParameter(GH_InputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access, bool optional = false)
        {
            var index = pManager.AddBoxParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
            SetLastParameterAsOptional(pManager, optional);
            return index;
        }

        protected int AddTextParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddTextParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddGenericParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddGenericParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddPlaneParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddPlaneParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddBoxParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddBoxParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddPointParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddPointParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddCurveParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddCurveParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddBooleanParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddBooleanParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }
        protected int AddNumberParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddNumberParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddIntegerParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddIntegerParameter(paramInfo.Name, paramInfo.NickName, paramInfo.Description, access);
        }

        protected int AddTeklaDbObjectParameter(GH_OutputParamManager pManager, GH_InstanceDescription paramInfo, GH_ParamAccess access)
        {
            return pManager.AddParameter(new TeklaDatabaseObjectParam(paramInfo, access));
        }

        protected void SetLastParameterAsOptional(GH_InputParamManager pManager, bool optional = true)
        {
            if (optional)
                pManager[pManager.ParamCount - 1].Optional = true;
        }
        protected void SetParametersAsOptional(GH_InputParamManager pManager, IEnumerable<int> indices)
        {
            foreach (int index in indices)
                pManager[index].Optional = true;
        }
    }
}
