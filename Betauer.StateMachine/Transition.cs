using System;
using System.Linq;

namespace Betauer.StateMachine {
    public abstract class Transition {
        public readonly TransitionType Type;

        internal Transition(TransitionType type) {
            Type = type;
        }

        public bool IsNone() => Type == TransitionType.None;
        public bool IsSet() => Type == TransitionType.Set;
        public bool IsPop() => Type == TransitionType.Pop;
        public bool IsPopPush() => Type == TransitionType.PopPush;
        public bool IsPush() => Type == TransitionType.Push;
    }
}