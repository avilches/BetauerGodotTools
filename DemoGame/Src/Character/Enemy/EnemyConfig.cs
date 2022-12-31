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
    public Vector2 MiniJumpOnAttack = new(130, -230);

    public float MaxSpeed = 60f; // pixels/seconds
    public float Acceleration = -1; // pixels/frame
    public float StopIfSpeedIsLessThan = 20f; // pixels/seconds
    public float Friction = 0; // pixels/seconds 0=stop immediately

    public EnemyConfig() {
        const float timeToMaxSpeed = 0.5f; // seconds to reach the max speed 0=immediate
        Acceleration = MotionConfig.ConfigureSpeed(MaxSpeed, timeToMaxSpeed);
        StopIfSpeedIsLessThan = 5f; // pixels / seconds
        Friction = 0.95f; // 0 = stop immediately 0.9 = 10 %/frame 0.99 = ice!!
    }
}