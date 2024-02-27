using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace DrawingLink.UI
{
    public static class UiPopulator
    {
        public static UiRoot GetSample()
        {
            var stringCounter = 0;
            var doubleCounter = 0;
            var intCounter = 0;

            var root = new UiRoot();

            var modelTab = new UiTab("Model");
            var geoGroup = new UiGroup("Geometry");
            geoGroup.AddParam(new InfoParam("Hi, let's see what we can do. This is the simplest info param"));
            geoGroup.AddParam(new InfoParam("https://www.google.com"));
            geoGroup.AddParam(new InfoParam("Sample file:///C:\\temp\\ReadMe.txt"));
            geoGroup.AddParam(new TextParam($"int_{intCounter++}", "Integer param", "10"));
            geoGroup.AddParam(new SliderParam($"double_{doubleCounter++}", "Slider", 0, 500, 20, 2));

            var bitmapImage = new BitmapImage(new Uri(@"C:\Users\grzeg\Downloads\tekla.png"));
            geoGroup.AddParam(new ImageParam(bitmapImage));
            modelTab.AddGroup(geoGroup);

            var secondGroup = new UiGroup("Second");
            secondGroup.AddParam(new TextParam($"string_{stringCounter++}", "Text param", "Default value"));
            secondGroup.AddParam(new ListParamData($"string_{stringCounter++}", "Toggle", new string[] { "False", "True" }, "False"));
            modelTab.AddGroup(secondGroup);
            root.AddTab(modelTab);

            var drawingTab = new UiTab("Drawing");
            var drawingGroup = new UiGroup("Main");
            drawingGroup.AddParam(new TextParam($"string_{stringCounter++}", "Text param", "Default value"));
            drawingTab.AddGroup(drawingGroup);
            root.AddTab(drawingTab);

            return root;
        }
    }

    public class UiRoot
    {
        private readonly List<UiTab> _tabs;
        public IReadOnlyList<UiTab> Tabs => _tabs;

        public UiRoot()
        {
            _tabs = new List<UiTab>();
        }

        public void AddTab(UiTab tab)
        {
            _tabs.Add(tab);
        }
    }

    public class UiTab
    {
        private readonly List<UiGroup> _groups;
        public IReadOnlyList<UiGroup> Groups => _groups;

        public string Name { get; }

        public UiTab(string name)
        {
            _groups = new List<UiGroup>();
            Name = name;
        }

        public void AddGroup(UiGroup tab)
        {
            _groups.Add(tab);
        }
    }

    public class UiGroup
    {
        private readonly List<Param> _params;
        public IReadOnlyList<Param> Params => _params;

        public string Name { get; }

        public UiGroup(string name)
        {
            _params = new List<Param>();
            Name = name;
        }

        public void AddParam(Param param)
        {
            _params.Add(param);
        }
    }

    public abstract class Param
    {
        public string Name { get; }

        protected Param(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }

    public abstract class PersistableParam : Param
    {
        public string FieldName { get; }

        protected PersistableParam(string fieldName, string name) : base(name)
        {
            FieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        }
    }

    public class InfoParam : Param
    {
        public string Value { get; }

        public InfoParam(string value) : base(string.Empty)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public class ImageParam : Param
    {
        public BitmapImage Value { get; }

        public ImageParam(BitmapImage value) : base(string.Empty)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public class TextParam : PersistableParam
    {
        public string Value { get; private set; }

        public TextParam(string fieldName, string name, string value) : base(fieldName, name)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public void SetValue(string value)
        {
            Value = value;
        }
    }

    public class SliderParam : PersistableParam
    {
        public double Minimum { get; }
        public double Maximum { get; }
        public double Value { get; }
        public int DecimalPlaces { get; }

        public SliderParam(string fieldName, string name, double minValue, double maxValue, double value, int decimalPlaces) : base(fieldName, name)
        {
            Minimum = minValue;
            Maximum = maxValue;
            Value = value;
            DecimalPlaces = decimalPlaces;
        }

        public double GetSmallChange()
            => Math.Pow(10.0, DecimalPlaces);

        public double GetLargeChange()
            => 0.01 * Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(Maximum - Minimum) / 0.9)));
    }

    public class ListParamData : PersistableParam
    {
        public IReadOnlyList<string> Items { get; }
        public string SelectedItem { get; }

        public ListParamData(string fieldName, string name, IEnumerable<string> items, string selectedItem) : base(fieldName, name)
        {
            Items = items.ToArray();
            SelectedItem = selectedItem;
        }
    }
}
