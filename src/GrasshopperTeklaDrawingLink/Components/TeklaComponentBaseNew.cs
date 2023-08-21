﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public abstract class TeklaComponentBaseNew<T> : TeklaComponentBase where T : CommandBase, new()
    {
        private static Type _databaseObjectType = typeof(DatabaseObject);
        protected readonly T _command = new T();

        protected TeklaComponentBaseNew(GH_InstanceDescription info) : base(info) { }

        protected sealed override void RegisterInputParams(GH_InputParamManager pManager)
        {
            RegisterInputParameters(pManager, _command.GetInputParameters());
        }

        protected sealed override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            RegisterOutputParameters(pManager, _command.GetOutputParameters());
        }

        protected sealed override void SolveInstance(IGH_DataAccess DA)
        {
            var evaulationResult = _command.EvaluateInput(DA);
            if (evaulationResult.Failure)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, evaulationResult.Error);
                return;
            }

            InvokeCommand(DA);
        }

        protected void RegisterInputParameters(GH_InputParamManager pManager, IEnumerable<InputParam> parameters)
        {
            var @switch = new Dictionary<Type, Action<InputParam, GH_InputParamManager>> {
                { typeof(int), (param, manager) => AddIntegerParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(bool), (param, manager) => AddBooleanParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(double), (param, manager) => AddNumberParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(string), (param, manager) => AddTextParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Point3d), (param, manager) => AddPointParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(LineTypeAttributes), (param, manager) => pManager.AddParameter(new LineTypeAttributesParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional}) },
                { typeof(ArrowheadAttributes), (param, manager) => pManager.AddParameter(new ArrowAttributesParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional}) },
                { typeof(FontAttributes), (param, manager) => pManager.AddParameter(new FontAttributesParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional} )},
                { typeof(ReinforcementBase.ReinforcementMeshAttributes), (param, manager) => pManager.AddParameter(new ReinforcementMeshAttributesParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional} )},
                { typeof(ReinforcementBase.ReinforcementSingleAttributes), (param, manager) => pManager.AddParameter(new ReinforcementAttributesParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional} )},
                { typeof(Frame), (param, manager) => pManager.AddParameter(new FrameAttributesParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional} )},
                { typeof(SymbolAttributes), (param, manager) => pManager.AddParameter(new SymbolAttributesParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional} )},
                { typeof(SymbolInfo), (param, manager) => pManager.AddParameter(new SymbolInfoParam(param.InstanceDescription, param.ParamAccess) {Optional = param.IsOptional} )},
            };

            foreach (var parameter in parameters)
            {
                if (parameter.DataType.IsEnum)
                {
                    var genericType = typeof(EnumParam<>).MakeGenericType(parameter.DataType);
                    var enumParam = (Param_Integer)Activator.CreateInstance(genericType, parameter.InstanceDescription, parameter.ParamAccess);
                    enumParam.Optional = parameter.IsOptional;
                    pManager.AddParameter(enumParam);
                    continue;
                }
                else if (IsDatabaseObject(parameter.DataType))
                {
                    AddTeklaDbObjectParameter(pManager, parameter.InstanceDescription, parameter.ParamAccess, parameter.IsOptional);
                    continue;
                }

                @switch[parameter.DataType](parameter, pManager);
            }
        }

        protected void RegisterOutputParameters(GH_OutputParamManager pManager, IEnumerable<OutputParam> parameters)
        {
            var @switch = new Dictionary<Type, Action<OutputParam, GH_OutputParamManager>> {
                { typeof(bool), (param, manager) => AddBooleanParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(double), (param, manager) => AddNumberParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(Point3d), (param, manager) => AddPointParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(LineTypeAttributes), (param, manager) => pManager.AddParameter(new LineTypeAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(ArrowheadAttributes), (param, manager) => pManager.AddParameter(new ArrowAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(FontAttributes), (param, manager) => pManager.AddParameter(new FontAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(Tekla.Structures.Drawing.Text.TextAttributes), (param, manager) => pManager.AddParameter(new TextAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(ReinforcementBase.ReinforcementMeshAttributes), (param, manager) => pManager.AddParameter(new ReinforcementMeshAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(ReinforcementBase.ReinforcementSingleAttributes), (param, manager) => pManager.AddParameter(new ReinforcementAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(Frame), (param, manager) => pManager.AddParameter(new FrameAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(SymbolAttributes), (param, manager) => pManager.AddParameter(new SymbolAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(SymbolInfo), (param, manager) => pManager.AddParameter(new SymbolInfoParam(param.InstanceDescription, param.ParamAccess))},
            };

            var typeOfDatabaseObject = typeof(DatabaseObject);
            foreach (var parameter in parameters)
            {
                if (IsDatabaseObject(parameter.DataType))
                {
                    AddTeklaDbObjectParameter(pManager, parameter.InstanceDescription, parameter.ParamAccess);
                    continue;
                }

                @switch[parameter.DataType](parameter, pManager);
            }
        }

        protected abstract void InvokeCommand(IGH_DataAccess DA);

        private bool IsDatabaseObject(Type type)
        {
            return type.InheritsFrom(_databaseObjectType);
        }
    }
}