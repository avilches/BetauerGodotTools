using System;

namespace Betauer.StateMachine {
    public class EventContext<TStateKey, TTransitionKey>
        where TStateKey : Enum 
        where TTransitionKey : Enum {
        
        internal EventContext() {
        }

        public Command<TStateKey, TTransitionKey> Push(TStateKey state) => new(CommandType.Push, state, default);
        public Command<TStateKey, TTransitionKey> PopPush(TStateKey state) => new(CommandType.PopPush, state, default);
        public Command<TStateKey, TTransitionKey> Pop() => new(CommandType.Pop, default, default);
        public Command<TStateKey, TTransitionKey> Set(TStateKey state) => new(CommandType.Set, state, default);
        public Command<TStateKey, TTransitionKey> None() => new(CommandType.None, default, default);

    }
}