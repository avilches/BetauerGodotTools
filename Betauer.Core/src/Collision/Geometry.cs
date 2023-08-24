using System;
using Godot;

namespace Betauer.Core.Collision;

public static class Geometry {
    public static bool IntersectCircles(float x1, float y1, float radius1, float x2, float y2, float radius2) {
        var dx = x1 - x2;
        var dy = y1 - y2;
        var distanceSquared = dx * dx + dy * dy;
        var radiiSum = radius1 + radius2;
        return distanceSquared <= radiiSum * radiiSum;
    }

    public static bool IntersectRectangles(float x1, float y1, float width1, float height1, float x2, float y2, float width2, float height2) {
        return x1 + width1 >= x2 &&
               x1 <= x2 + width2 &&
               y1 + height1 >= y2 &&
               y1 <= y2 + height2;
    }

    public static bool IntersectCircleRectangle(float circleX, float circleY, float radius, float rectX, float rectY, float width, float height) {
        var closestX = Math.Clamp(circleX, rectX, rectX + width);
        var closestY = Math.Clamp(circleY, rectY, rectY + height);
        var dx = circleX - closestX;
        var dy = circleY - closestY;
        return dx * dx + dy * dy <= radius * radius;
    }

    public static bool IsPointInsideCircle(float px, float py, float cx, float cy, float radius) {
        var dx = cx - px;
        var dy = cy - py;
        return dx * dx + dy * dy <= radius * radius;
    }

    public static bool IsPointInsideRectangle(float px, float py, float rx, float ry, float width, float height) {
        return px >= rx && 
               px <= rx + width && 
               py >= ry && 
               py <= ry + height;
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

}