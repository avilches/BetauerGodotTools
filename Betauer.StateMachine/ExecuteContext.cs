using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public class ExecuteContext<TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum {
        
        public float Delta { get; internal set; }

        internal ExecuteContext() {
        }

        public Response Push(TStateKey name) => CachePush[name]!;
        public Response PopPush(TStateKey name) => CachePopPush[name]!;
        public Response Pop() => CachePop;
        public Response Set(TStateKey name) => CacheSet[name]!;
        public Response None() => CachedNone;
        public Response Trigger(TTransitionKey transitionKey) => CacheTransition[transitionKey]!;

        internal static readonly Response CachePop = new(TransitionType.Pop);
        internal static readonly Response CachedNone = new(TransitionType.None);
        internal static readonly EnumDictionary<TStateKey, Response> CachePush = EnumDictionary<TStateKey, Response>.Create(s =>
                new Response(s, TransitionType.Push));
        internal static readonly EnumDictionary<TStateKey, Response> CachePopPush = EnumDictionary<TStateKey, Response>.Create(s =>
                new Response(s, TransitionType.PopPush));
        internal static readonly EnumDictionary<TStateKey, Response> CacheSet = EnumDictionary<TStateKey, Response>.Create(s =>
                new Response(s, TransitionType.Set));
        internal static readonly EnumDictionary<TTransitionKey, Response> CacheTransition =
                EnumDictionary<TTransitionKey, Response>.Create(s => new Response(s));

        public class Response : Transition {
        
            public readonly TStateKey StateKey;
            public readonly TTransitionKey TransitionKey;

            internal Response(TransitionType type) : base(type) {
                if (type != TransitionType.Pop && type != TransitionType.None)
                    throw new ArgumentException("ExecuteContext.Response without state can be only Pop or None");
            }

            internal Response(TTransitionKey transitionKey) : base(TransitionType.Trigger) {
                TransitionKey = transitionKey;
            }

            internal Response(TStateKey stateKey, TransitionType type) : base(type) {
                if (type == TransitionType.Trigger)
                    throw new ArgumentException("ExecuteContext.Response with state can't have a Trigger type");
                StateKey = stateKey;
            }

            public bool IsTrigger() => Type == TransitionType.Trigger;

            public bool IsSet(TStateKey? key) {
                return IsSet() && EqualityComparer<TStateKey>.Default.Equals(StateKey, key);
            }
        }
    }
}