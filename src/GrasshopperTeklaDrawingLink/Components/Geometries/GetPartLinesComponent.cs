using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components.Geometries
{
    public class GetPartLinesComponent : TeklaComponentBase
    {
        private CutAndFittingMode _mode = CutAndFittingMode.Skip;
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetPartLines;

        public GetPartLinesComponent() : base(ComponentInfos.GetPartLinesComponent)
        {
            SetCustomMessage();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.ModelObject, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddCurveParameter(pManager, ParamInfos.PartReferenceLine, GH_ParamAccess.item);
            AddCurveParameter(pManager, ParamInfos.PartCenterLine, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dynamic inputObject = null;
            if (!DA.GetData(ParamInfos.ModelObject.Name, ref inputObject))
                return;

            Part part = GetPartFromInput(inputObject.Value);
            if (part == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Part not found");
                return;
            }

            part.Select();

            var includeCutsAndFittings = _mode == CutAndFittingMode.Include;
            var refPoints = part.GetReferenceLine(includeCutsAndFittings).ToTeklaPoints();
            var centerPoints = part.GetCenterLine(includeCutsAndFittings).ToTeklaPoints();

            DA.SetData(
                ParamInfos.PartReferenceLine.Name,
                new GH_Curve(new Rhino.Geometry.PolylineCurve(refPoints.Select(p => p.ToRhino()))));

            DA.SetData(
                ParamInfos.PartCenterLine.Name,
                new GH_Curve(new Rhino.Geometry.PolylineCurve(centerPoints.Select(p => p.ToRhino()))));
        }

        private Part GetPartFromInput(ModelObject modelObject)
        {
            switch (modelObject)
            {
                case Part inputPart:
                    return inputPart;
                case Assembly assembly:
                    return assembly.GetMainPart() as Part;
                default:
                    return null;
            }
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            Menu_AppendItem(menu, ParamInfos.CutAndFittingModeSkip.Name, SkipModeMenuItem_Clicked, true, _mode == CutAndFittingMode.Skip).ToolTipText = ParamInfos.CutAndFittingModeSkip.Description;
            Menu_AppendItem(menu, ParamInfos.CutAndFittingModeInclude.Name, IncludeModeMenuItem_Clicked, true, _mode == CutAndFittingMode.Include).ToolTipText = ParamInfos.CutAndFittingModeInclude.Description;
            Menu_AppendSeparator(menu);
        }

        private void SkipModeMenuItem_Clicked(object sender, System.EventArgs e)
        {
            _mode = CutAndFittingMode.Skip;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void IncludeModeMenuItem_Clicked(object sender, System.EventArgs e)
        {
            _mode = CutAndFittingMode.Include;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            Message = _mode switch
            {
                CutAndFittingMode.Skip => "Skip cuts and fittings",
                CutAndFittingMode.Include => "Include cuts and fittings",
                _ => "",
            };
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.CutAndFittingModeSkip.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.CutAndFittingModeSkip.Name, ref serializedInt);
            _mode = (CutAndFittingMode)serializedInt;
            SetCustomMessage();

            return base.Read(reader);
        }

        enum CutAndFittingMode
        {
            Skip,
            Include
        }
    }
}
