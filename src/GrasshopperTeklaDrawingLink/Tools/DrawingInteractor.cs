using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Tools
{
    internal class DrawingInteractor
    {
        internal static DrawingHandler DrawingHandler { get; private set; }

        static DrawingInteractor()
        {
            var modelStatus = new Tekla.Structures.Model.Model().GetConnectionStatus();
            DrawingHandler = new DrawingHandler();
            var drawingStatus = DrawingHandler.GetConnectionStatus();
        }

        internal static bool IsConnected()
        {
            return DrawingHandler.GetConnectionStatus();
        }

        public static Drawing GetActiveDrawing() => DrawingHandler.GetActiveDrawing();
        public static bool CommitChanges() => GetActiveDrawing().CommitChanges();

        public static DrawingObject PickObject()
        {
            try
            {
                var picker = DrawingHandler.GetPicker();
                var result = picker.PickObject("Pick object");
                return result.Item1;
            }
            catch (ApplicationException)
            {
                return null;
            }
        }

        public static IList<DrawingObject> PickObjects()
        {
            var drawingObjects = new List<DrawingObject>();
            var picker = DrawingHandler.GetPicker();
            try
            {
                while (true)
                {
                    var result = picker.PickObject("Pick object");
                    drawingObjects.Add(result.Item1);
                }
            }
            catch (PickerInterruptedException)
            {
                return drawingObjects;
            }
            catch (ApplicationException)
            {
                return null;
            }
        }

        public static Point PickPoint()
        {
            try
            {
                var picker = DrawingHandler.GetPicker();
                var result = picker.PickPoint("Pick point");
                return result.Item1;
            }
            catch (ApplicationException)
            {
                return null;
            }
        }

        public static IList<Point> PickPoints()
        {
            var points = new List<Point>();
            var picker = DrawingHandler.GetPicker();
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
            catch (ApplicationException)
            {
                return null;
            }
        }

        public static void Highlight<T>(T drawingObject) where T : DrawingObject
        {
            if (drawingObject != null)
                Highlight(new ArrayList() { drawingObject });
        }

        public static void Highlight<T>(IEnumerable<T> objects) where T : DrawingObject
        {
            if (objects != null)
                Highlight(new ArrayList(objects.ToList()));
        }

        public static void Highlight(ArrayList objects)
        {
            var dos = DrawingHandler.GetDrawingObjectSelector();

            if (objects == null)
                dos.UnselectAllObjects();
            else
                dos.SelectObjects(objects, false);
        }

        public static void UnHighlight()
        {
            var dos = DrawingHandler.GetDrawingObjectSelector();
            dos.UnselectAllObjects();
        }
    }
}
