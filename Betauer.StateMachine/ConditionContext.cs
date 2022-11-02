using System;

namespace Betauer.StateMachine {
    public class ConditionContext<TStateKey, TEventKey> 
        where TStateKey : Enum
        where TEventKey : Enum {
        
        internal ConditionContext() {
        }

        public Command<TStateKey, TEventKey> Push(TStateKey state) => new(CommandType.Push, state, default);
        public Command<TStateKey, TEventKey> PopPush(TStateKey state) => new(CommandType.PopPush, state, default);
        public Command<TStateKey, TEventKey> Pop() => new(CommandType.Pop, default, default);
        public Command<TStateKey, TEventKey> Set(TStateKey state) => new(CommandType.Set, state, default);
        public Command<TStateKey, TEventKey> None() => new(CommandType.None, default, default);
        public Command<TStateKey, TEventKey> Trigger(TEventKey eventKey) => new(CommandType.Trigger, default, eventKey);
    }
}