using System;

namespace Betauer.StateMachine {
    public class TriggerTransition<TStateKey> : Transition
        where TStateKey : Enum {
        
        public readonly TStateKey StateKey;

        internal TriggerTransition(TransitionType type) : base(type) {
            if (type != TransitionType.Pop && type != TransitionType.None)
                throw new ArgumentException("TriggerTransition without transition can be only Pop or None");
        }

        internal TriggerTransition(TStateKey stateKey, TransitionType type) : base(type) {
            StateKey = stateKey;
            if (type == TransitionType.Trigger)
                throw new ArgumentException("TriggerTransition type can't be a trigger");
        }

        internal ExecuteTransition<TStateKey, TTransitionKey> ToTransition<TTransitionKey>()
            where TTransitionKey : Enum {
            return Type switch {
                TransitionType.None => ExecuteTransitionCache<TStateKey, TTransitionKey>.CachedNone,
                TransitionType.Pop => ExecuteTransitionCache<TStateKey, TTransitionKey>.CachePop,
                TransitionType.Push => ExecuteTransitionCache<TStateKey, TTransitionKey>.CachePush[StateKey]!,
                TransitionType.Set => ExecuteTransitionCache<TStateKey, TTransitionKey>.CacheSet[StateKey]!,
                TransitionType.PopPush => ExecuteTransitionCache<TStateKey, TTransitionKey>.CachePopPush[StateKey]!,
                _ => throw new ArgumentException("TriggerTransition type can't be a trigger")
            };
        }
    }
}