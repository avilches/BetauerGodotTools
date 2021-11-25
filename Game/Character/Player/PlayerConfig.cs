namespace Veronenger.Game.Character.Player {
    public class PlayerConfig {
        public const float CoyoteJumpTime = 0.1f; // seconds. How much time the player can jump when falling
        public const float JumpHelperTime = 0.1f; // seconds. If the user press jump just before land

        public readonly MotionConfig MotionConfig;

        public PlayerConfig() {
            MotionConfig = new MotionConfig();
            const float _maxSpeed = 110.0f; // pixels/seconds
            const float _timeToMaxSpeed = 0.2f; // seconds to reach the max speed 0=immediate
            MotionConfig.ConfigureSpeed(_maxSpeed, _timeToMaxSpeed);
            MotionConfig.StopIfSpeedIsLessThan = 20f; // pixels / seconds
            MotionConfig.Friction = 0.8f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!

            // CONFIG: air
            const float jumpHeight = 80f; // jump max pixels
            const float maxJumpTime = 0.5f; // jump max time
            var jump = MotionConfig.ConfigureJump(jumpHeight, maxJumpTime);
            MotionConfig.Gravity = jump.Gravity;
            MotionConfig.JumpForce = jump.JumpForce;
            MotionConfig.JumpForceMin = MotionConfig.JumpForce / 2;

            MotionConfig.MaxFallingSpeed = 2000; // max speed in free fall
            MotionConfig.StartFallingSpeed = 100; // speed where the player changes to falling(test with fast downwards platform!)
            MotionConfig.AirResistance = 0; // 0 = stop immediately, 1 = keep lateral movement until the end of the jump
        }
    }
}