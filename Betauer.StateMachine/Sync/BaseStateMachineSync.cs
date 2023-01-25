using System;

namespace Betauer.StateMachine.Sync; 

public abstract class BaseStateMachineSync<TStateKey, TEventKey, TState> : 
    BaseStateMachine<TStateKey, TEventKey, TState>, 
    IStateMachineSync<TStateKey, TEventKey, TState> 
    where TStateKey : Enum 
    where TEventKey : Enum
    where TState : class, IStateSync<TStateKey, TEventKey> {

        
        
    protected BaseStateMachineSync(TStateKey initialState, string? name = null) : base(initialState, name) {
    }

    public void Execute() {
        if (IsDisposed) return;
            
        var currentStateBackup = CurrentState;
        try {
            var change = NoChange;
            if (HasPendingEvent) {
                ConsumeEvent(PendingEvent, out var eventCommand);
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
                    newState.Exit();
                } else {
                    // Special case: 
                    // Exit from all the states from the stack, in order
                    while (Stack.Count > 0) {
                        var exitingState = Stack.Pop();
                        var to = Stack.Count > 0 ? Stack.Peek().Key : change.Destination.Key;
                        ExitEvent(exitingState, to);
                        exitingState.Exit();
                    }
                }
                var oldState = CurrentState;
                CurrentState = change.Destination;
                Stack.Push(CurrentState);
                TransitionEvent(oldState, CurrentState);
                // There is no CurrentState (oldState) the first time, so the enter event is executed with from = itself
                EnterEvent(CurrentState, oldState != null ? oldState.Key: CurrentState.Key);
                CurrentState.Enter();
            } else if (change.Type == CommandType.Pop) {
                var newState = Stack.Pop();
                ExitEvent(newState, change.Destination.Key);
                newState.Exit();
                var oldState = CurrentState;
                CurrentState = change.Destination;
                TransitionEvent(oldState, CurrentState);
                AwakeEvent(CurrentState, oldState.Key);
                CurrentState.Awake();
            } else if (change.Type == CommandType.Push) {
                if (CurrentState != null) { // CurrentState is null the first time only
                    SuspendEvent(CurrentState, change.Destination!.Key);
                    CurrentState.Suspend();
                }
                var oldState = CurrentState;
                CurrentState = change.Destination;
                Stack.Push(CurrentState);
                TransitionEvent(oldState, CurrentState);
                EnterEvent(CurrentState, oldState != null ? oldState.Key: CurrentState.Key);
                CurrentState.Enter();
            } else if (change.Type == CommandType.PopPush) {
                var oldState = Stack.Pop();
                ExitEvent(oldState, change.Destination.Key);
                oldState.Exit();
                CurrentState = change.Destination;
                Stack.Push(CurrentState);
                TransitionEvent(oldState, CurrentState);
                EnterEvent(CurrentState, oldState.Key);
                CurrentState.Enter();
            }
            BeforeExecute();
            CurrentState.Execute();
            var conditionCommand = CurrentState.Next(ConditionContext);
            if (conditionCommand.IsTrigger() ) {
                ConsumeEvent(conditionCommand.EventKey, out conditionCommand);
            }
            NextChange = CreateChange(ref conditionCommand);
            AfterExecute();
            IsInitialized = true;
        } catch (Exception) {
            NextChange = NoChange;
            CurrentState = currentStateBackup;
            throw;
        }
            
            
            
    }
}