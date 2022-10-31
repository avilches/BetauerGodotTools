using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public class Context<TStateKey, TTransitionKey>
        where TStateKey : Enum
        where TTransitionKey : Enum {
        
        internal Context() {
        }

        public Response Push(TStateKey name) => Response.CachePush[name]!;
        public Response PopPush(TStateKey name) => Response.CachePopPush[name]!;
        public Response Pop() => Response.CachePop;
        public Response Set(TStateKey name) => Response.CacheSet[name]!;
        public Response None() => Response.CachedNone;
        public Response Trigger(TTransitionKey transitionKey) => Response.CacheTransition[transitionKey]!;

        public class Response : Transition {

            internal static readonly Response CachePop = new(TransitionType.Pop);
            internal static readonly Response CachedNone = new(TransitionType.None);
            internal static readonly EnumDictionary<TStateKey, Response> CachePush = StateDict(s => new Response(s, TransitionType.Push));
            internal static readonly EnumDictionary<TStateKey, Response> CachePopPush = StateDict(s => new Response(s, TransitionType.PopPush));
            internal static readonly EnumDictionary<TStateKey, Response> CacheSet = StateDict(s => new Response(s, TransitionType.Set));
            internal static readonly EnumDictionary<TTransitionKey, Response> CacheTransition = TransitionDict(s => new Response(s));

            private static EnumDictionary<TStateKey, Response> StateDict(Func<TStateKey, Response> filler) =>
                EnumDictionary<TStateKey, Response>.Create(filler);
            
            private static EnumDictionary<TTransitionKey, Response> TransitionDict(Func<TTransitionKey, Response> filler) =>
                EnumDictionary<TTransitionKey, Response>.Create(filler);
            
            public readonly TStateKey StateKey;
            public readonly TTransitionKey TransitionKey;

            private Response(TransitionType type) : base(type) {
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