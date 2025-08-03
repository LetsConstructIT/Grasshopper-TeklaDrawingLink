using DrawingLink.UI.GH.Models;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace DrawingLink.UI.GH
{
    public class ParameterTransformer
    {
        public static ParametersRoot Transform(IEnumerable<ActiveObjectWrapper> parameters)
        {
            var root = new ParametersRoot();
            foreach (var item in parameters.OrderBy(p => p.TabAndGroup.Tab == "Settings").ThenBy(p => p.ActiveObject.Attributes.Bounds.Top))
            {
                var param = item.ActiveObject;
                var tabAndGroup = item.TabAndGroup;
                var connectivity = item.Connectivity;
                var fieldName = item.FieldName;
                var top = param.Attributes.Bounds.Top;

                var tab = root.Tabs.FirstOrDefault(t => t.Name == tabAndGroup.Tab);
                if (tab is null)
                {
                    tab = new UiTab(tabAndGroup.Tab);
                    root.AddTab(tab);
                }

                var group = tab.Groups.FirstOrDefault(g => g.Name == tabAndGroup.Group);
                if (group is null)
                {
                    group = new UiGroup(tabAndGroup.Group);
                    tab.AddGroup(group);
                }

                if (param is Param_FilePath paramFilePath)
                {
                    if (CheckIfDirectory(param))
                        group.AddParam(new DirectoryInfoParam(fieldName,
                                                     param.NickName,
                                                     (paramFilePath.PersistentData.AllData(skipNulls: true).FirstOrDefault() as GH_String)?.Value ?? "",
                                                     top));
                    else
                        group.AddParam(new FileInfoParam(fieldName,
                                                         param.NickName,
                                                         (paramFilePath.PersistentData.AllData(skipNulls: true).FirstOrDefault() as GH_String)?.Value ?? "",
                                                         top));
                }
                else if (param is GH_BooleanToggle toggle)
                {
                    group.AddParam(new ListParamData(fieldName,
                                                     param.NickName,
                                                     new string[2] { "False", "True" },
                                                     toggle.Value ? "True" : "False",
                                                     top));
                }
                else if (param is GH_Panel panel)
                {
                    if ((connectivity.SourceCount == 0 && connectivity.RecipientCount > 0) ||
                        tabAndGroup.Tab == "Settings")
                        group.AddParam(new TextParam(fieldName,
                                                     param.NickName,
                                                     panel.UserText,
                                                     top));
                    else
                        group.AddParam(new InfoParam(panel.UserText, top));
                }
                else if (param is GH_ImageSampler imageSampler)
                {
                    group.AddParam(new ImageParam(imageSampler.Image, top));
                }
                else if (param is GH_ValueList valueList)
                {
                    group.AddParam(new ListParamData(fieldName,
                                                     param.NickName,
                                                     valueList.ListItems.Select((GH_ValueListItem i) => i.Name),
                                                     valueList.FirstSelectedItem.Name,
                                                     top));
                }
                else if (param is GH_NumberSlider slider)
                {
                    group.AddParam(new SliderParam(fieldName,
                                                   param.NickName,
                                                   Convert.ToDouble(slider.Slider.Minimum),
                                                   Convert.ToDouble(slider.Slider.Maximum),
                                                   Convert.ToDouble(slider.CurrentValue),
                                                   (slider.Slider.Type == Grasshopper.GUI.Base.GH_SliderAccuracy.Float) ? slider.Slider.DecimalPlaces : 0,
                                                   top));
                }
                else if (param is GH_PersistentParam<GH_String>)
                {
                    group.AddParam(new TextParam(fieldName,
                                                 param.NickName,
                                                 ((GH_Goo<string>)param)?.Value ?? "",
                                                 top));
                }
                else if (param is GH_PersistentParam<GH_Integer>)
                {
                    group.AddParam(new TextParam(fieldName,
                                                 param.NickName,
                                                 ((GH_Goo<int>)param)?.Value.ToString(CultureInfo.InvariantCulture) ?? "",
                                                 top));
                }
                else if (param is GH_PersistentParam<GH_Number>)
                {
                    group.AddParam(new TextParam(fieldName,
                                                 param.NickName,
                                                 ((GH_Goo<int>)param)?.Value.ToString(CultureInfo.InvariantCulture) ?? "",
                                                 top));
                }
                else if (param.GetType().BaseType.Name == "CatalogBaseComponent")
                {
                    var textValue = (string)param.GetType().GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public).Invoke(param, null);
                    var pickFromCatalog = delegate (string currentValue)
                    {
                        var method = param.GetType().GetMethod("PickFromCatalogExternal", BindingFlags.Instance | BindingFlags.Public);
                        return (string)method.Invoke(param, new string[1] { currentValue });
                    };

                    group.AddParam(new CatalogParamData(fieldName, param.NickName, textValue, pickFromCatalog, top));
                }
            }

            SetMissingNameForTab(root);
            return root;
        }

        private static bool CheckIfDirectory(IGH_ActiveObject activeObject)
        {
            var nickName = activeObject.NickName.Trim().ToUpperInvariant();
            return nickName.StartsWith("DIRECTORY") || nickName.StartsWith("DIR:");
        }

        private static void SetMissingNameForTab(ParametersRoot root)
        {
            if (root.Tabs.Count != 1)
                return;

            var tab = root.Tabs[0];
            if (string.IsNullOrEmpty(tab.Name))
                tab.ChangeName("Attributes");
        }
    }
}
