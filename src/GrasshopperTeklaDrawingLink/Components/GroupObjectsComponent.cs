using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Tekla.Structures.Model;

namespace GTDrawingLink.Components
{
    public class GroupObjectsComponent : TeklaComponentBase
    {
        private GroupingMode _mode = GroupingMode.ByAssemblyPosition;
        public override GH_Exposure Exposure => GH_Exposure.primary;
        protected override Bitmap Icon => Properties.Resources.GroupObjects;

        public GroupObjectsComponent() : base(ComponentInfos.GroupObjectsComponent)
        {
            SetCustomMessage();
        }

        protected override void AppendAdditionalComponentMenuItems(System.Windows.Forms.ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.GroupByPosition.Name, ByPositionMenuItem_Clicked, true, _mode == GroupingMode.ByAssemblyPosition).ToolTipText = ParamInfos.GroupByPosition.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.GroupByName.Name, ByNameMenuItem_Clicked, true, _mode == GroupingMode.ByName).ToolTipText = ParamInfos.GroupByName.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.GroupByClass.Name, ByClassMenuItem_Clicked, true, _mode == GroupingMode.ByClass).ToolTipText = ParamInfos.GroupByClass.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.GroupByUDA.Name, ByUdaMenuItem_Clicked, true, _mode == GroupingMode.ByUDA).ToolTipText = ParamInfos.GroupByUDA.Description;
            GH_DocumentObject.Menu_AppendItem(menu, ParamInfos.GroupByReport.Name, ByReportMenuItem_Clicked, true, _mode == GroupingMode.ByReport).ToolTipText = ParamInfos.GroupByReport.Description;
            GH_DocumentObject.Menu_AppendSeparator(menu);
        }

        private void ByPositionMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = GroupingMode.ByAssemblyPosition;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ByNameMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = GroupingMode.ByName;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ByClassMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = GroupingMode.ByClass;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ByUdaMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = GroupingMode.ByUDA;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void ByReportMenuItem_Clicked(object sender, EventArgs e)
        {
            _mode = GroupingMode.ByReport;
            SetCustomMessage();
            ExpireSolution(recompute: true);
        }

        private void SetCustomMessage()
        {
            switch (_mode)
            {
                case GroupingMode.ByAssemblyPosition:
                    base.Message = "Assembly Position";
                    break;
                case GroupingMode.ByName:
                    base.Message = "Name";
                    break;
                case GroupingMode.ByClass:
                    base.Message = "Class";
                    break;
                case GroupingMode.ByUDA:
                    base.Message = "UDA";
                    break;
                case GroupingMode.ByReport:
                    base.Message = "Report";
                    break;
                default:
                    break;
            }
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32(ParamInfos.GroupByPosition.Name, (int)_mode);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var serializedInt = 0;
            reader.TryGetInt32(ParamInfos.GroupByPosition.Name, ref serializedInt);
            _mode = (GroupingMode)serializedInt;
            SetCustomMessage();
            return base.Read(reader);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            AddGenericParameter(pManager, ParamInfos.ModelObject, GH_ParamAccess.list);
            AddTextParameter(pManager, ParamInfos.PropertyName, GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            AddIntegerParameter(pManager, ParamInfos.GroupingIndices, GH_ParamAccess.tree);
            AddTextParameter(pManager, ParamInfos.GroupingKeys, GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var inputObjects = new List<dynamic>();
            if (!DA.GetDataList(ParamInfos.ModelObject.Name, inputObjects))
                return;

            var modelObjects = inputObjects.Select(i => i.Value as ModelObject).ToList();

            var propertyName = string.Empty;
            DA.GetData(ParamInfos.PropertyName.Name, ref propertyName);

            if (string.IsNullOrEmpty(propertyName) &&
                (_mode == GroupingMode.ByUDA || _mode == GroupingMode.ByReport))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Property name not specified");
                return;
            }

            var dictionary = new Dictionary<string, List<ModelObject>>();
            foreach (var modelObject in modelObjects)
            {
                if (modelObject is null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Provided object is not Tekla Model one");
                    continue;
                }

                string propertyValue = GetPropertyValue(modelObject, propertyName);
                if (dictionary.ContainsKey(propertyValue))
                    dictionary[propertyValue].Add(modelObject);
                else
                    dictionary.Add(propertyValue, new List<ModelObject> { modelObject });
            }


            DA.SetDataTree(0, GetOutputTree(dictionary, modelObjects));
            DA.SetDataList(1, dictionary.Select(d => d.Key));
        }

        private string GetPropertyValue(ModelObject inputObject, string inputProperty)
        {
            string propertyName;
            switch (_mode)
            {
                case GroupingMode.ByAssemblyPosition:
                    propertyName = "ASSEMBLY_POS";
                    break;
                case GroupingMode.ByName:
                    propertyName = "NAME";
                    break;
                case GroupingMode.ByClass:
                    propertyName = "CLASS_ATTR";
                    break;
                case GroupingMode.ByUDA:
                    propertyName = $"USERDEFINED.{inputProperty}";
                    break;
                case GroupingMode.ByReport:
                    propertyName = inputProperty;
                    break;
                default:
                    return "";
            }

            if (string.IsNullOrEmpty(propertyName))
            {
                return "";
            }

            var mainPart = GetMainPart(inputObject);
            if (mainPart == null)
            {
                return "";
            }

            var strValue = "";
            if (mainPart.GetReportProperty(propertyName, ref strValue))
            {
                return strValue;
            }

            var dblValue = 0.0;
            if (mainPart.GetReportProperty(propertyName, ref dblValue))
            {
                return dblValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            var intValue = 0;
            if (mainPart.GetReportProperty(propertyName, ref intValue))
            {
                return intValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            return "";
        }

        private Part GetMainPart(ModelObject modelObject)
        {
            if (modelObject is Part part)
            {
                return part;
            }
            else if (modelObject is Assembly assembly)
            {
                return assembly.GetMainPart() as Part;
            }
            else if (modelObject is CustomPart customPart)
            {
                var moe = customPart.GetChildren();
                while (moe.MoveNext())
                {
                    if (moe.Current is Part innerPart)
                    {
                        return innerPart.GetAssembly().GetMainPart() as Part;
                    }
                }
            }

            return null;
        }

        private IGH_Structure GetOutputTree(Dictionary<string, List<ModelObject>> dictionary, List<ModelObject> modelObjects)
        {
            var output = new GH_Structure<GH_Integer>();

            var index = 0;
            foreach (var keyValuePair in dictionary)
            {
                var currentObjects = keyValuePair.Value;

                var indicies = currentObjects.Select(o => new GH_Integer(modelObjects.IndexOf(o)));
                output.AppendRange(indicies, new GH_Path(0, index));

                index++;
            }
            return output;
        }

        enum GroupingMode
        {
            ByAssemblyPosition,
            ByName,
            ByClass,
            ByUDA,
            ByReport
        }
    }
}
