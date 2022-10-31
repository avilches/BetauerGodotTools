using System;

namespace Betauer.StateMachine {
    public class TriggerContext<TStateKey>
        where TStateKey : Enum {

        internal TriggerContext() {
        }
        
        public Response Push(TStateKey name) => Response.CachePush[name]!;
        public Response PopPush(TStateKey name) => Response.CachePopPush[name]!;
        public Response Pop() => Response.CachePop;
        public Response Set(TStateKey name) => Response.CacheSet[name]!;
        public Response None() => Response.CachedNone;

        public class Response : Transition {
            
            internal static readonly Response CachedNone = new(TransitionType.None);
            internal static readonly Response CachePop = new(TransitionType.Pop);
            internal static readonly EnumDictionary<TStateKey, Response> CachePush = StateDict(s => new Response(s, TransitionType.Push));
            internal static readonly EnumDictionary<TStateKey, Response> CachePopPush = StateDict(s => new Response(s, TransitionType.PopPush));
            internal static readonly EnumDictionary<TStateKey, Response> CacheSet = StateDict(s => new Response(s, TransitionType.Set));

            private static EnumDictionary<TStateKey, Response> StateDict(Func<TStateKey, Response> filler) =>
                EnumDictionary<TStateKey, Response>.Create(filler);

            public readonly TStateKey StateKey;

            internal Response(TransitionType type) : base(type) {
                if (type != TransitionType.Pop && type != TransitionType.None)
                    throw new ArgumentException("TriggerContext.Response without transition can be only Pop or None");
            }

            internal Response(TStateKey stateKey, TransitionType type) : base(type) {
                StateKey = stateKey;
                if (type == TransitionType.Trigger)
                    throw new ArgumentException("TriggerContext.Response type can't be a trigger");
            }

            internal Context<TStateKey, TTransitionKey>.Response ToTransition<TTransitionKey>()
                where TTransitionKey : Enum {
                return Type switch {
                    TransitionType.None => Context<TStateKey, TTransitionKey>.Response.CachedNone,
                    TransitionType.Pop => Context<TStateKey, TTransitionKey>.Response.CachePop,
                    TransitionType.Push => Context<TStateKey, TTransitionKey>.Response.CachePush[StateKey]!,
                    TransitionType.Set => Context<TStateKey, TTransitionKey>.Response.CacheSet[StateKey]!,
                    TransitionType.PopPush => Context<TStateKey, TTransitionKey>.Response.CachePopPush[StateKey]!,
                    _ => throw new ArgumentException("TriggerContext.Response type can't be a trigger")
                };
            }
        }
    }

}
