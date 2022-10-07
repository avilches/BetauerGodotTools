using System;
using Godot;

namespace Betauer.StateMachine.Sync {
    public class StateMachineNodeSync<TStateKey, TTransitionKey> : BaseStateMachineNode, IStateMachineSync<TStateKey, TTransitionKey, StateNodeSync<TStateKey, TTransitionKey>> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        
        private class RealStateMachineNode : BaseStateMachineSync<TStateKey, TTransitionKey, IStateSync<TStateKey, TTransitionKey>> { 
        
            internal RealStateMachineNode(TStateKey initialState, string? name = null) : base(initialState, name) {
            }

            public StateNodeBuilderSync<TStateKey, TTransitionKey> State(TStateKey stateKey) {
                return new StateNodeBuilderSync<TStateKey, TTransitionKey>(stateKey, AddState);
            }
        }

        private readonly RealStateMachineNode _stateMachineSyncSync;

        public ProcessMode Mode { get; set; }
        public IStateMachineSync<TStateKey, TTransitionKey, IStateSync<TStateKey, TTransitionKey>> StateMachineSyncSync => _stateMachineSyncSync;
        public StateNodeSync<TStateKey, TTransitionKey> CurrentState => (StateNodeSync<TStateKey, TTransitionKey>)_stateMachineSyncSync.CurrentState;
        
        public string? Name => _stateMachineSyncSync.Name; 

        public StateMachineNodeSync(TStateKey initialState, string? name = null, ProcessMode mode = ProcessMode.Idle) {
            _stateMachineSyncSync = new RealStateMachineNode(initialState, name);
            Mode = mode;
        }
        public bool IsState(TStateKey state) => _stateMachineSyncSync.IsState(state);
        public void AddOnEnter(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.AddOnEnter(e);
        public void AddOnAwake(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.AddOnAwake(e);
        public void AddOnSuspend(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.AddOnSuspend(e);
        public void AddOnExit(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.AddOnExit(e);
        public void AddOnTransition(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.AddOnTransition(e);
        public void AddOnExecuteStart(Action<float, TStateKey> e) => _stateMachineSyncSync.AddOnExecuteStart(e);
        public void AddOnExecuteEnd(Action<TStateKey> e) => _stateMachineSyncSync.AddOnExecuteEnd(e);
        public void RemoveOnEnter(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.RemoveOnEnter(e);
        public void RemoveOnAwake(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.RemoveOnAwake(e);
        public void RemoveOnSuspend(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.RemoveOnSuspend(e);
        public void RemoveOnExit(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.RemoveOnExit(e);
        public void RemoveOnTransition(Action<TransitionArgs<TStateKey>> e) => _stateMachineSyncSync.RemoveOnTransition(e);
        public void RemoveOnExecuteStart(Action<float, TStateKey> e) => _stateMachineSyncSync.RemoveOnExecuteStart(e);
        public void RemoveOnExecuteEnd(Action<TStateKey> e) => _stateMachineSyncSync.RemoveOnExecuteEnd(e);
        public StateNodeBuilderSync<TStateKey, TTransitionKey> State(TStateKey stateKey) => _stateMachineSyncSync.State(stateKey);
        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerContext<TStateKey>.Response> transition) => _stateMachineSyncSync.On(transitionKey, transition);
        public void AddState(StateNodeSync<TStateKey, TTransitionKey> stateSync) => _stateMachineSyncSync.AddState(stateSync);
        public void Enqueue(TTransitionKey name) => _stateMachineSyncSync.Enqueue(name);
        public void Execute(float delta) => _stateMachineSyncSync.Execute(delta);

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