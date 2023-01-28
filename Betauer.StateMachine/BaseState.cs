using System;
using System.Collections.Generic;

namespace Betauer.StateMachine; 

public abstract class BaseState<TStateKey, TEventKey> : 
    IState<TStateKey, TEventKey>
    where TStateKey : Enum 
    where TEventKey : Enum {
        
    public TStateKey Key { get; }
    private readonly Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? _eventRules;
    private readonly Condition<TStateKey, TEventKey>[]? _conditions;

    protected BaseState(
        TStateKey key,
        Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? eventRules,
        Condition<TStateKey, TEventKey>[]? conditions) {
        Key = key;
        _conditions = conditions;
        _eventRules = eventRules;
    }
    
    public bool TryGetEventRule(TEventKey eventKey, out EventRule<TStateKey, TEventKey> result) {
        if (_eventRules != null) return _eventRules.TryGetValue(eventKey, out result);
        result = null;
        return false;
    }

    public void EvaluateConditions(CommandContext<TStateKey, TEventKey> ctx, out Command<TStateKey, TEventKey> command,
        ConditionStage stage) {
        if (_conditions == null || _conditions.Length == 0) {
            command = ctx.Stay();
            return;
        }
        var span = _conditions.AsSpan();
        for (var i = 0; i < span.Length; i++) {
            var condition = span[i];
            var result = stage switch {
                ConditionStage.StageChanged => 
                    // Evaluate all the conditions because the state is a new state. This happens after the execution of the state
                    condition.IsPredicateTrue(),
                ConditionStage.SameStateBeforeExecution => 
                    condition.IsPredicateTrue(Condition.Type.Asap, Condition.Type.Always),
                ConditionStage.SameStateAfterExecution => 
                    condition.IsPredicateTrue(Condition.Type.Lazy, Condition.Type.Always),
                _ => false
            };

            if (result) {
                command = condition.GetResult(ctx);
                return;
            }
        }
        command = ctx.Stay();
    }
}