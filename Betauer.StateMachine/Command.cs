using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public readonly struct Command<TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum {  
        
        public readonly TransitionType Type;
        public readonly TStateKey StateKey;
        public readonly TTransitionKey TransitionKey;

        public bool IsNone() => Type == TransitionType.None;
        public bool IsSet() => Type == TransitionType.Set;
        public bool IsPop() => Type == TransitionType.Pop;
        public bool IsPopPush() => Type == TransitionType.PopPush;
        public bool IsPush() => Type == TransitionType.Push;
        public bool IsTrigger() => Type == TransitionType.Trigger;

        public bool IsSet(TStateKey? key) {
            return IsSet() && EqualityComparer<TStateKey>.Default.Equals(StateKey, key);
        }

        internal Command(TransitionType type, TStateKey stateKey, TTransitionKey transitionKey) {
            Type = type;
            StateKey = stateKey;
            TransitionKey = transitionKey;
        }
    }
}