using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using System.Drawing;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class GetCustomPartPointsComponent : TeklaComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.GetCustomPartPoints;

        public GetCustomPartPointsComponent() : base(ComponentInfos.GetCustomPartPointsComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.ModelObject, GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddPointParameter(pManager, ParamInfos.StartPoint, GH_ParamAccess.item);
            AddPointParameter(pManager, ParamInfos.EndPoint, GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            dynamic inputObject = null;
            if (!DA.GetData(ParamInfos.ModelObject.Name, ref inputObject))
                return;

            CustomPart customPart = GetCustomPartFromInput(inputObject.Value);
            if (customPart == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Custom part not found");
                return;
            }

            customPart.Select();

            var startPoint = new Tekla.Structures.Geometry3d.Point();
            var endPoint = new Tekla.Structures.Geometry3d.Point();
            customPart.GetStartAndEndPositions(ref startPoint, ref endPoint);

            DA.SetData(
                ParamInfos.StartPoint.Name,
                new GH_Point(startPoint.ToRhino()));

            DA.SetData(
                ParamInfos.EndPoint.Name,
                new GH_Point(endPoint.ToRhino()));
        }

        private CustomPart GetCustomPartFromInput(ModelObject modelObject)
        {
            if (modelObject is CustomPart customPart)
            {
                return customPart;
            }

            Part part = null;
            if (modelObject is Part)
            {
                part = modelObject as Part;
            }
            else if (modelObject is Assembly)
            {
                part = (modelObject as Assembly).GetMainPart() as Part;
            }

            if (part == null)
                return null;

            return part.GetFatherComponent() as CustomPart;
        }
    }
}
