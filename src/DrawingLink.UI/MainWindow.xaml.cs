using DrawingLink.UI.GH;
using DrawingLink.UI.TeklaInteraction;
using Fusion;
using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Xml.Linq;
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

        public MainWindow(MainWindowViewModel viewModel)
        {
            _messageBoxWindow = new MessageBoxWindow();
            InitializeComponent();

            _viewModel = viewModel;
            InitializeDataStorage(_viewModel);
            parameterViewer.GhAttributeLoaded += ParameterViewer_SetAttributeValue;

            HideApplyButton();
        }

        private void HideApplyButton()
        {
            var type = this.teklaBottomBar.GetType();
            var field = type.GetField("applyButton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var button = (Button)field.GetValue(this.teklaBottomBar);
            button.Visibility = Visibility.Collapsed;
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

            var instance = GrasshopperCaller.GetInstance();
            var teklaParams = instance.GetInputParams(path).TeklaParams;

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

            var messages = instance.Solve(userFormData, teklaInput);

            var remarks = messages[GH_RuntimeMessageLevel.Remark];

            _messageBoxWindow.ClearMessages();

            _messageBoxWindow.ShowMessages(GetTitle(path), remarks);

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

            tbDefinitionPath.Text = RelativePathHelper.ShortenIfPossible(dialog.FileName);
            tbDefinitionPath
                .GetBindingExpression(TextBox.TextProperty)
                .UpdateSource();

            parameterViewer.ShowControls(dialog.FileName, true);
        }

        private void ReloadGrasshopperFile_Click(object sender, RoutedEventArgs e)
        {
            var path = GetFullPath(tbDefinitionPath.Text);
            parameterViewer.ShowControls(path, true);
        }

        private void OpenGrasshopperFile_Click(object sender, RoutedEventArgs e)
        {
            var path = GetFullPath(tbDefinitionPath.Text);

            var instance = GrasshopperCaller.GetInstance();
            instance.OpenGrasshopperDefinition(path);
        }

        private string GetFullPath(string path)
        {
            return RelativePathHelper.ExpandRelativePath(path);
        }
    }
}
