using System;
using Betauer.Core.Collision.Spatial2D;
using Godot;

namespace Betauer.Core.Collision;

public static class Geometry {
    public static bool IsPointInsideCircle(float px, float py, float cx, float cy, float radius) {
        if (radius == 0) { // No area, no intersection
            return false;
        }
        var dx = cx - px;
        var dy = cy - py;
        return dx * dx + dy * dy <= radius * radius;
    }

    public static bool IsPointInsideRectangle(float px, float py, float rx, float ry, float width, float height) {
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

    public static Vector2 ExtendLineAtStart(float x0, float y0, float x1, float y1, float extension) {
        var length = Distance(x0, y0, x1, y1);
        var dx = (x1 - x0) / length;
        var dy = (y1 - y0) / length;

        return new Vector2(x0 - dx * extension, y0 - dy * extension);
    }

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

    public static bool IsPointInsideRectangle(Vector2 point, Rectangle rectangle) {
        return IsPointInsideRectangle(point.X, point.Y, rectangle);
    }

    public static bool IsPointInsideRectangle(float px, float py, Rectangle rectangle) {
        return IsPointInsideRectangle(px, py, rectangle.Position.X, rectangle.Position.Y, rectangle.Size.X, rectangle.Size.Y);
    }

    public static bool Intersect(Rectangle r1, Rectangle r2) {
        return IntersectRectangles(
            r1.Position.X, r1.Position.Y, r1.Size.X, r1.Size.Y,
            r2.Position.X, r2.Position.Y, r2.Size.X, r2.Size.Y);
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
}