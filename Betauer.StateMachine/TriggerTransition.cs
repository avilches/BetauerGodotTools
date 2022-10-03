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
            return new ExecuteTransition<TStateKey, TTransitionKey>(StateKey, Type);
        }
    }
}