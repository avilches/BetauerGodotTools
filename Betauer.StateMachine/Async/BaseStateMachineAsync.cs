using System;
using System.Threading.Tasks;
using Betauer.StateMachine.Sync;

namespace Betauer.StateMachine.Async {
    public abstract class BaseStateMachineAsync<TStateKey, TEventKey, TState> : 
        BaseStateMachine<TStateKey, TEventKey, TState>, 
        IStateMachineAsync<TStateKey, TEventKey, TState> 

        where TStateKey : Enum 
        where TEventKey : Enum
        where TState : class, IStateAsync<TStateKey, TEventKey> {

        public bool Available { get; protected set; } = true;

        protected BaseStateMachineAsync(TStateKey initialState, string? name = null) : base(initialState, name) {
        }

        public async Task Execute() {
            if (IsDisposed || !Available) return;
            Available = false;
            var currentStateBackup = CurrentState;
            try {
                var change = NoChange;
                if (HasPendingEvent) {
                    HasPendingEvent = false;
                    ExecuteEvent(PendingEvent, out var eventCommand);
                    change = CreateChange(ref eventCommand);
                } else if (!IsInitialized) {
                    var state = FindState(InitialState); // Call to ensure initial state exists
                    change = new Change(state, CommandType.Set);
                } else {
                    change = NextChange;
                }
                if (change.Type == CommandType.Set) {
                    if (Stack.Count == 1) {
                        var newState = Stack.Pop();
                        ExitEvent(newState, change.Destination.Key);
                        await newState.Exit();
                    } else {
                        // Special case: 
                        // Exit from all the states from the stack, in order
                        while (Stack.Count > 0) {
                            var exitingState = Stack.Pop();
                            var to = Stack.Count > 0 ? Stack.Peek().Key : change.Destination.Key;
                            ExitEvent(exitingState, to);
                            await exitingState.Exit();
                        }
                    }
                    var oldState = CurrentState;
                    CurrentState = change.Destination;
                    Stack.Push(CurrentState);
                    TransitionEvent(oldState, CurrentState);
                    // There is no CurrentState (oldState) the first time, so the enter event is executed with from = itself
                    EnterEvent(CurrentState, oldState != null ? oldState.Key: CurrentState.Key);
                    await CurrentState.Enter();
                } else if (change.Type == CommandType.Pop) {
                    var newState = Stack.Pop();
                    ExitEvent(newState, change.Destination.Key);
                    await newState.Exit();
                    var oldState = CurrentState;
                    CurrentState = change.Destination;
                    TransitionEvent(oldState, CurrentState);
                    AwakeEvent(CurrentState, oldState.Key);
                    await CurrentState.Awake();
                } else if (change.Type == CommandType.Push) {
                    if (CurrentState != null) { // CurrentState is null the first time only
                        SuspendEvent(CurrentState, change.Destination!.Key);
                        await CurrentState.Suspend();
                    }
                    var oldState = CurrentState;
                    CurrentState = change.Destination;
                    Stack.Push(CurrentState);
                    TransitionEvent(oldState, CurrentState);
                    EnterEvent(CurrentState, oldState != null ? oldState.Key: CurrentState.Key);
                    await CurrentState.Enter();
                } else if (change.Type == CommandType.PopPush) {
                    var oldState = Stack.Pop();
                    ExitEvent(oldState, change.Destination.Key);
                    await oldState.Exit();
                    CurrentState = change.Destination;
                    Stack.Push(CurrentState);
                    TransitionEvent(oldState, CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    await CurrentState.Enter();
                }
                await CurrentState.Execute();
                var conditionCommand = CurrentState.Next(ConditionContext);
                if (conditionCommand.IsTrigger() ) {
                    ExecuteEvent(conditionCommand.EventKey, out conditionCommand);
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