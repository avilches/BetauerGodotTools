using System;

namespace Betauer.StateMachine {
    public class ExecuteContext<TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum {
        public float Delta { get; internal set; }

        internal ExecuteContext() {
        }

        public ExecuteTransition<TStateKey, TTransitionKey> Push(TStateKey name) =>
            ExecuteTransitionCache<TStateKey, TTransitionKey>.CachePush[name]!;

        public ExecuteTransition<TStateKey, TTransitionKey> PopPush(TStateKey name) =>
            ExecuteTransitionCache<TStateKey, TTransitionKey>.CachePopPush[name]!;

        public ExecuteTransition<TStateKey, TTransitionKey> Pop() =>
            ExecuteTransitionCache<TStateKey, TTransitionKey>.CachePop;

        public ExecuteTransition<TStateKey, TTransitionKey> Set(TStateKey name) =>
            ExecuteTransitionCache<TStateKey, TTransitionKey>.CacheSet[name]!;

        public ExecuteTransition<TStateKey, TTransitionKey> None() =>
            ExecuteTransitionCache<TStateKey, TTransitionKey>.CachedNone;

        public ExecuteTransition<TStateKey, TTransitionKey> Trigger(TTransitionKey transitionKey) =>
            ExecuteTransitionCache<TStateKey, TTransitionKey>.CacheTransition[transitionKey]!;
    }

    internal static class ExecuteTransitionCache<TStateKey, TTransitionKey>
        where TStateKey : Enum where TTransitionKey : Enum {
        
        internal static readonly ExecuteTransition<TStateKey, TTransitionKey>
            CachePop = new(TransitionType.Pop);

        internal static readonly ExecuteTransition<TStateKey, TTransitionKey>
            CachedNone = new(TransitionType.None);

        internal static readonly EnumDictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>
            CachePush = EnumDictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>.Create(s =>
                new ExecuteTransition<TStateKey, TTransitionKey>(s, TransitionType.Push));

        internal static readonly EnumDictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>
            CachePopPush = EnumDictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>.Create(s =>
                new ExecuteTransition<TStateKey, TTransitionKey>(s, TransitionType.PopPush));

        internal static readonly EnumDictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>
            CacheSet = EnumDictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>.Create(s =>
                new ExecuteTransition<TStateKey, TTransitionKey>(s, TransitionType.Set));

        internal static readonly EnumDictionary<TTransitionKey, ExecuteTransition<TStateKey, TTransitionKey>>
            CacheTransition =
                EnumDictionary<TTransitionKey, ExecuteTransition<TStateKey, TTransitionKey>>.Create(s =>
                    new ExecuteTransition<TStateKey, TTransitionKey>(s));
    }
}