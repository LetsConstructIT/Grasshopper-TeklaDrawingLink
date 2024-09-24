using DrawingLink.UI.GH;
using DrawingLink.UI.Rhino;
using DrawingLink.UI.TeklaInteraction;
using Grasshopper.Kernel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Tekla.Structures.Dialog;

namespace DrawingLink.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ApplicationWindowBase
    {
        private readonly MainWindowViewModel _viewModel;
        private readonly MessageBoxWindow _messageBoxWindow;
        private GrasshopperCaller _instance;
        private bool _loaded;

        public MainWindow(MainWindowViewModel viewModel, List<RhinoInfo> rhinoVersions)
        {
            _messageBoxWindow = new MessageBoxWindow();
            InitializeComponent();

            _viewModel = viewModel;
            InitializeDataStorage(_viewModel);
            parameterViewer.GhAttributeLoaded += ParameterViewer_SetAttributeValue;

            AdjustUI();
            PopulateRhinoVersions(rhinoVersions);

            ShowInTaskbar = true;
        }

        private void PopulateRhinoVersions(List<RhinoInfo> rhinoVersions)
        {
            this.cmbRhinoVersion.Items.Clear();
            foreach (var rhinoVersion in rhinoVersions)
                this.cmbRhinoVersion.Items.Add(rhinoVersion.Version);

            var neededVersion = rhinoVersions.FirstOrDefault(v => v.Version == Properties.Settings.Default.RhinoVersion);
            if (neededVersion != null)
            {
                var index = rhinoVersions.IndexOf(neededVersion);
                this.cmbRhinoVersion.SelectedIndex = index;
            }

            this.cmbRhinoVersion.SelectionChanged += cmbRhinoVersion_SelectionChanged;
        }

        private void cmbRhinoVersion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.cmbRhinoVersion.SelectedIndex == -1)
                return;

            Properties.Settings.Default.RhinoVersion = (int)this.cmbRhinoVersion.SelectedItem;
            Properties.Settings.Default.Save();

            MessageBox.Show(this, "Restart application to load new Rhino version");
        }

        private void ApplicationWindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            _instance = GrasshopperCaller.GetInstance();
            _loaded = true;

            this.tbLaunchingRhino.Text = "Waiting for definition file ...";
        }

        private void AdjustUI()
        {
            this.tbLaunchingRhino.Text = $"Launching Rhino {Properties.Settings.Default.RhinoVersion} ...";

            var type = this.teklaBottomBar.GetType();
            var applyField = type.GetField("applyButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var applyButton = (Button)applyField.GetValue(this.teklaBottomBar);
            applyButton.Visibility = Visibility.Collapsed;

            var createField = type.GetField("createButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var createButton = (Button)createField.GetValue(this.teklaBottomBar);
            createButton.Content = "Run";
        }

        private void ParameterViewer_SetAttributeValue(object sender, EventArgs e)
        {
            if (e is not SetAttributeEventArgs args)
                return;

            var property = _viewModel.GetType().GetProperty(args.AttributeName);
            property.SetValue(_viewModel, args.Value);
        }

        private void WpfOkCreateCancel_CreateClicked(object sender, EventArgs e)
        {
            var path = GetFullPath(_viewModel.DefinitionPath);

            var userFormData = _viewModel.ToDataModel();

            var teklaParams = _instance.GetInputParams(path).TeklaParams;

            var userPicker = new UserInputPicker();
            if (!userPicker.CanObjectsBePicked(teklaParams, out string warningMessage))
            {
                _messageBoxWindow.ShowMessages(GetTitle(path), warningMessage);
                return;
            }

            var teklaInput = new Dictionary<string, TeklaObjects>();

            try
            {
                teklaInput = userPicker.PickInput(teklaParams);
            }
            catch (Exception exception)
            {
                _messageBoxWindow.ShowMessages(GetTitle(path), exception.Message);
                return;
            }

            var messages = new Dictionary<GH_RuntimeMessageLevel, List<string>>();
            try
            {
                messages = _instance.Solve(userFormData, teklaInput);
            }
            catch (Exception ex)
            {
                messages.Add(GH_RuntimeMessageLevel.Error, new List<string> { ex.Message });
            }

            _messageBoxWindow.ClearMessages();

            _messageBoxWindow.ShowMessages(GetTitle(path), messages);

            if (teklaParams.ModelParams.Count > 0)
                new Tekla.Structures.Model.Model().CommitChanges();
        }

        private string GetTitle(string definitionPath)
        {
            return File.Exists(definitionPath) ? ("[GH Drawing] Messages from the " + Path.GetFileNameWithoutExtension(definitionPath) + " definition") : "[GH Drawing] [No definition loaded]";
        }

        private void WpfOkCreateCancel_ApplyClicked(object sender, EventArgs e)
        {

        }

        private void SelectGrasshopperFile_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".gh",
                Filter = "Grasshopper file (.gh)|*.gh",
            };

            if (File.Exists(_viewModel.DefinitionPath))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(_viewModel.DefinitionPath);
            }

            if (dialog.ShowDialog() != true)
                return;

            HideLoadingMessage();

            tbDefinitionPath.Text = RelativePathHelper.ShortenIfPossible(dialog.FileName);
            tbDefinitionPath
                .GetBindingExpression(TextBox.TextProperty)
                .UpdateSource();

            DisplayScriptName(tbDefinitionPath.Text);
            ShowRightPortionOfTextBox(tbDefinitionPath);

            parameterViewer.ShowControls(_instance, dialog.FileName, true);
        }

        private void DisplayScriptName(string path)
        {
            if (string.IsNullOrEmpty(tbDefinitionPath.Text))
                return;

            Title = Path.GetFileNameWithoutExtension(path);
        }

        private void ShowRightPortionOfTextBox(TextBox tbDefinitionPath)
        {
            if (string.IsNullOrEmpty(tbDefinitionPath.Text))
                return;

            tbDefinitionPath.Focus();
            tbDefinitionPath.Select(tbDefinitionPath.Text.Length, 0);
        }

        private void ReloadGrasshopperFile_Click(object sender, RoutedEventArgs e)
        {
            var path = GetFullPath(tbDefinitionPath.Text);
            if (string.IsNullOrEmpty(path))
            {
                DisplayWarning("Path to Grasshopper file is empty.");
                return;
            }

            parameterViewer.ShowControls(_instance, path, true);
        }

        private void DisplayWarning(string message)
        {
            MessageBox.Show(this, message, "Warning");
        }

        private void OpenGrasshopperFile_Click(object sender, RoutedEventArgs e)
        {
            var path = GetFullPath(tbDefinitionPath.Text);
            if (string.IsNullOrEmpty(path))
            {
                DisplayWarning("Path to Grasshopper file is empty.");
                return;
            }

            _instance.OpenGrasshopperDefinition(path);
        }

        private string GetFullPath(string path)
        {
            return RelativePathHelper.ExpandRelativePath(path);
        }

        private void ApplicationWindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _instance.Dispose();
        }

        private void WpfSaveLoad_AttributesLoaded(object sender, EventArgs e)
        {
            if (!_loaded)
                return;

            var path = GetFullPath(tbDefinitionPath.Text);
            if (string.IsNullOrEmpty(path))
            {
                HideLoadingMessage();
                return;
            }

            parameterViewer.ShowControls(_instance, path, false);

            HideLoadingMessage();
        }

        private void HideLoadingMessage()
        {
            gridLoading.Visibility = Visibility.Collapsed;
        }
    }
}
