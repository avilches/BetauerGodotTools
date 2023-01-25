using System;
using System.Collections.Generic;

namespace Betauer.StateMachine; 

public readonly struct Command<TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum {  
        
    public readonly CommandType Type;
    public readonly TStateKey StateKey;
    public readonly TEventKey EventKey;

    public bool IsNone() => Type == CommandType.None;
    public bool IsSet() => Type == CommandType.Set;
    public bool IsPop() => Type == CommandType.Pop;
    public bool IsPopPush() => Type == CommandType.PopPush;
    public bool IsPush() => Type == CommandType.Push;
    public bool IsTrigger() => Type == CommandType.Trigger;

    public bool IsSet(TStateKey? key) {
        return IsSet() && EqualityComparer<TStateKey>.Default.Equals(StateKey, key);
    }

    internal Command(CommandType type, TStateKey stateKey, TEventKey eventKey) {
        Type = type;
        StateKey = stateKey;
        EventKey = eventKey;
    }
}