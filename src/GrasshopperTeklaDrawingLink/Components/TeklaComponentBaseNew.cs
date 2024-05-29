using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using Rhino.Geometry;
using Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;
using TSD = Tekla.Structures.Drawing;

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
                { typeof(GH_Number), (param, manager) => AddNumberParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(string), (param, manager) => AddTextParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(GH_String), (param, manager) => AddTextParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Point3d), (param, manager) => AddPointParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(GH_Point), (param, manager) => AddPointParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(GH_Brep), (param, manager) => AddBrepParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(GH_Circle), (param, manager) => AddCurveParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Plane), (param, manager) => AddPlaneParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(BoundingBox), (param, manager) => AddBoxParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Rhino.Geometry.Curve), (param, manager) => AddCurveParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Rhino.Geometry.Brep), (param, manager) => AddBrepParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Vector3d), (param, manager) => AddVectorParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Rectangle3d), (param, manager) => AddRectangleParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(IGH_GeometricGoo), (param, manager) => AddGeometryParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(IGH_Goo), (param, manager) => AddGenericParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(Rhino.Geometry.Line), (param, manager) => AddLineParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(TSM.ModelObject), (param, manager) => AddGenericParameter(manager, param.InstanceDescription, param.ParamAccess, param.IsOptional) },
                { typeof(StraightDimensionSet.StraightDimensionSetAttributes), (param, manager) => pManager.AddParameter(new StraightDimensionSetAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(LineTypeAttributes), (param, manager) => pManager.AddParameter(new LineTypeAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(ArrowheadAttributes), (param, manager) => pManager.AddParameter(new ArrowAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(TSD.Line.LineAttributes), (param, manager) => pManager.AddParameter(new LineAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(TSD.Circle.CircleAttributes), (param, manager) => pManager.AddParameter(new CircleAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(TSD.Arc.ArcAttributes), (param, manager) => pManager.AddParameter(new ArcAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(TSD.TextFile.TextFileAttributes), (param, manager) => pManager.AddParameter(new TextFileAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(TSD.Polyline.PolylineAttributes), (param, manager) => pManager.AddParameter(new PolylineAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(Polygon.PolygonAttributes), (param, manager) => pManager.AddParameter(new PolygonAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional}) },
                { typeof(FontAttributes), (param, manager) => pManager.AddParameter(new FontAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(ModelObjectHatchAttributes), (param, manager) => pManager.AddParameter(new ModelObjectHatchAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(Part.PartAttributes), (param, manager) => pManager.AddParameter(new PartAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(Text.TextAttributes), (param, manager) => pManager.AddParameter(new TextAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(Mark.MarkAttributes), (param, manager) => pManager.AddParameter(new MarkAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(Bolt.BoltAttributes), (param, manager) => pManager.AddParameter(new BoltAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(Weld.WeldAttributes), (param, manager) => pManager.AddParameter(new WeldAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(PluginPickerInput), (param, manager) => pManager.AddParameter(new PluginPickerInputParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},                
                { typeof(PlacingBase), (param, manager) => pManager.AddParameter(new PlacingBaseParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(EmbeddedObjectAttributes), (param, manager) => pManager.AddParameter(new EmbeddedObjectAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(ReinforcementBase.ReinforcementMeshAttributes), (param, manager) => pManager.AddParameter(new ReinforcementMeshAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(ReinforcementBase.ReinforcementSingleAttributes), (param, manager) => pManager.AddParameter(new ReinforcementAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(Frame), (param, manager) => pManager.AddParameter(new FrameAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(SymbolAttributes), (param, manager) => pManager.AddParameter(new SymbolAttributesParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
                { typeof(SymbolInfo), (param, manager) => pManager.AddParameter(new SymbolInfoParam(param.InstanceDescription, param.ParamAccess) { Optional = param.IsOptional })},
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
                { typeof(int), (param, manager) => AddIntegerParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(bool), (param, manager) => AddBooleanParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(string), (param, manager) => AddTextParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(GH_String), (param, manager) => AddTextParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(double), (param, manager) => AddNumberParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(Point3d), (param, manager) => AddPointParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(Vector3d), (param, manager) => AddVectorParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(Rectangle3d), (param, manager) => AddRectangleParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(BoundingBox), (param, manager) => AddBoxParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(Surface), (param, manager) => AddSurfaceParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(GH_Circle), (param, manager) => AddCurveParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(Rhino.Geometry.Line), (param, manager) => AddLineParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(Rhino.Geometry.Polyline), (param, manager) => AddCurveParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(IGH_GeometricGoo), (param, manager) => AddGeometryParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(IGH_Goo), (param, manager) => AddGenericParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(TSM.ModelObject), (param, manager) => AddGenericParameter(manager, param.InstanceDescription, param.ParamAccess) },
                { typeof(StraightDimensionSet.StraightDimensionSetAttributes), (param, manager) => pManager.AddParameter(new StraightDimensionSetAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(LineTypeAttributes), (param, manager) => pManager.AddParameter(new LineTypeAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(ArrowheadAttributes), (param, manager) => pManager.AddParameter(new ArrowAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(TSD.Line.LineAttributes), (param, manager) => pManager.AddParameter(new LineAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(TSD.Circle.CircleAttributes), (param, manager) => pManager.AddParameter(new CircleAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(TSD.TextFile.TextFileAttributes), (param, manager) => pManager.AddParameter(new TextFileAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(TSD.Arc.ArcAttributes), (param, manager) => pManager.AddParameter(new ArcAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(TSD.Polyline.PolylineAttributes), (param, manager) => pManager.AddParameter(new PolylineAttributesParam(param.InstanceDescription, param.ParamAccess)) },
                { typeof(Polygon.PolygonAttributes), (param, manager) => pManager.AddParameter(new PolygonAttributesParam(param.InstanceDescription, param.ParamAccess)) },
                { typeof(FontAttributes), (param, manager) => pManager.AddParameter(new FontAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(ModelObjectHatchAttributes), (param, manager) => pManager.AddParameter(new ModelObjectHatchAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(Part.PartAttributes), (param, manager) => pManager.AddParameter(new PartAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(Text.TextAttributes), (param, manager) => pManager.AddParameter(new TextAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(Mark.MarkAttributes), (param, manager) => pManager.AddParameter(new MarkAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(Bolt.BoltAttributes), (param, manager) => pManager.AddParameter(new BoltAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(Weld.WeldAttributes), (param, manager) => pManager.AddParameter(new WeldAttributesParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(PlacingBase), (param, manager) => pManager.AddParameter(new PlacingBaseParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(PluginPickerInput), (param, manager) => pManager.AddParameter(new PluginPickerInputParam(param.InstanceDescription, param.ParamAccess))},
                { typeof(EmbeddedObjectAttributes), (param, manager) => pManager.AddParameter(new EmbeddedObjectAttributesParam(param.InstanceDescription, param.ParamAccess))},
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
