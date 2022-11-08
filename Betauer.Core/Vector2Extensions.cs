using Godot;

namespace Betauer.Core {
    public static class Vector2Extensions {
        
        /// <summary>
        /// Rotate 180º counter-clockwise
        /// </summary>
        /// <param name="floorDirection"></param>
        /// <returns></returns>
        public static Vector2 Down(this Vector2 floorDirection) {
            return new Vector2(-floorDirection.x, -floorDirection.y);
        }

        /// <summary>
        /// Rotate 90º counter-clockwise
        /// </summary>
        /// <param name="floorDirection"></param>
        /// <returns></returns>
        public static Vector2 Left(this Vector2 floorDirection) {
            return new Vector2(floorDirection.y, -floorDirection.x); // same as vector2.Perpendicular();
        }

        /// <summary>
        /// Rotate 270º counter-clockwise
        /// </summary>
        /// <param name="floorDirection"></param>
        /// <returns></returns>
        public static Vector2 Right(this Vector2 floorDirection) {
            return new Vector2(-floorDirection.y, floorDirection.x);
        }

        public static Vector2 AngleToVector(this float angle) {
            var rad = Mathf.DegToRad(angle);
            return new Vector2(Mathf.Cos(rad),-Mathf.Sin(rad));
        }
        
        /// <summary>
        /// From 0º to 180º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsUp(this Vector2 normal, Vector2 floorDirection) =>
            normal.Dot(floorDirection) > 0; 

        /// <summary>
        /// From 180º to 360º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsDown(this Vector2 normal, Vector2 floorDirection) =>
            normal.Dot(floorDirection.Down()) > 0;

        /// <summary>
        /// From 90º to 270º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsLeft(this Vector2 normal, Vector2 floorDirection) =>
            normal.Dot(floorDirection.Left()) > 0; 

        /// <summary>
        /// From 270º to 90º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsRight(this Vector2 normal, Vector2 floorDirection) =>
            normal.Dot(floorDirection.Right()) > 0; 

        /// <summary>
        /// From 0º to 90º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsUpRight(this Vector2 normal, Vector2 floorDirection) => 
            normal.Dot(floorDirection) > 0 && normal.Cross(floorDirection) < 0; 
        
        /// <summary>
        /// From 90º to 180º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsUpLeft(this Vector2 normal, Vector2 floorDirection) => 
            normal.Dot(floorDirection) > 0 && normal.Cross(floorDirection) > 0; 

        /// <summary>
        /// From 180º to 270º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsDownLeft(this Vector2 normal, Vector2 floorDirection) => 
            normal.Dot(floorDirection.Down()) > 0 && normal.Cross(floorDirection.Down()) < 0; 

        /// <summary>
        /// From 270º to 360º, non inclusive, facing floorDirection
        /// </summary>
        public static bool IsDownRight(this Vector2 normal, Vector2 floorDirection) => 
            normal.Dot(floorDirection.Down()) > 0 && normal.Cross(floorDirection.Down()) > 0; 

        public static bool IsFloor(this Vector2 normal, Vector2 floorUpDirection, float maxSlope = 0.785398f) {
            return Mathf.Acos(normal.Dot(floorUpDirection)) <= maxSlope + 0.01f;
        }
        
        public static bool IsCeiling(this Vector2 normal, Vector2 floorUpDirection, float maxSlope = 0.785398f) {
            return Mathf.Acos(normal.Dot(-floorUpDirection)) <= maxSlope + 0.01f;
        }
        
        
        
    }
}