using System;
using System.Collections.Generic;

namespace Betauer.StateMachine; 

public abstract class BaseStateBuilder<TBuilder, TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum
    where TBuilder : class {
    protected readonly TStateKey Key;
    protected readonly List<Condition<TStateKey, TEventKey>> Conditions = new();
    protected Dictionary<TEventKey, Event<TStateKey, TEventKey>>? Events;

    protected BaseStateBuilder(TStateKey key) {
        Key = key;
    }


    public ConditionBuilder<TBuilder, TStateKey, TEventKey> If(Func<bool> condition) {
        return new ConditionBuilder<TBuilder, TStateKey, TEventKey>(this as TBuilder, condition, c => {
            if (c.Execute != null) {
                Conditions.Add(new Condition<TStateKey, TEventKey>(c.Predicate, c.Execute));
            } else {
                Conditions.Add(new Condition<TStateKey, TEventKey>(c.Predicate, c.Result));
            }
        });
    }

    public EventBuilder<TBuilder, TStateKey, TEventKey> On(TEventKey eventKey) {
        Events ??= new Dictionary<TEventKey, Event<TStateKey, TEventKey>>();
        return new EventBuilder<TBuilder, TStateKey, TEventKey>(this as TBuilder, eventKey, c => {
            if (c.Execute != null) {
                Events[eventKey] = new Event<TStateKey, TEventKey>(c.EventKey, c.Execute);
            } else {
                Events[eventKey] = new Event<TStateKey, TEventKey>(c.EventKey, c.Result);
            }
        });
    }
}