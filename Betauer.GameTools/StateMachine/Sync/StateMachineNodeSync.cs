using System;
using Godot;

namespace Betauer.StateMachine.Sync {
    public class StateMachineNodeSync<TStateKey, TTransitionKey> : StateMachineNode, IStateMachineSync<TStateKey, TTransitionKey, StateNodeSync<TStateKey, TTransitionKey>> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        
        private class RealStateMachineNode : BaseStateMachineSync<TStateKey, TTransitionKey, IStateSync<TStateKey, TTransitionKey>> { 
        
            internal RealStateMachineNode(TStateKey initialState, string? name = null) : base(initialState, name) {
            }

            public StateNodeBuilderSync<TStateKey, TTransitionKey> State(TStateKey stateKey) {
                return new StateNodeBuilderSync<TStateKey, TTransitionKey>(stateKey, AddState);
            }
        }

        private readonly RealStateMachineNode _stateMachine;

        public ProcessMode Mode { get; set; }
        public IStateMachineSync<TStateKey, TTransitionKey, IStateSync<TStateKey, TTransitionKey>> StateMachine => _stateMachine;
        public StateNodeSync<TStateKey, TTransitionKey> CurrentState => (StateNodeSync<TStateKey, TTransitionKey>)_stateMachine.CurrentState;
        
        public string? Name => _stateMachine.Name; 

        public StateMachineNodeSync(TStateKey initialState, string? name = null, ProcessMode mode = ProcessMode.Idle) {
            _stateMachine = new RealStateMachineNode(initialState, name);
            Mode = mode;
        }
        public bool IsState(TStateKey state) => _stateMachine.IsState(state);
        public void AddOnEnter(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnEnter(e);
        public void AddOnAwake(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnAwake(e);
        public void AddOnSuspend(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnSuspend(e);
        public void AddOnExit(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnExit(e);
        public void AddOnTransition(Action<TransitionArgs<TStateKey>> e) => _stateMachine.AddOnTransition(e);
        public void AddOnExecuteStart(Action<float, TStateKey> e) => _stateMachine.AddOnExecuteStart(e);
        public void AddOnExecuteEnd(Action<TStateKey> e) => _stateMachine.AddOnExecuteEnd(e);
        public void RemoveOnEnter(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnEnter(e);
        public void RemoveOnAwake(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnAwake(e);
        public void RemoveOnSuspend(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnSuspend(e);
        public void RemoveOnExit(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnExit(e);
        public void RemoveOnTransition(Action<TransitionArgs<TStateKey>> e) => _stateMachine.RemoveOnTransition(e);
        public void RemoveOnExecuteStart(Action<float, TStateKey> e) => _stateMachine.RemoveOnExecuteStart(e);
        public void RemoveOnExecuteEnd(Action<TStateKey> e) => _stateMachine.RemoveOnExecuteEnd(e);
        public StateNodeBuilderSync<TStateKey, TTransitionKey> State(TStateKey stateKey) => _stateMachine.State(stateKey);
        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response> transition) => _stateMachine.On(transitionKey, transition);
        public void AddState(StateNodeSync<TStateKey, TTransitionKey> stateSync) => _stateMachine.AddState(stateSync);
        public void Enqueue(TTransitionKey name) => _stateMachine.Enqueue(name);
        public void Execute(float delta) => _stateMachine.Execute(delta);

        public override void _Input(InputEvent e) {
            CurrentState._Input(e);
        }

        public override void _UnhandledInput(InputEvent e) {
            CurrentState._UnhandledInput(e);
        }

        public override void _PhysicsProcess(float delta) {
            if (Mode == ProcessMode.Physics) Process(delta);
            else SetPhysicsProcess(false);
        }

        public override void _Process(float delta) {
            if (Mode == ProcessMode.Idle) Process(delta);
            else SetProcess(false);
        }

        private void Process(float delta) => Execute(delta);
    }
}