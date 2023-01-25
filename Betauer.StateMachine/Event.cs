using System;

namespace Betauer.StateMachine; 

public class Event<TStateKey, TEventKey> 
    where TStateKey : Enum
    where TEventKey : Enum {
        
    private TEventKey EventKey { get; }
    private Func<EventContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>>? Execute { get; }
    private Command<TStateKey, TEventKey> Result { get; }

    internal Event(TEventKey eventKey, Func<EventContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>> execute) {
        EventKey = eventKey;
        Execute = execute;
    }

    internal Event(TEventKey eventKey, Command<TStateKey, TEventKey> result) {
        EventKey = eventKey;
        Result = result;
    }

    internal Command<TStateKey, TEventKey> GetResult(EventContext<TStateKey, TEventKey> ctx) =>
        Execute?.Invoke(ctx) ?? Result;

}