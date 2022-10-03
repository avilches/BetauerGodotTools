using System;

namespace Betauer.StateMachine {
    public class TriggerContext<TStateKey>
        where TStateKey : Enum {

        internal TriggerContext() {
        }
        
        public TriggerTransition<TStateKey> Push(TStateKey name) => 
            TriggerTransitionCache<TStateKey>.CachePush[name]!;

        public TriggerTransition<TStateKey> PopPush(TStateKey name) =>
            TriggerTransitionCache<TStateKey>.CachePopPush[name]!;

        public TriggerTransition<TStateKey> Pop() => 
            TriggerTransitionCache<TStateKey>.CachePop;
        
        public TriggerTransition<TStateKey> Set(TStateKey name) => 
            TriggerTransitionCache<TStateKey>.CacheSet[name]!;
    }
    
    internal static class TriggerTransitionCache<TStateKey>
        where TStateKey : Enum {
        internal static readonly TriggerTransition<TStateKey> CachePop = new(TransitionType.Pop);

        internal static readonly EnumDictionary<TStateKey, TriggerTransition<TStateKey>> CachePush =
            new(s => new TriggerTransition<TStateKey>(s, TransitionType.Push));

        internal static readonly EnumDictionary<TStateKey, TriggerTransition<TStateKey>> CachePopPush =
            new(s => new TriggerTransition<TStateKey>(s, TransitionType.PopPush));

        internal static readonly EnumDictionary<TStateKey, TriggerTransition<TStateKey>> CacheSet =
            new(s => new TriggerTransition<TStateKey>(s, TransitionType.Set));

    }
}