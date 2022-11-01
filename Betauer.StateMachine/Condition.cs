using System;

namespace Betauer.StateMachine {
    public class Condition<TStateKey, TTransitionKey> 
        where TStateKey : Enum
        where TTransitionKey : Enum {
        
        private Func<bool> Predicate { get; }
        private Func<ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>? Execute { get; }
        private Command<TStateKey, TTransitionKey> Result { get; }

        internal Condition(Func<bool> predicate, Func<ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>> execute) {
            Predicate = predicate;
            Execute = execute;
        }

        internal Condition(Func<bool> predicate, Command<TStateKey, TTransitionKey> result) {
            Predicate = predicate;
            Result = result;
        }

        internal bool IsPredicateTrue() => Predicate();

        internal Command<TStateKey, TTransitionKey> GetResult(ConditionContext<TStateKey, TTransitionKey> ctx) =>
            Execute?.Invoke(ctx) ?? Result;

    }
}