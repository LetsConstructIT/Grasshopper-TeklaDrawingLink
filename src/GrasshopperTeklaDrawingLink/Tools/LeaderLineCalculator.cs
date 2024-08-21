using Grasshopper.Kernel.Types;
using GTDrawingLink.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace GTDrawingLink.Tools
{
    public static class LeaderLineCalculator
    {
        public static GH_Line GuessLeaderLine(RectangleBoundingBox objectBoundingBox, PlacingBase placingBase, Point insertionPt, FrameTypes frameType, double angle)
        {
            if (placingBase is LeaderLinePlacing leaderLinePlacing)
            {
                var startPt = leaderLinePlacing.StartPoint;
                if (frameType == FrameTypes.Line)
                {
                    var endPt = insertionPt.X < leaderLinePlacing.StartPoint.X ? objectBoundingBox.LowerRight : objectBoundingBox.LowerLeft;
                    if (angle == 90)
                        endPt = insertionPt.Y < leaderLinePlacing.StartPoint.Y ? objectBoundingBox.LowerRight : objectBoundingBox.LowerLeft;

                    return new GH_Line(new Rhino.Geometry.Line(startPt.ToRhino(), endPt.ToRhino()));
                }
                else
                {
                    var pts = new List<Point> { objectBoundingBox.LowerRight, objectBoundingBox.UpperRight, objectBoundingBox.UpperLeft, objectBoundingBox.LowerLeft };
                    var endPt = pts.OrderBy(p => Distance.PointToPoint(p, startPt)).First();

                    return new GH_Line(new Rhino.Geometry.Line(startPt.ToRhino(), endPt.ToRhino()));
                }
            }
            else
                return new GH_Line();
        }

        public static GH_Curve GetCurve(LeaderLine leaderLine)
        {
            var points = new List<Point> { leaderLine.StartPoint };
            foreach (Point elbowPoint in leaderLine.ElbowPoints)
                points.Add(elbowPoint);

            points.Add(leaderLine.EndPoint);

            return new GH_Curve(new Rhino.Geometry.PolylineCurve(points.Select(p => p.ToRhino())));
        }
    }
}
