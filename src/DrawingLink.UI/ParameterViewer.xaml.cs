using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tekla.Structures.Dialog.UIControls;

namespace DrawingLink.UI
{
    /// <summary>
    /// Interaction logic for ParameterViewer.xaml
    /// </summary>
    public partial class ParameterViewer : UserControl
    {
        public ParameterViewer()
        {
            InitializeComponent();
        }

        internal void Load()
        {
            var sample = UiPopulator.GetSample();
            PopulateUi(sample);
        }
        private void PopulateUi(UiRoot sample)
        {
            mainPanel.Children.Clear();

            var tabControl = new TabControl()
            {
                Margin = new Thickness(5),
            };
            mainPanel.Children.Add(tabControl);

            foreach (var tab in sample.Tabs)
            {
                var tabItem = new TabItem() { Header = tab.Name };
                tabControl.Items.Add(tabItem);

                var stackPanel = new StackPanel()
                {
                    Margin = new Thickness(5),
                };
                tabItem.Content = stackPanel;

                foreach (var group in tab.Groups)
                {
                    if (group.Params.Any())
                        stackPanel.Children.Add(PopulateGroup(group));
                }
            }

            UpdateLayout();

            if (tabControl.Items.Count > 0)
                tabControl.SelectedIndex = 0;
        }

        private UIElement PopulateGroup(UiGroup group)
        {
            var groupBox = new GroupBox() { Header = group.Name };
            var grid = new Grid();
            groupBox.Content = grid;

            foreach (var param in group.Params)
            {
                var rowIdx = AddRow(grid);
                if (param is ImageParam imageParam)
                {
                    DisplayImage(imageParam, grid, rowIdx);
                    continue;
                }
                else if (param is InfoParam infoParam)
                {
                    DisplayInfo(infoParam, grid, rowIdx);
                    continue;
                }
                else if (param is PersistableParam persistableParam)
                {
                    InsertEditableParam(persistableParam, grid, rowIdx);
                    continue;
                }
            }

            return groupBox;
        }

        private void InsertEditableParam(PersistableParam param, Grid grid, int rowIdx)
        {
            var colIdx = 1;
            SetCell(grid, CreateAttributeCheckBox(param.Name, param.FieldName), rowIdx, 0);

            switch (param)
            {
                case TextParam textParam:
                    var tb = new TextBox() { Text = textParam.Value, MinWidth = 110 };
                    SetCell(grid, tb, rowIdx, colIdx);

                    tb.SetBinding(TextBox.TextProperty, new Binding(param.FieldName));
                    break;
                case SliderParam sliderParam:
                    var dockPanel = new DockPanel();
                    var slider = new Slider()
                    {
                        Name = "testAbc",
                        Minimum = sliderParam.Minimum,
                        Maximum = sliderParam.Maximum,
                        Value = sliderParam.Value,
                        SmallChange = sliderParam.GetSmallChange(),
                        LargeChange = sliderParam.GetLargeChange(),
                        IsSnapToTickEnabled = true
                    };

                    slider.SetBinding(Slider.ValueProperty, new Binding(param.FieldName));

                    var textBox = new TextBox() { MinWidth = 80, Text = slider.Value.ToString() };
                    textBox.TextChanged += (sender, e) =>
                    {
                        if (double.TryParse(textBox.Text, out double result))
                            slider.Value = result;
                        else
                            textBox.Text = "Wrong input";
                    };
                    slider.ValueChanged += (sender, e) =>
                    {
                        textBox.Text = slider.Value.ToString();
                    };

                    dockPanel.Children.Add(textBox);
                    dockPanel.Children.Add(slider);
                    SetCell(grid, dockPanel, rowIdx, colIdx);
                    break;
                case ListParamData listParam:
                    var combobox = new ComboBox
                    {
                        ItemsSource = listParam.Items,
                        SelectedItem = listParam.SelectedItem
                    };
                    combobox.SetBinding(ComboBox.SelectedItemProperty, new Binding(param.FieldName));

                    SetCell(grid, combobox, rowIdx, colIdx);
                    break;
                default:
                    break;
            }
        }

        private void DisplayInfo(InfoParam infoParam, Grid grid, int rowIdx)
        {
            var textBlock = new TextBlock()
            {
                Margin = new Thickness(0, 2, 0, 2),
                VerticalAlignment = VerticalAlignment.Center,
            };

            foreach (var part in infoParam.Value.Split(null))
            {
                if (Uri.TryCreate(part, UriKind.Absolute, out Uri validUri))
                {
                    var hyperlink = new Hyperlink()
                    {
                        NavigateUri = validUri
                    };
                    hyperlink.Inlines.Add(part);
                    hyperlink.RequestNavigate += Hyperlink_RequestNavigate;

                    textBlock.Inlines.Add(hyperlink);
                    textBlock.Inlines.Add(" ");
                }
                else
                    textBlock.Inlines.Add($"{part} ");
            }

            grid.Children.Add(textBlock);
            Grid.SetRow(textBlock, rowIdx);
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumnSpan(textBlock, 2);
        }

        private void DisplayImage(ImageParam imageParam, Grid grid, int rowIdx)
        {
            var image = new Image()
            {
                Source = imageParam.Value,
                Stretch = System.Windows.Media.Stretch.None,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(5)
            };

            grid.Children.Add(image);
            Grid.SetRow(image, rowIdx);
            Grid.SetColumn(image, 0);
            Grid.SetColumnSpan(image, 2);
        }

        private void SetCell(Grid grid, UIElement control, int rowIdx, int colIdx)
        {
            grid.Children.Add(control);
            Grid.SetRow(control, rowIdx);
            Grid.SetColumn(control, colIdx);
        }

        private WpfFilterCheckBox CreateAttributeCheckBox(string content, string fieldName)
        {
            return new WpfFilterCheckBox()
            {
                Content = content,
                VerticalAlignment = VerticalAlignment.Center,
                AttributeName = fieldName
            };
        }

        private int AddRow(Grid grid)
        {
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });

            return grid.RowDefinitions.Count - 1;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch (Exception ex)
            {
                ShowWarning($"Hyperlink could not be navigated: {ex.Message}");
            }

            e.Handled = true;
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
