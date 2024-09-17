using System.Collections.Generic;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace DrawingLink.UI.TeklaInteraction
{
    internal class DrawingPicker
    {
        internal List<DatabaseObject> PickObject(bool isMultiple, string prompt)
        {
            var picker = new DrawingHandler().GetPicker();
            var drawingObjects = new List<DatabaseObject>();

            if (isMultiple)
            {
                try
                {
                    while (true)
                    {
                        var result = picker.PickObject(prompt);
                        drawingObjects.Add(result.Item1);
                    }
                }
                catch (PickerInterruptedException)
                {
                    return drawingObjects;
                }
            }
            else
            {
                var result = picker.PickObject(prompt);
                drawingObjects.Add(result.Item1);
            }

            return drawingObjects;
        }


        internal List<Point> PickPoint(bool isMultiple, string prompt)
        {
            var picker = new DrawingHandler().GetPicker();
            var points = new List<Point>();
            if (isMultiple)
            {
                try
                {
                    while (true)
                    {
                        var result = picker.PickPoint("Pick point");
                        points.Add(result.Item1);
                    }
                }
                catch (PickerInterruptedException)
                {
                    return points;
                }
            }
            {
                var result = picker.PickPoint("Pick point");
                points.Add(result.Item1);
            }

            return points;
        }
    }
}
