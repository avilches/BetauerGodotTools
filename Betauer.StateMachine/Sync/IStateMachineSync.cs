using System;

namespace Betauer.StateMachine.Sync {
    public interface IStateMachineSync<TStateKey, TTransitionKey, TState> : 
        IStateMachine<TStateKey, TTransitionKey, TState> 
        where TTransitionKey : Enum where TStateKey : Enum {
        
        public void Execute(float delta);
        
    }
}