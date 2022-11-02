using System;

namespace Betauer.StateMachine.Sync {
    public interface IStateMachineSync<TStateKey, TEventKey, TState> : 
        IStateMachine<TStateKey, TEventKey, TState> 
        where TEventKey : Enum where TStateKey : Enum {
        
        public void Execute();
        
    }
}