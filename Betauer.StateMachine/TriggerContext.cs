using System;

namespace Betauer.StateMachine {
    public class TriggerContext<TStateKey, TTransitionKey>
        where TStateKey : Enum 
        where TTransitionKey : Enum {
        
        internal TriggerContext() {
        }

        public Command<TStateKey, TTransitionKey> Push(TStateKey state) => new(TransitionType.Push, state, default);
        public Command<TStateKey, TTransitionKey> PopPush(TStateKey state) => new(TransitionType.PopPush, state, default);
        public Command<TStateKey, TTransitionKey> Pop() => new(TransitionType.Pop, default, default);
        public Command<TStateKey, TTransitionKey> Set(TStateKey state) => new(TransitionType.Set, state, default);
        public Command<TStateKey, TTransitionKey> None() => new(TransitionType.None, default, default);

    }
}