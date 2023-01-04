using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.StateMachine.Sync;
using Betauer.Core.Time;
using Veronenger.Character.Handler;
using Veronenger.Character.Player;
using Veronenger.Managers;

namespace Veronenger.Character.Enemy; 

public enum ZombieEvent {
    Dead,
    Attacked
}

public enum ZombieState {
    Idle,
    Run,
    Landing,
    Jump,
        
    Attacked,
    Destroy,
        
    FallShort
}

public class EnemyStatus {
    public float Health = 50;
    public float MaxHealth = 50;
    public float HealthPercent => Health / MaxHealth;

    public void Attack(Attack attack) {
        Health -= attack.Damage;
    }

    public bool IsDead() => Health <= 0f;
}

[Service(Lifetime.Transient)]
public partial class ZombieStateMachine : StateMachineNodeSync<ZombieState, ZombieEvent> {
    public ZombieStateMachine() : base(ZombieState.Idle, "Zombie.StateMachine") {
    }

    [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
    [Inject] private CharacterManager CharacterManager { get; set; }
        
    [Inject] private PlayerConfig PlayerConfig { get; set; }
    [Inject] private EnemyConfig EnemyConfig { get; set; }
    [Inject] private ICharacterHandler Handler { get; set; }

    private ZombieNode _zombieNode;

    private float XInput => Handler.Directional.XInput;
    private float YInput => Handler.Directional.YInput;
    private IAction Jump => Handler.Jump;
    private IAction Attack => Handler.Attack;
    private IAction Float => Handler.Float;

    private float MotionX => Body.MotionX;
    private float MotionY => Body.MotionY;

    public KinematicPlatformMotion Body;
    public readonly EnemyStatus Status = new();
    private readonly GodotStopwatch _stateTimer = new();

    public void ApplyFloorGravity(float factor = 1.0F) {
        Body.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
    }

    public void ApplyAirGravity(float factor = 1.0F) {
        Body.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
    }

    private ICharacterAI _zombieAi;

    public void Start(string name, ZombieNode zombie) {
        _zombieNode = zombie;
        zombie.AddChild(this);

        Body = new KinematicPlatformMotion(zombie, zombie.Flipper, zombie.Marker2D, MotionConfig.FloorUpDirection);
        zombie.FloorStopOnSlope = true;
        // playerController.FloorBlockOnWall = true;
        zombie.FloorConstantSpeed = true;
        zombie.FloorSnapLength = MotionConfig.SnapLength;

        OnBeforeExecute += Body.SetDelta;

        // AI
        _zombieAi = ZombieAI.Create(Handler, new ZombieAI.Sensor(zombie, this, Body, () => CharacterManager.PlayerNode.Marker2D.GlobalPosition));
        OnBeforeExecute += delta => _zombieAi.Execute();
        OnAfterExecute += _zombieAi.EndFrame;
        OnAfterExecute += () => zombie.Label.Text = _zombieAi.GetState();

        var overlay = DebugOverlayManager.Follow(zombie).Title("Zombie");
        AddOverlayStates(overlay);
        AddOverlayMotion(overlay);
        AddOverlayCollisions(overlay);
        
        ConfigureStates();
    }
    
    public void AddOverlayStates(DebugOverlay overlay) {    
        overlay
            .OpenBox()
                .Text("State", () => CurrentState.Key.ToString()).EndMonitor()
                .Text("IA", () => _zombieAi.GetState()).EndMonitor()
                .Text("Pos", () => {
                    var playerMark = CharacterManager.PlayerNode.Marker2D;
                    return Body.IsFacingTo(playerMark)?
                        Body.IsToTheRightOf(playerMark)?"P <me|":"|me> P":
                        Body.IsToTheRightOf(playerMark)?"P |me>":"<me| P";
                }).EndMonitor()
            .CloseBox()
            .OpenBox()
            .Angle("Player angle", () => Body.AngleTo(CharacterManager.PlayerNode.Marker2D)).EndMonitor()
            .Text("Player is", () => Body.IsToTheRightOf(CharacterManager.PlayerNode.Marker2D)?"Left":"Right").EndMonitor()
            .Text("FacingPlayer", () => Body.IsFacingTo(CharacterManager.PlayerNode.Marker2D)).EndMonitor()
            .CloseBox();
            
    }
    
    public void AddOverlayMotion(DebugOverlay overlay) {    
        overlay
            .OpenBox()
                .Vector("Motion", () => Body.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
                .Graph("MotionX", () => Body.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
                .AddSerie("MotionY").Load(() => Body.MotionY).EndSerie().EndMonitor()
            .CloseBox()
            .GraphSpeed("Speed", PlayerConfig.JumpSpeed * 2).EndMonitor();
    }
    
    public void AddOverlayCollisions(DebugOverlay overlay) {    
        overlay
            .Graph("Floor", () => Body.IsOnFloor()).Keep(10).SetChartHeight(10)
                .AddSerie("Slope").Load(() => Body.IsOnSlope()).EndSerie()
            .EndMonitor()
            .Text("Floor", () => Body.GetFloorCollisionInfo()).EndMonitor()
            .Text("Ceiling", () => Body.GetCeilingCollisionInfo()).EndMonitor()
            .Text("Wall", () => Body.GetWallCollisionInfo()).EndMonitor();
    }
    
    public void ConfigureStates() {    

        On(ZombieEvent.Attacked).Then(context => IsState(ZombieState.Attacked) ? context.None() : context.Push(ZombieState.Attacked));
        On(ZombieEvent.Dead).Then(context=> context.Set(ZombieState.Destroy));
            
        State(ZombieState.Landing)
            .If(() => XInput == 0).Set(ZombieState.Idle)
            .If(() => true).Set(ZombieState.Run)
            .Build();

        State(ZombieState.Idle)
            .Enter(() => {
                _zombieNode.AnimationIdle.PlayLoop();
            })
            .Execute(() => {
                ApplyFloorGravity();
                Body.Stop(EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan);
            })
            .If(() => !Body.IsOnFloor()).Set(ZombieState.FallShort)
            .If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
            .If(() => XInput != 0).Set(ZombieState.Run)
            .Build();

        State(ZombieState.Run)
            .Enter(() => {
                _zombieNode.AnimationStep.PlayLoop();
            })
            .Execute(() => {
                Body.Flip(XInput);
                ApplyFloorGravity();
                Body.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed, 
                    EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
            })
            .If(() => !Body.IsOnFloor()).Set(ZombieState.FallShort)
            .If(() => Jump.IsJustPressed()).Set(ZombieState.Jump)
            .If(() => XInput == 0 && MotionX == 0).Set(ZombieState.Idle)
            .Build();

        State(ZombieState.Attacked)
            .Enter(() => {
                Body.MotionY = EnemyConfig.MiniJumpOnAttack.y;
                Body.MotionX = EnemyConfig.MiniJumpOnAttack.x * (Body.IsToTheRightOf(CharacterManager.PlayerNode.Marker2D) ? 1 : -1);
                _zombieNode.PlayAnimationAttacked();
                _stateTimer.Restart().SetAlarm(0.3f);
            })
            .Execute(() => {
                ApplyAirGravity();
                Body.Move();
            })
            .If(() => _stateTimer.IsAlarm()).Pop()
            .Build();

        State(ZombieState.Destroy)
            .Enter(() => {
                _zombieNode.DisableAll();

                if (Body.IsToTheRightOf(CharacterManager.PlayerNode.Marker2D)) {
                    _zombieNode.AnimationDieRight.PlayOnce(true);
                } else {
                    _zombieNode.AnimationDieLeft.PlayOnce(true);
                }
            })
            .Execute(() => {
                if (!_zombieNode.AnimationDieRight.Playing && !_zombieNode.AnimationDieLeft.Playing) {
                    _zombieNode.QueueFree();
                }
            })
            .Build();

        State(ZombieState.Jump)
            .Enter(() => {
                Body.MotionY = -PlayerConfig.JumpSpeed;
                // DebugJump($"Jump start: decelerating to {(-PlayerConfig.JumpSpeed).ToString()}");
                // _player.AnimationJump.PlayLoop();
            })
            .Execute(() => {
                ApplyAirGravity();
                Body.Flip(XInput);
                Body.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
                    EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
            })
            .If(() => MotionY >= 0).Set(ZombieState.FallShort)
            .If(Body.IsOnFloor).Set(ZombieState.Landing)
            .Build();

        State(ZombieState.FallShort)
            .Execute(() => {
                ApplyAirGravity();
                Body.Flip(XInput);
                Body.Lateral(XInput, EnemyConfig.Acceleration, EnemyConfig.MaxSpeed,
                    EnemyConfig.Friction, EnemyConfig.StopIfSpeedIsLessThan, 0);
            })
            .If(Body.IsOnFloor).Set(ZombieState.Landing)
            .Build();
    }

    public void TriggerAttacked(Attack attack) {
        Status.Attack(attack);
        Enqueue(Status.IsDead() ? ZombieEvent.Dead : ZombieEvent.Attacked);
    }
}