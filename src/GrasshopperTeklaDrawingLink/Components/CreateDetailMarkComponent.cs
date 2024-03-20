using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateDetailMarkComponent : CreateDatabaseObjectComponentBaseNew<CreateDetailMarkCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.DetailMark;

        public CreateDetailMarkComponent() : base(ComponentInfos.CreateDetailMarkComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (View view, Point3d centerPoint, double radius, Point3d labelPoint, string attributes, string name) = _command.GetInputValues();

            var boundaryPoint = centerPoint + new Point3d(radius, 0, 0);
            var detailAttributes = new DetailMark.DetailMarkAttributes(attributes);
            detailAttributes.MarkName = name;

            var mark = new DetailMark(view, centerPoint.ToTekla(), boundaryPoint.ToTekla(), labelPoint.ToTekla(), detailAttributes);
            mark.Insert();

            _command.SetOutputValues(DA, mark);

            DrawingInteractor.CommitChanges();
            return new List<DatabaseObject>() { mark };
        }
    }

    public class CreateDetailMarkCommand : CommandBase
    {
        private readonly InputParam<View> _inView = new InputParam<View>(ParamInfos.View);
        private readonly InputPoint _inCenterPoint = new InputPoint(ParamInfos.DetailCenterPoint);
        private readonly InputOptionalStructParam<double> _inRadius = new InputOptionalStructParam<double>(ParamInfos.DetailRadius, 500);
        private readonly InputPoint _inLabelPoint = new InputPoint(ParamInfos.DetailLabelPoint);
        private readonly InputOptionalParam<string> _inAttributes = new InputOptionalParam<string>(ParamInfos.DetailMarkAttributes, "standard");
        private readonly InputParam<string> _inName = new InputParam<string>(ParamInfos.Name);

        private readonly OutputParam<DatabaseObject> _outMark = new OutputParam<DatabaseObject>(ParamInfos.Mark);

        internal (View view, Point3d centerPoint, double radius, Point3d labelPoint, string attributes, string name) GetInputValues()
        {
            return (_inView.Value,
                    _inCenterPoint.Value,
                    _inRadius.Value,
                    _inLabelPoint.Value,
                    _inAttributes.Value,
                    _inName.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, DetailMark mark)
        {
            _outMark.Value = mark;

            return SetOutput(DA);
        }
    }
}
