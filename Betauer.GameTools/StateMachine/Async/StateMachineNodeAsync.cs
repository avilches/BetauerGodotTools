using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine.Async {
    public class StateMachineNodeAsync<TStateKey, TEventKey> : 
        StateMachineNode<TStateKey>, IStateMachineAsync<TStateKey, TEventKey, StateNodeAsync<TStateKey, TEventKey>> 
        where TStateKey : Enum 
        where TEventKey : Enum {
        
        private class RealStateMachineNodeAsync : BaseStateMachineAsync<TStateKey, TEventKey, IStateAsync<TStateKey, TEventKey>> {
            private StateMachineNodeAsync<TStateKey, TEventKey> _owner;
            
            internal RealStateMachineNodeAsync(StateMachineNodeAsync<TStateKey, TEventKey> owner, TStateKey initialState, string? name = null) : base(initialState, name) {
                _owner = owner;
            }

            public EventBuilder<StateMachineNodeAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(
                TEventKey transitionKey) {
                return On(_owner, transitionKey);
            }

            public StateNodeBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) {
                return new StateNodeBuilderAsync<TStateKey, TEventKey>(stateKey, AddState);
            }
        }

        private readonly RealStateMachineNodeAsync _stateMachine;

        public IStateMachineAsync<TStateKey, TEventKey, IStateAsync<TStateKey, TEventKey>> StateMachine => _stateMachine;
        public StateNodeAsync<TStateKey, TEventKey> CurrentState => (StateNodeAsync<TStateKey, TEventKey>)_stateMachine.CurrentState;
        public bool Available => _stateMachine.Available;
        public string? Name => _stateMachine.Name; 

        public StateMachineNodeAsync(TStateKey initialState, string? name = null, ProcessMode mode = ProcessMode.Idle) {
            _stateMachine = new RealStateMachineNodeAsync(this, initialState, name);
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
        public StateNodeBuilderAsync<TStateKey, TEventKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
        public EventBuilder<StateMachineNodeAsync<TStateKey, TEventKey>, TStateKey, TEventKey> On(TEventKey transitionKey) => _stateMachine.On(transitionKey);
        public void AddEvent(TEventKey transitionKey, Event<TStateKey, TEventKey> @event) => _stateMachine.AddEvent(transitionKey, @event);
        public void AddState(StateNodeAsync<TStateKey, TEventKey> state) => _stateMachine.AddState(state);
        public void Enqueue(TEventKey name) => _stateMachine.Enqueue(name);

        public Task Execute() {
            throw new Exception("Don't call directly to execute. Instead, add the node to the tree");
        }

        public void Execute(float delta) {
            if (!Available) return;
            ExecuteStart(delta, CurrentState.Key);
            _stateMachine.Execute().ContinueWith(t => ExecuteEnd(CurrentState.Key));
        }

        public override void _Input(InputEvent e) {
            if (Available) CurrentState._Input(e);
        }

        public override void _UnhandledInput(InputEvent e) {
            if (Available) CurrentState._UnhandledInput(e);
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