namespace Veronenger.Game.Character.Enemy {
    public class EnemyConfig : CharacterConfig {
        public EnemyConfig() {
            const float maxSpeed = 110.0f; // pixels/seconds
            const float timeToMaxSpeed = 0.2f; // seconds to reach the max speed 0=immediate
            ConfigureSpeed(maxSpeed, timeToMaxSpeed);
            STOP_IF_SPEED_IS_LESS_THAN = 20f; // pixels / seconds
            FRICTION = 0.8f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!

            // CONFIG: air
            const float jumpHeight = 80f; // jump max pixels
            const float maxJumpTime = 0.5f; // jump max time
            ConfigureJump(jumpHeight, maxJumpTime);
            JUMP_FORCE_MIN = JUMP_FORCE / 2;

            MAX_FALLING_SPEED = 2000; // max speed in free fall
            START_FALLING_SPEED = 100; // speed where the player changes to falling(test with fast downwards platform!)
            AIR_RESISTANCE = 0; // 0 = stop immediately, 1 = keep lateral movement until the end of the jump
        }
    }
}