using DrawingLink.UI.GH;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Navigation;
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

        public event EventHandler GhAttributeLoaded;

        private void OnGhAttributeLoaded(SetAttributeEventArgs e)
        {
            GhAttributeLoaded?.Invoke(this, e);
        }

        public void ShowControls(GrasshopperCaller ghCaller, string filePath, bool loadValuesFromGh)
        {
            var parameters = ghCaller.GetInputParams(filePath);
            var uiDefinition = ParameterTransformer.Transform(parameters.AttributeParams);
            PopulateUi(uiDefinition, loadValuesFromGh);
        }

        private void PopulateUi(ParametersRoot sample, bool loadValuesFromGh)
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
                        stackPanel.Children.Add(PopulateGroup(group, loadValuesFromGh));
                }
            }

            UpdateLayout();

            if (tabControl.Items.Count > 0)
                tabControl.SelectedIndex = 0;
        }

        private UIElement PopulateGroup(UiGroup group, bool loadValuesFromGh)
        {
            var groupBox = new GroupBox() { Header = group.Name };
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

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
                    InsertEditableParam(persistableParam, grid, rowIdx, loadValuesFromGh);
                    continue;
                }
            }

            return groupBox;
        }

        private void InsertEditableParam(PersistableParam param, Grid grid, int rowIdx, bool loadValuesFromGh)
        {
            var colIdx = 1;
            SetCell(grid, CreateAttributeCheckBox(param.Name, param.FieldName), rowIdx, 0);

            switch (param)
            {
                case TextParam textParam:
                    var tb = new TextBox() { Text = textParam.Value, MinWidth = 110 };
                    SetCell(grid, tb, rowIdx, colIdx);

                    if (param.HasValidFieldName())
                    {
                        tb.SetBinding(TextBox.TextProperty, new Binding(param.FieldName));
                        if (loadValuesFromGh)
                            OnGhAttributeLoaded(new SetAttributeEventArgs(param.FieldName, textParam.Value));
                    }
                    break;
                case SliderParam sliderParam:
                    var dockPanel = new DockPanel() { LastChildFill = true };
                    var slider = new Slider()
                    {
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

                    if (loadValuesFromGh)
                        OnGhAttributeLoaded(new SetAttributeEventArgs(param.FieldName, sliderParam.Value));

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
                    if (loadValuesFromGh)
                        OnGhAttributeLoaded(new SetAttributeEventArgs(param.FieldName, listParam.SelectedItem));

                    SetCell(grid, combobox, rowIdx, colIdx);
                    break;
                case CatalogParamData catalogParam:
                    var stackPanel = new StackPanel() { Orientation = Orientation.Horizontal };

                    var textb = new TextBox { Text = catalogParam.Value, MinWidth = 110 };
                    textb.SetBinding(TextBox.TextProperty, new Binding(param.FieldName));
                    if (loadValuesFromGh)
                        OnGhAttributeLoaded(new SetAttributeEventArgs(param.FieldName, catalogParam.Value));

                    var button = new Button { Content = "..." };
                    button.Click += delegate
                    {
                        var value = catalogParam.PickFromCatalog(textb.Text);
                        OnGhAttributeLoaded(new SetAttributeEventArgs(param.FieldName, value));
                    };

                    stackPanel.Children.Add(textb);
                    stackPanel.Children.Add(button);

                    SetCell(grid, stackPanel, rowIdx, colIdx);
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
                if (TryGetCorrectUri(part, out Uri validUri))
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

            bool TryGetCorrectUri(string phrase, out Uri uri)
            {
                var allowedSchemas = new string[] { "file", "https" };
                uri = null;
                if (Uri.TryCreate(phrase, UriKind.Absolute, out Uri parsedUri) && allowedSchemas.Any(s => s == parsedUri.Scheme))
                {
                    uri = parsedUri;
                    return true;
                }
                else if (phrase.StartsWith("www."))
                {
                    if (Uri.TryCreate($"https://{phrase}", UriKind.Absolute, out Uri urlUri))
                    {
                        uri = urlUri;
                        return true;
                    }
                }

                return false;
            }
        }

        private void DisplayImage(ImageParam imageParam, Grid grid, int rowIdx)
        {
            var image = new Image()
            {
                Source = imageParam.Value.ToBitmapImage(),
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

    public class SetAttributeEventArgs : EventArgs
    {
        public string AttributeName { get; set; }
        public object Value { get; set; }

        public SetAttributeEventArgs(string attributeName, object value)
        {
            AttributeName = attributeName ?? throw new ArgumentNullException(nameof(attributeName));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static SetAttributeEventArgs EmptyString(string attributeName)
            => new(attributeName, string.Empty);
        public static SetAttributeEventArgs EmptyInt(string attributeName)
            => new(attributeName, int.MinValue);
        public static SetAttributeEventArgs EmptyDouble(string attributeName)
            => new(attributeName, double.MinValue);
    }
}
