using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public abstract class BaseStateBuilderAsync<TBuilder, TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum
        where TBuilder : class {
        
        protected Func<Task>? EnterFunc;
        protected Func<Task>? AwakeFunc;
        protected List<Tuple<Func<bool>, Func<ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>> Conditions = new();
        protected Func<Task>? ExecuteFunc;
        protected Func<Task>? ExitFunc;
        protected Func<Task>? SuspendFunc;
        protected EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>? Events;
        protected readonly TStateKey Key;
        protected readonly Action<IStateAsync<TStateKey, TTransitionKey>> OnBuild;

        protected BaseStateBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TTransitionKey>> build) {
            OnBuild = build;
            Key = key;
        }

        public IStateAsync<TStateKey, TTransitionKey> Build() {
            IStateAsync<TStateKey, TTransitionKey> state = CreateState();
            OnBuild(state);
            return state;
        }

        protected abstract IStateAsync<TStateKey, TTransitionKey> CreateState();

        public ConditionBuilder<TBuilder, TStateKey, TTransitionKey> If(Func<bool> condition) {
            return new ConditionBuilder<TBuilder, TStateKey, TTransitionKey>(this as TBuilder, condition, (c) => {
                Condition(c.Condition, c.Execute);
            });
        }
        
        public TBuilder Condition(
            Func<bool> condition,
            Func<ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>> execute) {
            Conditions.Add(new(condition, execute));
            return this as TBuilder;
        }

        public TBuilder On(
            TTransitionKey transitionKey,
            Func<TriggerContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>> transition) {
            Events ??= EnumDictionary<TTransitionKey, Func<TriggerContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>>.Create();
            Events.Add(transitionKey, transition);
            return this as TBuilder;
        }

        /*
         * Enter
         */
        public TBuilder Enter(Func<Task> enter) {
            EnterFunc = enter;
            return this as TBuilder;
        }

        public TBuilder Enter(Action enter) {
            EnterFunc = () => {
                enter();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        /*
         * Awake
         */
        public TBuilder Awake(Func<Task> awake) {
            AwakeFunc = awake;
            return this as TBuilder;
        }

        public TBuilder Awake(Action awake) {
            AwakeFunc = () => {
                awake();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        /*
         * Execute
         */
        public TBuilder Execute(Func<Task> execute) {
            ExecuteFunc = execute;
            return this as TBuilder;
        }

        public TBuilder Execute(Action execute) {
            ExecuteFunc = () => {
                execute();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        /*
         * Suspend
         */
        public TBuilder Suspend(Func<Task> suspend) {
            SuspendFunc = suspend;
            return this as TBuilder;
        }

        public TBuilder Suspend(Action suspend) {
            SuspendFunc = () => {
                suspend();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

        /*
         * Exit
         */
        public TBuilder Exit(Func<Task> exit) {
            ExitFunc = exit;
            return this as TBuilder;
        }

        public TBuilder Exit(Action exit) {
            ExitFunc = () => {
                exit();
                return Task.CompletedTask;
            };
            return this as TBuilder;
        }

    }
}