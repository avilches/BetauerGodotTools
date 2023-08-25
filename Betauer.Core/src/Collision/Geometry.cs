using System;
using System.Collections.Generic;
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
        var dx = x1 - x2;
        var dy = y1 - y2;
        var distanceSquared = dx * dx + dy * dy;
        var radiiSum = radius1 + radius2;
        return distanceSquared <= radiiSum * radiiSum;
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

    public static bool IsPointInsideCircle(Vector2 point, Circle circle) {
        return IsPointInsideCircle(point.X, point.Y, circle);
    }

    public static bool IsPointInsideCircle(float px, float py, Circle circle) {
        return IsPointInsideCircle(px, py, circle.Position.X, circle.Position.Y, circle.Radius);
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

    public static bool Intersect(Circle c1, Circle c2) {
        return IntersectCircles(c1.Position.X, c1.Position.Y, c1.Radius, c2.Position.X, c2.Position.Y, c2.Radius);
    }

    public static bool Intersect(Rectangle rectangle, Circle circle) {
        return Intersect(circle, rectangle);
    }

    public static bool Intersect(Circle circle, Rectangle rectangle) {
        return IntersectCircleRectangle(
            circle.Position.X, circle.Position.Y, circle.Radius,
            rectangle.Position.X, rectangle.Position.Y, rectangle.Size.X, rectangle.Size.Y);
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