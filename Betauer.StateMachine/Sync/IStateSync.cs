using System;

namespace Betauer.StateMachine.Sync {
    public interface IStateSync<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public void Enter(TStateKey from);
        public void Awake(TStateKey from);
        public ExecuteContext<TStateKey, TTransitionKey>.Response Execute(ExecuteContext<TStateKey, TTransitionKey> executeContext);
        public void Suspend(TStateKey to);
        public void Exit(TStateKey to);
    }
}