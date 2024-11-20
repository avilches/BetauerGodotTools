using Godot;

namespace Betauer.Core.DataMath.Geometry;

public static partial class Geometry {
    /// <summary>
    /// Returns true if the point px, py is inside a circle located at cx, cy with radius radius 
    /// </summary>
    public static bool IsPointInCircle(Vector2 point, Vector2 center, float radius) {
        return IsPointInCircle(point.X, point.Y, center.X, center.Y, radius);
    }

    public static bool IsPointInCircle(float px, float py, float cx, float cy, float radius) {
        var dx = cx - px;
        var dy = cy - py;
        return dx * dx + dy * dy <= radius * radius;
    }

    /// <summary>
    /// Returns true if the point px, py is inside a rectangle located at rx, ry with width and height
    /// </summary>
    public static bool IsPointInRectangle(Vector2 point, Rect2 rect) {
        return IsPointInRectangle(point.X, point.Y, rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);
    }

    public static bool IsPointInRectangle(Vector2I point, Rect2I rect) {
        return IsPointInRectangle(point.X, point.Y, rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);
    }

    public static bool IsPointInRectangle(int px, int py, int rx, int ry, int width, int height) {
        if (width == 0 || height == 0) { // No area, no intersection
            return false;
        }
        return px >= rx &&
               px < rx + width &&
               py >= ry &&
               py < ry + height;
    }

    public static bool IsPointInRectangle(float px, float py, float rx, float ry, float width, float height) {
        if (width == 0 || height == 0) { // No area, no intersection
            return false;
        }
        return px >= rx &&
               px <= rx + width &&
               py >= ry &&
               py <= ry + height;
    }

    /// <summary>
    /// Returns true if the point px, py is inside a rectangle located at rx, ry with width and height
    /// </summary>
    public static bool IntersectCircles(Vector2 center1, float radius1, Vector2 center2, float radius2) {
        return IntersectCircles(center1.X, center1.Y, radius1, center2.X, center2.Y, radius2);
    }

    /// <summary>
    /// Returns true if the point px, py is inside a rectangle located at rx, ry with width and height
    /// </summary>
    public static bool IntersectCircles(float x1, float y1, float radius1, float x2, float y2, float radius2) {
        if (radius1 == 0 || radius2 == 0) { // No area, no intersection
            return false;
        }
        var distanceSquared = DistanceSquared(x1, y1, x2, y2);
        var radiiSum = radius1 + radius2;
        return distanceSquared <= radiiSum * radiiSum;
    }

    public static bool IntersectRectangles(Rect2 rect1, Rect2 rect2) {
        return IntersectRectangles(rect1.Position.X, rect1.Position.Y, rect1.Size.X, rect1.Size.Y,
            rect2.Position.X, rect2.Position.Y, rect2.Size.X, rect2.Size.Y);
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

    public static bool IntersectCircleRectangle(Vector2 circleCenter, float radius, Rect2 rect) {
        return IntersectCircleRectangle(circleCenter.X, circleCenter.Y, radius, rect.Position.X, rect.Position.Y, rect.Size.X, rect.Size.Y);
    }

    /// <summary>
    /// Returns true if the point px, py is inside a rectangle located at rx, ry with width and height
    /// </summary>
    public static bool IntersectCircleRectangle(float circleX, float circleY, float radius, float rectX, float rectY, float width, float height) {
        if (width == 0 || height == 0 || radius == 0) { // No area, no intersection
            return false;
        }
        var closestX = Mathf.Clamp(circleX, rectX, rectX + width);
        var closestY = Mathf.Clamp(circleY, rectY, rectY + height);
        var dx = circleX - closestX;
        var dy = circleY - closestY;
        return dx * dx + dy * dy <= radius * radius;
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


}