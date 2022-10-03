using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public class ExecuteTransition<TStateKey, TTransitionKey> : Transition
        where TStateKey : Enum where TTransitionKey : Enum {
        
        public readonly TStateKey StateKey;
        public readonly TTransitionKey TransitionKey;

        internal ExecuteTransition(TransitionType type) : base(type) {
            if (type != TransitionType.Pop && type != TransitionType.None)
                throw new ArgumentException("ExecuteTransition without state can be only Pop or None");
        }

        internal ExecuteTransition(TTransitionKey transitionKey) : base(TransitionType.Trigger) {
            TransitionKey = transitionKey;
        }

        internal ExecuteTransition(TStateKey stateKey, TransitionType type) : base(type) {
            if (type == TransitionType.Trigger)
                throw new ArgumentException("ExecuteTransition with state can't have a Trigger type");
            StateKey = stateKey;
        }

        public bool IsTrigger() => Type == TransitionType.Trigger;

        public bool IsSet(TStateKey? key) {
            return IsSet() && EqualityComparer<TStateKey>.Default.Equals(StateKey, key);
        }
    }
}