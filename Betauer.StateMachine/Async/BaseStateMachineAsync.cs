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

        public async Task Execute() {
            if (IsDisposed || !Available) return;
            Available = false;
            var currentStateBackup = CurrentState;
            try {
                var change = NoChange;
                if (HasNextTransitionEnqueued) {
                    HasNextTransitionEnqueued = false;
                    ExecuteTransition(NextTransition, out var transitionCommand);
                    change = CreateChange(ref transitionCommand);
                } else if (!IsInitialized) {
                    var state = FindState(InitialState); // Call to ensure initial state exists
                    change = new Change(state, CommandType.Set);
                } else {
                    change = NextChange;
                }
                if (change.Type == CommandType.Set) {
                    if (Stack.Count == 1) {
                        var newState = Stack.Pop();
                        ExitEvent(newState, change.State.Key);
                        await newState.Exit();
                    } else {
                        // Special case: 
                        // Exit from all the states from the stack, in order
                        while (Stack.Count > 0) {
                            var exitingState = Stack.Pop();
                            var to = Stack.Count > 0 ? Stack.Peek().Key : change.State.Key;
                            ExitEvent(exitingState, to);
                            await exitingState.Exit();
                        }
                    }
                    CurrentState = TransitionTo(change, out var oldState);
                    Stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    await CurrentState.Enter();
                } else if (change.Type == CommandType.Pop) {
                    var newState = Stack.Pop();
                    ExitEvent(newState, change.State.Key);
                    await newState.Exit();
                    CurrentState = TransitionTo(change, out var oldState);
                    AwakeEvent(CurrentState, oldState.Key);
                    await CurrentState.Awake();
                } else if (change.Type == CommandType.Push) {
                    SuspendEvent(CurrentState, change.State!.Key);
                    await CurrentState.Suspend();
                    CurrentState = TransitionTo(change, out var oldState);
                    Stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    await CurrentState.Enter();
                } else if (change.Type == CommandType.PopPush) {
                    var newState = Stack.Pop();
                    ExitEvent(newState, change.State.Key);
                    await newState.Exit();
                    CurrentState = TransitionTo(change, out var oldState);
                    Stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    await CurrentState.Enter();
                }
                await CurrentState.Execute();
                var conditionCommand = CurrentState.Next(ConditionContext);
                if (conditionCommand.IsTrigger() ) {
                    ExecuteTransition(conditionCommand.TransitionKey, out conditionCommand);
                }
                NextChange = CreateChange(ref conditionCommand);
                IsInitialized = true;
            } catch (Exception) {
                NextChange = NoChange;
                CurrentState = currentStateBackup;
                throw;
            } finally {
                Available = true;
            }
        }
    }
}