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
using System.Text.RegularExpressions;

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
                                                     top,
                                                     item.TableColumnInfo));
                    else
                        group.AddParam(new FileInfoParam(fieldName,
                                                         param.NickName,
                                                         (paramFilePath.PersistentData.AllData(skipNulls: true).FirstOrDefault() as GH_String)?.Value ?? "",
                                                         top,
                                                     item.TableColumnInfo));
                }
                else if (param is GH_BooleanToggle toggle)
                {
                    group.AddParam(new ListParamData(fieldName,
                                                     param.NickName,
                                                     new string[2] { "False", "True" },
                                                     toggle.Value ? "True" : "False",
                                                     top,
                                                     item.TableColumnInfo));
                }
                else if (param is GH_Panel panel)
                {
                    if ((connectivity.SourceCount == 0 && connectivity.RecipientCount > 0) ||
                        tabAndGroup.Tab == "Settings")
                        group.AddParam(new TextParam(fieldName,
                                                     param.NickName,
                                                     panel.UserText,
                                                     top,
                                                     item.TableColumnInfo));
                    else
                        group.AddParam(new InfoParam(panel.UserText, top,
                                                     item.TableColumnInfo));
                }
                else if (param is GH_ImageSampler imageSampler)
                {
                    var style = ParseImageStyle(param.NickName);
                    group.AddParam(new ImageParam(imageSampler.Image, top, style,
                                                     item.TableColumnInfo));
                }
                else if (param is GH_ValueList valueList)
                {
                    group.AddParam(new ListParamData(fieldName,
                                                     param.NickName,
                                                     valueList.ListItems.Select((GH_ValueListItem i) => i.Name),
                                                     valueList.FirstSelectedItem.Name,
                                                     top,
                                                     item.TableColumnInfo));
                }
                else if (param is GH_NumberSlider slider)
                {
                    group.AddParam(new SliderParam(fieldName,
                                                   param.NickName,
                                                   Convert.ToDouble(slider.Slider.Minimum),
                                                   Convert.ToDouble(slider.Slider.Maximum),
                                                   Convert.ToDouble(slider.CurrentValue),
                                                   (slider.Slider.Type == Grasshopper.GUI.Base.GH_SliderAccuracy.Float) ? slider.Slider.DecimalPlaces : 0,
                                                   top,
                                                   item.TableColumnInfo));
                }
                else if (param is GH_PersistentParam<GH_String>)
                {
                    group.AddParam(new TextParam(fieldName,
                                                 param.NickName,
                                                 ((GH_Goo<string>)param)?.Value ?? "",
                                                 top,
                                                 item.TableColumnInfo));
                }
                else if (param is GH_PersistentParam<GH_Integer>)
                {
                    group.AddParam(new TextParam(fieldName,
                                                 param.NickName,
                                                 ((GH_Goo<int>)param)?.Value.ToString(CultureInfo.InvariantCulture) ?? "",
                                                 top,
                                                 item.TableColumnInfo));
                }
                else if (param is GH_PersistentParam<GH_Number>)
                {
                    group.AddParam(new TextParam(fieldName,
                                                 param.NickName,
                                                 ((GH_Goo<int>)param)?.Value.ToString(CultureInfo.InvariantCulture) ?? "",
                                                 top,
                                                 item.TableColumnInfo));
                }
                else if (param.GetType().BaseType.Name == "CatalogBaseComponent")
                {
                    var textValue = (string)param.GetType().GetMethod("GetValue", BindingFlags.Instance | BindingFlags.Public).Invoke(param, null);
                    var pickFromCatalog = delegate (string currentValue)
                    {
                        var method = param.GetType().GetMethod("PickFromCatalogExternal", BindingFlags.Instance | BindingFlags.Public);
                        return (string)method.Invoke(param, new string[1] { currentValue });
                    };

                    group.AddParam(new CatalogParamData(fieldName, param.NickName, textValue, pickFromCatalog, top, item.TableColumnInfo));
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

        private static ImageStyle ParseImageStyle(string imageStyleString)
        {
            var source = (from c in imageStyleString.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                          select c.Trim().ToUpperInvariant()).ToList();
            var isBackground = source.Any((string c) => c == "BACKGROUND" || c == "BG");
            var positionX = (source.Any((string c) => c == "LEFT" || c == "L" || Regex.IsMatch(c, "^L-?(\\d+)")) ? "LEFT" : (source.Any((string c) => c == "RIGHT" || c == "R" || Regex.IsMatch(c, "^R-?(\\d+)")) ? "RIGHT" : (source.Any((string c) => c == "CENTER") ? "CENTER" : "CENTER")));
            var positionY = (source.Any((string c) => c == "TOP" || c == "T" || Regex.IsMatch(c, "^T-?(\\d+)")) ? "TOP" : (source.Any((string c) => c == "BOTTOM" || c == "B" || Regex.IsMatch(c, "^B-?(\\d+)")) ? "BOTTOM" : (source.Any((string c) => c == "MIDDLE") ? "MIDDLE" : "TOP")));

            int.TryParse(source.FirstOrDefault((string c) => Regex.IsMatch(c, "^L-?(\\d+)"))?.Substring(1) ?? source.FirstOrDefault((string c) => c.StartsWith("LEFT"))?.Substring(4) ?? source.FirstOrDefault((string c) => Regex.IsMatch(c, "^R-?(\\d+)"))?.Substring(1) ?? source.FirstOrDefault((string c) => c.StartsWith("RIGHT"))?.Substring(5), out int paddingX);
            int.TryParse(source.FirstOrDefault((string c) => Regex.IsMatch(c, "^T-?(\\d+)"))?.Substring(1) ?? source.FirstOrDefault((string c) => c.StartsWith("TOP"))?.Substring(3) ?? source.FirstOrDefault((string c) => Regex.IsMatch(c, "^B-?(\\d+)"))?.Substring(1) ?? source.FirstOrDefault((string c) => c.StartsWith("BOTTOM"))?.Substring(6), out int paddingY);

            int width = 0;
            bool sizeTypePercent = false;
            string text = source.FirstOrDefault((string c) => Regex.IsMatch(c, "^W?(\\d+)"));
            if (text != null)
            {
                int.TryParse(text.TrimStart('W').TrimEnd('%'), out width);
                sizeTypePercent = text[text.Length - 1] == '%';
            }

            int.TryParse(source.FirstOrDefault((string c) => Regex.IsMatch(c, "^H(\\d+)"))?.Substring(1), out int height);

            return new ImageStyle
            (
                 isBackground,
                 positionX,
                 positionY,
                 paddingX,
                 paddingY,
                 width,
                 height,
                 sizeTypePercent
            );
        }
    }
}
