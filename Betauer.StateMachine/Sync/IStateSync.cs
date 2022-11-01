using System;

namespace Betauer.StateMachine.Sync {
    public interface IStateSync<TStateKey, TTransitionKey> : IState<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public void Enter();
        public void Awake();
        public void Execute();
        public Command<TStateKey, TTransitionKey> Next(ConditionContext<TStateKey, TTransitionKey> conditionContext);
        public void Suspend();
        public void Exit();
    }
}