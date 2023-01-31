using System;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async; 

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
            InvokeBeforeEvent(); // Execute OnBefore, so it can change the values used in the conditions...
            var change = GetNextStateChange(); // then evaluate conditions to get the new state.
            if (change.Type == CommandType.Stay) {
            } else if (change.Type == CommandType.Set) {
                while (Stack.Count > 0) {
                    var exitingState = Stack.Pop();
                    var to = Stack.Count > 0 ? Stack.Peek().Key : change.Destination.Key;
                    InvokeExitEvent(exitingState.Key, to, change.Type);
                    await exitingState.Exit();
                }
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                await CurrentState.Enter();
            } else if (change.Type == CommandType.Pop) {
                Stack.Pop();
                InvokeExitEvent(CurrentState.Key, change.Destination.Key, change.Type);
                await CurrentState.Exit();
                var oldState = ChangeState(change.Destination);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeAwakeEvent(oldState.Key, change.Destination.Key, change.Type);
                await CurrentState.Awake();
            } else if (change.Type == CommandType.Push) {
                InvokeSuspendEvent(CurrentState.Key, change.Destination.Key, change.Type);
                await CurrentState.Suspend();
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                await CurrentState.Enter();
            } else if (change.Type == CommandType.PopPush) {
                Stack.Pop();
                InvokeExitEvent(CurrentState.Key, change.Destination.Key, change.Type);
                await CurrentState.Exit();
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                await CurrentState.Enter();
            } else if (change.Type == CommandType.Init) {
                var oldState = ChangeState(change.Destination);
                Stack.Push(CurrentState);
                InvokeTransitionEvent(oldState.Key, change.Destination.Key, change.Type);
                InvokeEnterEvent(oldState.Key, change.Destination.Key, change.Type);
                await CurrentState.Enter();
            }
            await CurrentState.Execute();
            InvokeAfterEvent();
        } catch (Exception) {
            ChangeState(currentStateBackup);
            throw;
        } finally {
            Available = true;
        }
    }
}