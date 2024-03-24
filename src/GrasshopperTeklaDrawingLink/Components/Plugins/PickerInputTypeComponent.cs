using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Plugins
{
    public class PickerInputTypeComponent : TeklaComponentBase
    {
        private InputTypeMode _type = InputTypeMode.Object;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.PickerInputType;

        public PickerInputTypeComponent() : base(ComponentInfos.PickerInputTypeComponent)
        {
            SetCustomMessage();
        }
        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, "Drawing object input", ObjectType_Clicked, true, _type == InputTypeMode.Object);
            Menu_AppendItem(menu, "Drawing object and point input", ObjectAndPointType_Clicked, true, _type == InputTypeMode.ObjectAndPoint);
            Menu_AppendItem(menu, "View and One point", OnePointType_Clicked, true, _type == InputTypeMode.OnePoint);
            Menu_AppendItem(menu, "View and Two points", TwoPointsType_Clicked, true, _type == InputTypeMode.TwoPoints);
            Menu_AppendItem(menu, "View and Three points", ThreePointsType_Clicked, true, _type == InputTypeMode.ThreePoints);
            Menu_AppendItem(menu, "View and N points", NPointsType_Clicked, true, _type == InputTypeMode.NPoints);
            Menu_AppendItem(menu, "Interrupt", InterruptType_Clicked, true, _type == InputTypeMode.Interrupt);
            Menu_AppendSeparator(menu);
        }

        private void ObjectType_Clicked(object sender, EventArgs e)
        {
            _type = InputTypeMode.Object;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ObjectAndPointType_Clicked(object sender, EventArgs e)
        {
            _type = InputTypeMode.ObjectAndPoint;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void OnePointType_Clicked(object sender, EventArgs e)
        {
            _type = InputTypeMode.OnePoint;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void TwoPointsType_Clicked(object sender, EventArgs e)
        {
            _type = InputTypeMode.TwoPoints;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ThreePointsType_Clicked(object sender, EventArgs e)
        {
            _type = InputTypeMode.ThreePoints;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void NPointsType_Clicked(object sender, EventArgs e)
        {
            _type = InputTypeMode.NPoints;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void InterruptType_Clicked(object sender, EventArgs e)
        {
            _type = InputTypeMode.Interrupt;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.ModelMacro.Name, (int)_type);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.ModelMacro.Name, ref serializedInt);
            _type = (InputTypeMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.View, GH_ParamAccess.item);
            AddGenericParameter(pManager, ParamInfos.PickerInputValue, GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.PickerInputInput, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var viewInput = DA.GetGooValue<DatabaseObject>(ParamInfos.View);
            if (viewInput == null)
                return;

            ViewBase viewBase = null;
            if (viewInput is ViewBase)
                viewBase = viewInput as ViewBase;
            else if (viewInput is Drawing drawing)
                viewBase = drawing.GetSheet();
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unrecognized View input");
                return;
            }

            List<object> inputs = new List<object>();
            DA.GetDataList(ParamInfos.PickerInputValue.Name, inputs);

            PickerInput pickerInput = null;
            switch (_type)
            {
                case InputTypeMode.Object:
                    pickerInput = new PickerInputObject(ParseAsObject(inputs[0]));
                    break;
                case InputTypeMode.ObjectAndPoint:
                    pickerInput = new PickerInputObjectAndAPoint(ParseAsObject(inputs[0]), ParseAsPoint(inputs[1]));
                    break;
                case InputTypeMode.OnePoint:
                    pickerInput = new PickerInputPoint(viewBase, ParseAsPoint(inputs[0]));
                    break;
                case InputTypeMode.TwoPoints:
                    pickerInput = new PickerInputTwoPoints(viewBase, ParseAsPoint(inputs[0]), ParseAsPoint(inputs[1]));
                    break;
                case InputTypeMode.ThreePoints:
                    pickerInput = new PickerInputThreePoints(viewBase, ParseAsPoint(inputs[0]), ParseAsPoint(inputs[1]), ParseAsPoint(inputs[2]));
                    break;
                case InputTypeMode.NPoints:
                    var pointList = new PointList();
                    foreach (var @object in inputs)
                        pointList.Add(ParseAsPoint(@object));

                    pickerInput = new PickerInputNPoints(viewBase, pointList);
                    break;
                case InputTypeMode.Interrupt:
                    pickerInput = new PickerInputInterrupt();
                    break;
                default:
                    break;
            }

            if (pickerInput == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Picker input not resolved");
                return;
            }

            DA.SetData(ParamInfos.PickerInputInput.Name, new PickerInputGoo(pickerInput));
        }

        private DrawingObject ParseAsObject(object @object)
        {
            if (@object is TeklaDatabaseObjectGoo)
                return (@object as TeklaDatabaseObjectGoo).Value as DrawingObject;
            else if (@object is DrawingObject)
                return @object as DrawingObject;

            return null;
        }

        private Tekla.Structures.Geometry3d.Point ParseAsPoint(object @object)
        {
            if (@object is TeklaPointGoo)
                return (@object as TeklaPointGoo).Value;
            else if (@object is Tekla.Structures.Geometry3d.Point)
                return @object as Tekla.Structures.Geometry3d.Point;
            else if (@object is GH_Point ghPoint)
                return ghPoint.Value.ToTekla();

            return null;
        }

        private void SetCustomMessage()
        {
            Message = _type.ToString();
        }

        enum InputTypeMode
        {
            Object,
            ObjectAndPoint,
            OnePoint,
            TwoPoints,
            ThreePoints,
            NPoints,
            Interrupt
        }
    }
}
