using Godot;

namespace Veronenger.Game.Platform.Character.Npc; 

public class NpcConfig {
    public Vector2 HurtKnockback = new(60, -100);
    public float HurtKnockbackTime = 0.4f; // Tween time without moving (just knockback inertia) flashing red

    public int KickbackEnergyThreshold = 10; // 10 means from -10 to 10 

    // Melee attack kickback
    public (int start, int end) KickbackMeleeAngle = (30, 80); 
    public int KickbackMeleeEnergyMultiplier = 15; 

    // Range attack kickback
    public (int start, int end) KickbackRangeAngle = (0, 25); 
    public int KickbackRangeEnergyMultiplier = 10; 

    public float MaxSpeed = 60f; // pixels/seconds
    public float Acceleration = -1; // pixels/frame
    public float StopIfSpeedIsLessThan = 20f; // pixels/seconds
    public float Friction = 0; // pixels/seconds 0=stop immediately

    public int Attack = 6;
    public float InitialHealth = 32;
    public float InitialMaxHealth = 32;
    public bool HealthBarVisible = false;
    
    public float VisionAngle = Mathf.DegToRad(48); // 47 up + 47 down, a 92º cone
    public float VisionDistance = 120; // 200f;
    
    public NpcConfig() {
        const float timeToMaxSpeed = 0f; // seconds to reach the max speed 0=immediate
        Acceleration = MotionConfig.ConfigureSpeed(MaxSpeed, timeToMaxSpeed);
        StopIfSpeedIsLessThan = 5f; // pixels / seconds
        Friction = 0f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!
    }
}