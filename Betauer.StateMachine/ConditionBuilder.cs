using System;

namespace Betauer.StateMachine {
    public class ConditionBuilder<TBuilder, TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum 
        where TBuilder : class {
        
        private readonly TBuilder _builder;
        private readonly Action<ConditionBuilder<TBuilder, TStateKey, TTransitionKey>> _onBuild;

        internal readonly Func<bool> Condition;
        internal Func<Context<TStateKey, TTransitionKey>, Context<TStateKey, TTransitionKey>.Response> Execute;

        internal ConditionBuilder(TBuilder builder, Func<bool> condition, Action<ConditionBuilder<TBuilder, TStateKey, TTransitionKey>> onBuild) {
            _builder = builder;
            Condition = condition;
            _onBuild = onBuild;
        }

        public TBuilder Push(TStateKey state) {
            TStateKey s = state;
            return Then(context => context.Push(s));
        }

        public TBuilder Set(TStateKey state) {
            TStateKey s = state;
            return Then(context => context.Set(s));
        }

        public TBuilder PopPush(TStateKey state) {
            TStateKey s = state;
            return Then(context => context.PopPush(s));
        }

        public TBuilder Pop() {
            return Then(context => context.Pop());
        }

        public TBuilder None() {
            return Then(context => context.None());
        }

        public TBuilder Trigger(TTransitionKey transition) {
            return Then(context => context.Trigger(transition));
        }

        public TBuilder Then(Func<Context<TStateKey, TTransitionKey>, Context<TStateKey, TTransitionKey>.Response> execute) {
            Execute = execute;
            _onBuild(this);
            return _builder;
        }

    }
}