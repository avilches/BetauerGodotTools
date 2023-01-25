using System;
using System.Collections.Generic;

namespace Betauer.StateMachine; 

public interface IState<TStateKey, TEventKey> 
    where TStateKey : Enum where TEventKey : Enum {
        
    public TStateKey Key { get; }
    public Dictionary<TEventKey, Event<TStateKey, TEventKey>>? Events { get; }
}