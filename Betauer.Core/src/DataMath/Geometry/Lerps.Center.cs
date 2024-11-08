using Godot;

namespace Betauer.Core.DataMath.Geometry;

public static partial class Lerps {

    public static float LerpToRectCustomCenter(Rect2 rect, Vector2 center, Vector2 point) {
        return LerpToRectCustomCenter(rect.Size.X, rect.Size.Y, center.X, center.Y, point.X, point.Y);
    }

    public static float LerpToRectCenter(Rect2 rect, Vector2 point) {
        return LerpToRectCenter(rect.Size.X, rect.Size.Y, point.X, point.Y);
    }

    public static float LerpToEllipseCenter(Vector2 radius, Vector2 point) {
        return LerpToEllipseCenter(radius.X, radius.Y, point.X, point.Y);
    }

    public static float LerpToEllipseCenterRotated(Vector2 radius, float rotation, Vector2 point) {
        return LerpToEllipseCenterRotated(radius.X, radius.Y, rotation, point.X, point.Y);
    }

    public static float LerpToCircleCenter(float radius, Vector2 point) {
        return LerpToCircleCenter(radius, point.X, point.Y);
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
    public static float LerpToRectCustomCenter(float width, float height, float centerX, float centerY, float x, float y) {
        var maxDistanceX = System.Math.Max(centerX, width - 1 - centerX);
        var maxDistanceY = System.Math.Max(centerY, height - 1 - centerY);
        var distanceX = System.Math.Abs(x - centerX);
        var distanceY = System.Math.Abs(y - centerY);
        var valueX = 1f - distanceX / maxDistanceX;
        var valueY = 1f - distanceY / maxDistanceY;
        return System.Math.Min(valueX, valueY);
    }

    /// <summary>
    /// Returns a value from 0 to 1 based on the (x, y) coords and a rectangle of width x height size.
    /// It will return 1 if the point matches the center of the rectangle (located at width/2 and height/2) and
    /// it goes to 0 gradually when the coords touch the border of the rectangle.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float LerpToRectCenter(float width, float height, float x, float y) {
        var centerX = width / 2;
        var centerY = height / 2;
        var distanceX = System.Math.Abs(x - centerX);
        var distanceY = System.Math.Abs(y - centerY);
        var valueX = 1f - distanceX / centerX;
        var valueY = 1f - distanceY / centerY;
        return System.Math.Min(valueX, valueY);
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
    public static float LerpToEllipseCenter(float rx, float ry, float x, float y) {
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
    public static float LerpToEllipseCenterRotated(float rx, float ry, float rotation, float x, float y) {
        var cos = Mathf.Cos(rotation);
        var sin = Mathf.Sin(rotation);
        var rotatedX = cos * x + sin * y;
        var rotatedY = -sin * x + cos * y;
        return LerpToEllipseCenter(rx, ry, rotatedX, rotatedY);
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
    public static float LerpToCircleCenter(float r, float x, float y) {
        return System.Math.Clamp(1 - ((x * x + y * y) / (r * r)), 0f, 1f);
    }

}