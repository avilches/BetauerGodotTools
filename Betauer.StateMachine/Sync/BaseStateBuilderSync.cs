using System;
using System.Collections.Generic;

namespace Betauer.StateMachine.Sync {
    public abstract class BaseStateBuilderSync<TBuilder, TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum
        where TBuilder : class {
        
        protected Action? EnterFunc;
        protected Action? AwakeFunc;
        protected List<Condition<TStateKey, TTransitionKey>> Conditions = new();
        protected Action ExecuteFunc;
        protected Action? SuspendFunc;
        protected Action? ExitFunc;
        protected EnumDictionary<TTransitionKey, Event<TStateKey, TTransitionKey>>? Events;
        protected readonly TStateKey Key;
        protected readonly Action<IStateSync<TStateKey, TTransitionKey>> OnBuild;

        protected BaseStateBuilderSync(TStateKey key, Action<IStateSync<TStateKey, TTransitionKey>> build) {
            OnBuild = build;
            Key = key;
        }

        public IStateSync<TStateKey, TTransitionKey> Build() {
            IStateSync<TStateKey, TTransitionKey> stateSync = CreateState();
            OnBuild(stateSync);
            return stateSync;
        }

        protected abstract IStateSync<TStateKey, TTransitionKey> CreateState();

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