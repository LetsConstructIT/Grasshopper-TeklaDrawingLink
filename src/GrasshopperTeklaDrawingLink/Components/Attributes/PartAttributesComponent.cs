using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class PartAttributesComponent : TeklaComponentBaseNew<PartAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.PartAttributes;
        public PartAttributesComponent() : base(ComponentInfos.PartAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (partAttributes, fileName, visibileLines, hiddenLines, referenceLines, faceHatch, sectionFaceHatch, representation) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                partAttributes.LoadAttributes(fileName);

            if (visibileLines != null)
                partAttributes.VisibleLines = visibileLines;

            if (hiddenLines != null)
                partAttributes.HiddenLines = hiddenLines;

            if (referenceLines != null)
                partAttributes.ReferenceLine = referenceLines;

            if (faceHatch != null)
                partAttributes.FaceHatch = faceHatch;

            if (sectionFaceHatch != null)
                partAttributes.SectionFaceHatch = sectionFaceHatch;

            if (representation.HasValue)
                partAttributes.Representation = representation.Value;

            _command.SetOutputValues(DA, partAttributes);
        }
    }

    public class PartAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<Part.PartAttributes> _inPartAttributes = new InputOptionalParam<Part.PartAttributes>(ParamInfos.PartAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);

        private readonly InputOptionalParam<LineTypeAttributes> _inVisibleLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.VisibleLineTypeAttributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inHiddenLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.HiddenLineTypeAttributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inReferenceLines = new InputOptionalParam<LineTypeAttributes>(ParamInfos.ReferenceLineTypeAttributes);

        private readonly InputOptionalParam<ModelObjectHatchAttributes> _inFaceHatch = new InputOptionalParam<ModelObjectHatchAttributes>(ParamInfos.PartFacesHatchAttributes);
        private readonly InputOptionalParam<ModelObjectHatchAttributes> _inSectionFaceHatch = new InputOptionalParam<ModelObjectHatchAttributes>(ParamInfos.SectionHatchAttributes);

        private readonly InputOptionalStructParam<Part.Representation> _inRepresentation = new InputOptionalStructParam<Part.Representation>(ParamInfos.PartRepresentation);

        private readonly OutputParam<Part.PartAttributes> _outAttributes = new OutputParam<Part.PartAttributes>(ParamInfos.PartAttributes);

        internal (Part.PartAttributes partAttributes,
            string fileName,
            LineTypeAttributes? visibileLines,
            LineTypeAttributes? hiddenLines,
            LineTypeAttributes? referenceLines,
            ModelObjectHatchAttributes? faceHatch,
            ModelObjectHatchAttributes? sectionFaceHatch,
            Part.Representation? representation)
            GetInputValues()
        {
            return (
                _inPartAttributes.Value ?? new Part.PartAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inVisibleLines.GetValueFromUserOrNull(),
                _inHiddenLines.GetValueFromUserOrNull(),
                _inReferenceLines.GetValueFromUserOrNull(),
                _inFaceHatch.GetValueFromUserOrNull(),
                _inSectionFaceHatch.GetValueFromUserOrNull(),
                _inRepresentation.GetValueFromUserOrNull()
                );
        }

        internal Result SetOutputValues(IGH_DataAccess DA, Part.PartAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
