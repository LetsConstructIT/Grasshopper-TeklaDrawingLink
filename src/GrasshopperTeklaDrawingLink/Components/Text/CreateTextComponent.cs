using Grasshopper.Kernel;

using GTDrawingLink.Extensions;
using GTDrawingLink.Tools;
using GTDrawingLink.Types;

using Rhino.Geometry;

using System;
using System.Collections.Generic;
using System.Linq;

using TSD = Tekla.Structures.Drawing;

namespace GTDrawingLink.Components.Text
{
    public class CreateTextComponent : CreateDatabaseObjectComponentBase
    {
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public CreateTextComponent() : base(ComponentInfos.CreateTextComponent)
        {
        }
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.View, GH_ParamAccess.item));
            AddTextParameter(pManager, ParamInfos.Text, GH_ParamAccess.list);
            AddPointParameter(pManager, ParamInfos.MarkInsertionPoint, GH_ParamAccess.list);
            SetParametersAsOptional(pManager, new List<int> {
                    pManager.AddPointParameter(ParamInfos.MarkLeaderLineEndPoint.Name, 
                                                ParamInfos.MarkLeaderLineEndPoint.NickName, 
                                                ParamInfos.MarkLeaderLineEndPoint.Description, 
                                                GH_ParamAccess.list),
                     pManager.AddParameter(new TextAttributesParam(ParamInfos.TextAttributes, GH_ParamAccess.list))
            });
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Text, GH_ParamAccess.list));
        }


        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess dA)
        {
            if (!(dA.GetGooValue<TSD.DatabaseObject>(ParamInfos.View) is TSD.View view))
                return null;

            var insertionPoints = new List<Point3d>();
            if (!dA.GetDataList(ParamInfos.MarkInsertionPoint.Name, insertionPoints))
                return null;
            var leaderLineEndPoints = new List<Point3d>();
            dA.GetDataList(ParamInfos.MarkLeaderLineEndPoint.Name, leaderLineEndPoints);

            var textElements = new List<string>();
            if (!dA.GetDataList(ParamInfos.Text.Name, textElements))
                return null;

            var attributes = dA.GetGooListValue<TSD.Text.TextAttributes>(ParamInfos.TextAttributes);

            attributes = (attributes is null)
                ?new List<TSD.Text.TextAttributes> { new TSD.Text.TextAttributes() } 
                :attributes;

            var textNumber = new int[]
            {
                insertionPoints.Count,
                leaderLineEndPoints.Count,
                textElements.Count,
                attributes.Count
            }.Max();

            var insertedTexts = new TSD.Text[textNumber];
            if (leaderLineEndPoints.Any())
            {
                CreateTextWithLeader(view,
                    insertionPoints,
                    leaderLineEndPoints,
                    textElements,
                    attributes,
                    textNumber,
                    insertedTexts);
            }
            else
            {
                CreateTextWithoutLeader(view,
                    insertionPoints,
                    textElements,
                    attributes,
                    textNumber,
                    insertedTexts);
            }
            DrawingInteractor.CommitChanges();
            dA.SetDataList(ParamInfos.Text.Name, insertedTexts);
            return insertedTexts;
        }

        private void CreateTextWithoutLeader(TSD.View view, List<Point3d> insertionPoints, List<string> textElements, List<TSD.Text.TextAttributes> attributes, int textNumber, TSD.Text[] insertedTexts)
        {
            for (int i = 0; i < textNumber; i++)
            {
                var text = InsertText(
                    view,
                    attributes.ElementAtOrLast(i),
                    insertionPoints.ElementAtOrLast(i),
                    textElements.ElementAtOrLast(i)
              );

                insertedTexts[i] = text;
            }
        }

        private void CreateTextWithLeader(TSD.View view, List<Point3d> insertionPoints, List<Point3d> leaderLineEndPoints, List<string> textElements, List<TSD.Text.TextAttributes> attributes, int textNumber, TSD.Text[] insertedTexts)
        {
            for (int i = 0; i < textNumber; i++)
            {
                var text = InsertText(
                    view,
                    attributes.ElementAtOrLast(i),
                    insertionPoints.ElementAtOrLast(i),
                    leaderLineEndPoints.ElementAtOrLast(i),
                    textElements.ElementAtOrLast(i)
              );

                insertedTexts[i] = text;
            }
        }

        /// <summary>
        /// Inserts the text without the leader line
        /// </summary>
        /// <param name="view"></param>
        /// <param name="attribute"></param>
        /// <param name="insertionPoint"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private TSD.Text InsertText(TSD.View view, TSD.Text.TextAttributes attribute, Point3d insertionPoint, string text)
        {
            Tekla.Structures.Drawing.Text textElement = new Tekla.Structures.Drawing.Text(view, insertionPoint.ToTekla(), text, attribute)
            {
                Placing = TSD.PlacingTypes.PointPlacing()
            };
            textElement.Insert();
            return textElement;
        }

        /// <summary>
        /// Inserts the text element with leader line
        /// </summary>
        /// <param name="view"></param>
        /// <param name="attribute"></param>
        /// <param name="insertionPoint"></param>
        /// <param name="leaderLinePoint"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private TSD.Text InsertText(TSD.View view, TSD.Text.TextAttributes attribute, Point3d insertionPoint, Point3d leaderLinePoint, string text)
        {

            Tekla.Structures.Drawing.Text textElement = new Tekla.Structures.Drawing.Text(view, insertionPoint.ToTekla(), text, attribute)
            {
                Placing = TSD.PlacingTypes.LeaderLinePlacing(leaderLinePoint.ToTekla())
            };

            textElement.Insert();
            return textElement;
        }

    }
}
