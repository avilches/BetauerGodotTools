using System;

namespace Betauer.StateMachine.Sync; 

public interface IStateMachineSync<TStateKey, TEventKey, TState> : 
    IStateMachine<TStateKey, TEventKey, TState> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public void Execute();
        
}