using System.Drawing;

namespace DrawingLink.UI.GH.Models
{
    public static class UiTestPopulator
    {
        public static ParametersRoot GetSample()
        {
            var stringCounter = 0;
            var doubleCounter = 0;
            var intCounter = 0;

            var root = new ParametersRoot();

            var modelTab = new UiTab("Model");
            var geoGroup = new UiGroup("Geometry");
            geoGroup.AddParam(new InfoParam("Hi, let's see what we can do. This is the simplest info param", 0));
            geoGroup.AddParam(new InfoParam("https://www.google.com", 0));
            geoGroup.AddParam(new InfoParam("Sample file:///C:\\temp\\ReadMe.txt", 2));
            geoGroup.AddParam(new TextParam($"int_{intCounter++}", "Integer param", "10", 3));
            geoGroup.AddParam(new SliderParam($"double_{doubleCounter++}", "Slider", 0, 500, 20, 2, 4));

            var bitmapImage = (Bitmap)Image.FromFile(@"C:\Users\grzeg\Downloads\tekla.png");
            geoGroup.AddParam(new ImageParam(bitmapImage, 5));
            modelTab.AddGroup(geoGroup);

            var secondGroup = new UiGroup("Second");
            secondGroup.AddParam(new TextParam($"string_{stringCounter++}", "Text param", "Default value", 6));
            secondGroup.AddParam(new ListParamData($"string_{stringCounter++}", "Toggle", new string[] { "False", "True" }, "False", 7));
            modelTab.AddGroup(secondGroup);
            root.AddTab(modelTab);

            var drawingTab = new UiTab("Drawing");
            var drawingGroup = new UiGroup("Main");
            drawingGroup.AddParam(new TextParam($"string_{stringCounter++}", "Text param", "Default value", 8));
            drawingTab.AddGroup(drawingGroup);
            root.AddTab(drawingTab);

            return root;
        }
    }
}
