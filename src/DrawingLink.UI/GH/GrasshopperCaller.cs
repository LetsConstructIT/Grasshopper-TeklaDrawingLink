using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Rhino.Runtime.InProcess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows;

namespace DrawingLink.UI.GH
{
    public sealed class GrasshopperCaller
    {
        private static GrasshopperCaller _instance;
        private static RhinoCore _rhinoCore;

        private const string STRING_SETTING_TOLERANCE = "RHINO TOLERANCE";
        private const string STRING_SETTING_ANGLE_TOLERANCE = "RHINO ANGLE TOLERANCE";

        private GrasshopperCaller()
        {
        }

        public static GrasshopperCaller GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GrasshopperCaller();
                _rhinoCore = new RhinoCore();
            }

            return _instance;
        }

        public void Solve(string definitionPath, Dictionary<GH_RuntimeMessageLevel, List<string>> messages)
        {
            var status = OperateOnGrasshopperScript(definitionPath, doc => SolveDocument(doc, messages));
        }

        private TemporaryResultObject SolveDocument(GH_Document document, Dictionary<GH_RuntimeMessageLevel, List<string>> messages)
        {
            var allowedComponentTypes = new string[]
            {
                "CreateModelObjectComponent",
                "DeconstructModelObjectBaseComponent",
                "Component_CSNET_Script",
                "Component_VBNET_Script",
                "ZuiPythonComponent",
                "TeklaComponentBase"
            };

            var inputParams = GetInputParams(document);
            foreach (var param in inputParams.AttributeParams)
            {
                param.ActiveObject.ExpireSolution(true);
            }

            foreach (var activeObject in document.Objects.OfType<IGH_ActiveObject>().Where(o => !o.Locked))
            {
                if (activeObject is GH_Component component && GetInheritanceHierarchy(activeObject.GetType()).Any(t => allowedComponentTypes.Contains(t.Name)))
                {
                    component.CollectData();
                    component.ComputeData();
                }
                else if (activeObject is GH_Panel && (activeObject as GH_Panel).Sources.Any())
                {
                    GH_Panel gh_Panel = activeObject as GH_Panel;
                    string panelName = gh_Panel.NickName.Trim().ToUpperInvariant();
                    if (IsOutputPanelName(panelName))
                    {
                        gh_Panel.CollectData();
                        gh_Panel.ComputeData();
                        AppendOutputMessages(gh_Panel, messages);
                    }
                    else if (IsSolvePanelName(panelName))
                    {
                        gh_Panel.CollectData();
                        gh_Panel.ComputeData();
                    }
                }
            }

            return new TemporaryResultObject();
        }

        private static IEnumerable<Type> GetInheritanceHierarchy(Type type)
        {
            Type current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public GHParams? GetInputParams(string definitionPath)
        {
            return OperateOnGrasshopperScript(definitionPath, doc => GetInputParams(doc));
        }

        private T? OperateOnGrasshopperScript<T>(string definitionPath, Func<GH_Document, T> action) where T : class
        {
            if (!File.Exists(definitionPath))
                return null;

            var archive = new GH_Archive();
            var text = "";
            bool flag;
            try
            {
                flag = archive.ReadFromFile(definitionPath);
            }
            catch (Exception ex)
            {
                flag = false;
                text = ex.Message;
            }

            if (!flag)
            {
                MessageBox.Show(("Failed to read definition file " + definitionPath + text != null) ? ("\n\n" + text) : "");
                return null;
            }

            var document = new GH_Document();
            try
            {
                if (!archive.ExtractObject(document, "Definition"))
                    throw new Exception("Couldn't extract definition from file " + definitionPath);

                if (document == null)
                    return null;

                return action.Invoke(document);
            }
            finally
            {
                document?.Dispose();
            }
        }

        private GHParams GetInputParams(GH_Document doc)
        {
            var activeObjects = GetValidParameters(doc);

            var groups = GetInputGroups(doc);
            if (groups.Hidden.Any())
                activeObjects = activeObjects.Where(p => !groups.Hidden.Contains(p.InstanceGuid)).ToList();

            var withoutTabsAndGroups = !groups.Tabs.Any() && !groups.Groups.Any();

            var modelParams = new List<IGH_ActiveObject>();
            var drawingParams = new List<IGH_ActiveObject>();
            var attributeParams = new List<ActiveObjectWrapper>();
            foreach (var activeObject in activeObjects)
            {
                if (activeObject is GH_PersistentParam<GH_Goo<Tekla.Structures.Model.ModelObject>>)
                {
                    modelParams.Add(activeObject);
                    continue;
                }
                else if (activeObject is GH_PersistentParam<GH_Goo<Tekla.Structures.Drawing.DatabaseObject>>)
                {
                    drawingParams.Add(activeObject);
                    continue;
                }

                var tab = groups.Tabs.FirstOrDefault(g => g.Value.Contains(activeObject.InstanceGuid)).Key;
                var group = groups.Groups.FirstOrDefault(g => g.Value.Contains(activeObject.InstanceGuid)).Key;
                if (IsSettingsPanel(activeObject) && tab == null)
                    tab = "Settings";

                if (withoutTabsAndGroups || tab != null || group != null)
                {
                    var connectivity = (activeObject is IGH_Param param) ?
                        new ObjectConnectivity(param.SourceCount, param.Recipients.Count()) :
                        new ObjectConnectivity(0, 0);

                    attributeParams.Add(new ActiveObjectWrapper(activeObject, new TabAndGroup(tab, group), connectivity));
                }
            }

            return new GHParams(modelParams, drawingParams, attributeParams);
        }

        private List<IGH_ActiveObject> GetValidParameters(GH_Document doc)
        {
            var parameters = new List<IGH_ActiveObject>();
            foreach (var activeObject in doc.Objects.OfType<IGH_ActiveObject>().Where(o => !o.Locked))
            {
                switch (activeObject)
                {
                    case IGH_Param param:
                        {
                            if (string.IsNullOrWhiteSpace(param.NickName))
                            {
                                var recipent = param.Recipients.FirstOrDefault();
                                if (recipent != null)
                                    param.NickName = (!string.IsNullOrEmpty(recipent.NickName)) ? recipent.NickName : "[no name]";
                            }

                            if (param.SourceCount == 0 &&
                                (param.Recipients.Any() || param is GH_ImageSampler || IsInfoPanel(param) || IsSettingsPanel(param)))
                            {
                                parameters.Add(param);
                            }

                            break;
                        }

                    case IGH_Component component:
                        if (component.GetType().BaseType.Name == "CatalogBaseComponent" &&
                            (component.Params.Input.Count == 0 || component.Params.Input[0].SourceCount == 0) &&
                            component.Params.Output[0].Recipients.Any())
                        {
                            parameters.Add(component);
                        }
                        break;
                }
            }

            return parameters;
        }

        private GHGroups GetInputGroups(GH_Document doc)
        {
            var gHGroups = new GHGroups();
            foreach (var group in doc.Objects.OfType<GH_Group>())
            {
                var nickName = group.NickName.Trim();
                var upperName = nickName.ToUpperInvariant();
                if (upperName.StartsWith("TAB:"))
                {
                    AddGroupObjectGuids(gHGroups.Tabs, nickName.Substring(4), group);
                }
                else if (upperName.StartsWith("T:"))
                {
                    AddGroupObjectGuids(gHGroups.Tabs, nickName.Substring(2), group);
                }
                else if (upperName.StartsWith("GROUP:"))
                {
                    AddGroupObjectGuids(gHGroups.Groups, nickName.Substring(6), group);
                }
                else if (upperName.StartsWith("G:"))
                {
                    AddGroupObjectGuids(gHGroups.Groups, nickName.Substring(2), group);
                }
                else if (upperName.StartsWith("HIDE") || upperName.StartsWith("HIDDEN"))
                {
                    gHGroups.Hidden.UnionWith(from o in @group.ObjectsRecursive()
                                              select o.InstanceGuid);
                }
            }
            return gHGroups;
        }

        private void AddGroupObjectGuids(Dictionary<string, HashSet<Guid>> groupGuidCollection, string groupName, GH_Group group)
        {
            var guids = group.ObjectsRecursive().Select(o => o.InstanceGuid);

            if (!guids.Any())
                return;

            groupName = groupName.Trim();
            if (!groupGuidCollection.ContainsKey(groupName))
                groupGuidCollection[groupName] = new HashSet<Guid>();

            groupGuidCollection[groupName].UnionWith(guids);
        }

        private bool IsSettingsPanel(IGH_ActiveObject param)
        {
            if (param is GH_Panel panel)
            {
                var name = panel.NickName.Trim().ToUpperInvariant();
                return name.StartsWith(STRING_SETTING_TOLERANCE) ||
                       name.StartsWith(STRING_SETTING_ANGLE_TOLERANCE);
            }

            return false;
        }

        private bool IsInfoPanel(IGH_ActiveObject param)
        {
            if (param is GH_Panel panel && string.IsNullOrWhiteSpace(panel.NickName) && panel.SourceCount == 0)
                return !panel.Recipients.Any();

            return false;
        }

        private bool IsOutputPanelName(string panelName)
        {
            var allowedNames = new string[] { "OUTPUT", "OUT:", "O", "O:" };
            return allowedNames.Any(n => panelName.StartsWith(n));
        }

        private bool IsSolvePanelName(string panelName)
        {
            var allowedNames = new string[] { "SOLVE", "S", "S:" };
            return allowedNames.Any(n => panelName.StartsWith(n));
        }

        private void AppendOutputMessages(GH_Panel panel, Dictionary<GH_RuntimeMessageLevel, List<string>> messages)
        {
            InitializeMessagesIfNeeded(messages);
            var outputPanelName = GetOutputPanelName(panel);
            if (panel.VolatileDataCount == 0)
            {
                messages[GH_RuntimeMessageLevel.Blank].Add("[ " + outputPanelName + " ] *** No output was collected ***");
                return;
            }
            string text = string.Join("\n", from d in panel.VolatileData.AllData(true)
                                            select d.ToString());
            if (string.IsNullOrEmpty(text))
            {
                messages[GH_RuntimeMessageLevel.Blank].Add("[ " + outputPanelName + " ] *** Output is empty ***");
                return;
            }
            messages[GH_RuntimeMessageLevel.Remark].Add("[ " + outputPanelName + " ]\n" + text);
        }

        private void InitializeMessagesIfNeeded(Dictionary<GH_RuntimeMessageLevel, List<string>> messages)
        {
            messages ??= new Dictionary<GH_RuntimeMessageLevel, List<string>>();

            foreach (object obj in Enum.GetValues(typeof(GH_RuntimeMessageLevel)))
            {
                GH_RuntimeMessageLevel key = (GH_RuntimeMessageLevel)obj;
                if (!messages.ContainsKey(key) || messages[key] == null)
                    messages[key] = new List<string>();
            }
        }

        private static string GetOutputPanelName(GH_Panel panel)
        {
            string text = panel.NickName.Trim();
            string text2 = text.ToUpperInvariant();
            if (text2.StartsWith("OUTPUT ") || text2.StartsWith("OUTPUT:"))
                return text.Substring(7).Trim();
            else if (text2.StartsWith("OUT:"))
                return text.Substring(4).Trim();
            else if (text2.StartsWith("O:"))
                return text.Substring(2).Trim();
            else
                return text;
        }

        private class TemporaryResultObject
        {
        }

    }
}
