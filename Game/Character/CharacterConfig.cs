using Godot;
using Tools.Statemachine;

namespace Veronenger.Game.Character {
    public abstract class CharacterConfig : StateMachineDebugConfig {
        public bool DEBUG_STATEMACHINE_FLOW => false;
        public bool DEBUG_STATEMACHINE_CHANGE => false;
        public bool DEBUG_MAX_SPEED = false;
        public bool DEBUG_ACCELERATION = false;
        public bool DEBUG_SLOPE_STAIRS = false;
        public bool DEBUG_COLLISION = false;
        public bool DEBUG_COLLISION_RAYSCAST_VS_SLIDECOUNT = false;
        public bool DEBUG_MOTION = false;
        public bool DEBUG_JUMP_VELOCITY = false;

        // Only for the player...
        public bool DEBUG_JUMP_HELPER = false;
        public bool DEBUG_JUMP_COYOTE = false;
        public bool DEBUG_INPUT_EVENTS = false;

        // CONFIG: ground
        public float MAX_SPEED = 120f; // pixels/seconds
        public float ACCELERATION = 120f; // pixels/frame
        public float STOP_IF_SPEED_IS_LESS_THAN = 20f; // pixels/seconds
        public float FRICTION = 0f; // 0=stop immediately, 0.9=10%/frame 0.99=ice!!

        // CONFIG: air
        public float GRAVITY;
        public float JUMP_FORCE;
        public float JUMP_FORCE_MIN;

        public float MAX_FALLING_SPEED = 2000; // max speed in free fall

        public float
            START_FALLING_SPEED = 400; // speed where the player changes to falling (test with fast downwards platform!)

        public float AIR_RESISTANCE = 0; // 0=stop immediately, 1=keep lateral movement until the end of the jump

        // CONFIG: squeeze effect
        public float SQUEEZE_JUMP_TIME = 0.1f; // % correction per frame (lerp). The bigger, the faster
        public Vector2 SQUEEZE_JUMP_SCALE = new Vector2(1, 1.4f); // Vector to scale when jump
        public float SQUEEZE_LAND_TIME = 0.4f; // % correction per frame (lerp). The bigger, the faster
        public Vector2 SQUEEZE_LAND_SCALE = new Vector2(1.2f, 0.8f); // Vector to scale when land


        // CONFIG: slope config
        public Vector2 FLOOR = Vector2.Up;
        public const float SLOW_ON_SLOPE_DOWN = 0.4f; // % speed slow % in slopes. 1 = no slow down, 0.5 = half
        public const float SLOW_ON_SLOPE_UP = 0.9f; // % speed slow % in slopes. 1 = no slow down, 0.5 = half
        public const float SNAP_LENGTH = 12f; // be sure this value is less than the smallest tile
        public Vector2 SLOPE_RAYCAST_VECTOR = Vector2.Down * SNAP_LENGTH;

        public void ConfigureSpeed(float maxSpeed, float timeToMaxSpeed = 0) {
            MAX_SPEED = maxSpeed;
            if (timeToMaxSpeed > 0) { // avoid divide by zero
                ACCELERATION = maxSpeed / timeToMaxSpeed;
            } else {
                ACCELERATION = maxSpeed;
            }
        }

        public void ConfigureJump(float jumpHeight, float maxJumpTime) {
            GRAVITY = (2 * jumpHeight) / Mathf.Pow(maxJumpTime, 2);
            JUMP_FORCE = GRAVITY * maxJumpTime;
            JUMP_FORCE_MIN = JUMP_FORCE / 2;
        }
    }
}