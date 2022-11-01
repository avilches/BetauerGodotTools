using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public readonly struct Command<TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum {  
        
        public readonly CommandType Type;
        public readonly TStateKey StateKey;
        public readonly TTransitionKey TransitionKey;

        public bool IsNone() => Type == CommandType.None;
        public bool IsSet() => Type == CommandType.Set;
        public bool IsPop() => Type == CommandType.Pop;
        public bool IsPopPush() => Type == CommandType.PopPush;
        public bool IsPush() => Type == CommandType.Push;
        public bool IsTrigger() => Type == CommandType.Trigger;

        public bool IsSet(TStateKey? key) {
            return IsSet() && EqualityComparer<TStateKey>.Default.Equals(StateKey, key);
        }

        internal Command(CommandType type, TStateKey stateKey, TTransitionKey transitionKey) {
            Type = type;
            StateKey = stateKey;
            TransitionKey = transitionKey;
        }
    }
}