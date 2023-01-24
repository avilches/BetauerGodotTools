using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betauer.StateMachine.Async {
    public abstract class BaseStateBuilderAsync<TBuilder, TStateKey, TEventKey>
        where TStateKey : Enum
        where TEventKey : Enum
        where TBuilder : class {
        
        protected Func<Task>? EnterFunc;
        protected Func<Task>? AwakeFunc;
        protected readonly List<Condition<TStateKey, TEventKey>> Conditions = new();
        protected Func<Task>? ExecuteFunc;
        protected Func<Task>? ExitFunc;
        protected Func<Task>? SuspendFunc;
        protected Dictionary<TEventKey, Event<TStateKey, TEventKey>>? Events;
        protected readonly TStateKey Key;
        protected readonly Action<IStateAsync<TStateKey, TEventKey>> OnBuild;

        protected BaseStateBuilderAsync(TStateKey key, Action<IStateAsync<TStateKey, TEventKey>> build) {
            OnBuild = build;
            Key = key;
        }

        public IStateAsync<TStateKey, TEventKey> Build() {
            IStateAsync<TStateKey, TEventKey> state = CreateState();
            OnBuild(state);
            return state;
        }

        protected abstract IStateAsync<TStateKey, TEventKey> CreateState();

        public ConditionBuilder<TBuilder, TStateKey, TEventKey> If(Func<bool> condition) {
            return new ConditionBuilder<TBuilder, TStateKey, TEventKey>(this as TBuilder, condition, (c) => {
                if (c.Execute != null) {
                    Conditions.Add(new Condition<TStateKey, TEventKey>(c.Predicate, c.Execute));
                } else {
                    Conditions.Add(new Condition<TStateKey, TEventKey>(c.Predicate, c.Result));
                }
            });
        }

        public EventBuilder<TBuilder, TStateKey, TEventKey> On(TEventKey eventKey) {
            Events ??= new();
            return new EventBuilder<TBuilder, TStateKey, TEventKey>(this as TBuilder, eventKey, (c) => {
                if (c.Execute != null) {
                    Events[eventKey] = new Event<TStateKey, TEventKey>(c.EventKey, c.Execute);
                } else {
                    Events[eventKey] = new Event<TStateKey, TEventKey>(c.EventKey, c.Result);
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