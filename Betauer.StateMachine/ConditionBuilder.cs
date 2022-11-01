using System;

namespace Betauer.StateMachine {
    public class ConditionBuilder<TBuilder, TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum 
        where TBuilder : class {
        
        private readonly TBuilder _builder;
        private readonly Action<ConditionBuilder<TBuilder, TStateKey, TTransitionKey>> _onBuild;

        internal readonly Func<bool> Predicate;
        internal Func<ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>? Execute;
        internal Command<TStateKey, TTransitionKey> Result;

        internal ConditionBuilder(TBuilder builder, Func<bool> predicate, Action<ConditionBuilder<TBuilder, TStateKey, TTransitionKey>> onBuild) {
            _builder = builder;
            Predicate = predicate;
            _onBuild = onBuild;
        }

        public TBuilder Push(TStateKey state) {
            return Then(new Command<TStateKey, TTransitionKey>(TransitionType.Push, state, default));
        }

        public TBuilder Set(TStateKey state) {
            return Then(new Command<TStateKey, TTransitionKey>(TransitionType.Set, state, default));
        }

        public TBuilder PopPush(TStateKey state) {
            return Then(new Command<TStateKey, TTransitionKey>(TransitionType.PopPush, state, default));
        }

        public TBuilder Pop() {
            return Then(new Command<TStateKey, TTransitionKey>(TransitionType.Pop, default, default));
        }

        public TBuilder None() {
            return Then(new Command<TStateKey, TTransitionKey>(TransitionType.None, default, default));
        }

        public TBuilder Trigger(TTransitionKey transition) {
            return Then(new Command<TStateKey, TTransitionKey>(TransitionType.Trigger, default, transition));
        }

        private TBuilder Then(Command<TStateKey, TTransitionKey> command) {
            Result = command;
            _onBuild(this);
            return _builder;
        }

        public TBuilder Then(Func<ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>> execute) {
            Execute = execute;
            _onBuild(this);
            return _builder;
        }

    }
}