using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class BoltAttributesComponent : TeklaComponentBaseNew<BoltAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.BoltAttributes;
        public BoltAttributesComponent() : base(ComponentInfos.BoltAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (boltAttributes, fileName, representation, color, axisVisibility, holeVisibility) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                boltAttributes.LoadAttributes(fileName);

            if (representation.HasValue)
                boltAttributes.Representation = representation.Value;

            if (color.HasValue)
                boltAttributes.Color = color.Value;

            if (axisVisibility.HasValue)
                boltAttributes.SymbolContainsHole = axisVisibility.Value;

            if (holeVisibility.HasValue)
                boltAttributes.SymbolContainsAxis = holeVisibility.Value;

            _command.SetOutputValues(DA, boltAttributes);
        }
    }

    public class BoltAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Bolt.BoltAttributes> _inBoltAttributes = new InputOptionalParam<Bolt.BoltAttributes>(ParamInfos.BoltAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);

        private readonly InputOptionalStructParam<Bolt.Representation> _inRepresentation = new InputOptionalStructParam<Bolt.Representation>(ParamInfos.BoltRepresentation);
        private readonly InputOptionalStructParam<DrawingColors> _inColor = new InputOptionalStructParam<DrawingColors>(ParamInfos.DrawingColor);
        private readonly InputOptionalStructParam<bool> _inAxis = new InputOptionalStructParam<bool>(ParamInfos.SymbolContainsAxis);
        private readonly InputOptionalStructParam<bool> _inHole = new InputOptionalStructParam<bool>(ParamInfos.SymbolContainsHole);

        private readonly OutputParam<Bolt.BoltAttributes> _outAttributes = new OutputParam<Bolt.BoltAttributes>(ParamInfos.BoltAttributes);

        internal (Bolt.BoltAttributes boltAttributes,
            string fileName,
            Bolt.Representation? representation,
            DrawingColors? color,
            bool? axis,
            bool? hole)
            GetInputValues()
        {
            return (
                _inBoltAttributes.Value ?? new Bolt.BoltAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inRepresentation.GetValueFromUserOrNull(),
                _inColor.GetValueFromUserOrNull(),
                _inAxis.GetValueFromUserOrNull(),
                _inHole.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Bolt.BoltAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
