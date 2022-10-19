using Godot;

namespace Betauer {
    public static class Vector2Extensions {
        
        public static Vector2 AngleToVector(this float angle) {
            var rad = Mathf.Deg2Rad(angle);
            return new Vector2(Mathf.Cos(rad),-Mathf.Sin(rad));
        }
        
        /// <summary>
        /// From 0º to 180º, non inclusive
        /// </summary>
        public static bool IsUp(this Vector2 normal) =>
            normal == Vector2.Up || normal.y is < 0 and > -1;

        /// <summary>
        /// From 180º to 360º, non inclusive
        /// </summary>
        public static bool IsDown(this Vector2 normal) =>
            normal == Vector2.Down || normal.y is > 0 and < 1;

        /// <summary>
        /// From 90º to 270º, non inclusive
        /// </summary>
        public static bool IsLeft(this Vector2 normal) =>
            normal == Vector2.Left || normal.x is < 0 and > -1;

        /// <summary>
        /// From 270º to 90º, non inclusive
        /// </summary>
        public static bool IsRight(this Vector2 normal) =>
            normal == Vector2.Right || normal.x is > 0 and < 1;

        /// <summary>
        /// From 0º to 90º, non inclusive
        /// </summary>
        public static bool IsUpRight(this Vector2 normal) => 
            (normal.IsUp() && normal.IsRight());
        
        /// <summary>
        /// From 90º to 180º, non inclusive
        /// </summary>
        public static bool IsUpLeft(this Vector2 normal) => 
            (normal.IsUp() && normal.IsLeft());

        /// <summary>
        /// From 180º to 270º, non inclusive
        /// </summary>
        public static bool IsDownLeft(this Vector2 normal) => 
            (normal.IsDown() && normal.IsLeft());

        /// <summary>
        /// From 270º to 360º, non inclusive
        /// </summary>
        public static bool IsDownRight(this Vector2 normal) => 
            (normal.IsDown() && normal.IsRight());

        public static bool IsFloor(this Vector2 normal, Vector2 floorUpDirection, float maxSlope = 0.785398f) {
            return Mathf.Acos(normal.Dot(floorUpDirection)) <= maxSlope + 0.01f;
        }
        
        public static bool IsCeiling(this Vector2 normal, Vector2 floorUpDirection, float maxSlope = 0.785398f) {
            return Mathf.Acos(normal.Dot(-floorUpDirection)) <= maxSlope + 0.01f;
        }
        
        
        
    }
}