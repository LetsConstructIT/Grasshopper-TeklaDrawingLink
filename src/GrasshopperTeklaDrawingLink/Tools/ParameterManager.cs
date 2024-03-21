using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Tools
{
    public abstract class Param
    {
        public Type DataType { get; }
        public GH_InstanceDescription InstanceDescription { get; }
        public GH_ParamAccess ParamAccess { get; }

        public Param(Type type, GH_InstanceDescription instanceDescription, GH_ParamAccess paramAccess)
        {
            DataType = type;
            InstanceDescription = instanceDescription;
            ParamAccess = paramAccess;
        }
    }

    public abstract class InputParam : Param
    {
        protected bool _isOptional;
        protected InputParam(Type type, GH_InstanceDescription instanceDescription, GH_ParamAccess paramAccess)
            : base(type, instanceDescription, paramAccess)
        {
        }

        public bool IsOptional { get; protected set; }
        public abstract Result EvaluateInput(IGH_DataAccess DA);

        protected Result GetWrongInputMessage(string parameterName)
        {
            return Result.Fail($"Wrong input: {parameterName}");
        }
    }

    public abstract class OutputParam : Param
    {
        protected OutputParam(Type type, GH_InstanceDescription instanceDescription, GH_ParamAccess paramAccess)
            : base(type, instanceDescription, paramAccess)
        {
        }

        public abstract Result SetOutput(IGH_DataAccess DA);
    }

    public class OutputParam<T> : OutputParam
    {
        public T Value { get; set; }

        public OutputParam(GH_InstanceDescription instanceDescription)
            : base(typeof(T), instanceDescription, GH_ParamAccess.item)
        {
        }

        public override Result SetOutput(IGH_DataAccess DA)
        {
            if (DA.SetData(InstanceDescription.Name, Value))
                return Result.Ok();
            else
                return Result.Fail("SetData failed");
        }
    }

    public class OutputListParam<T> : OutputParam
    {
        public IEnumerable<T> Value { get; set; }

        public OutputListParam(GH_InstanceDescription instanceDescription)
            : base(typeof(T), instanceDescription, GH_ParamAccess.list)
        {
        }

        public override Result SetOutput(IGH_DataAccess DA)
        {
            if (DA.SetDataList(InstanceDescription.Name, Value))
                return Result.Ok();
            else
                return Result.Fail("SetDataList failed");
        }
    }

    public class InputPoint : InputParam
    {
        protected bool _properlySet;
        protected Point3d _value;
        public Point3d Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        public InputPoint(GH_InstanceDescription instanceDescription)
            : base(typeof(Point3d), instanceDescription, GH_ParamAccess.item)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            Point3d point = new Point3d();
            if (DA.GetData(InstanceDescription.Name, ref point))
            {
                _value = point;
                _properlySet = true;
                return Result.Ok();
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }
    }

    public class InputListPoint : InputParam
    {
        protected bool _properlySet;
        protected List<Point3d> _value;
        public List<Point3d> Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        public InputListPoint(GH_InstanceDescription instanceDescription)
            : base(typeof(Point3d), instanceDescription, GH_ParamAccess.list)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var points = new List<Point3d>();
            if (DA.GetDataList(InstanceDescription.Name, points))
            {
                _value = points;
                _properlySet = true;
                return Result.Ok();
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }
    }

    public class InputOptionalListPoint : InputListPoint
    {
        private List<Point3d> _defaultValue;

        public bool ValueProvidedByUser { get; private set; }

        public InputOptionalListPoint(GH_InstanceDescription instanceDescription, List<Point3d> defaultValue)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = defaultValue;
        }

        public InputOptionalListPoint(GH_InstanceDescription instanceDescription)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = default;
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var resultFromUserInput = base.EvaluateInput(DA);
            if (resultFromUserInput.Success)
            {
                ValueProvidedByUser = true;
            }
            if (resultFromUserInput.Failure)
            {
                _value = _defaultValue;
                _properlySet = true;
            }

            return Result.Ok();
        }

        public List<Point3d>? GetValueFromUserOrNull()
        {
            return ValueProvidedByUser ? Value : null;
        }
    }

    public class InputLine : InputParam
    {
        protected bool _properlySet;
        protected Rhino.Geometry.Line _value;
        public Rhino.Geometry.Line Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        public InputLine(GH_InstanceDescription instanceDescription)
            : base(typeof(Rhino.Geometry.Line), instanceDescription, GH_ParamAccess.item)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            Rhino.Geometry.Line line = new Rhino.Geometry.Line();
            if (DA.GetData(InstanceDescription.Name, ref line))
            {
                _value = line;
                _properlySet = true;
                return Result.Ok();
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }
    }

    public class InputOptionalLine : InputLine
    {
        private Rhino.Geometry.Line _defaultValue;

        public bool ValueProvidedByUser { get; private set; }

        public InputOptionalLine(GH_InstanceDescription instanceDescription, Rhino.Geometry.Line defaultValue)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = defaultValue;
        }

        public InputOptionalLine(GH_InstanceDescription instanceDescription)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = default;
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var resultFromUserInput = base.EvaluateInput(DA);
            if (resultFromUserInput.Success)
            {
                ValueProvidedByUser = true;
            }
            if (resultFromUserInput.Failure)
            {
                _value = _defaultValue;
                _properlySet = true;
            }

            return Result.Ok();
        }

        public Rhino.Geometry.Line? GetValueFromUserOrNull()
        {
            return ValueProvidedByUser ? Value : new Rhino.Geometry.Line?();
        }
    }

    public class InputParam<T> : InputParam where T : class
    {
        protected bool _properlySet;
        protected T _value;
        public T Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        public InputParam(GH_InstanceDescription instanceDescription)
            : base(typeof(T), instanceDescription, GH_ParamAccess.item)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var typeOfInput = typeof(T);
            if (typeOfInput.InheritsFrom(typeof(DatabaseObject)))
            {
                GH_Goo<DatabaseObject> objectGoo = null;
                if (DA.GetData(InstanceDescription.Name, ref objectGoo))
                {
                    if (typeOfInput == typeof(ViewBase) && objectGoo.Value.GetType().InheritsFrom(typeof(Drawing)))
                    {
                        _properlySet = true;
                        var sheet = (objectGoo.Value as Drawing).GetSheet();
                        _value = sheet as T;
                    }
                    else
                    {
                        _properlySet = true;
                        _value = objectGoo.Value as T;
                        if (_value == null)
                        {
                            return Result.Fail($"Provided input is not type of {typeOfInput.ToShortString()}");
                        }
                    }

                    _properlySet = true;
                    return Result.Ok();
                }
            }
            else
            {
                object input = null;
                if (DA.GetData(InstanceDescription.Name, ref input))
                {
                    if (input is GH_Goo<T> ghGoo)
                    {
                        _value = ghGoo.Value;
                        _properlySet = true;
                        return Result.Ok();
                    }
                    else if (input is T)
                    {
                        _value = input as T;
                        _properlySet = true;
                        return Result.Ok();
                    }
                }
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }
    }
    public class InputOptionalParam<T> : InputParam<T> where T : class
    {
        private T _defaultValue;

        public bool ValueProvidedByUser { get; private set; }

        public InputOptionalParam(GH_InstanceDescription instanceDescription, T defaultValue)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = defaultValue;
        }

        public InputOptionalParam(GH_InstanceDescription instanceDescription)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = default;
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var resultFromUserInput = base.EvaluateInput(DA);
            if (resultFromUserInput.Success)
            {
                ValueProvidedByUser = true;
            }
            if (resultFromUserInput.Failure)
            {
                _value = _defaultValue;
                _properlySet = true;
            }

            return Result.Ok();
        }

        public T? GetValueFromUserOrNull()
        {
            return ValueProvidedByUser ? Value : null;
        }
    }

    public class InputStructParam<T> : InputParam where T : struct
    {
        protected bool _properlySet;
        protected T _value;
        public T Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        public InputStructParam(GH_InstanceDescription instanceDescription)
            : base(typeof(T), instanceDescription, GH_ParamAccess.item)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var typeOfInput = typeof(T);
            if (typeOfInput.IsEnum)
            {
                object inputObject = null;
                DA.GetData(InstanceDescription.Name, ref inputObject);
                var inputCastedToEnum = EnumHelpers.ObjectToEnumValue<T>(inputObject);
                if (inputCastedToEnum.HasValue)
                {
                    _value = inputCastedToEnum.Value;
                    _properlySet = true;
                    return Result.Ok();
                }
            }
            else
            {
                GH_Goo<T> objectGoo = null;
                if (DA.GetData(InstanceDescription.Name, ref objectGoo))
                {
                    _value = objectGoo.Value;
                    _properlySet = true;
                    return Result.Ok();
                }
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }
    }
    public class InputOptionalStructParam<T> : InputStructParam<T> where T : struct
    {
        private T _defaultValue;

        public bool ValueProvidedByUser { get; private set; }

        public InputOptionalStructParam(GH_InstanceDescription instanceDescription, T defaultValue)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = defaultValue;
        }

        public InputOptionalStructParam(GH_InstanceDescription instanceDescription)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = default;
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            ValueProvidedByUser = false;
            _properlySet = false;

            var resultFromUserInput = base.EvaluateInput(DA);
            if (resultFromUserInput.Success)
            {
                ValueProvidedByUser = true;
            }
            if (resultFromUserInput.Failure)
            {
                _value = _defaultValue;
                _properlySet = true;
            }

            return Result.Ok();
        }

        public T? GetValueFromUserOrNull()
        {
            return ValueProvidedByUser ? Value : new T?();
        }
    }

    public class InputListParam<T> : InputParam where T : class
    {
        protected bool _properlySet;
        protected List<T> _value;
        public List<T> Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        public InputListParam(GH_InstanceDescription instanceDescription)
            : base(typeof(T), instanceDescription, GH_ParamAccess.list)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var typeOfInput = typeof(T);
            if (typeOfInput.InheritsFrom(typeof(DatabaseObject)))
            {
                var value = new List<GH_Goo<DatabaseObject>>();
                if (DA.GetDataList(InstanceDescription.Name, value))
                {
                    var castedToExpectedType = value.Select(v => v.Value as T);
                    if (castedToExpectedType.Any(o => o is null))
                    {
                        return Result.Fail($"One of the provided inputs is not type of {typeOfInput.ToShortString()}");
                    }

                    _value = castedToExpectedType.ToList();

                    _properlySet = true;
                    return Result.Ok();
                }
            }
            else if (typeOfInput == typeof(IGH_GeometricGoo))
            {
                var value = new List<IGH_GeometricGoo>();
                if (DA.GetDataList(InstanceDescription.Name, value))
                {
                    var castedToExpectedType = value.Select(v => v as T);
                    if (castedToExpectedType.Any(o => o is null))
                    {
                        return Result.Fail($"One of the provided inputs is not type of {typeOfInput.ToShortString()}");
                    }

                    _value = castedToExpectedType.ToList();

                    _properlySet = true;
                    return Result.Ok();
                }
            }
            else if (typeOfInput == typeof(IGH_Goo))
            {
                var value = new List<IGH_Goo>();
                if (DA.GetDataList(InstanceDescription.Name, value))
                {
                    var castedToExpectedType = value.Select(v => v as T);
                    if (castedToExpectedType.Any(o => o is null))
                    {
                        return Result.Fail($"One of the provided inputs is not type of {typeOfInput.ToShortString()}");
                    }

                    _value = castedToExpectedType.ToList();

                    _properlySet = true;
                    return Result.Ok();
                }
            }
            else
            {
                var objectsGoo = new List<GH_Goo<T>>();
                if (DA.GetDataList(InstanceDescription.Name, objectsGoo))
                {
                    _value = objectsGoo.Select(v => v?.Value).ToList();
                    _properlySet = true;
                    return Result.Ok();
                }
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }
    }
    public class InputOptionalListParam<T> : InputListParam<T> where T : class
    {
        private List<T> _defaultValue;

        public bool ValueProvidedByUser { get; private set; }

        public InputOptionalListParam(GH_InstanceDescription instanceDescription, List<T> defaultValue)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = defaultValue;
        }

        public InputOptionalListParam(GH_InstanceDescription instanceDescription)
            : base(instanceDescription)
        {
            IsOptional = true;
            _defaultValue = new List<T>();
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var resultFromUserInput = base.EvaluateInput(DA);
            if (resultFromUserInput.Success)
            {
                ValueProvidedByUser = true;
            }
            if (resultFromUserInput.Failure)
            {
                _value = _defaultValue;
                _properlySet = true;
            }

            return Result.Ok();
        }

        public List<T>? GetValueFromUserOrNull()
        {
            return ValueProvidedByUser ? Value : null;
        }
    }

    public class InputStructListParam<T> : InputParam where T : struct
    {
        protected bool _properlySet;
        private List<T> _value;
        public List<T> Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        public InputStructListParam(GH_InstanceDescription instanceDescription)
            : base(typeof(T), instanceDescription, GH_ParamAccess.list)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var typeOfInput = typeof(T);
            if (typeOfInput.IsEnum)
            {
                var inputObjects = new List<object>();
                if (DA.GetDataList(InstanceDescription.Name, inputObjects))
                {
                    var castedAsEnums = inputObjects.Select(o => EnumHelpers.ObjectToEnumValue<T>(o));
                    if (castedAsEnums.All(c => c.HasValue))
                    {
                        _value = castedAsEnums.Select(c => c.Value).ToList();
                        _properlySet = true;
                        return Result.Ok();
                    }
                }
            }
            else
            {
                var objectGoos = new List<GH_Goo<T>>();
                if (DA.GetDataList(InstanceDescription.Name, objectGoos))
                {
                    _value = objectGoos.Select(o => o.Value).ToList();
                    _properlySet = true;
                    return Result.Ok();
                }
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }
    }

    public class InputTreeParam<T> : InputParam where T : class
    {
        private bool _properlySet;
        private List<List<T>> _value;
        public List<List<T>> Value => _properlySet ? _value : throw new InvalidOperationException(InstanceDescription.Name);

        private IReadOnlyList<GH_Path> _paths;
        public IReadOnlyList<GH_Path> Paths => _properlySet ? _paths : throw new InvalidOperationException(InstanceDescription.Name);

        private IGH_Structure _tree;
        public IGH_Structure Tree => _properlySet ? _tree : throw new InvalidOperationException(InstanceDescription.Name);

        public InputTreeParam(GH_InstanceDescription instanceDescription)
            : base(typeof(T), instanceDescription, GH_ParamAccess.tree)
        {
        }

        public override Result EvaluateInput(IGH_DataAccess DA)
        {
            _properlySet = false;

            var typeOfInput = typeof(T);
            if (typeOfInput.InheritsFrom(typeof(DatabaseObject)))
            {
                if (DA.GetDataTree(InstanceDescription.Name, out GH_Structure<GH_Goo<DatabaseObject>> tree))
                {
                    _tree = tree;
                    var castedToExpectedType = tree.Branches.Select(b => b.Select(i => i.Value as T).ToList());
                    return ProcessResults(typeOfInput, tree, castedToExpectedType);
                }
            }
            else if (typeOfInput.InheritsFrom(typeof(TSM.ModelObject)))
            {
                if (DA.GetDataTree(InstanceDescription.Name, out GH_Structure<IGH_Goo> tree))
                {
                    _tree = tree;
                    var castedToExpectedType = tree.Branches.Select(b => b.Select(i => (i as GH_Goo<TSM.ModelObject>).Value as T).ToList());
                    return ProcessResults(typeOfInput, tree, castedToExpectedType);
                }
            }
            else if (typeOfInput == typeof(GH_Brep))
            {
                if (DA.GetDataTree(InstanceDescription.Name, out GH_Structure<GH_Brep> tree))
                {
                    _tree = tree;
                    var castedToExpectedType = tree.Branches.Select(b => b.Select(i => i as T).ToList());
                    return ProcessResults(typeOfInput, tree, castedToExpectedType);
                }
            }
            else if (typeOfInput == typeof(IGH_Goo))
            {
                if (DA.GetDataTree(InstanceDescription.Name, out GH_Structure<IGH_Goo> tree))
                {
                    _tree = tree;
                    var castedToExpectedType = tree.Branches.Select(b => b.Select(i => i as T).ToList());
                    return ProcessResults(typeOfInput, tree, castedToExpectedType);
                }
            }
            else if (typeOfInput == typeof(IGH_GeometricGoo))
            {
                if (DA.GetDataTree(InstanceDescription.Name, out GH_Structure<IGH_GeometricGoo> tree))
                {
                    _tree = tree;
                    var castedToExpectedType = tree.Branches.Select(b => b.Select(i => i as T).ToList());
                    return ProcessResults(typeOfInput, tree, castedToExpectedType);
                }
            }
            else if (typeOfInput == typeof(string))
            {
                if (DA.GetDataTree(InstanceDescription.Name, out GH_Structure<GH_String> tree))
                {
                    _tree = tree;
                    var castedToExpectedType = tree.Branches.Select(b => b.Select(i => i.Value as T).ToList());
                    return ProcessResults(typeOfInput, tree, castedToExpectedType);
                }
            }

            return GetWrongInputMessage(InstanceDescription.Name);
        }

        private Result ProcessResults(Type typeOfInput, IGH_Structure tree, IEnumerable<List<T>> castedToExpectedType)
        {
            if (castedToExpectedType.Any(o => o.Any(e => e is null)))
            {
                return Result.Fail($"One of the provided inputs is not type of {typeOfInput.ToShortString()}");
            }

            _value = castedToExpectedType.ToList();
            _paths = tree.Paths.ToList();

            _properlySet = true;
            return Result.Ok();
        }

        internal TreeData<T> AsTreeData()
        {
            return new TreeData<T>(Value, Paths);
        }
    }
    public class OutputTreeParam<T> : OutputParam
    {
        private readonly int _index;

        public IGH_Structure Value { get; set; }

        public OutputTreeParam(GH_InstanceDescription instanceDescription, int index)
            : base(typeof(T), instanceDescription, GH_ParamAccess.tree)
        {
            this._index = index;
        }

        public override Result SetOutput(IGH_DataAccess DA)
        {
            if (DA.SetDataTree(_index, Value))
                return Result.Ok();
            else
                return Result.Fail("SetDataList failed");
        }
    }

    public abstract class CommandBase
    {
        private IEnumerable<InputParam> _inputParameters;
        private IEnumerable<OutputParam> _outputParameters;

        public IEnumerable<InputParam> GetInputParameters()
        {
            if (_inputParameters == null)
                _inputParameters = GetPrivateFieldsWithType<InputParam>();

            return _inputParameters;
        }

        public IEnumerable<OutputParam> GetOutputParameters()
        {
            if (_outputParameters == null)
                _outputParameters = GetPrivateFieldsWithType<OutputParam>();

            return _outputParameters;
        }

        public Result EvaluateInput(IGH_DataAccess DA)
        {
            var results = _inputParameters.Select(p => p.EvaluateInput(DA));
            return Result.Combine(results.ToArray());
        }

        protected Result SetOutput(IGH_DataAccess DA)
        {
            var results = _outputParameters.Select(p => p.SetOutput(DA));
            return Result.Combine(results.ToArray());
        }

        private IEnumerable<T> GetPrivateFieldsWithType<T>() where T : class
        {
            var type = typeof(T);
            var parameters = new List<T>();
            foreach (var property in GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            {
                if (property.FieldType.BaseType.InheritsFrom(type))
                {
                    parameters.Add(property.GetValue(this) as T);
                }
            }

            return parameters;
        }
    }
    public class Result
    {
        public bool Success { get; private set; }
        public string Error { get; private set; }

        public bool Failure
        {
            get { return !Success; }
        }

        protected Result(bool success, string error)
        {
            Success = success;
            Error = error;
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default, false, message);
        }

        public static Result Ok()
        {
            return new Result(true, String.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, String.Empty);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.Failure)
                    return result;
            }

            return Ok();
        }
    }

    public class Result<T> : Result
    {
        private T _value;

        public T Value
        {
            get { return _value; }
            private set { _value = value; }
        }

        protected internal Result(T value, bool success, string error)
            : base(success, error)
        {
            Value = value;
        }
    }

    public class TreeData<T>
    {
        public List<List<T>> Objects { get; }
        public IReadOnlyList<GH_Path> Paths { get; }
        public int Count => Paths.Count;

        public TreeData(List<List<T>> objects, IReadOnlyList<GH_Path> paths)
        {
            Objects = objects ?? throw new ArgumentNullException(nameof(objects));
            Paths = paths ?? throw new ArgumentNullException(nameof(paths));
        }
    }
}
