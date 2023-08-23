using System;

namespace Betauer.Core.Collision;

public static class Geometry {
    public static bool Overlaps(Circle c1, Circle c2) {
        var dx = c1.Position.X - c2.Position.X;
        var dy = c1.Position.Y - c2.Position.Y;
        var distanceSquared = dx * dx + dy * dy;
        var radiiSum = c1.Radius + c2.Radius;
        return distanceSquared <= radiiSum * radiiSum;
    }

    public static bool Overlaps(Rectangle r1, Rectangle r2) {
        return r1.MaxX >= r2.MinX && r1.MinX <= r2.MaxX && r1.MaxY >= r2.MinY && r1.MinY <= r2.MaxY;
    }

    public static bool Overlaps(Rectangle rectangle, Circle circle) {
        return Overlaps(circle, rectangle);
    }

    public static bool Overlaps(Circle circle, Rectangle rectangle) {
        var closestX = Math.Clamp(circle.Position.X, rectangle.MinX, rectangle.MaxX);
        var closestY = Math.Clamp(circle.Position.Y, rectangle.MinY, rectangle.MaxY);
        var dx = circle.Position.X - closestX;
        var dy = circle.Position.Y - closestY;
        return dx * dx + dy * dy <= circle.Radius * circle.Radius;
    }

    public static bool IsPointInsideCircle(double px, double py, Circle circle) {
        var dx = circle.Position.X - px;
        var dy = circle.Position.Y - py;
        return dx * dx + dy * dy <= circle.Radius * circle.Radius;
    }
}