using System;
using System.Collections.Generic;

namespace Betauer.StateMachine;

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

    public ConditionBuilder<TBuilder, TStateKey, TEventKey> AsapIf(Func<bool> condition) =>
        If(condition, Condition.Type.Asap);

    public ConditionBuilder<TBuilder, TStateKey, TEventKey> LazyIf(Func<bool> condition) =>
        If(condition, Condition.Type.Lazy);

    public ConditionBuilder<TBuilder, TStateKey, TEventKey> If(Func<bool> condition, Condition.Type type = Condition.Type.Always) {
        Conditions ??= new List<Condition<TStateKey, TEventKey>>();
        return new ConditionBuilder<TBuilder, TStateKey, TEventKey>(type, (this as TBuilder)!, condition, c => {
            var condition = c.Execute != null
                ? new Condition<TStateKey, TEventKey>(c.Type, c.Predicate, c.Execute)
                : new Condition<TStateKey, TEventKey>(c.Type, c.Predicate, c.Result);
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