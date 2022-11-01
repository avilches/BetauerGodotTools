using System;

namespace Betauer.StateMachine {
    public class Event<TStateKey, TTransitionKey> 
        where TStateKey : Enum
        where TTransitionKey : Enum {
        
        private TTransitionKey TransitionKey { get; }
        private Func<EventContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>? Execute { get; }
        private Command<TStateKey, TTransitionKey> Result { get; }

        internal Event(TTransitionKey transitionKey, Func<EventContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>> execute) {
            TransitionKey = transitionKey;
            Execute = execute;
        }

        internal Event(TTransitionKey transitionKey, Command<TStateKey, TTransitionKey> result) {
            TransitionKey = transitionKey;
            Result = result;
        }

        internal Command<TStateKey, TTransitionKey> GetResult(EventContext<TStateKey, TTransitionKey> ctx) =>
            Execute?.Invoke(ctx) ?? Result;

    }
}