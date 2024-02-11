using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;
using TSD = Tekla.Structures.Drawing;
using TSG = Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Components
{
    public class CreateEmbeddedObjectComponent : CreateDatabaseObjectComponentBaseNew<CreateEmbeddedObjectCommand>
    {
        public override GH_Exposure Exposure => GH_Exposure.tertiary;
        protected override Bitmap Icon => Properties.Resources.Dwgdxf;

        public CreateEmbeddedObjectComponent() : base(ComponentInfos.CreateEmbeddedObjectComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (ViewBase View, List<Point3d> Points, string FileName, EmbeddedObjectAttributes Attributes) = _command.GetInputValues();

            var dwgs = new List<DwgObject>();
            if (Points.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The specified point group could not be parsed");
                return dwgs;
            }

            foreach (var point in Points)
            {
                var line = new DwgObject(View, point.ToTekla(), FileName, Attributes);
                line.Insert();

                dwgs.Add(line);
            }

            _command.SetOutputValues(DA, dwgs);

            DrawingInteractor.CommitChanges();
            return dwgs;
        }
    }

    public class CreateEmbeddedObjectCommand : CommandBase
    {
        private readonly InputParam<ViewBase> _inView = new InputParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputListPoint _inPoints = new InputListPoint(ParamInfos.Points);
        private readonly InputParam<string> _inFileName = new InputParam<string>(ParamInfos.DwgFileName);
        private readonly InputParam<EmbeddedObjectAttributes> _inAttributes = new InputParam<EmbeddedObjectAttributes>(ParamInfos.DwgAttributes);

        private readonly OutputListParam<DwgObject> _outDwgs = new OutputListParam<DwgObject>(ParamInfos.Dwg);

        internal (ViewBase View, List<Point3d> points, string fileName, EmbeddedObjectAttributes attributes) GetInputValues()
        {
            return (_inView.Value,
                    _inPoints.Value,
                    _inFileName.Value,
                    _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, List<DwgObject> dwgs)
        {
            _outDwgs.Value = dwgs;

            return SetOutput(DA);
        }
    }
}
