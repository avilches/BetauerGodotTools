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
            BeforeExecute();
            var change = GetChangeFromNextCommand();
            if (change.Type == CommandType.Stay) {
                // Do nothing
            } else if (change.Type == CommandType.Set) {
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
                EnterEvent(CurrentState, oldState.Key);
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
                SuspendEvent(CurrentState, change.Destination!.Key);
                await CurrentState.Suspend();
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
            } else if (change.Type == CommandType.Init) {
                CurrentState = change.Destination;
                Stack.Push(CurrentState);
                EnterEvent(CurrentState, CurrentState.Key);
                await CurrentState.Enter();
            }
            await CurrentState.Execute();
            CurrentState.EvaluateConditions(CommandContext, out NextCommand);
            AfterExecute();
        } catch (Exception) {
            NextCommand = CommandContext.Stay();
            CurrentState = currentStateBackup;
            throw;
        } finally {
            Available = true;
        }
    }
}