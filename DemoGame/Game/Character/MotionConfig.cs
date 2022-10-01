using Godot;

namespace Veronenger.Game.Character {
    public sealed class MotionConfig {

        // CONFIG: ground
        public float MaxSpeed = 120f; // pixels/seconds
        public float Acceleration = 120f; // pixels/frame
        public float StopIfSpeedIsLessThan = 20f; // pixels/seconds
        public float Friction = 0f; // 0=stop immediately, 0.9=10%/frame 0.99=ice!!

        // CONFIG: air
        public float Gravity;
        public float JumpForce;
        public float JumpForceMin;

        public float MaxFallingSpeed = 2000; // max speed in free fall
        public float StartFallingSpeed = 400; // speed where the player changes to falling (test with fast downwards platform!)
        public float AirResistance = 0; // 0=stop immediately, 1=keep lateral movement until the end of the jump

        // CONFIG: squeeze effect
        // public float SQUEEZE_JUMP_TIME = 0.1f; // % correction per frame (lerp). The bigger, the faster
        // public Vector2 SQUEEZE_JUMP_SCALE = new Vector2(1, 1.4f); // Vector to scale when jump
        // public float SQUEEZE_LAND_TIME = 0.4f; // % correction per frame (lerp). The bigger, the faster
        // public Vector2 SQUEEZE_LAND_SCALE = new Vector2(1.2f, 0.8f); // Vector to scale when land

        // CONFIG: slope config
        public Vector2 FloorVector = Vector2.Up;
        // public const float SLOW_ON_SLOPE_DOWN = 0.4f; // % speed slow % in slopes. 1 = no slow down, 0.5 = half
        // public const float SLOW_ON_SLOPE_UP = 0.9f; // % speed slow % in slopes. 1 = no slow down, 0.5 = half
        public const float SnapLength = 12f; // be sure this value is less than the smallest tile
        public Vector2 SlopeRayCastVector = Vector2.Down * SnapLength;

        public void ConfigureSpeed(float maxSpeed, float timeToMaxSpeed = 0) {
            MaxSpeed = maxSpeed;
            if (timeToMaxSpeed > 0) { // avoid divide by zero
                Acceleration = maxSpeed / timeToMaxSpeed;
            } else {
                Acceleration = maxSpeed;
            }
        }

        public static (float gravity, float jumpForce) ConfigureJump(float jumpHeight, float maxJumpTime) {
            var gravity = (2 * jumpHeight) / Mathf.Pow(maxJumpTime, 2);
            var jumpForce = gravity * maxJumpTime;
            return (gravity, jumpForce);
        }

        public void Configure(IKinematicPlatformMotionBodyConfig kinematicPlatformMotionBody) {
            kinematicPlatformMotionBody.DefaultGravity = Gravity;
            kinematicPlatformMotionBody.DefaultMaxSpeed = MaxSpeed;
            kinematicPlatformMotionBody.DefaultMaxFallingSpeed = MaxFallingSpeed;
            kinematicPlatformMotionBody.SlopeRayCastVector = SlopeRayCastVector;
            kinematicPlatformMotionBody.FloorVector = FloorVector;
        }

        public void Configure(IKinematicTopDownMotionBodyConfig kinematicPlatformMotionBody) {
            kinematicPlatformMotionBody.DefaultMaxSpeed = new Vector2(MaxSpeed, MaxSpeed);
            kinematicPlatformMotionBody.DefaultSlideOnSlopes = true;

        }
    }
}