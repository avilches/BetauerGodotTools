using Betauer.DI;
using Godot;

namespace Veronenger.Character.Player; 

[Service]
public class PlayerConfig {
    public const float CoyoteJumpTime = 0.1f; // seconds. How much time the player can jump when falling
    public const float JumpHelperTime = 0.1f; // seconds. If the user press jump just before land

    public float MaxSpeed = 110f; // pixels/seconds
    public float SpeedToPlayRunStop;
    public float Acceleration; // pixels/frame
    public float StopIfSpeedIsLessThan = 20f; // pixels/seconds
    public float ChangeDirectionFactor = 0f;
    public float Friction = 0.9f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!
    public float FloorGravity = 1000;

    // CONFIG: air
    public float AirGravity; // pixels/seconds (it's accumulative)
    public float JumpSpeed;
    public float JumpSpeedMin;

    public float MaxFallingSpeed = 2000; // max speed in free fall
    public float StartFallingSpeed = 100; // speed where the player animation changes to falling (test with fast downwards platform!)
    public float AirResistance = 0.86f; // 0=stop immediately, 1=keep lateral movement until the end of the jump

    public Vector2 HurtKnockback = new(-180, -180);
    public float HurtKnockbackTime = 0.2f; // Tween time
    public float HurtTime = 0.4f; // Player flashing time, without movement
    public float HurtInvincibleTime = 1.6f; // Player invincible time (includes hurt animation time)

    public float InitialHealth = 32;
    public float InitialMaxHealth = 32;

    public PlayerConfig() {
        SpeedToPlayRunStop = MaxSpeed * 0.95f;
        const float timeToMaxSpeed = 0.2f; // seconds to reach the max speed 0=immediate
        Acceleration = MotionConfig.ConfigureSpeed(MaxSpeed, timeToMaxSpeed);

        // CONFIG: air
        const float jumpHeight = 80f; // jump max pixels
        const float maxJumpTime = 0.5f; // jump max time
        (AirGravity, JumpSpeed) = MotionConfig.ConfigureJump(jumpHeight, maxJumpTime);
        JumpSpeedMin = JumpSpeed / 2;
    }
}