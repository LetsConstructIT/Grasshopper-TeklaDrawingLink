﻿using DrawingLink.UI.Utils;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Grasshopper.Plugin;
using Rhino;
using Rhino.Runtime.InProcess;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Tekla.Structures.Datatype;
using Tekla.Structures.Model;

namespace DrawingLink.UI.GH
{
    public sealed class GrasshopperCaller : IDisposable
    {
        private static GrasshopperCaller _instance;
        private static RhinoCore _rhinoCore;
        private static RhinoDoc _rhinoDoc;
        private static GH_RhinoScriptInterface _grasshopperInstance;
        private static readonly Guid _ghPluginGuid = new("b45a29b1-4343-4035-989e-044e8580d9cf");

        private const string STRING_SETTING_TOLERANCE = "RHINO TOLERANCE";
        private const string STRING_SETTING_ANGLE_TOLERANCE = "RHINO ANGLE TOLERANCE";
        private const string STRING_SETTING_MAX_NUMBER_OF_LOOPS = "MAX NUMBER OF LOOPS";

        private int _maxNumberOfLoops = 10000;

        private GrasshopperCaller()
        {
        }

        public static GrasshopperCaller GetInstance()
        {
            if (_instance == null)
            {
                _instance = new GrasshopperCaller();
                _rhinoCore = new RhinoCore();
                SetHeadlessRhinoDoc();
            }

            return _instance;
        }
        private static void SetHeadlessRhinoDoc()
        {
            _rhinoDoc ??= RhinoDoc.CreateHeadless(null);

            if (_rhinoDoc == null)
            {
                MessageBox.Show("Failed to create a headless Rhino doc");
                return;
            }

            UnitSystem rhinoUnitSystem = UnitSystem.Millimeters;
            if (Settings.TryGetValue("distance_unit", out var teklaUnit))
            {
                rhinoUnitSystem = (Distance.UnitType)teklaUnit switch
                {
                    Distance.UnitType.Millimeter => UnitSystem.Millimeters,
                    Distance.UnitType.Centimeter => UnitSystem.Centimeters,
                    Distance.UnitType.Meter => UnitSystem.Meters,
                    Distance.UnitType.Inch => UnitSystem.Inches,
                    Distance.UnitType.Foot => UnitSystem.Feet,
                    _ => UnitSystem.Millimeters,
                };
            }
            _rhinoDoc.ModelUnitSystem = rhinoUnitSystem;
            _rhinoDoc.ModelAbsoluteTolerance = GetAbsoluteTolerance(0.1, rhinoUnitSystem);
            _rhinoDoc.ModelAngleToleranceDegrees = 0.1;
            RhinoDoc.ActiveDoc = _rhinoDoc;
        }

        public Dictionary<GH_RuntimeMessageLevel, List<string>> Solve(UserFormData grasshopperData, Dictionary<string, TeklaObjects> teklaInput)
        {
            return OperateOnGrasshopperScript(grasshopperData.DefinitionPath, doc => SolveDocument(doc, grasshopperData, teklaInput));
        }

        private Dictionary<GH_RuntimeMessageLevel, List<string>> SolveDocument(GH_Document document, UserFormData userFormData, Dictionary<string, TeklaObjects> teklaInput)
        {
            var messages = InitializeMessageDictionary();

            var allowedComponentTypes = new string[]
            {
                "CreateModelObjectComponent",
                "DeconstructModelObjectBaseComponent",
                "Component_CSNET_Script",
                "Component_VBNET_Script",
                "ZuiPythonComponent",
                "TeklaComponentBase"
            };

            var docParams = GetInputParams(document);
            SetValuesInGrasshopper(docParams, userFormData, teklaInput);

            var activeObjects = document.Objects.OfType<IGH_ActiveObject>().Where(o => !o.Locked).ToList();
            DisableTeklaLiveness(activeObjects);

            var loopStarts = activeObjects.Where(o => o.Name.StartsWith("Loop Start")).ToList();
            document.Enabled = true;
            document.NewSolution(true);

            for (int i = 0; i < _maxNumberOfLoops; i++)
            {
                document.NewSolution(false);
                if (HaveLoopsFinished(loopStarts))
                {
                    System.Diagnostics.Debug.WriteLine($"----COMPLETED AFTER {i} iterations");
                    break;
                }
            }

            foreach (var activeObject in activeObjects)
            {
                if (activeObject is GH_Component component && GetInheritanceHierarchy(activeObject.GetType()).Any(t => allowedComponentTypes.Contains(t.Name)))
                {
                    component.CollectData();
                    component.ComputeData();
                    AppendComponentErrorMessages(component, messages);
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

            return messages;
        }

        private bool HaveLoopsFinished(List<IGH_ActiveObject> loopStarts)
        {
            return loopStarts.All(l => (bool)((dynamic)l).Completed);
        }

        private void SetValuesInGrasshopper(GHParams inputParams, UserFormData data, Dictionary<string, TeklaObjects> teklaInput)
        {
            SetTeklaInputs(inputParams.TeklaParams, teklaInput);

            foreach (ActiveObjectWrapper activeObjectWrapper in inputParams.AttributeParams)
            {
                var fieldName = activeObjectWrapper.FieldName;
                var ighActiveObject = activeObjectWrapper.ActiveObject;
                if (ighActiveObject is GH_PersistentParam<GH_String> stringParam)
                {
                    if (data.TryGetStringValue(fieldName, out string stringValue))
                    {
                        SetValue(stringParam, stringValue);
                    }
                }
                else if (ighActiveObject is GH_PersistentParam<GH_Integer> integerParam)
                {
                    if (data.TryGetIntValue(fieldName, out int intValue))
                    {
                        SetValue(integerParam, intValue);
                    }
                }
                else if (ighActiveObject is GH_PersistentParam<GH_Number> numberParam)
                {
                    if (data.TryGetDoubleValue(fieldName, out double dblValue))
                    {
                        SetValue(numberParam, dblValue);
                    }
                }
                else if (ighActiveObject is GH_Panel panel)
                {
                    if (IsInfoPanel(panel))
                        continue;

                    if (data.TryGetStringValue(fieldName, out string panelText))
                    {
                        panelText = panelText.Replace("\\n", "\r\n");
                        panel.RemoveAllSources();
                        panel.SetUserText(panelText);
                        panel.ExpireSolution(true);
                        if (IsSettingsPanel(panel))
                        {
                            var settingText = panel.NickName.Trim().ToUpperInvariant();
                            if (settingText.StartsWith(STRING_SETTING_TOLERANCE) && double.TryParse(panelText.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double rhinoTolerance))
                                SetRhinoTolerance(rhinoTolerance);
                            else if (settingText.StartsWith(STRING_SETTING_ANGLE_TOLERANCE) && double.TryParse(panelText.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double rhinoAngleTolerance))
                                SetRhinoAngleTolerance(rhinoAngleTolerance);
                            else if (settingText.StartsWith(STRING_SETTING_MAX_NUMBER_OF_LOOPS) && int.TryParse(panelText, out int maxLoopNumber))
                                _maxNumberOfLoops = maxLoopNumber;
                        }
                    }
                }
                else if (ighActiveObject is GH_ValueList valueList)
                {
                    if (data.TryGetStringValue(fieldName, out string stringValue))
                    {
                        int num = Math.Max(valueList.ListItems.FindIndex((GH_ValueListItem i) => i.Name == stringValue), 0);
                        valueList.RemoveAllSources();
                        valueList.SelectItem(num);
                        valueList.ExpireSolution(true);
                    }
                }
                else if (ighActiveObject is GH_BooleanToggle booleanToggle)
                {
                    if (data.TryGetStringValue(fieldName, out string stringValue))
                    {
                        booleanToggle.RemoveAllSources();
                        booleanToggle.Value = (stringValue.Trim().ToUpperInvariant() == "TRUE");
                        booleanToggle.ExpireSolution(true);
                    }
                }
                else if (ighActiveObject is GH_ButtonObject buttonObject)
                {
                    if (data.TryGetStringValue(fieldName, out string stringValue))
                    {
                        buttonObject.RemoveAllSources();
                        buttonObject.ButtonDown = (stringValue.Trim().ToUpperInvariant() == buttonObject.ExpressionPressed.Trim().ToUpperInvariant());
                        buttonObject.ExpireSolution(true);
                    }
                }
                else if (ighActiveObject is GH_NumberSlider gh_NumberSlider)
                {
                    if (data.TryGetDoubleValue(fieldName, out double dblValue))
                    {
                        gh_NumberSlider.RemoveAllSources();
                        gh_NumberSlider.TrySetSliderValue((decimal)dblValue);
                        gh_NumberSlider.ExpireSolution(true);
                    }
                }
                else if (ighActiveObject.GetType().BaseType.Name == "CatalogBaseComponent")
                {
                    if (data.TryGetStringValue(fieldName, out string stringValue))
                    {
                        var method = ighActiveObject.GetType().GetMethod("SetValue", BindingFlags.Instance | BindingFlags.Public);
                        var parameters = new string[] { stringValue };
                        method.Invoke(ighActiveObject, parameters);
                        ighActiveObject.ExpireSolution(true);
                    }
                }
            }

        }

        private void DisableTeklaLiveness(IEnumerable<IGH_ActiveObject> activeObjects)
        {
            var guid = Guid.NewGuid().ToString();

            foreach (IGH_DocumentObject igh_DocumentObject in activeObjects)
            {
                var property = igh_DocumentObject.GetType().GetProperty("TSPluginGuid");
                if (property != null)
                    property.SetValue(igh_DocumentObject, guid);
            }
        }

        private void SetTeklaInputs(TeklaParams teklaParams, Dictionary<string, TeklaObjects> teklaInputs)
        {
            foreach (var teklaModelParam in teklaParams.ModelParams)
            {
                var ighActiveObject = teklaModelParam.ActiveObject;
                if (!teklaInputs.ContainsKey(teklaModelParam.FieldName) ||
                    ighActiveObject is not GH_PersistentParam<GH_Goo<ModelObject>> modelPersistentParam)
                {
                    //Raise warning flag about missing input
                    continue;
                }

                modelPersistentParam.PersistentData.Clear();
                var teklaInput = teklaInputs[teklaModelParam.FieldName];

                var objectsToSet = teklaInput.GetCorrectObject();
                modelPersistentParam.SetPersistentData(objectsToSet);
                modelPersistentParam.ExpireSolution(true);
            }

            foreach (var teklaDrawingParam in teklaParams.DrawingParams)
            {
                var ighActiveObject = teklaDrawingParam.ActiveObject;
                if (!teklaInputs.ContainsKey(teklaDrawingParam.FieldName))
                {
                    //Raise warning flag about missing input
                    continue;
                }

                var teklaInput = teklaInputs[teklaDrawingParam.FieldName];
                var objectsToSet = teklaInput.GetCorrectObject();
                if (ighActiveObject is GH_PersistentParam<GH_Goo<Tekla.Structures.Drawing.DatabaseObject>> drawingPersistentParam)
                {
                    drawingPersistentParam.PersistentData.Clear();

                    drawingPersistentParam.SetPersistentData(objectsToSet);
                    drawingPersistentParam.ExpireSolution(true);
                }
                else if (ighActiveObject.GetType().ToString() == "GTDrawingLink.Types.TeklaDrawingPointParam" || ighActiveObject.GetType().Name == "TeklaLineSegmentParam")
                {
                    var type = ighActiveObject.GetType();

                    var persistentData = type.GetProperty("PersistentData").GetValue(ighActiveObject);
                    var persistentDataType = persistentData.GetType();
                    persistentDataType.GetMethod("Clear").Invoke(persistentData, null);

                    var method = type.GetMethod("SetPersistentData", new Type[] { typeof(object[]) });
                    method.Invoke(ighActiveObject, new object[] { objectsToSet });
                    type.GetMethod("ExpireSolution").Invoke(ighActiveObject, new object[] { true });
                }
            }
        }

        private void SetValue<T>(GH_PersistentParam<T> param, object valueToSet) where T : class, IGH_Goo
        {
            param.RemoveAllSources();
            param.PersistentData.Clear();
            param.SetPersistentData(new object[] { valueToSet });
            param.ExpireSolution(true);
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
        public static void SetRhinoTolerance(double toleranceMm)
        {
            RhinoDoc.ActiveDoc.ModelAbsoluteTolerance = GetAbsoluteTolerance(toleranceMm, UnitSystem.Unset);
        }

        public static double GetAbsoluteTolerance(double toleranceMm, UnitSystem unitSystem = UnitSystem.Unset)
        {
            if (unitSystem == UnitSystem.Unset)
                unitSystem = RhinoDoc.ActiveDoc.ModelUnitSystem;

            switch (unitSystem)
            {
                case UnitSystem.Millimeters:
                    return toleranceMm / 1.0;
                case UnitSystem.Centimeters:
                    return toleranceMm / 10.0;
                case UnitSystem.Meters:
                    return toleranceMm / 1000.0;
                case UnitSystem.Inches:
                    return toleranceMm / 25.4;
                case UnitSystem.Feet:
                    return toleranceMm / 304.8;
                default:
                    return toleranceMm / 1.0;
            }
        }

        public static void SetRhinoAngleTolerance(double angleToleranceDegrees)
        {
            RhinoDoc.ActiveDoc.ModelAngleToleranceDegrees = angleToleranceDegrees;
        }

        private GHParams GetInputParams(GH_Document doc)
        {
            var activeObjects = GetValidParameters(doc);

            var groups = GetInputGroups(doc);
            if (groups.HasHidden())
                activeObjects = activeObjects.Where(p => !groups.ContainsHidden(p.InstanceGuid)).ToList();

            var modelParams = new List<TeklaModelParam>();
            var drawingParams = new List<TeklaDrawingParam>();
            var attributeParams = new List<ActiveObjectWrapper>();
            foreach (var activeObject in activeObjects)
            {
                var isMultiple = CheckIfMultiple(activeObject);
                if (activeObject is GH_PersistentParam<GH_Goo<Tekla.Structures.Model.ModelObject>> ghModelParam)
                {
                    modelParams.Add(TransformToModelParam(ghModelParam, isMultiple));
                    continue;
                }
                else if (activeObject is GH_PersistentParam<GH_Goo<Tekla.Structures.Drawing.DatabaseObject>> ghDrawingParam ||
                    activeObject.GetType().Name == "TeklaDrawingPointParam" || activeObject.GetType().Name == "TeklaLineSegmentParam")
                {
                    drawingParams.Add(TransformToDrawingParam(activeObject, isMultiple));
                    continue;
                }

                var tab = groups.FindTabName(activeObject.InstanceGuid);
                var group = groups.FindGroupName(activeObject.InstanceGuid);
                if (IsSettingsPanel(activeObject) && tab == null)
                    tab = "Settings";

                if (groups.WithoutTabsAndGroups() || tab != null || group != null)
                {
                    var connectivity = (activeObject is IGH_Param param) ?
                        new ObjectConnectivity(param.SourceCount, param.Recipients.Count()) :
                        new ObjectConnectivity(0, 0);

                    attributeParams.Add(new ActiveObjectWrapper(activeObject, new TabAndGroup(tab, group), connectivity));
                }
            }

            return new GHParams(new TeklaParams(modelParams, drawingParams), attributeParams);
        }

        private bool CheckIfMultiple(IGH_ActiveObject activeObject)
        {
            var nickName = activeObject.NickName.Trim().ToUpperInvariant();
            return nickName.StartsWith("MULTIPLE") || nickName.StartsWith("M:");
        }

        private string GetUserFriendlyParmName(string input)
        {
            var name = input.Trim();
            if (name.ToUpperInvariant().StartsWith("M:"))
                name = name.Substring(2).Trim();

            return name;
        }

        private TeklaModelParam TransformToModelParam(GH_PersistentParam<GH_Goo<ModelObject>> ghModelParam, bool isMultiple)
        {
            var paramName = ghModelParam.GetType().Name;
            var prompt = $"Pick a {GetUserFriendlyParmName(ghModelParam.NickName)}";

            return paramName switch
            {
                "TeklaPointParam" => new TeklaModelParam(ghModelParam, ModelParamType.Point, isMultiple, prompt),
                "TeklaLineParam" => new TeklaModelParam(ghModelParam, ModelParamType.Line, isMultiple, prompt),
                "TeklaPolylineParam" => new TeklaModelParam(ghModelParam, ModelParamType.Polyline, isMultiple, prompt),
                "TeklaFaceParam" => new TeklaModelParam(ghModelParam, ModelParamType.Face, isMultiple, prompt),
                _ => new TeklaModelParam(ghModelParam, ModelParamType.Object, isMultiple, prompt),
            };
        }

        private TeklaDrawingParam TransformToDrawingParam(IGH_ActiveObject ghDrawingParam, bool isMultiple)
        {
            var paramName = ghDrawingParam.GetType().Name;
            var prompt = $"Pick a drawing {GetUserFriendlyParmName(ghDrawingParam.NickName)}";

            return paramName switch
            {
                "TeklaDrawingPointParam" => new TeklaDrawingParam(ghDrawingParam, DrawingParamType.Point, isMultiple, prompt),
                "TeklaLineSegmentParam" => new TeklaDrawingParam(ghDrawingParam, DrawingParamType.Line, isMultiple, prompt),
                _ => new TeklaDrawingParam(ghDrawingParam, DrawingParamType.Object, isMultiple, prompt),
            };
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
                var guids = group.ObjectsRecursive().Select(o => o.InstanceGuid);

                var nickName = group.NickName.Trim();
                var upperName = nickName.ToUpperInvariant();

                if (upperName.StartsWith("TAB:"))
                    gHGroups.AddTab(guids, nickName.Substring(4));
                else if (upperName.StartsWith("T:"))
                    gHGroups.AddTab(guids, nickName.Substring(2));
                else if (upperName.StartsWith("GROUP:"))
                    gHGroups.AddGroup(guids, nickName.Substring(6));
                else if (upperName.StartsWith("G:"))
                    gHGroups.AddGroup(guids, nickName.Substring(2));
                else if (upperName.StartsWith("HIDE") || upperName.StartsWith("HIDDEN"))
                    gHGroups.AddHidden(guids);
            }

            return gHGroups;
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

        private void AppendComponentErrorMessages(IGH_Component component, Dictionary<GH_RuntimeMessageLevel, List<string>> messages)
        {
            var toCheck = new GH_RuntimeMessageLevel[] { GH_RuntimeMessageLevel.Warning, GH_RuntimeMessageLevel.Error };

            foreach (var level in toCheck)
            {
                foreach (var message in component.RuntimeMessages(level))
                    messages[level].Add($"[ {component.NickName} component ] {message}");
            }
        }

        private void AppendOutputMessages(GH_Panel panel, Dictionary<GH_RuntimeMessageLevel, List<string>> messages)
        {
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

        private Dictionary<GH_RuntimeMessageLevel, List<string>> InitializeMessageDictionary()
        {
            var messages = new Dictionary<GH_RuntimeMessageLevel, List<string>>();
            foreach (GH_RuntimeMessageLevel key in Enum.GetValues(typeof(GH_RuntimeMessageLevel)))
            {
                if (!messages.ContainsKey(key) || messages[key] == null)
                    messages[key] = new List<string>();
            }

            return messages;
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

        internal void OpenGrasshopperDefinition(string definitionPath)
        {
            if (_grasshopperInstance == null || !_grasshopperInstance.IsEditorLoaded())
                _grasshopperInstance = (GH_RhinoScriptInterface)RhinoApp.GetPlugInObject(_ghPluginGuid);

            if (_grasshopperInstance == null)
            {
                System.Windows.Forms.MessageBox.Show("Rhino didn't return the Grasshopper plugin.", "Exception when launching Grasshopper");
                return;
            }

            _grasshopperInstance.DisableBanner();
            _grasshopperInstance.DisableSolver();
            _grasshopperInstance.OpenDocument(definitionPath);
            _grasshopperInstance.ShowEditor();
        }

        public void Dispose()
        {
            if (_grasshopperInstance != null)
            {
                if (_grasshopperInstance.IsEditorLoaded())
                    _grasshopperInstance.CloseAllDocuments();

                _grasshopperInstance = null;
            }

            _rhinoCore.Dispose();
            _rhinoCore = null;
            Application.Current.Shutdown();
        }
    }
}
