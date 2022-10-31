using System;

namespace Betauer.StateMachine.Sync {
    public interface IStateSync<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public void Enter();
        public void Awake();
        public void Execute();
        public Context<TStateKey, TTransitionKey>.Response Next(Context<TStateKey, TTransitionKey> context);
        public void Suspend();
        public void Exit();
    }
}