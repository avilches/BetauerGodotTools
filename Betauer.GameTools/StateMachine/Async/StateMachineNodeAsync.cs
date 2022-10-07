using System;
using System.Threading.Tasks;
using Godot;

namespace Betauer.StateMachine.Async {
    public class StateMachineNodeAsync<TStateKey, TTransitionKey> : StateMachineNode, IStateMachineAsync<TStateKey, TTransitionKey, StateNodeAsync<TStateKey, TTransitionKey>> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        private class RealStateMachineNodeAsync : BaseStateMachineAsync<TStateKey, TTransitionKey, IStateAsync<TStateKey, TTransitionKey>> { 
            internal RealStateMachineNodeAsync(TStateKey initialState, string? name = null) : base(initialState, name) {
            }
            public StateNodeBuilderAsync<TStateKey, TTransitionKey> State(TStateKey stateKey) {
                return new StateNodeBuilderAsync<TStateKey, TTransitionKey>(stateKey, AddState);
            }
        }

        private readonly RealStateMachineNodeAsync _stateMachine;

        public IStateMachineAsync<TStateKey, TTransitionKey, IStateAsync<TStateKey, TTransitionKey>> StateMachine => _stateMachine;
        public StateNodeAsync<TStateKey, TTransitionKey> CurrentState => (StateNodeAsync<TStateKey, TTransitionKey>)_stateMachine.CurrentState;
        public bool Available => _stateMachine.Available;
        public string? Name => _stateMachine.Name; 

        public StateMachineNodeAsync(TStateKey initialState, string? name = null, ProcessMode mode = ProcessMode.Idle) {
            _stateMachine = new RealStateMachineNodeAsync(initialState, name);
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
        
        
        public StateNodeBuilderAsync<TStateKey, TTransitionKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response> transition) => _stateMachine.On(transitionKey, transition);
        public void AddState(StateNodeAsync<TStateKey, TTransitionKey> state) => _stateMachine.AddState(state);
        public void Enqueue(TTransitionKey name) => _stateMachine.Enqueue(name);
        public Task Execute(float delta) => _stateMachine.Execute(delta);

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