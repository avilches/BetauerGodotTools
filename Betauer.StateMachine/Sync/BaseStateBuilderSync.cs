using System;
using System.Collections.Generic;

namespace Betauer.StateMachine.Sync {
    public abstract class BaseStateBuilderSync<TBuilder, TStateKey, TEventKey>
        where TStateKey : Enum
        where TEventKey : Enum
        where TBuilder : class {
        
        protected Action? EnterFunc;
        protected Action? AwakeFunc;
        protected List<Condition<TStateKey, TEventKey>> Conditions = new();
        protected Action ExecuteFunc;
        protected Action? SuspendFunc;
        protected Action? ExitFunc;
        protected Dictionary<TEventKey, Event<TStateKey, TEventKey>>? Events;
        protected readonly TStateKey Key;
        protected readonly Action<IStateSync<TStateKey, TEventKey>> OnBuild;

        protected BaseStateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TEventKey>> build) {
            OnBuild = build;
            Key = key;
        }

        public IStateSync<TStateKey, TEventKey> Build() {
            IStateSync<TStateKey, TEventKey> stateSync = CreateState();
            OnBuild(stateSync);
            return stateSync;
        }

        protected abstract IStateSync<TStateKey, TEventKey> CreateState();

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
            Events ??= new Dictionary<TEventKey, Event<TStateKey, TEventKey>>();
            return new EventBuilder<TBuilder, TStateKey, TEventKey>(this as TBuilder, eventKey, (c) => {
                if (c.Execute != null) {
                    Events[eventKey] = new Event<TStateKey, TEventKey>(c.EventKey, c.Execute);
                } else {
                    Events[eventKey] = new Event<TStateKey, TEventKey>(c.EventKey, c.Result);
                }
            });
        }

        public TBuilder Enter(Action enter) {
            EnterFunc = enter;
            return this as TBuilder;
        }

        public TBuilder Awake(Action awake) {
            AwakeFunc = awake;
            return this as TBuilder;
        }

        public TBuilder Execute(Action execute) {
            ExecuteFunc = execute;
            return this as TBuilder;
        }

        public TBuilder Suspend(Action suspend) {
            SuspendFunc = suspend;
            return this as TBuilder;
        }

        public TBuilder Exit(Action exit) {
            ExitFunc = exit;
            return this as TBuilder;
        }
    }
}