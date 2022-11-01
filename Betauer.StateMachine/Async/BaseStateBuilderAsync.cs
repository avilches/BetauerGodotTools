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
        protected List<Condition<TStateKey, TTransitionKey>> Conditions = new();
        protected Func<Task>? ExecuteFunc;
        protected Func<Task>? ExitFunc;
        protected Func<Task>? SuspendFunc;
        protected EnumDictionary<TTransitionKey, Event<TStateKey, TTransitionKey>>? Events;
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
                if (c.Execute != null) {
                    Conditions.Add(new Condition<TStateKey, TTransitionKey>(c.Predicate, c.Execute));
                } else {
                    Conditions.Add(new Condition<TStateKey, TTransitionKey>(c.Predicate, c.Result));
                }
            });
        }

        public EventBuilder<TBuilder, TStateKey, TTransitionKey> On(TTransitionKey transitionKey) {
            Events ??= EnumDictionary<TTransitionKey, Event<TStateKey, TTransitionKey>>.Create();
            return new EventBuilder<TBuilder, TStateKey, TTransitionKey>(this as TBuilder, transitionKey, (c) => {
                if (c.Execute != null) {
                    Events[transitionKey] = new Event<TStateKey, TTransitionKey>(c.TransitionKey, c.Execute);
                } else {
                    Events[transitionKey] = new Event<TStateKey, TTransitionKey>(c.TransitionKey, c.Result);
                }
            });
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