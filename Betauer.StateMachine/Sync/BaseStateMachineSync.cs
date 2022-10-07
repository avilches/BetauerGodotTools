using System;

namespace Betauer.StateMachine.Sync {
    public abstract class BaseStateMachineSync<TStateKey, TTransitionKey, TState> : 
        BaseStateMachine<TStateKey, TTransitionKey, TState>, 
        IStateMachineSync<TStateKey, TTransitionKey, TState> 

        where TStateKey : Enum 
        where TTransitionKey : Enum
        where TState : class, IStateSync<TStateKey, TTransitionKey> {

        protected BaseStateMachineSync(TStateKey initialState, string? name = null) : base(initialState, name) {
        }

        public void Execute(float delta) {
            if (IsDisposed) return;

            
            
            
            var currentStateBackup = CurrentState;
            try {
                var change = NoChange;
                if (HasNextTransitionEnqueued) {
                    HasNextTransitionEnqueued = false;
                    var triggerTransition = FindTransition(NextTransition);
                    change = CreateChange(triggerTransition);
                } else if (!IsInitialized) {
                    var state = FindState(InitialState); // Call to ensure initial state exists
                    change = new Change(state, TransitionType.Set);
                } else {
                    change = NextChange;
                }
                if (change.Type == TransitionType.Pop) {
                    var newState = _stack.Pop();
                    ExitEvent(newState, change.State.Key);
                    newState.Exit(change.State.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    AwakeEvent(CurrentState, oldState.Key);
                    CurrentState.Awake(oldState.Key);
                } else if (change.Type == TransitionType.Push) {
                    SuspendEvent(CurrentState, change.State!.Key);
                    CurrentState.Suspend(change.State!.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    CurrentState.Enter(oldState.Key);
                } else if (change.Type == TransitionType.PopPush) {
                    var newState = _stack.Pop();
                    ExitEvent(newState, change.State.Key);
                    newState.Exit(change.State.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    CurrentState.Enter(oldState.Key);
                } else if (change.Type == TransitionType.Set) {
                    if (_stack.Count == 1) {
                        var newState = _stack.Pop();
                        ExitEvent(newState, change.State.Key);
                        newState.Exit(change.State.Key);
                    } else {
                        // Special case: 
                        // Exit from all the states from the stack, in order
                        while (_stack.Count > 0) {
                            var exitingState = _stack.Pop();
                            var to = _stack.Count > 0 ? _stack.Peek().Key : change.State.Key;
                            ExitEvent(exitingState, to);
                            exitingState.Exit(to);
                        }
                    }
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    CurrentState.Enter(oldState.Key);
                }
                OnExecuteStart?.Invoke(delta, CurrentState.Key);
                ExecuteContext.Delta = delta;
                var transition = CurrentState.Execute(ExecuteContext);
                OnExecuteEnd?.Invoke(CurrentState.Key);
                NextChange = CreateChange(transition);
                IsInitialized = true;
            } catch (Exception e) {
                NextChange = NoChange;
                CurrentState = currentStateBackup;
                throw;
            }
        }

        public event Action<float, TStateKey>? OnExecuteStart;
        public event Action<TStateKey>? OnExecuteEnd;
        
        public void AddOnExecuteStart(Action<float, TStateKey> e) => OnExecuteStart += e;
        public void AddOnExecuteEnd(Action<TStateKey> e) => OnExecuteEnd += e;
        
        public void RemoveOnExecuteStart(Action<float, TStateKey> e) => OnExecuteStart -= e;
        public void RemoveOnExecuteEnd(Action<TStateKey> e) => OnExecuteEnd -= e;
    }
}