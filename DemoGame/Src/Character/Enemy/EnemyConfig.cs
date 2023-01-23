using Betauer.DI;
using Godot;
using Veronenger.Character.Handler;

namespace Veronenger.Character.Enemy; 

[Configuration]
public class EnemyConfiguration {
    [Service]
    public ICharacterHandler ZombieHandler => new CharacterController();
}

[Service]
public class EnemyConfig {
    public Vector2 HurtKnockback = new(70, -70);
    public float HurtKnockbackTime = 0.2f; // Tween time without moving (just knockback inertia) flashing red

    public float MaxSpeed = 60f; // pixels/seconds
    public float Acceleration = -1; // pixels/frame
    public float StopIfSpeedIsLessThan = 20f; // pixels/seconds
    public float Friction = 0; // pixels/seconds 0=stop immediately

    public int Attack = 6;
    public float InitialHealth = 1000;
    public float InitialMaxHealth = 32;
    
    public float VisionAngle = Mathf.DegToRad(48); // 47 up + 47 down, a 92º cone
    public float VisionDistance = 120; // 200f;
    
    public EnemyConfig() {
        const float timeToMaxSpeed = 0f; // seconds to reach the max speed 0=immediate
        Acceleration = MotionConfig.ConfigureSpeed(MaxSpeed, timeToMaxSpeed);
        StopIfSpeedIsLessThan = 5f; // pixels / seconds
        Friction = 0f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!
    }
}