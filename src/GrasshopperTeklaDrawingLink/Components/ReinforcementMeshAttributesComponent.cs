using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System;
using static Tekla.Structures.Drawing.ReinforcementBase;

namespace GTDrawingLink.Components
{
    public class ReinforcementMeshAttributesComponent : TeklaComponentBaseNew<ReinforcementMeshAttributesCommand>
    {
        public ReinforcementMeshAttributesComponent() : base(ComponentInfos.ReinforcementMeshAttributesComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            (ReinforcementMeshAttributes meshAttributes, string fileName, ReinforcementVisibilityTypes? visibilityCross, ReinforcementVisibilityTypes? visibilityLongitudinal, int? symbolIndex, double? symbolSize) = _command.GetInputValues();

            if (!string.IsNullOrEmpty(fileName))
                meshAttributes.LoadAttributes(fileName);

            if (visibilityCross.HasValue)
                meshAttributes.MeshReinforcementVisibilityCrossing = visibilityCross.Value;

            if (visibilityLongitudinal.HasValue)
                meshAttributes.MeshReinforcementVisibilityLongitudinal = visibilityLongitudinal.Value;

            if (symbolIndex.HasValue)
                meshAttributes.MeshReinforcementSymbolIndex = symbolIndex.Value;

            if (symbolSize.HasValue)
                meshAttributes.MeshReinforcementSymbolSize = symbolSize.Value;

            _command.SetOutputValues(DA, meshAttributes);
        }
    }
    public class ReinforcementMeshAttributesCommand : CommandBase
    {
        private readonly InputOptionalParam<ReinforcementMeshAttributes> _inMeshAttributes = new InputOptionalParam<ReinforcementMeshAttributes>(ParamInfos.MeshAttributes);
        private readonly InputOptionalParam<string> _inAttributesFileName = new InputOptionalParam<string>(ParamInfos.Attributes);
        private readonly InputOptionalStructParam<ReinforcementVisibilityTypes> _inVisibilityCross = new InputOptionalStructParam<ReinforcementVisibilityTypes>(ParamInfos.MeshVisibilityCross);
        private readonly InputOptionalStructParam<ReinforcementVisibilityTypes> _inVisibilityLongitudinal = new InputOptionalStructParam<ReinforcementVisibilityTypes>(ParamInfos.MeshVisibilityLongitudinal);
        private readonly InputOptionalStructParam<int> _inSymbolIndex = new InputOptionalStructParam<int>(ParamInfos.MeshReinforcementSymbolIndex);
        private readonly InputOptionalStructParam<double> _inSymbolSize = new InputOptionalStructParam<double>(ParamInfos.MeshReinforcementSymbolSize);

        private readonly OutputParam<ReinforcementMeshAttributes> _outAttributes = new OutputParam<ReinforcementMeshAttributes>(ParamInfos.MeshAttributes);

        internal (ReinforcementMeshAttributes Attributes,
            string fileName,
            ReinforcementVisibilityTypes? visibilityCross,
            ReinforcementVisibilityTypes? visibilityLongitudinal,
            int? symbolIndex,
            double? symbolSize)
            GetInputValues()
        {
            return (
                _inMeshAttributes.Value ?? new ReinforcementMeshAttributes(),
                _inAttributesFileName.Value,
                _inVisibilityCross.ValueProvidedByUser ? _inVisibilityCross.Value : new ReinforcementVisibilityTypes?(),
                _inVisibilityLongitudinal.ValueProvidedByUser ? _inVisibilityLongitudinal.Value : new ReinforcementVisibilityTypes?(),
                _inSymbolIndex.ValueProvidedByUser ? _inSymbolIndex.Value : new int?(),
                _inSymbolSize.ValueProvidedByUser ? _inSymbolSize.Value : new double?()
                );
        }

        internal Result SetOutputValues(IGH_DataAccess DA, ReinforcementMeshAttributes attributes)
        {
            _outAttributes.Value = attributes;

            return SetOutput(DA);
        }
    }
}
