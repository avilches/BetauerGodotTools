using Betauer.DI.Attributes;

namespace Veronenger.Game.Platform.Character.Player; 

[Singleton]
public class PlayerConfig {
    public const float CoyoteJumpTime = 0.16f; // seconds. How much time the player can jump when falling
    public const float JumpHelperTime = 0.16f; // seconds. If the user press jump just before land

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

    public float HurtInvincibleTime = 1.6f; // Player invincible time (includes hurt animation time)

    public float InitialHealth = 32;
    public float InitialMaxHealth = 32;

    public float PickingUpSpeed = 300f; // 300px per second
    public float PickingUpAcceleration = 1.1f; // 2x per second
    public float PickupDoneDistance = 10; // 10px and the item is picked up
    public float DropLateralFriction = 0.95f;
    public float DroppingTime = 0.550f;
    public float DropLateralSpeed = 100;

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