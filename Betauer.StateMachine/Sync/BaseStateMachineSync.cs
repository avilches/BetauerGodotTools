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
            BeforeEvent();
            var change = GetNextStateChange(); // If there is no change in state, the conditions will be evaluated here
            if (change.Type == CommandType.Stay) {
                // Do nothing
            } else if (change.Type == CommandType.Set) {
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
                EnterEvent(CurrentState, oldState.Key);
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
                SuspendEvent(CurrentState, change.Destination.Key);
                CurrentState.Suspend();
                var oldState = CurrentState;
                CurrentState = change.Destination;
                Stack.Push(CurrentState);
                TransitionEvent(oldState, CurrentState);
                EnterEvent(CurrentState, oldState.Key);
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
            } else if (change.Type == CommandType.Init) {
                CurrentState = change.Destination;
                Stack.Push(CurrentState);
                EnterEvent(CurrentState, CurrentState.Key);
                CurrentState.Enter();
            }
            CurrentState.Execute();
            AfterEvent();
        } catch (Exception) {
            CurrentState = currentStateBackup;
            throw;
        }
        
            
    }
}