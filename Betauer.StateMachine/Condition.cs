using System;

namespace Betauer.StateMachine {
    public class Condition<TStateKey, TEventKey> 
        where TStateKey : Enum
        where TEventKey : Enum {
        
        private Func<bool> Predicate { get; }
        private Func<ConditionContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>>? Execute { get; }
        private Command<TStateKey, TEventKey> Result { get; }

        internal Condition(Func<bool> predicate, Func<ConditionContext<TStateKey, TEventKey>, Command<TStateKey, TEventKey>> execute) {
            Predicate = predicate;
            Execute = execute;
        }

        internal Condition(Func<bool> predicate, Command<TStateKey, TEventKey> result) {
            Predicate = predicate;
            Result = result;
        }

        internal bool IsPredicateTrue() => Predicate();

        internal Command<TStateKey, TEventKey> GetResult(ConditionContext<TStateKey, TEventKey> ctx) =>
            Execute?.Invoke(ctx) ?? Result;

    }
}