using Godot;

namespace Veronenger.Character {
    public static class MotionConfig {

        public static Vector2 FloorUpDirection = Vector2.Up;
        public const float SnapLength = 12f; // be sure this value is less than the smallest tile
        public static Vector2 SnapToFloorVector = Vector2.Down * SnapLength;


        public static float ConfigureSpeed(float maxSpeed, float timeToMaxSpeed = 0) {
            if (timeToMaxSpeed > 0) { // avoid divide by zero
                return maxSpeed / timeToMaxSpeed;
            } else {
                return maxSpeed;
            }
        }

        public static (float gravity, float jumpSpeed) ConfigureJump(float jumpHeight, float maxJumpTime) {
            var gravity = (2 * jumpHeight) / Mathf.Pow(maxJumpTime, 2);
            var jumpSpeed = gravity * maxJumpTime;
            return (gravity, jumpSpeed);
        }
    }
}