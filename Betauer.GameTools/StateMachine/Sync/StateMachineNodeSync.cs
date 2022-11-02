using System;
using Godot;

namespace Betauer.StateMachine.Sync {
    public class StateMachineNodeSync<TStateKey, TEventKey> : 
        StateMachineNode<TStateKey>, IStateMachineSync<TStateKey, TEventKey, StateNodeSync<TStateKey, TEventKey>> 
        where TStateKey : Enum 
        where TEventKey : Enum {
        
        private class RealStateMachineNode : BaseStateMachineSync<TStateKey, TEventKey, IStateSync<TStateKey, TEventKey>> { 
            private StateMachineNodeSync<TStateKey, TEventKey> _owner;

            internal RealStateMachineNode(StateMachineNodeSync<TStateKey, TEventKey> owner, TStateKey initialState, string? name = null) : base(initialState, name) {
                _owner = owner;
            }

            public EventBuilder<StateMachineNodeSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
                TEventKey transitionKey) {
                return On(_owner, transitionKey);
            }

            public StateNodeBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) {
                return new StateNodeBuilderSync<TStateKey, TEventKey>(stateKey, AddState);
            }
        }

        private readonly RealStateMachineNode _stateMachine;

        public IStateMachineSync<TStateKey, TEventKey, IStateSync<TStateKey, TEventKey>> StateMachine => _stateMachine;
        public StateNodeSync<TStateKey, TEventKey> CurrentState => (StateNodeSync<TStateKey, TEventKey>)_stateMachine.CurrentState;
        
        public string? Name => _stateMachine.Name; 

        public StateMachineNodeSync(TStateKey initialState, string? name = null, ProcessMode mode = ProcessMode.Idle) {
            _stateMachine = new RealStateMachineNode(this, initialState, name);
            Mode = mode;
        }
        public bool IsState(TStateKey state) => _stateMachine.IsState(state);
        public void AddOnEnter(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnEnter(e);
        public void AddOnAwake(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnAwake(e);
        public void AddOnSuspend(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnSuspend(e);
        public void AddOnExit(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnExit(e);
        public void AddOnTransition(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnTransition(e);
        public void RemoveOnEnter(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnEnter(e);
        public void RemoveOnAwake(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnAwake(e);
        public void RemoveOnSuspend(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnSuspend(e);
        public void RemoveOnExit(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnExit(e);
        public void RemoveOnTransition(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnTransition(e);
        public StateNodeBuilderSync<TStateKey, TEventKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
        public EventBuilder<StateMachineNodeSync<TStateKey, TEventKey>, TStateKey, TEventKey> On(TEventKey transitionKey) => _stateMachine.On(transitionKey);
        public void AddEvent(TEventKey transitionKey, Event<TStateKey, TEventKey> @event) => _stateMachine.AddEvent(transitionKey, @event);
        public void AddState(StateNodeSync<TStateKey, TEventKey> stateSync) => _stateMachine.AddState(stateSync);
        public void Enqueue(TEventKey name) => _stateMachine.Enqueue(name);

        public void Execute() {
            throw new Exception("Don't call directly to execute. Instead, add the node to the tree");
        }

        public void Execute(float delta) {
            ExecuteStart(delta, CurrentState.Key);
            _stateMachine.Execute();
            ExecuteEnd(CurrentState.Key);
        }

        public override void _Input(InputEvent e) {
            CurrentState._Input(e);
        }

        public override void _UnhandledInput(InputEvent e) {
            CurrentState._UnhandledInput(e);
        }

        public override void _PhysicsProcess(float delta) {
            if (Mode == ProcessMode.Physics) Execute(delta);
            else SetPhysicsProcess(false);
        }

        public override void _Process(float delta) {
            if (Mode == ProcessMode.Idle) Execute(delta);
            else SetProcess(false);
        }
    }
}