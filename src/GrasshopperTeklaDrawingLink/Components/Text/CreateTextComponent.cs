using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

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
            AddPointParameter(pManager, ParamInfos.MarkLeaderLineEndPoint, GH_ParamAccess.list, true);
            AddTextParameter(pManager, ParamInfos.Text, GH_ParamAccess.list, true);
            SetParametersAsOptional(pManager, new List<int> {
                    pManager.AddParameter(new TextAttributesParam(ParamInfos.TextAttributes, GH_ParamAccess.list))
            });

            AddTextParameter(pManager, ParamInfos.Attributes, GH_ParamAccess.list, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddParameter(new TeklaDatabaseObjectParam(ParamInfos.Text, GH_ParamAccess.list));
        }


        protected override IEnumerable<TSD.DatabaseObject> InsertObjects(IGH_DataAccess dA) {
            if(!(dA.GetGooValue<TSD.DatabaseObject>(ParamInfos.View) is TSD.View view))
                return null;

            var insertionPoints = new List<Point3d>();
            if(!dA.GetDataList(ParamInfos.MarkInsertionPoint.Name, insertionPoints))
                return null;
            var leaderLineEndPoints = new List<Point3d>();
            dA.GetDataList(ParamInfos.MarkLeaderLineEndPoint.Name, leaderLineEndPoints);

            var attributeFiles = new List<string>();
            dA.GetDataList(ParamInfos.Attributes.Name, attributeFiles);

            var textElements = new List<string>();
            if(!dA.GetDataList(ParamInfos.Text.Name, textElements))
                return null;

            var attributes = dA.GetGooListValue<TSD.Text.TextAttributes>(ParamInfos.TextAttributes);

            attributes=ProcessAttributes(attributeFiles, attributes);

            var textNumber = new int[]
            {
                insertionPoints.Count,
                leaderLineEndPoints.Count,
                textElements.Count,
                attributes.Count
            }.Max();

            var insertedTexts = new TSD.Text[textNumber];
            if(leaderLineEndPoints.Any()) {
                CreateTextWithLeader(view,
                    insertionPoints,
                    leaderLineEndPoints,
                    textElements,
                    attributes,
                    textNumber,
                    insertedTexts);
            }
            else {
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

        private void CreateTextWithoutLeader(View view, List<Point3d> insertionPoints, List<string> textElements, List<TSD.Text.TextAttributes> attributes, int textNumber, TSD.Text[] insertedTexts) {
            for(int i = 0; i<textNumber; i++) {
                var text = InsertText(
                    view,
                    attributes.ElementAtOrLast(i),
                    insertionPoints.ElementAtOrLast(i),
                    textElements.ElementAtOrLast(i)
              );

                insertedTexts[i]=text;
            }
        }

        private void CreateTextWithLeader(View view, List<Point3d> insertionPoints, List<Point3d> leaderLineEndPoints, List<string> textElements, List<TSD.Text.TextAttributes> attributes, int textNumber, TSD.Text[] insertedTexts) {
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
        }

        /// <summary>
        /// Compares the attributes from files and components and returns the attributes to be inserted. 
        /// As long as the component attribute is not the default value defined in <see cref="_propertiesWithDefaultValues"/>
        /// assign its value.
        /// </summary>
        /// <param name="attributesFromFile"></param>
        /// <param name="attributesFromComponent"></param>
        /// <returns></returns>
        private static List<TSD.Text.TextAttributes> ProcessAttributes(List<string> attributesFromFile, List<TSD.Text.TextAttributes> attributesFromComponent) {
            var attributes = new List<TSD.Text.TextAttributes>();
            bool attributesIsNull = attributesFromComponent is null;
            bool attributeFilesIsEmpty = !attributesFromFile.Any();
            byte flag = (byte)((attributesIsNull ? 1 : 0)<<1|(attributeFilesIsEmpty ? 1 : 0));


            switch(flag) {
                //No attributes from components
                //Attributes provided from files
                case 0b10: {
                        for(int i = 0; i<attributesFromFile.Count; i++) {
                            attributes.Add(new TSD.Text.TextAttributes(attributesFromFile[i]));
                        }
                        return attributes;
                    }
                //Attributes both from components and from files
                case 0b00: {
                        for(int i = 0; i<Math.Min(attributesFromFile.Count, attributesFromComponent.Count); i++) {
                            var attributeFromFile = new TSD.Text.TextAttributes(attributesFromFile[i]);
                            var attributeFromComponent = attributesFromComponent[i];

                            UpdateAttributesFromDefaults(attributeFromComponent, attributeFromFile, _propertiesWithDefaultValues);

                            attributes.Add(attributeFromComponent);
                        }
                        return attributes;
                    }
                //Attributes from components only
                //Do nothing
                case 0b01: {
                        attributes = attributesFromComponent;
                        return attributes;
                    }
                //No attributes provided
                default: {
                        attributes.Add(new TSD.Text.TextAttributes());
                        return attributes;
                    }
            }
        }
        /// <summary>
        /// Map the properties to be used with their respective default values
        /// </summary>
        static readonly Dictionary<string, object> _propertiesWithDefaultValues = new Dictionary<string, object>
        {
            { nameof(TSD.Text.TextAttributes.Font), default },
            { nameof(TSD.Text.TextAttributes.Frame), new Dictionary<string, object>
                {
                    { nameof(TSD.Text.TextAttributes.Frame.Color), DrawingColors.Black },
                    { nameof(TSD.Text.TextAttributes.Frame.Type), FrameTypes.None },
                }
            },
            { nameof(TSD.Text.TextAttributes.Angle), 0 },
            { nameof(TSD.Text.TextAttributes.TransparentBackground), false },
            { nameof(TSD.Text.TextAttributes.Alignment), TSD.TextAlignment.Left },
            { nameof(TSD.Text.TextAttributes.PlacingAttributes), default(TSD.PlacingAttributes) },
            { nameof(TSD.Text.TextAttributes.PreferredPlacing), default(PointPlacingType) },
            { nameof(TSD.Text.TextAttributes.RulerWidth), 0 },
            { nameof(TSD.Text.TextAttributes.UseWordWrapping), false },
            { nameof(TSD.Text.TextAttributes.ArrowHead), new Dictionary<string, object>
                {
                    { nameof(TSD.Text.TextAttributes.ArrowHead.Head), ArrowheadTypes.FilledArrow },
                    { nameof(TSD.Text.TextAttributes.ArrowHead.Width), 1.0 },
                    { nameof(TSD.Text.TextAttributes.ArrowHead.Height), 2.0 },
                } 
            }
        };

        /// <summary>
        /// Update the properties from <paramref name="fileAttribute"/> when the 
        /// <paramref name="componentAttribute"/> is equal to the default value
        /// </summary>
        /// <param name="componentAttribute">The atrribute retrieved from the component of type <see cref="TextAttributesComponent"/></param>
        /// <param name="fileAttribute">The attribute retrieve from the string type input of the component.</param>
        /// <param name="defaultValues">The dictionary with default values.</param>
        private static void UpdateAttributesFromDefaults(object componentAttribute, object fileAttribute, Dictionary<string, object> defaultValues) {
            foreach(var entry in defaultValues) {
                var type = componentAttribute.GetType();
                var property = type.GetProperty(entry.Key);

                if(property==null)
                    continue;

                var componentValue = property.GetValue(componentAttribute);

                if(entry.Value is Dictionary<string, object> nestedDefaults) {
                    // Recursively process nested properties
                    var fileValue = property.GetValue(fileAttribute);
                    UpdateAttributesFromDefaults(componentValue, fileValue, nestedDefaults);
                }
                else if(componentValue.Equals(entry.Value)) {
                    property.SetValue(componentAttribute, property.GetValue(fileAttribute));
                }
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
        private TSD.Text InsertText(View view, TSD.Text.TextAttributes attribute, Point3d insertionPoint, string text) {
            Tekla.Structures.Drawing.Text textElement = new Tekla.Structures.Drawing.Text(view, insertionPoint.ToTekla(), text, attribute) {
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
        private TSD.Text InsertText(TSD.View view, TSD.Text.TextAttributes attribute, Point3d insertionPoint, Point3d leaderLinePoint, string text) {

            Tekla.Structures.Drawing.Text textElement = new Tekla.Structures.Drawing.Text(view, insertionPoint.ToTekla(), text, attribute) {
                Placing = TSD.PlacingTypes.LeaderLinePlacing(leaderLinePoint.ToTekla())
            };

            textElement.Insert();
            return textElement;
        }

    }
}
