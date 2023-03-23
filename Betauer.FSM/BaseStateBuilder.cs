using System;
using System.Collections.Generic;

namespace Betauer.FSM;

public abstract class BaseStateBuilder<TBuilder, TStateKey, TEventKey>
    where TStateKey : Enum
    where TEventKey : Enum
    where TBuilder : class {
    protected readonly TStateKey Key;
    protected List<Condition<TStateKey, TEventKey>>? Conditions;
    protected Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? EventRules;

    protected BaseStateBuilder(TStateKey key) {
        Key = key;
    }
    public ConditionBuilder<TBuilder, TStateKey, TEventKey> If(Func<bool> condition) {
        Conditions ??= new List<Condition<TStateKey, TEventKey>>();
        return new ConditionBuilder<TBuilder, TStateKey, TEventKey>((this as TBuilder)!, condition, c => {
            var condition = c.Execute != null
                ? new Condition<TStateKey, TEventKey>(c.Predicate, c.Execute)
                : new Condition<TStateKey, TEventKey>(c.Predicate, c.Result);
            Conditions.Add(condition);
        });
    }

    public EventBuilder<TBuilder, TStateKey, TEventKey> On(TEventKey eventKey) {
        EventRules ??= new Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>();
        return new EventBuilder<TBuilder, TStateKey, TEventKey>((this as TBuilder)!, eventKey, c => {
            var eventRule = c.Execute != null
                ? new EventRule<TStateKey, TEventKey>(c.Execute)
                : new EventRule<TStateKey, TEventKey>(c.Result);
            EventRules[eventKey] = eventRule;
        });
    }
}