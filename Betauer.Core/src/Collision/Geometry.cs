using System;
using System.Collections.Generic;
using Betauer.Core.Collision.Spatial2D;
using Godot;

namespace Betauer.Core.Collision;

public static class Geometry {
    /// <summary>
    /// Returns true if the point px, py is inside a circle located at cx, cy with radius radius 
    /// </summary>
    /// <param name="px"></param>
    /// <param name="py"></param>
    /// <param name="cx"></param>
    /// <param name="cy"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static bool IsPointInCircle(float px, float py, float cx, float cy, float radius) {
        var dx = cx - px;
        var dy = cy - py;
        return dx * dx + dy * dy <= radius * radius;
    }
    
    /// <summary>
    /// Returns true if the point px, py is inside a rectangle located at rx, ry with width and height
    /// </summary>
    /// <param name="px"></param>
    /// <param name="py"></param>
    /// <param name="rx"></param>
    /// <param name="ry"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static bool IsPointInRectangle(float px, float py, float rx, float ry, float width, float height) {
        if (width == 0 || height == 0) { // No area, no intersection
            return false;
        }
        return px >= rx &&
               px <= rx + width &&
               py >= ry &&
               py <= ry + height;
    }

    public static bool IntersectCircles(float x1, float y1, float radius1, float x2, float y2, float radius2) {
        if (radius1 == 0 || radius2 == 0) { // No area, no intersection
            return false;
        }
        var distanceSquared = DistanceSquared(x1, y1, x2, y2);
        var radiiSum = radius1 + radius2;
        return distanceSquared <= radiiSum * radiiSum;
    }

    public static float Distance(float x1, float y1, float x2, float y2) {
        return Mathf.Sqrt(DistanceSquared(x1, y1, x2, y2));
    }

    public static float DistanceSquared(float x1, float y1, float x2, float y2) {
        var dx = x1 - x2;
        var dy = y1 - y2;
        var distanceSquared = dx * dx + dy * dy;
        return distanceSquared;
    }

    /// <summary>
    /// Returns the new start of a segment that starts at x0, y0 and ends at x1, y1 and is extended by extension
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="y0"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static Vector2 ExtendLineAtStart(float x0, float y0, float x1, float y1, float extension) {
        var length = Distance(x0, y0, x1, y1);
        var dx = (x1 - x0) / length;
        var dy = (y1 - y0) / length;

        return new Vector2(x0 - dx * extension, y0 - dy * extension);
    }

    /// <summary>
    /// Returns the new ned of a segment that starts at x0, y0 and ends at x1, y1 and is extended by extension
    /// </summary>
    /// <param name="x0"></param>
    /// <param name="y0"></param>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="extension"></param>
    /// <returns></returns>
    public static Vector2 ExtendLineAtEnd(float x0, float y0, float x1, float y1, float extension) {
        var length = Distance(x0, y0, x1, y1);
        var dx = (x1 - x0) / length;
        var dy = (y1 - y0) / length;

        return new Vector2(x1 + dx * extension, y1 + dy * extension);
    }

    public static bool IntersectRectangles(float x1, float y1, float width1, float height1, float x2, float y2, float width2, float height2) {
        if (width1 == 0 || height1 == 0 || width2 == 0 || height2 == 0) {// No area, no intersection
            return false;
        }
        return x1 + width1 >= x2 &&
               x1 <= x2 + width2 &&
               y1 + height1 >= y2 &&
               y1 <= y2 + height2;
    }

    public static bool IntersectCircleRectangle(float circleX, float circleY, float radius, float rectX, float rectY, float width, float height) {
        if (width == 0 || height == 0 || radius == 0) { // No area, no intersection
            return false;
        }
        var closestX = Math.Clamp(circleX, rectX, rectX + width);
        var closestY = Math.Clamp(circleY, rectY, rectY + height);
        var dx = circleX - closestX;
        var dy = circleY - closestY;
        return dx * dx + dy * dy <= radius * radius;
    }

    public static bool IsPointInRectangle(Vector2 point, Rectangle rectangle) {
        return IsPointInRectangle(point.X, point.Y, rectangle);
    }

    public static bool IsPointInRectangle(float px, float py, Rectangle rectangle) {
        return IsPointInRectangle(px, py, rectangle.X, rectangle.Y, rectangle.Size.X, rectangle.Size.Y);
    }

    public static bool Intersect(Rectangle r1, Rectangle r2) {
        return IntersectRectangles(
            r1.X, r1.Y, r1.Size.X, r1.Size.Y,
            r2.X, r2.Y, r2.Size.X, r2.Size.Y);
    }
    
    public static bool IsPointInEllipse(float px, float py, float rx, float ry) {
        return (px * px) / (rx * rx) + (py * py) / (ry * ry) <= 1f;
    }
    
    public static bool IsPointInEllipse(float px, float py, float rx, float ry, double rotation) {
        var cos = Mathf.Cos(rotation);
        var sin = Mathf.Sin(rotation);
        var tdx = cos * px + sin * py;
        var tdy = sin * px - cos * py;
        return (tdx * tdx) / (rx * rx) + (tdy * tdy) / (ry * ry) <= 1f;
    }

    public static (int, int) GetPointIntersectingCell(float px, float py, float cellSize) {
        var minCellX = (int)Mathf.Floor(px / cellSize);
        var minCellY = (int)Mathf.Floor(py / cellSize);
        return (minCellX, minCellY);
    }

    public static (int, int)[] GetRectangleIntersectingCells(float rx, float ry, float width, float height, float cellSize) {
        var minCellX = (int)Mathf.Floor(rx / cellSize);
        var maxCellX = (int)Mathf.Floor((rx + width) / cellSize);
        var minCellY = (int)Mathf.Floor(ry / cellSize);
        var maxCellY = (int)Mathf.Floor((ry + height) / cellSize);
        
        (int, int)[] cells = new (int, int)[(maxCellX - minCellX + 1) * (maxCellY - minCellY + 1)];
        var cellIndex = 0;
        
        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                cells[cellIndex++] = (x, y);
            }
        }
        return cells;
    }

    public static (int, int)[] GetCircleIntersectingCells(float cx, float cy, float radius, float cellSize) {
        var minCellX = (int)Mathf.Floor((cx - radius) / cellSize);
        var maxCellX = (int)Mathf.Floor((cx + radius) / cellSize);
        var minCellY = (int)Mathf.Floor((cy - radius) / cellSize);
        var maxCellY = (int)Mathf.Floor((cy + radius) / cellSize);

        (int, int)[] cells = new (int, int)[(maxCellX - minCellX + 1) * (maxCellY - minCellY + 1)];
        var cellIndex = 0;
        
        for (var x = minCellX; x <= maxCellX; x++) {
            for (var y = minCellY; y <= maxCellY; y++) {
                cells[cellIndex++] = (x, y);
            }
        }
        return cells;
    }
    
        /// <summary>
    /// Returns a value from 0 to 1 based on the (x, y) cords and a rectangle of width x height size.
    /// It will return 1 if the point matches the center of the rectangle (located at centerX and centerY, it could be at any place inside the rectangle) and
    /// it goes to 0 gradually when the coords touch the border of the rectangle.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float LerpRect2D(float width, float height, float centerX, float centerY, float x, float y) {
        var maxDistanceX = Math.Max(centerX, width - 1 - centerX);
        var maxDistanceY = Math.Max(centerY, height - 1 - centerY);
        var distanceX = Math.Abs(x - centerX);
        var distanceY = Math.Abs(y - centerY);
        var valueX = 1f - distanceX / maxDistanceX;
        var valueY = 1f - distanceY / maxDistanceY;
        return Math.Min(valueX, valueY);
    }

    /// <summary>
    /// Returns a value from 0 to 1 based on the (x, y) cords and a rectangle of width x height size.
    /// It will return 1 if the point matches the center of the rectangle (located at width/2 and height/2) and
    /// it goes to 0 gradually when the coords touch the border of the rectangle.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float LerpRect2D(float width, float height, float x, float y) {
        var centerX = width / 2;
        var centerY = height / 2;
        var distanceX = Math.Abs(x - centerX);
        var distanceY = Math.Abs(y - centerY);
        var valueX = 1f - distanceX / centerX;
        var valueY = 1f - distanceY / centerY;
        return Math.Min(valueX, valueY);
    }
    
    /// <summary>
    /// Returns a value from 0 to 1 based on the (x, y) coords and a ellipse of radius (rx, ry)
    /// It will return 1 when the coords requested are located at the center of the ellipse (0,0) and it goes
    /// to 0 gradually when the coords reach the border (radius rx, ry)
    /// </summary>
    /// <param name="rx"></param>
    /// <param name="ry"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float LerpEllipse(float rx, float ry, float x, float y) {
        return Mathf.Clamp(1 - ((x * x) / (rx * rx) + (y * y) / (ry * ry)), 0f, 1f);
    }

    /// <summary>
    /// Returns a value from 0 to 1 based on the (x, y) coords and a ellipse of radius (rx, ry) and a rotation of rotation radians.
    /// It will return 1 when the coords requested are located at the center of the ellipse (0,0) and it goes
    /// to 0 gradually when the coords reach the border (radius rx, ry)
    /// </summary>
    /// <param name="rx"></param>
    /// <param name="ry"></param>
    /// <param name="rotation"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float LerpEllipseRotated(float rx, float ry, float rotation, float x, float y) {
        var cos = Mathf.Cos(rotation);
        var sin = Mathf.Sin(rotation);
        var rotatedX = cos * x + sin * y;
        var rotatedY = -sin * x + cos * y;
        return LerpEllipse(rx, ry, rotatedX, rotatedY);
    }    

    /// <summary>
    /// Returns a value from 0 to 1 based on the (x, y) coords and a circle of radius (rx).
    /// It will return 1 when the coords requested are located at the center of the circle (0,0) and it goes
    /// to 0 gradually when the coords reach the border (radius r)
    /// </summary>
    /// <param name="r"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float LerpCircle(float r, float x, float y) {
        return Math.Clamp(1 - ((x * x + y * y) / (r * r)), 0f, 1f);
    }
    
    /// <summary>
    /// Connects all vectors in the grid using a Minimum Spanning Tree (MST) approach.
    /// </summary>
    public static List<(Vector2, Vector2)> GetConnections(IEnumerable<Vector2> points) {
        List<(Vector2, Vector2)> connections = new List<(Vector2, Vector2)>();
        HashSet<Vector2> connectedShapes = new HashSet<Vector2>();
        List<Vector2> unconnectedShapes = new List<Vector2>(points);

        // Start with the first shape
        connectedShapes.Add(unconnectedShapes[0]);
        unconnectedShapes.RemoveAt(0);

        // Continue until all shapes are connected using a Minimum Spanning Tree approach
        while (unconnectedShapes.Count > 0) {
            Vector2? shapeA = null;
            Vector2? shapeB = null;
            var shortestDistance = float.MaxValue;

            // Find the closest pair of shapes (one connected and one unconnected)
            foreach (Vector2 connected in connectedShapes) {
                foreach (Vector2 unconnected in unconnectedShapes) {
                    var distance = Math.Abs(connected.X - unconnected.X) + Math.Abs(connected.Y - unconnected.Y);

                    if (distance < shortestDistance) {
                        shortestDistance = distance;
                        shapeA = connected;
                        shapeB = unconnected;
                    }
                }
            }

            // Add the connection to the list
            if (shapeA != null && shapeB != null) {
                connections.Add((shapeA.Value, shapeB.Value));
                connectedShapes.Add(shapeB.Value);
                unconnectedShapes.Remove(shapeB.Value);
            }
        }

        return connections;
    }        

    /// <summary>
    /// Connects all vectors in the grid using a Minimum Spanning Tree (MST) approach.
    /// </summary>
    public static List<(Vector2I, Vector2I)> GetConnections(IEnumerable<Vector2I> points) {
        List<(Vector2I, Vector2I)> connections = new List<(Vector2I, Vector2I)>();
        HashSet<Vector2I> connectedShapes = new HashSet<Vector2I>();
        List<Vector2I> unconnectedShapes = new List<Vector2I>(points);

        // Start with the first shape
        connectedShapes.Add(unconnectedShapes[0]);
        unconnectedShapes.RemoveAt(0);

        // Continue until all shapes are connected using a Minimum Spanning Tree approach
        while (unconnectedShapes.Count > 0) {
            Vector2I? shapeA = null;
            Vector2I? shapeB = null;
            var shortestDistance = float.MaxValue;

            // Find the closest pair of shapes (one connected and one unconnected)
            foreach (Vector2I connected in connectedShapes) {
                foreach (Vector2I unconnected in unconnectedShapes) {
                    var distance = Math.Abs(connected.X - unconnected.X) + Math.Abs(connected.Y - unconnected.Y);

                    if (distance < shortestDistance) {
                        shortestDistance = distance;
                        shapeA = connected;
                        shapeB = unconnected;
                    }
                }
            }

            // Add the connection to the list
            if (shapeA != null && shapeB != null) {
                connections.Add((shapeA.Value, shapeB.Value));
                connectedShapes.Add(shapeB.Value);
                unconnectedShapes.Remove(shapeB.Value);
            }
        }
        return connections;
    }
    
    public static Rect2I ShrinkRect(Rect2I rect, int maxPadding, Random random) {
        return ShrinkRect(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, maxPadding, random);
    }
    
    public static Rect2I ShrinkRect(int x, int y, int width, int height, int maxPadding, Random random) {
        var maxAllowedPadding = Math.Min(maxPadding, Math.Min(width / 2-1, height / 2-1));

        var paddingLeft = random.Next(0, maxAllowedPadding + 1);
        var paddingTop = random.Next(0, maxAllowedPadding + 1);
        var paddingRight = random.Next(0, maxAllowedPadding + 1);
        var paddingBottom = random.Next(0, maxAllowedPadding + 1);

        var newX = x + paddingLeft;
        var newY = y + paddingTop;
        var newWidth = width - (paddingLeft + paddingRight);
        var newHeight = height - (paddingTop + paddingBottom);

        return new Rect2I(newX, newY, newWidth, newHeight);
    }

    public static Rect2I ShrinkRect(Rect2I rect, int by) {
        return ShrinkRect(rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y, by);
    }
    
    public static Rect2I ShrinkRectProportional(int x, int y, int width, int height, float factor) {
        if (factor < 0 || factor > 1) {
            throw new ArgumentOutOfRangeException(nameof(factor), "Factor must be between 0 and 1.");
        }
        var newWidth = Mathf.RoundToInt(width * factor);
        var newHeight = Mathf.RoundToInt(height * factor);
        var newX = Mathf.RoundToInt(x + (width - newWidth) / 2f);
        var newY = Mathf.RoundToInt(y + (height - newHeight) / 2f);
        return new Rect2I(newX, newY, newWidth, newHeight);
    }
    
    public static Rect2I ShrinkRect(int x, int y, int width, int height, int by) {
        var maxPadding = Math.Min(by, Math.Min(width / 2 - 1, height / 2 - 1));
        var newX = x + maxPadding;
        var newY = y + maxPadding;
        var newWidth = width - maxPadding * 2;
        var newHeight = height - maxPadding * 2;
        return new Rect2I(newX, newY, newWidth, newHeight);
    }

    public static void Main() {
        Console.WriteLine(ShrinkRect(0, 0, 10, 10, 0));
        Console.WriteLine(ShrinkRect(0, 0, 10, 10, 1));
        Console.WriteLine(ShrinkRect(0, 0, 10, 10, 2));
        Console.WriteLine(ShrinkRect(0, 0, 10, 10, 4));
        Console.WriteLine(ShrinkRect(0, 0, 10, 10, 5));
        Console.WriteLine(ShrinkRect(0, 0, 10, 10, 6));
        Console.WriteLine(ShrinkRect(0, 0, 10, 9, 0));
        Console.WriteLine(ShrinkRect(0, 0, 10, 9, 1));
        Console.WriteLine(ShrinkRect(0, 0, 10, 9, 2));
        Console.WriteLine(ShrinkRect(0, 0, 10, 9, 4));
        Console.WriteLine(ShrinkRect(0, 0, 10, 9, 5));
        Console.WriteLine(ShrinkRect(0, 0, 10, 9, 6));
    }
}