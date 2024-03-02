using DrawingLink.UI.GH;
using Fusion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            InitializeDataStorage(_viewModel);
            parameterViewer.GhAttributeLoaded += ParameterViewer_SetAttributeValue;
        }

        private void ParameterViewer_SetAttributeValue(object sender, EventArgs e)
        {
            if (e is SetAttributeEventArgs args)
            {
                var property = _viewModel.GetType().GetProperty(args.AttributeName);
                property.SetValue(_viewModel, args.Value);
            }
        }

        private void WpfOkCreateCancel_CreateClicked(object sender, EventArgs e)
        {
            var instance = GrasshopperCaller.GetInstance();
            instance.Solve(_viewModel.DefinitionPath);

        }

        private void WpfOkCreateCancel_ApplyClicked(object sender, EventArgs e)
        {

        }

        private void SelectGrasshopperFile_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".gh",
                Filter = "Grasshopper file (.gh)|*.gh"
            };

            if (dialog.ShowDialog() != true)
                return;

            var fileName = dialog.FileName;

            tbDefinitionPath.Text = fileName;
            tbDefinitionPath
                .GetBindingExpression(TextBox.TextProperty)
                .UpdateSource();

            parameterViewer.ShowControls(fileName, true);
        }

        private void ReloadGrasshopperFile_Click(object sender, RoutedEventArgs e)
        {
            parameterViewer.ShowControls(tbDefinitionPath.Text, true);
        }
    }
}
