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
            var change = GetNextStateChange(); // Evaluate conditions to get the new state
            InvokeBeforeEvent(CurrentState.Key, change.Destination.Key, change.Type);
            if (change.Type == CommandType.Stay) {
                InvokeTransitionEvent(CurrentState.Key, CurrentState.Key, change.Type);
            } else if (change.Type == CommandType.Set) {
                while (Stack.Count > 0) {
                    var exitingState = Stack.Pop();
                    var to = Stack.Count > 0 ? Stack.Peek().Key : change.Destination.Key;
                    InvokeExitEvent(exitingState.Key, to, change.Type);
                    exitingState.Exit();
                }
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                CurrentState.Enter();
            } else if (change.Type == CommandType.Pop) {
                Stack.Pop();
                InvokeExitEvent(CurrentState.Key, change.Destination.Key, change.Type);
                CurrentState.Exit();
                var oldState = ChangeState(change.Destination);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeAwakeEvent(oldState.Key, change.Destination.Key, change.Type);
                CurrentState.Awake();
            } else if (change.Type == CommandType.Push) {
                InvokeSuspendEvent(CurrentState.Key, change.Destination.Key, change.Type);
                CurrentState.Suspend();
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                CurrentState.Enter();
            } else if (change.Type == CommandType.PopPush) {
                Stack.Pop();
                InvokeExitEvent(CurrentState.Key, change.Destination.Key, change.Type);
                CurrentState.Exit();
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                CurrentState.Enter();
            } else if (change.Type == CommandType.Init) {
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                CurrentState.Enter();
            }
            CurrentState.Execute();
            InvokeAfterEvent(currentStateBackup.Key, change.Destination.Key, change.Type);
        } catch (Exception) {
            ChangeState(currentStateBackup);
            throw;
        }
        
            
    }
}