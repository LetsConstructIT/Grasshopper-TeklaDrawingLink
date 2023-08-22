using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class ModifyPartComponent : TeklaComponentBaseNew<ModifyPartCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary
        protected override Bitmap Icon => Properties.Resources.ModifyPart;
        public ModifyPartComponent() : base(ComponentInfos.ModifyPartComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (List<Part> parts, List<Part.PartAttributes> attributes) = _command.GetInputValues();

            for (int i = 0; i < parts.Count; i++)
            {
                ApplyAttributes(parts[i], attributes.ElementAtOrLast(i));
            }

            DrawingInteractor.CommitChanges();

            _command.SetOutputValues(DA, parts);
        }

        private void ApplyAttributes(Part part, Part.PartAttributes attributes)
        {
            part.Attributes = attributes;
            part.Modify();
        }
    }

    public class ModifyPartCommand : CommandBase
    {
        private readonly InputListParam<Part> _inParts = new InputListParam<Part>(ParamInfos.TeklaDrawingPart);
        private readonly InputListParam<Part.PartAttributes> _inAttributes = new InputListParam<Part.PartAttributes>(ParamInfos.PartAttributes);


        private readonly OutputListParam<Part> _outReinforcements = new OutputListParam<Part>(ParamInfos.TeklaDrawingPart);

        internal (List<Part> reinforcements, List<Part.PartAttributes> attributes) GetInputValues()
        {
            return (_inParts.Value, _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<Part> parts)
        {
            _outReinforcements.Value = parts;

            return SetOutput(DA);
        }
    }
}
