using System;

namespace Betauer.StateMachine.Sync {
    public interface IStateSync<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public void Enter();
        public void Awake();
        public void Execute();
        public ExecuteContext<TStateKey, TTransitionKey>.Response Next(ExecuteContext<TStateKey, TTransitionKey> executeContext);
        public void Suspend();
        public void Exit();
    }
}