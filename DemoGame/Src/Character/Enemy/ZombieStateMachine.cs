using Betauer;
using Betauer.Application.Monitor;
using Betauer.DI;
using Betauer.DI.ServiceProvider;
using Betauer.Input;
using Betauer.StateMachine.Sync;
using Betauer.Core.Time;
using Godot;
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

    public void Attack(Attack attack) {
        Health -= attack.Damage;
    }

    public bool IsDead() {
        return Health <= 0f;
    }
}

public class ZombieIA {
    private readonly CharacterController _handler;
    private readonly ZombieNode _node;
    private readonly GodotStopwatch _stateTimer = new GodotStopwatch().Start();
        
    public ZombieIA(ICharacterHandler handler, ZombieNode node) {
        _handler = handler is CharacterController h ? h : null;
        _node = node;
    }


    public void HandleIA(double delta) {
        if (_handler == null) return;
            
        if (_stateTimer.Elapsed > 1f) {
            _stateTimer.Reset();
            ChangeDirection();
        } else {
            KeepMoving();
        }
        // GD.Print("Pressed:"+handler.HandlerJump.IsPressed()+
        // " JustPressed:"+handler.HandlerJump.IsJustPressed()+
        // " Released:"+handler.HandlerJump.IsReleased());
    }

    private void ChangeDirection() {
        _handler.DirectionalController.XInput = _node.IsFacingRight ? -1 : 1;
    }
        
    private void KeepMoving() {
        _handler.DirectionalController.XInput = _node.IsFacingRight ? 1 : -1;
    }
}

[Service(Lifetime.Transient)]
public partial class ZombieStateMachine : StateMachineNodeSync<ZombieState, ZombieEvent> {
    public ZombieStateMachine() : base(ZombieState.Idle, "Zombie.StateMachine") {
    }

    [Inject] private DebugOverlayManager DebugOverlayManager { get; set; }
    [Inject] private CharacterManager CharacterManager { get; set; }
        
    [Inject] private PlayerConfig PlayerConfig { get; set; }
    [Inject] private EnemyConfig EnemyConfig { get; set; }
    [Inject] public KinematicPlatformMotion Body { get; set; }
    // [Inject] private InputActionCharacterHandler Handler { get; set; }
    [Inject] private CharacterController Handler { get; set; }

    private ZombieNode _zombieNode;

    private float XInput => Handler.Directional.XInput;
    private float YInput => Handler.Directional.YInput;
    private IAction Jump => Handler.Jump;
    private IAction Attack => Handler.Attack;
    private IAction Float => Handler.Float;

    private float MotionX => Body.MotionX;
    private float MotionY => Body.MotionY;

    public readonly EnemyStatus Status = new();
    private readonly GodotStopwatch _stateTimer = new();

    public void ApplyFloorGravity(float factor = 1.0F) {
        Body.ApplyGravity(PlayerConfig.FloorGravity * factor, PlayerConfig.MaxFallingSpeed);
    }

    public void ApplyAirGravity(float factor = 1.0F) {
        Body.ApplyGravity(PlayerConfig.AirGravity * factor, PlayerConfig.MaxFallingSpeed);
    }

    private ZombieIA _zombieIA;

    public void Start(string name, ZombieNode zombie, IFlipper flippers, Marker2D marker2D) {
        _zombieNode = zombie;
        zombie.AddChild(this);

        Body.Configure(name, zombie, flippers, marker2D, MotionConfig.FloorUpDirection);
        zombie.FloorStopOnSlope = true;
        // playerController.FloorBlockOnWall = true;
        zombie.FloorConstantSpeed = true;
        zombie.FloorSnapLength = MotionConfig.SnapLength;

        _zombieIA = new ZombieIA(Handler, zombie);

        OnExecuteStart += Body.SetDelta;
        OnExecuteStart += _zombieIA.HandleIA;
        OnExecuteEnd += Handler.EndFrame;
        OnTransition += args => zombie.Label.Text = args.To.ToString();

        var debugOverlay = DebugOverlayManager.Follow(zombie);

        debugOverlay
            .Text("State", () => CurrentState.Key.ToString()).EndMonitor()
            .OpenBox()
            .Vector("Motion", () => Body.Motion, PlayerConfig.MaxSpeed).SetChartWidth(100).EndMonitor()
            .Graph("MotionX", () => Body.MotionX, -PlayerConfig.MaxSpeed, PlayerConfig.MaxSpeed).AddSeparator(0)
            .AddSerie("MotionY").Load(() => Body.MotionY).EndSerie().EndMonitor()
            .CloseBox()
            .Graph("Floor", () => Body.IsOnFloor()).Keep(10).SetChartHeight(10)
            .AddSerie("Slope").Load(() => Body.IsOnSlope()).EndSerie().EndMonitor()
            .GraphSpeed("Speed", PlayerConfig.JumpSpeed * 2).EndMonitor()
            .Text("Floor", () => Body.GetFloorCollisionInfo()).EndMonitor()
            .Text("Ceiling", () => Body.GetCeilingCollisionInfo()).EndMonitor()
            .Text("Wall", () => Body.GetWallCollisionInfo()).EndMonitor();

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
                Body.FaceTo(CharacterManager.PlayerNode.PlayerDetector);
                Body.MotionY = EnemyConfig.MiniJumpOnAttack.y;
                Body.MotionX = EnemyConfig.MiniJumpOnAttack.x * (Body.IsToTheLeftOf(CharacterManager.PlayerNode.PlayerDetector) ? 1 : -1);
                _zombieNode.PlayAnimationAttacked();
                _stateTimer.Restart().SetAlarm(1f);
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

                if (Body.IsToTheLeftOf(CharacterManager.PlayerNode.PlayerDetector)) {
                    _zombieNode.AnimationDieLeft.PlayOnce(true);
                } else {
                    _zombieNode.AnimationDieRight.PlayOnce(true);
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