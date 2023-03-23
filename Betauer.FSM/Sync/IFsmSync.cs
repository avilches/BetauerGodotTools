using System;

namespace Betauer.FSM.Sync; 

public interface IFsmSync<TStateKey, TEventKey, TState> : 
    IFsm<TStateKey, TEventKey, TState> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    public void Execute();
        
}