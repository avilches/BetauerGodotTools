using System;
using Betauer.Core.Time;
using Betauer.StateMachine.Sync;
using Godot;
using Veronenger.Character.Handler;

namespace Veronenger.Character.Enemy;

public class ZombieAI : StateMachineSync<ZombieAI.State, ZombieAI.Event>, ICharacterAI {
    private readonly CharacterController _controller;
    private readonly Sensor _sensor;
    private readonly GodotStopwatch _stateTimer = new GodotStopwatch().Start();

    public enum State { 
        Patrol,
        PatrolStop,
        Attacked,
        Flee
    }
    
    public enum Event {
    }

    public static ICharacterAI Create(ICharacterHandler handler, Sensor sensor) {
        if (handler is CharacterController controller) return new ZombieAI(controller, sensor);
        if (handler is InputActionCharacterHandler) return DoNothingAI.Instance;
        throw new Exception($"Unknown handler: {handler.GetType()}");
    }

    public ZombieAI(CharacterController controller, Sensor sensor) : base(State.Patrol, "ZombieIA") {
        _controller = controller;
        _sensor = sensor;
        Config();
    }

    public string GetState() {
        return CurrentState.Key.ToString();
    }

    private void Config() {
        State(State.Patrol)
            .Enter(() => _stateTimer.Reset())
            .Execute(Advance)
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(() => _stateTimer.Elapsed > 2f).Set(State.PatrolStop)
            .Build();
        
        State(State.Attacked)
            .If(() => !_sensor.IsAttacked()).Set(State.Flee)
            .Build();
        
        State(State.Flee)
            .Enter(() => {
                _stateTimer.Reset();
            })
            .Execute(() => {
                FaceOppositePlayer();
                Advance();
            })
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .If(() => _stateTimer.Elapsed > 4f).Set(State.PatrolStop)
            .Build();
        
        State(State.PatrolStop)
            .Enter(() => {
                _stateTimer.Reset();
            })
            .If(() => _stateTimer.Elapsed > 4f).Then((ctx) => {
                _sensor.Flip();
                return ctx.Set(State.Patrol);
            })
            .If(_sensor.IsAttacked).Set(State.Attacked)
            .Build();

    }

    private void FaceOppositePlayer() {
        _sensor.FaceOppositePlayer();
    }

    private void Advance() {
        _controller.DirectionalController.XInput = _sensor.IsFacingRight ? 1 : -1;
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
        private readonly ZombieStateMachine _zombieStateMachine;
        private readonly KinematicPlatformMotion _body;
        private readonly Func<Vector2> GetPlayerGlobalPosition;

        public Sensor(ZombieNode zombieNode, ZombieStateMachine zombieStateMachine, KinematicPlatformMotion body, Func<Vector2> playerGlobalPosition) {
            _zombieNode = zombieNode;
            _zombieStateMachine = zombieStateMachine;
            _body = body;
            GetPlayerGlobalPosition = playerGlobalPosition;
        }

        public bool IsFacingRight => _body.IsFacingRight;
        public void Flip() => _body.Flip();
        public bool IsAttacked() => _zombieStateMachine.IsState(ZombieState.Attacked);

        public void FaceOppositePlayer() {
            _body.FaceOppositeTo(GetPlayerGlobalPosition());
        }
    }

}