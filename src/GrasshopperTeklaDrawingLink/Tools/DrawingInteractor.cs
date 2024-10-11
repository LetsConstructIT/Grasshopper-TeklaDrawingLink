using GTDrawingLink.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace GTDrawingLink.Tools
{
    internal class DrawingInteractor
    {
        internal static DrawingHandler DrawingHandler { get; private set; }

        static DrawingInteractor()
        {
            var modelStatus = new TSM.Model().GetConnectionStatus();
            DrawingHandler = new DrawingHandler();
            var drawingStatus = DrawingHandler.GetConnectionStatus();
        }

        internal static bool IsConnected()
        {
            return DrawingHandler.GetConnectionStatus();
        }

        public static Drawing GetActiveDrawing() => DrawingHandler.GetActiveDrawing();
        public static bool CommitChanges()
        {
            if (Tekla.Structures.DrawingInternal.Operation.GetEditMode() != Tekla.Structures.DrawingInternal.EditMode.DrawingEditMode)
                return false;

            return GetActiveDrawing().CommitChanges();
        }

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

        public static LineSegment PickLine()
        {
            try
            {
                var picker = DrawingHandler.GetPicker();
                picker.PickTwoPoints("Pick 1st point", "Pick 2nd point", out Point firstPt, out Point secondPt, out ViewBase view);
                return new LineSegment(firstPt, secondPt);
            }
            catch (ApplicationException)
            {
                return null;
            }
        }

        public static IList<LineSegment> PickLines()
        {
            var lines = new List<LineSegment>();
            var picker = DrawingHandler.GetPicker();
            try
            {
                while (true)
                {
                    picker.PickTwoPoints("Pick 1st point", "Pick 2nd point", out Point firstPt, out Point secondPt, out ViewBase view);
                    lines.Add(new LineSegment(firstPt, secondPt));
                }
            }
            catch (PickerInterruptedException)
            {
                return lines;
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
            if (!IsAnyDrawingOpened())
                return;

            var dos = DrawingHandler.GetDrawingObjectSelector();

            if (objects == null)
                dos.UnselectAllObjects();
            else
                dos.SelectObjects(objects, false);
        }

        public static void UnHighlight()
        {
            if (!IsAnyDrawingOpened())
                return;

            var dos = DrawingHandler.GetDrawingObjectSelector();
            dos.UnselectAllObjects();
        }

        private static bool IsAnyDrawingOpened()
        {
            if (!DrawingHandler.GetConnectionStatus())
                return false;

            return DrawingHandler.GetActiveDrawing() != null;
        }

        internal static bool DeleteObjects(IEnumerable<DrawingObject> drawingObjects)
        {
            if (!drawingObjects.Any() || !IsInTheActiveDrawing(drawingObjects.First()))
                return false;

            if (HasAnyTrueMarks(drawingObjects))
            {
                Highlight(drawingObjects);

                var macroContent = Macros.DeleteSelection();
                var macroPath = new LightweightMacroBuilder()
                            .SaveMacroAndReturnRelativePath(macroContent);

                Tekla.Structures.Model.Operations.Operation.RunMacro(macroPath);

                return true;
            }
            else
            {
                var statuses = drawingObjects.Select(d => d.Delete()).ToList();
                return statuses.All(s => s);
            }
        }

        private static bool HasAnyTrueMarks(IEnumerable<DrawingObject> drawingObjects)
            => drawingObjects.Any(o => o is Mark && !(o as Mark).IsAssociativeNote);

        public static bool IsInTheActiveDrawing(DrawingObject drawingObject)
        {
            if (drawingObject is null)
                return false;

            var sourceId = drawingObject.GetDrawingIdentifier();
            var currentId = GetActiveDrawing().GetId();

            return sourceId == currentId;
        }

        public static bool IsTheActiveDrawing(Drawing drawing)
        {
            var sourceId = drawing.GetId();
            var currentId = GetActiveDrawing().GetId();

            return sourceId == currentId;
        }

        internal static bool CanDrawingBeOpened(Drawing drawing, out string message)
        {
            if (drawing is null)
            {
                message = "Provided drawing is empty";
                return false;
            }

            drawing.Select();
            if (drawing.IsLocked)
            {
                message = "Cannot edit a locked drawing";
                return false;
            }

            var identifier = (Tekla.Structures.Identifier)drawing.GetPropertyValue("ModelObjectIdentifier");
            if (identifier.ID > 0)
            {
                var modelObject = new TSM.Model().SelectModelObject(identifier);
                if (modelObject != null)
                {
                    if (!TSM.Operations.Operation.IsNumberingUpToDate(modelObject))
                    {
                        message = "The position numbers of modified objects need to be updated";
                        return false;
                    }
                }
            }

            message = string.Empty;
            return true;
        }
    }
}
