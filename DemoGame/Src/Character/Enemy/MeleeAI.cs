using System;
using Betauer;
using Betauer.Core;
using Betauer.Core.Time;
using Betauer.StateMachine.Sync;
using Godot;
using Pcg;
using Veronenger.Character.InputActions;

namespace Veronenger.Character.Enemy;

public class MeleeAI : StateMachineSync<MeleeAI.State, MeleeAI.Event>, ICharacterAI {
    private static readonly PcgRandom Random = new();

    private readonly NpcController _controller;
    private readonly Sensor _sensor;

    public enum State { 
        Sleep,
        Patrol,
        PatrolStop,
        ConfusionLoop,
        Confusion,
        Hurt,
        EndAttacked,
        ChasePlayer,
        Flee
    }
    
    public enum Event {
    }

    public static ICharacterAI Create(ICharacterHandler handler, Sensor sensor) {
        if (handler is NpcController controller) return new MeleeAI(controller, sensor);
        if (handler is PlayerInputActions) return DoNothingAI.Instance;
        throw new Exception($"Unknown handler: {handler.GetType()}");
    }

    public MeleeAI(NpcController controller, Sensor sensor) : base(State.Patrol, "ZombieIA") {
        _controller = controller;
        _sensor = sensor;
        Config();
    }

    public string GetState() {
        return CurrentState.Key.ToString();
    }

    
    private void Config() {
        float SleepTime() => 8; // random.Range(7, 9);
        float PatrolStopTime() => 1; // random.Range(1, 3);
        float PatrolTime() => 5; // random.Range(2, 6); 
        float FleeTimeout() => 10; // random.Range(6, 8); 
        // float ChaseTimeout() => random.Range(8, 12);
        float ChaseTimeout() => 5; // random.Range(4, 6);
        int ConfusionTimes() => 3; 
        float ConfusionTime() => 1f;

        var fleeSpeed = 2f;
        var chasingSpeed = Random.Range(0.9f, 1.1f);
        var patrolSpeed = Random.Range(0.4f, 0.6f);

        var fleeIsHealthPercentIsLessThan = 0.25f;

        var minDistanceToApproach = 30f + Random.Range(-10, 10);
        var attacksPerSecondsProbability = Random.Range(1f, 1.3f);
        var minDistanceToAttack = 40f + Random.Range(-15, 15);

        GodotStopwatch stateTimer = new(false, true);

        State(State.Sleep)
            .Enter(() => {
                stateTimer.Restart().SetAlarm(SleepTime());
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => _sensor.DistanceToPlayer() < 100).Set(State.ConfusionLoop)
            .If(stateTimer.IsAlarm).Set(State.Patrol)
            .Build();

        var patrols = 4;
        State(State.Patrol)
            .Enter(() => {
                stateTimer.Restart().SetAlarm(PatrolTime());
                patrols = 4;
            })
            .Awake(() => {
                stateTimer.Restart().SetAlarm(PatrolTime());
                patrols--;
            })
            .Execute(() => Advance(patrolSpeed))
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => _sensor.IsOnWall() || _sensor.IsFrontFloorFinishing() || stateTimer.IsAlarm()).Push(State.PatrolStop)
            .If(() => _sensor.CanSeeThePlayer() && !_sensor.IsFrontFloorFinishing()).Set(State.ChasePlayer)
            .If(() => patrols == 0).Set(State.Sleep)
            .Build();

        State(State.PatrolStop)
            .Enter(() => {
                stateTimer.Restart().SetAlarm(PatrolStopTime());
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => _sensor.CanSeeThePlayer() && !_sensor.IsFrontFloorFinishing()).Set(State.ChasePlayer)
            .If(stateTimer.IsAlarm).Then((ctx) => {
                _sensor.Flip();
                return ctx.Pop();
            })
            .Build();
        
        State(State.Hurt).If(() => !_sensor.IsHurt()).Set(State.EndAttacked).Build();
        State(State.EndAttacked)
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => _sensor.Status.HealthPercent < fleeIsHealthPercentIsLessThan).Set(State.Flee)
            .If(() => true).Set(State.ConfusionLoop)
            .Build();

        State(State.Flee)
            .Enter(() => {
                stateTimer.Restart().SetAlarm(FleeTimeout());
            })
            .Execute(() => {
                _sensor.FaceOppositePlayer();
                Advance(fleeSpeed);
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => stateTimer.IsAlarm()).Set(State.Sleep)
            .Build();

        State(State.ChasePlayer)
            .Enter(() => {
                stateTimer.Restart().SetAlarm(ChaseTimeout());
            })
            .Execute(() => {
                if (_sensor.IsAttacking()) return;
                _sensor.FaceToPlayer();
                var distanceToPlayer = _sensor.DistanceToPlayer();
                if (distanceToPlayer > minDistanceToApproach) {
                    Advance(chasingSpeed);
                }

                if (distanceToPlayer < minDistanceToAttack && Random.NextFloat() < attacksPerSecondsProbability * _sensor.Delta) {
                    stateTimer.Restart();
                    _controller.Attack.SimulatePress();
                }
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => !_sensor.CanSeeThePlayer()).Set(State.ConfusionLoop)
            .If(() => stateTimer.IsAlarm()).Then(ctx => {
                _sensor.FaceOppositePlayer();
                return ctx.Set(State.Patrol);
            })
            .If(() => _sensor.IsFrontFloorFinishing()).Set(State.Patrol)
            .Build();

        var times = 0;
        State(State.ConfusionLoop)
            .Enter(() => times = ConfusionTimes())
            .If(() => times > 0).Push(State.Confusion)
            .If(() => true).Set(State.Patrol)
            .Suspend(() => times--)
            .Build();

        State(State.Confusion)
            .Enter(() => stateTimer.Restart().SetAlarm(ConfusionTime()))
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => _sensor.CanSeeThePlayer() && !_sensor.IsFrontFloorFinishing()).Set(State.ChasePlayer)
            .If(() => stateTimer.IsAlarm()).Pop()
            .Exit(() => _sensor.Flip())
            .Build();

        // OnTransition += (args) => GD.Print(args.From + " " + args.To);
    }

    private void Advance(float factor) {
        _controller.Lateral.SimulatePress(_sensor.FacingRight * factor);
    }

    public void EndFrame() {
        // GD.Print("Pressed:"+_controller.Jump.IsPressed()+
        // " JustPressed:"+_controller.Jump.IsJustPressed+
        // " Released:"+_controller.Jump.IsReleased());

        _controller.ClearState();
    }

    public class Sensor {
        private readonly ZombieNode _zombieNode;
        private readonly KinematicPlatformMotion _body;
        private readonly Func<Vector2> GetPlayerGlobalPosition;

        public Sensor(ZombieNode zombieNode, KinematicPlatformMotion body, Func<Vector2> playerGlobalPosition) {
            _zombieNode = zombieNode;
            _body = body;
            GetPlayerGlobalPosition = playerGlobalPosition;
        }

        public int FacingRight => _body.FacingRight;
        public bool IsFacingRight => _body.IsFacingRight;
        public void Flip() => _body.Flip();
        public bool IsHurt() => _zombieNode.IsState(ZombieState.Hurt);
        public bool IsAttacking() => _zombieNode.IsState(ZombieState.Attacking);
        public bool IsOnWall() => _body.IsOnWall() && _body.IsOnWallRight() == IsFacingRight;

        public bool IsFacingToPlayer() => _zombieNode.IsFacingToPlayer();
        public bool CanSeeThePlayer() => _zombieNode.CanSeeThePlayer();

        public float DistanceToPlayer() => _zombieNode.DistanceToPlayer();                                                            

        public bool IsFrontFloorFinishing() => IsFacingRight
            ? !_zombieNode.FinishFloorRight.IsColliding()
            : !_zombieNode.FinishFloorLeft.IsColliding();

        public bool IsBackFloorFinishing() => IsFacingRight
            ? !_zombieNode.FinishFloorLeft.IsColliding()
            : !_zombieNode.FinishFloorRight.IsColliding();

        public void FaceOppositePlayer() => _body.FaceOppositeTo(GetPlayerGlobalPosition());
        public void FaceToPlayer() => _body.FaceTo(GetPlayerGlobalPosition());
        public double Delta => _zombieNode.Delta;

        public EnemyStatus Status => _zombieNode.Status;
    }
}