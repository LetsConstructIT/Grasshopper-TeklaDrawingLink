using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    [Obsolete]
    public class CreateEmbeddedObjectComponentOLD : CreateDatabaseObjectComponentBaseNew<CreateEmbeddedObjectCommandOLD>
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.Dwgdxf;

        public CreateEmbeddedObjectComponentOLD() : base(ComponentInfos.CreateEmbeddedObjectComponent) { }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            (ViewBase View, Point3d Point, string FileName, EmbeddedObjectAttributes Attributes) = _command.GetInputValues();

            var dwg = new DwgObject(View, Point.ToTekla(), FileName, Attributes);
            dwg.Insert();

            _command.SetOutputValues(DA, dwg);

            DrawingInteractor.CommitChanges();
            return new List<DatabaseObject>() { dwg };
        }
    }

    public class CreateEmbeddedObjectCommandOLD : CommandBase
    {
        private readonly InputParam<ViewBase> _inView = new InputParam<ViewBase>(ParamInfos.ViewBase);
        private readonly InputPoint _inPoint = new InputPoint(ParamInfos.InsertionPoint);
        private readonly InputParam<string> _inFileName = new InputParam<string>(ParamInfos.DwgFileName);
        private readonly InputParam<EmbeddedObjectAttributes> _inAttributes = new InputParam<EmbeddedObjectAttributes>(ParamInfos.DwgAttributes);

        private readonly OutputParam<DwgObject> _outDwgs = new OutputParam<DwgObject>(ParamInfos.Dwg);

        internal (ViewBase View, Point3d point, string fileName, EmbeddedObjectAttributes attributes) GetInputValues()
        {
            return (_inView.Value,
                    _inPoint.Value,
                    _inFileName.Value,
                    _inAttributes.Value);
        }

        internal Result SetOutputValues(IGH_DataAccess DA, DwgObject dwg)
        {
            _outDwgs.Value = dwg;

            return SetOutput(DA);
        }
    }
}
