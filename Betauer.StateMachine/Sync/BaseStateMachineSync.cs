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

        public void Execute() {
            if (IsDisposed) return;
            
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
                        newState.Exit();
                    } else {
                        // Special case: 
                        // Exit from all the states from the stack, in order
                        while (Stack.Count > 0) {
                            var exitingState = Stack.Pop();
                            var to = Stack.Count > 0 ? Stack.Peek().Key : change.State.Key;
                            ExitEvent(exitingState, to);
                            exitingState.Exit();
                        }
                    }
                    CurrentState = TransitionTo(change, out var oldState);
                    Stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    CurrentState.Enter();
                } else if (change.Type == CommandType.Pop) {
                    var newState = Stack.Pop();
                    ExitEvent(newState, change.State.Key);
                    newState.Exit();
                    CurrentState = TransitionTo(change, out var oldState);
                    AwakeEvent(CurrentState, oldState.Key);
                    CurrentState.Awake();
                } else if (change.Type == CommandType.Push) {
                    SuspendEvent(CurrentState, change.State!.Key);
                    CurrentState.Suspend();
                    CurrentState = TransitionTo(change, out var oldState);
                    Stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    CurrentState.Enter();
                } else if (change.Type == CommandType.PopPush) {
                    var newState = Stack.Pop();
                    ExitEvent(newState, change.State.Key);
                    newState.Exit();
                    CurrentState = TransitionTo(change, out var oldState);
                    Stack.Push(CurrentState);
                    EnterEvent(CurrentState, oldState.Key);
                    CurrentState.Enter();
                }
                CurrentState.Execute();
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
            }
            
            
        }
    }
}