using System;

namespace Betauer.StateMachine {
    public class ConditionContext<TStateKey, TTransitionKey> 
        where TStateKey : Enum
        where TTransitionKey : Enum {
        
        internal ConditionContext() {
        }

        public Command<TStateKey, TTransitionKey> Push(TStateKey state) => new(TransitionType.Push, state, default);
        public Command<TStateKey, TTransitionKey> PopPush(TStateKey state) => new(TransitionType.PopPush, state, default);
        public Command<TStateKey, TTransitionKey> Pop() => new(TransitionType.Pop, default, default);
        public Command<TStateKey, TTransitionKey> Set(TStateKey state) => new(TransitionType.Set, state, default);
        public Command<TStateKey, TTransitionKey> None() => new(TransitionType.None, default, default);
        public Command<TStateKey, TTransitionKey> Trigger(TTransitionKey transitionKey) => new(TransitionType.Trigger, default, transitionKey);
    }
}