using Betauer.DI;
using Godot;

namespace Veronenger.Game.Character.Player {
    [Service]
    public class PlayerConfig {
        public const float CoyoteJumpTime = 0.1f; // seconds. How much time the player can jump when falling
        public const float JumpHelperTime = 0.1f; // seconds. If the user press jump just before land

        public float MaxSpeed = 110f; // pixels/seconds
        public float Acceleration = -1; // pixels/frame
        public float StopIfSpeedIsLessThan = 20f; // pixels/seconds
        public float Friction = 0.9f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!

        public float MaxFloorGravity = 80; // pixels/seconds. This value must be high enough to climb down a slope in constant speed.
                                           // It should match the negative vertical speed needed to climb up a slope.
        
        // CONFIG: air
        public float AirGravity; // pixels/frame (it's accumulative)
        public float JumpSpeed;
        public float JumpSpeedMin;

        public float MaxFallingSpeed = 2000; // max speed in free fall
        public float StartFallingSpeed = 100; // speed where the player animation changes to falling (test with fast downwards platform!)
        public float AirResistance = 0.86f; // 0=stop immediately, 1=keep lateral movement until the end of the jump


        public PlayerConfig() {
            const float timeToMaxSpeed = 0.2f; // seconds to reach the max speed 0=immediate
            Acceleration = MotionConfig.ConfigureSpeed(MaxSpeed, timeToMaxSpeed);

            // CONFIG: air
            const float jumpHeight = 80f; // jump max pixels
            const float maxJumpTime = 0.5f; // jump max time
            (AirGravity, JumpSpeed) = MotionConfig.ConfigureJump(jumpHeight, maxJumpTime);
            JumpSpeedMin = JumpSpeed / 2;
        }
    }
}