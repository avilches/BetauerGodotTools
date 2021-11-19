namespace Veronenger.Game.Character.Enemy {
    public class EnemyConfig : Platform2DCharacterConfig {
        public EnemyConfig() {
            const float maxSpeed = 15.0f; // pixels/seconds
            const float timeToMaxSpeed = 0.2f; // seconds to reach the max speed 0=immediate
            ConfigureSpeed(maxSpeed, timeToMaxSpeed);
            StopIfSpeedIsLessThan = 20f; // pixels / seconds
            Friction = 0.8f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!

            // CONFIG: air
            const float jumpHeight = 80f; // jump max pixels
            const float maxJumpTime = 0.5f; // jump max time
            ConfigureJump(jumpHeight, maxJumpTime);
            JumpForceMin = JumpForce / 2;

            MaxFallingSpeed = 2000; // max speed in free fall
            StartFallingSpeed = 100; // speed where the player changes to falling(test with fast downwards platform!)
            AirResistance = 0; // 0 = stop immediately, 1 = keep lateral movement until the end of the jump
        }
    }
}