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
                        var result = picker.PickPoint(prompt);
                        points.Add(result.Item1);
                    }
                }
                catch (PickerInterruptedException)
                {
                    return points;
                }
            }
            else
            {
                var result = picker.PickPoint(prompt);
                points.Add(result.Item1);
            }

            return points;
        }

        internal List<LineSegment> PickLine(bool isMultiple, string prompt)
        {
            var picker = new DrawingHandler().GetPicker();
            var segments = new List<LineSegment>();
            if (isMultiple)
            {
                try
                {
                    while (true)
                    {
                        picker.PickTwoPoints(prompt, "Pick 2nd point", out Point firstPt, out Point secondPt, out ViewBase view);
                        segments.Add(new LineSegment(firstPt, secondPt));
                    }
                }
                catch (PickerInterruptedException)
                {
                    return segments;
                }
            }
            else
            {
                picker.PickTwoPoints(prompt, "Pick 2nd point", out Point firstPt, out Point secondPt, out ViewBase view);
                segments.Add(new LineSegment(firstPt, secondPt));
            }

            return segments;
        }
    }
}
