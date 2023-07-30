using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry;
using Grasshopper.Kernel.Types;

using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using Rhino.Geometry;

using Tekla.Structures.Drawing;

using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text {
    public class CreateTextComponent : CreateDatabaseObjectComponentBase {
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.Text;

        public CreateTextComponent() : base(ComponentInfos.CreateTextComponent) {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            AddPointParameter(pManager, ParamInfos.MarkInsertionPoint, GH_ParamAccess.list);
            AddPointParameter(pManager, ParamInfos.MarkLeaderLineEndPoint, GH_ParamAccess.list);
            AddTextParameter(pManager, ParamInfos.Text, GH_ParamAccess.list);
            pManager.AddParameter(new TextAttributesParam(ParamInfos.TextAttributes, GH_ParamAccess.list));
            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Text, GH_ParamAccess.list));
        }




        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess DA) {
            var view = DA.GetGooValue<TSD.DatabaseObject>(ParamInfos.View) as TSD.View;
            if(view==null)
                return null;

            var insertionPoints = new List<Point3d>();
            if(!DA.GetDataList(ParamInfos.MarkInsertionPoint.Name, insertionPoints))
                return null;
            var leaderLineEndPoints
                = new List<Point3d>();
            if(!DA.GetDataList(ParamInfos.MarkLeaderLineEndPoint.Name, leaderLineEndPoints))
                return null;

            var textElements = new List<string>();
            if(!DA.GetDataList(ParamInfos.Text.Name, textElements))
                return null;

            var attributes = DA.GetGooListValue<TSD.Text.TextAttributes>(ParamInfos.TextAttributes);
            if(attributes==null) {
                attributes=new List<TSD.Text.TextAttributes>
                {
                    new TSD.Text.TextAttributes( "standard")
                };
            }
            var textNumber = new int[]
            {
                insertionPoints.Count,
                leaderLineEndPoints.Count,
                textElements.Count,
                attributes.Count
            }.Max();

            var insertedTexts = new TSD.Text[textNumber];
            for(int i = 0; i<textNumber; i++) {

                var text = InsertText(
                    view,
                      attributes.ElementAtOrLast(i),
                     insertionPoints.ElementAtOrLast(i),
                     leaderLineEndPoints.ElementAtOrLast(i),
                    textElements.ElementAtOrLast(i)
              );

                insertedTexts[i]=text;
            }

            DrawingInteractor.CommitChanges();

            DA.SetDataList(ParamInfos.Text.Name, insertedTexts);
            return insertedTexts;
        }


        private TSD.Text InsertText(TSD.View view, TSD.Text.TextAttributes attribute, Point3d point3d1, Point3d point3d, string text) {

            Tekla.Structures.Drawing.Text textElement = new Tekla.Structures.Drawing.Text(view, point3d1.ToTekla(), text, attribute);
            textElement.Placing=TSD.PlacingTypes.LeaderLinePlacing(point3d.ToTekla());

            textElement.Insert();
            return textElement;
        }

    }
}
