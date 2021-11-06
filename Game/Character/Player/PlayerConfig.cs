namespace Veronenger.Game.Character.Player {
	public class PlayerConfig : CharacterConfig {
		public const float COYOTE_TIME = 0.1f; // seconds. How much time the player can jump when falling
		public const float JUMP_HELPER_TIME = 0.2f; // seconds. If the user press jump just before land
		public const int MAX_JUMPS = 1;

		public PlayerConfig() {
			// DEBUG_SLOPE_STAIRS = true;

			float MAX_SPEED = 110.0f; // pixels/seconds
			float TIME_TO_MAX_SPEED = 0.2f; // seconds to reach the max speed 0=immediate
			ConfigureSpeed(MAX_SPEED, TIME_TO_MAX_SPEED);
			STOP_IF_SPEED_IS_LESS_THAN = 20f; // pixels / seconds
			FRICTION = 0.8f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!


			// CONFIG: air
			float JUMP_HEIGHT = 80f; // jump max pixels
			float MAX_JUMP_TIME = 0.5f; // jump max time
			ConfigureJump(JUMP_HEIGHT, MAX_JUMP_TIME);
			JUMP_FORCE_MIN = JUMP_FORCE / 2;

			MAX_FALLING_SPEED = 2000; // max speed in free fall
			START_FALLING_SPEED = 100; // speed where the player changes to falling(test with fast downwards platform!)
			AIR_RESISTANCE = 0; // 0 = stop immediately, 1 = keep lateral movement until the end of the jump
		}
	}
}
