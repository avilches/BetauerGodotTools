using System;
using Betauer.Core.Time;
using Betauer.StateMachine.Sync;
using Godot;
using Veronenger.Character.Handler;

namespace Veronenger.Character.Enemy;

public class MeleeAI : StateMachineSync<MeleeAI.State, MeleeAI.Event>, ICharacterAI {
    private readonly CharacterController _controller;
    private readonly Sensor _sensor;
    private readonly GodotStopwatch _stateTimer = new();
    private readonly GodotStopwatch _inStateTimer = new();

    public enum State { 
        Patrol,
        PatrolStop,
        Confusion,
        Hurt,
        EndAttacked,
        ChasePlayer,
        Flee
    }
    
    public enum Event {
    }

    public static ICharacterAI Create(ICharacterHandler handler, Sensor sensor) {
        if (handler is CharacterController controller) return new MeleeAI(controller, sensor);
        if (handler is InputActionCharacterHandler) return DoNothingAI.Instance;
        throw new Exception($"Unknown handler: {handler.GetType()}");
    }

    public MeleeAI(CharacterController controller, Sensor sensor) : base(State.Patrol, "ZombieIA") {
        _controller = controller;
        _sensor = sensor;
        Config();
    }

    public string GetState() {
        return CurrentState.Key.ToString();
    }

    private void Config() {
        State(State.Patrol)
            .Enter(() => _stateTimer.Restart())
            .Execute(() => Advance(0.5f))
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(_sensor.IsOnWall).Set(State.PatrolStop)
            .If(_sensor.IsFrontFloorFinishing).Set(State.PatrolStop)
            .If(() => _stateTimer.Elapsed > 4f).Set(State.PatrolStop)
            .Build();
        
        State(State.Hurt).If(() => !_sensor.IsHurt()).Set(State.EndAttacked).Build();
        State(State.EndAttacked)
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => _sensor.Status.HealthPercent < 0.25f).Set(State.Flee)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(() => true).Set(State.ChasePlayer)
            .Build();
        
        State(State.Flee)
            .Enter(() => {
                _stateTimer.Restart();
            })
            .Execute(() => {
                _sensor.FaceOppositePlayer();
                Advance(2f);
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => _stateTimer.Elapsed > 6f).Set(State.PatrolStop)
            .Build();
        
        State(State.ChasePlayer)
            .Execute(() => {
                _sensor.FaceToPlayer();
                var distanceToPlayer = _sensor.DistanceToPlayer();
                if (distanceToPlayer > 30f) {
                    if (!_sensor.IsFrontFloorFinishing()) {
                        // Avoid fall chasing the player
                        Advance();
                    }
                } else if (distanceToPlayer < 30) {
                    if (!_sensor.IsBackFloorFinishing()) {
                        // Avoid fall chasing the player backwards
                        Advance(-3f);
                    }
                }

                if (distanceToPlayer < 40 && !_sensor.IsAttacking() && GD.Randf() < 0.02f) {
                    _controller.AttackController.QuickPress();
                }
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(() => !_sensor.IsPlayerInsight()).Set(State.Confusion)
            .Build();

        State(State.Confusion)
            .Enter(() => {
                _stateTimer.Restart();
                _inStateTimer.Restart();
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(() => _stateTimer.Elapsed > 1.2f * 6).Then((ctx) => {
                _sensor.Flip();
                return ctx.Set(State.Patrol);
            })
            .If(() => _inStateTimer.Elapsed > 1.2f).Then((ctx) => {
                _sensor.Flip();
                _inStateTimer.Restart();
                return ctx.None();
            })
            .Build();
        
        State(State.PatrolStop)
            .Enter(() => {
                _stateTimer.Restart();
            })
            .If(_sensor.IsHurt).Set(State.Hurt)
            .If(_sensor.IsPlayerInsight).Set(State.ChasePlayer)
            .If(() => _stateTimer.Elapsed > 4f).Then((ctx) => {
                _sensor.Flip();
                return ctx.Set(State.Patrol);
            })
            .Build();
    }

    private void Advance(float factor = 1f) {
        _controller.DirectionalController.XInput = _sensor.FacingRight * factor;
    }

    public void EndFrame() {
        // GD.Print("Pressed:"+_controller.Jump.IsPressed()+
        // " JustPressed:"+_controller.Jump.IsJustPressed()+
        // " Released:"+_controller.Jump.IsReleased());

        _controller.DirectionalController.XInput = 0;
        _controller.EndFrame();
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
        public bool IsPlayerInsight() => _zombieNode.FacePlayerDetector.IsColliding(); // || _zombieNode.BackPlayerDetector.IsColliding();
        public float DistanceToPlayer() => _zombieNode.DistanceToPlayer();                                                            

        public bool IsFrontFloorFinishing() => IsFacingRight
            ? !_zombieNode.FinishFloorRight.IsColliding()
            : !_zombieNode.FinishFloorLeft.IsColliding();

        public bool IsBackFloorFinishing() => IsFacingRight
            ? !_zombieNode.FinishFloorLeft.IsColliding()
            : !_zombieNode.FinishFloorRight.IsColliding();

        public void FaceOppositePlayer() => _body.FaceOppositeTo(GetPlayerGlobalPosition());
        public void FaceToPlayer() => _body.FaceTo(GetPlayerGlobalPosition());

        public EnemyStatus Status => _zombieNode.Status;
    }
}