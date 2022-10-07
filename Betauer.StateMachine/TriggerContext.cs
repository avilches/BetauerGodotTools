using System;

namespace Betauer.StateMachine {
    public class TriggerContext<TStateKey>
        where TStateKey : Enum {

        internal TriggerContext() {
        }
        
        public Response Push(TStateKey name) => CachePush[name]!;
        public Response PopPush(TStateKey name) => CachePopPush[name]!;
        public Response Pop() => CachePop;
        public Response Set(TStateKey name) => CacheSet[name]!;
        public Response None() => CachedNone;

        internal static readonly Response CachedNone = new(TransitionType.None);
        internal static readonly Response CachePop = new(TransitionType.Pop);
        internal static readonly EnumDictionary<TStateKey, Response> CachePush =
            EnumDictionary<TStateKey, Response>.Create(s => new Response(s, TransitionType.Push));
        internal static readonly EnumDictionary<TStateKey, Response> CachePopPush =
            EnumDictionary<TStateKey, Response>.Create(s => new Response(s, TransitionType.PopPush));
        internal static readonly EnumDictionary<TStateKey, Response> CacheSet =
            EnumDictionary<TStateKey, Response>.Create(s => new Response(s, TransitionType.Set));

        public class Response : Transition {
            
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

            internal ExecuteContext<TStateKey, TTransitionKey>.Response ToTransition<TTransitionKey>()
                where TTransitionKey : Enum {
                return Type switch {
                    TransitionType.None => ExecuteContext<TStateKey, TTransitionKey>.CachedNone,
                    TransitionType.Pop => ExecuteContext<TStateKey, TTransitionKey>.CachePop,
                    TransitionType.Push => ExecuteContext<TStateKey, TTransitionKey>.CachePush[StateKey]!,
                    TransitionType.Set => ExecuteContext<TStateKey, TTransitionKey>.CacheSet[StateKey]!,
                    TransitionType.PopPush => ExecuteContext<TStateKey, TTransitionKey>.CachePopPush[StateKey]!,
                    _ => throw new ArgumentException("TriggerContext.Response type can't be a trigger")
                };
            }
        }
    }

}
