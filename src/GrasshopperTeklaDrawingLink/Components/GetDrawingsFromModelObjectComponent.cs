using Grasshopper.Kernel;
using GTDrawingLink.Tools;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Model;
using TSMUI = Tekla.Structures.Model.UI;
using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class GetDrawingsFromModelObjectComponent : TeklaComponentBaseNew<GetDrawingsFromModelObjectCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;
        protected override Bitmap Icon => Properties.Resources.GetDrawingsFromModelObject;

        public GetDrawingsFromModelObjectComponent() : base(ComponentInfos.GetDrawingsFromModelObjectComponent) { }

        protected override void InvokeCommand(IGH_DataAccess DA)
        {
            var modelObject = _command.GetInputValues();

            var mos = new TSMUI.ModelObjectSelector();
            var preSelectedObjects = GetSelectedObjects(mos);

            SelectObject(mos, modelObject);

#if API2020
            var drawingIds = Tekla.Structures.DrawingInternal.Operation.GetDrawingsBySelectedParts();

#else
            var drawingIds = Tekla.Structures.DrawingInternal.Operation.GetDrawingsBySelectedParts(true, true);
#endif

            SelectObjects(mos, preSelectedObjects);

            var drawings = new List<TSD.Drawing>();
            foreach (var id in drawingIds)
            {
                var identifier = new Tekla.Structures.Identifier(id);
                var drawing = Tekla.Structures.DrawingInternal.Operation.GetDrawing(identifier);
                drawings.Add(drawing);
            }

            _command.SetOutputValues(DA, drawings);
        }

        private ArrayList GetSelectedObjects(TSMUI.ModelObjectSelector mos)
        {
            var preselected = new ArrayList();
            var moe = mos.GetSelectedObjects();
            while (moe.MoveNext())
            {
                preselected.Add(moe.Current);
            }

            return preselected;
        }

        private void SelectObjects(TSMUI.ModelObjectSelector mos, ArrayList arrayList)
        {
            mos.Select(arrayList);
        }

        private void SelectObject(TSMUI.ModelObjectSelector mos, ModelObject modelObject)
        {
            SelectObjects(mos, new ArrayList { modelObject });
        }
    }

    public class GetDrawingsFromModelObjectCommand : CommandBase
    {
        private readonly InputParam<ModelObject> _inModelObject = new InputParam<ModelObject>(ParamInfos.ModelObject);
        private readonly OutputListParam<TSD.Drawing> _outResult = new OutputListParam<TSD.Drawing>(ParamInfos.Drawing);

        internal ModelObject GetInputValues()
        {
            return _inModelObject.Value;
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<TSD.Drawing> drawings)
        {
            _outResult.Value = drawings;

            return SetOutput(DA);
        }
    }
}
