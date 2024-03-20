using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components
{
    public class CreateSectionMarkComponent : CreateDatabaseObjectComponentBaseNew<CreateSectionMarkCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.SectionMark;

        public CreateSectionMarkComponent() : base(ComponentInfos.CreateSectionMarkComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (View view, Point3d startPoint, Point3d endPoint, string attributes, string name) = _command.GetInputValues();

            var sectionAttributes = new SectionMark.SectionMarkAttributes(attributes);
            sectionAttributes.MarkName = name;

            var mark = new SectionMark(view, startPoint.ToTekla(), endPoint.ToTekla(), sectionAttributes);
            mark.Insert();

            _command.SetOutputValues(DA, mark);

            DrawingInteractor.CommitChanges();
            return new List<DatabaseObject>() { mark };
        }
    }

    public class CreateSectionMarkCommand : CommandBase
    {
        private readonly InputParam<View> _inView = new InputParam<View>(ParamInfos.View);
        private readonly InputPoint _inStartPoint = new InputPoint(ParamInfos.SectionStartPoint);
        private readonly InputPoint _inEndPoint = new InputPoint(ParamInfos.SectionEndPoint);
        private readonly InputOptionalParam<string> _inAttributes = new InputOptionalParam<string>(ParamInfos.DetailMarkAttributes, "standard");
        private readonly InputParam<string> _inName = new InputParam<string>(ParamInfos.Name);

        private readonly OutputParam<DatabaseObject> _outMark = new OutputParam<DatabaseObject>(ParamInfos.Mark);

        internal (View view, Point3d startPoint, Point3d endPoint, string attributes, string name) GetInputValues()
        {
            return (_inView.Value,
                    _inStartPoint.Value,
                    _inEndPoint.Value,
                    _inAttributes.Value,
                    _inName.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, SectionMark mark)
        {
            _outMark.Value = mark;

            return SetOutput(DA);
        }
    }
}
