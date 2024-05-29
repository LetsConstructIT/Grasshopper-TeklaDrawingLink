using Grasshopper.Kernel;
using GTDrawingLink.Properties;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.AttributesComponents
{
    public class TextFileAttributesComponent : TeklaComponentBaseNew<TextFileAttributesCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.quarternary;
        protected override Bitmap Icon => Resources.TextFileAttributes;

        public TextFileAttributesComponent() : base(ComponentInfos.TextFileAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var (textAttributes, fileName, lineType, fontAttributes) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                textAttributes.LoadAttributes(fileName);

            if (lineType != null)
                textAttributes.Line = lineType;

            if (fontAttributes != null)
                textAttributes.Font = fontAttributes;

            _command.SetOutputValues(DA, textAttributes);
        }
    }

    public class TextFileAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<TextFile.TextFileAttributes> _inAttributes = new InputOptionalParam<TextFile.TextFileAttributes>(ParamInfos.TextFileAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalParam<LineTypeAttributes> _inLineType = new InputOptionalParam<LineTypeAttributes>(ParamInfos.LineTypeAttributes2);
        private readonly InputOptionalParam<FontAttributes> _inFontAttributes = new InputOptionalParam<FontAttributes>(ParamInfos.FontAttributes);

        private readonly OutputParam<TextFile.TextFileAttributes> _outAttributes = new OutputParam<TextFile.TextFileAttributes>(ParamInfos.TextFileAttributes);

        internal (TextFile.TextFileAttributes textAttributes, string fileName, LineTypeAttributes? lineTypeAttributes, FontAttributes? fontAttributes) GetInputValues()
        {
            return (
                _inAttributes.Value ?? new TextFile.TextFileAttributes(),
                _inAttributesFileName.GetValueFromUserOrNull(),
                _inLineType.GetValueFromUserOrNull(),
                _inFontAttributes.GetValueFromUserOrNull());
        }

        internal Result SetOutputValues(IGH_DataAccess DA, TextFile.TextFileAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
