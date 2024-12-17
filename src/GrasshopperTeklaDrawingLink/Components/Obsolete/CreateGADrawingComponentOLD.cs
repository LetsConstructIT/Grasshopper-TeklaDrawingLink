using Grasshopper.Kernel;
using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Obsolete
{
    [Obsolete]
    public class CreateGADrawingComponentOLD : CreateDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.hidden;
        protected override Bitmap Icon => Properties.Resources.CreateGADrawing;

        public CreateGADrawingComponentOLD() : base(ComponentInfos.CreateGADrawingComponent)
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddTextParameter(pManager, ParamInfos.Name, GH_ParamAccess.list);
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddTeklaDbObjectParameter(pManager, ParamInfos.Drawing, GH_ParamAccess.list);
        }

        protected override IEnumerable<DatabaseObject> InsertObjects(IGH_DataAccess DA)
        {
            var viewNames = new List<string>();
            if (!DA.GetDataList(ParamInfos.Name.Name, viewNames))
                return null;

            var attributeFileNames = new List<string>();
            DA.GetDataList(ParamInfos.Attributes.Name, attributeFileNames);

            var drawingsNumber = new int[]
            {
                viewNames.Count,
                attributeFileNames.Count
            }.Max();

            var createdDrawings = new Drawing[drawingsNumber];
            for (int i = 0; i < drawingsNumber; i++)
            {
                var createdDrawing = CreateGADrawing(
                    viewNames.ElementAtOrLast(i),
                    attributeFileNames.Count > 0 ? attributeFileNames.ElementAtOrLast(i) : null);

                createdDrawings[i] = createdDrawing;
            }

            DA.SetDataList(ParamInfos.Drawing.Name, createdDrawings.Select(d => new TeklaDatabaseObjectGoo(d)));

            return createdDrawings;
        }

        private GADrawing CreateGADrawing(string viewName, string attributesFileName)
        {
            if (string.IsNullOrEmpty(attributesFileName))
                attributesFileName = "standard";

            var drawing = new GADrawing(viewName, attributesFileName);
            drawing.Insert();

            return drawing;
        }
    }
}
