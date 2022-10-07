using System;
using System.Threading.Tasks;
using Betauer.StateMachine.Sync;

namespace Betauer.StateMachine.Async {
    public abstract class BaseStateMachineAsync<TStateKey, TTransitionKey, TState> : 
        BaseStateMachine<TStateKey, TTransitionKey, TState>, 
        IStateMachineAsync<TStateKey, TTransitionKey, TState> 

        where TStateKey : Enum 
        where TTransitionKey : Enum
        where TState : class, IStateAsync<TStateKey, TTransitionKey> {

        public bool Available { get; protected set; } = true;

        protected BaseStateMachineAsync(TStateKey initialState, string? name = null) : base(initialState, name) {
        }

        public async Task Execute(float delta) {
            if (IsDisposed || !Available) return;
            lock (LockObject) {
                if (!Available) return;
                Available = false;
            }
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
                    await newState.Exit(change.State.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    AwakeEvent(CurrentState, oldState.Key);
                    await CurrentState.Awake(oldState.Key);
                } else if (change.Type == TransitionType.Push) {
                    SuspendEvent(CurrentState, change.State!.Key);
                    await CurrentState.Suspend(change.State!.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    await CurrentState.Enter(oldState.Key);
                } else if (change.Type == TransitionType.PopPush) {
                    var newState = _stack.Pop();
                    ExitEvent(newState, change.State.Key);
                    await newState.Exit(change.State.Key);
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    await CurrentState.Enter(oldState.Key);
                } else if (change.Type == TransitionType.Set) {
                    if (_stack.Count == 1) {
                        var newState = _stack.Pop();
                        ExitEvent(newState, change.State.Key);
                        await newState.Exit(change.State.Key);
                    } else {
                        // Special case: 
                        // Exit from all the states from the stack, in order
                        while (_stack.Count > 0) {
                            var exitingState = _stack.Pop();
                            var to = _stack.Count > 0 ? _stack.Peek().Key : change.State.Key;
                            ExitEvent(exitingState, to);
                            await exitingState.Exit(to);
                        }
                    }
                    CurrentState = TransitionTo(change, out var oldState);
                    _stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    await CurrentState.Enter(oldState.Key);
                }
                
                ExecuteContext.Delta = delta;
                var transition = await CurrentState.Execute(ExecuteContext);
                
                NextChange = CreateChange(transition);
                IsInitialized = true;
            } catch (Exception) {
                NextChange = NoChange;
                CurrentState = currentStateBackup;
                throw;
            } finally {
                lock (LockObject) {
                    Available = true;
                }
            }
        }
    }
}