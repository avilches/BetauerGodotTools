using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.StateMachine {
    public enum TransitionType {
        Push,
        PopPush,
        Pop,
        Replace,
        Trigger,
        None
    }

    public class ExecuteContext<TStateKey, TTransitionKey> 
        where TStateKey : Enum where TTransitionKey : Enum {

        private static readonly ExecuteTransition<TStateKey,TTransitionKey> CachePop = 
            new ExecuteTransition<TStateKey, TTransitionKey>(TransitionType.Pop);
        
        private static readonly ExecuteTransition<TStateKey, TTransitionKey> CachedNone = 
            new ExecuteTransition<TStateKey, TTransitionKey>(TransitionType.None);

        private static readonly Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>> CachePush =
            Enum.GetValues(typeof(TStateKey)).Cast<TStateKey>().ToDictionary(s => s,
                s => new ExecuteTransition<TStateKey, TTransitionKey>(s, TransitionType.Push));

        private static readonly Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>> CachePopPush =
            Enum.GetValues(typeof(TStateKey)).Cast<TStateKey>().ToDictionary(s => s,
                s => new ExecuteTransition<TStateKey, TTransitionKey>(s, TransitionType.PopPush));

        private static readonly Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>> CacheReplace =
            Enum.GetValues(typeof(TStateKey)).Cast<TStateKey>().ToDictionary(s => s,
                s => new ExecuteTransition<TStateKey, TTransitionKey>(s, TransitionType.Replace));

        private static readonly Dictionary<TTransitionKey, ExecuteTransition<TStateKey, TTransitionKey>> CacheTransition =
            Enum.GetValues(typeof(TTransitionKey)).Cast<TTransitionKey>().ToDictionary(s => s,
                s => new ExecuteTransition<TStateKey, TTransitionKey>(s));

        public ExecuteTransition<TStateKey, TTransitionKey> Push(TStateKey name) => CachePush[name];
        public ExecuteTransition<TStateKey, TTransitionKey> PopPush(TStateKey name) => CachePopPush[name];
        public ExecuteTransition<TStateKey, TTransitionKey> Pop() => CachePop;
        public ExecuteTransition<TStateKey, TTransitionKey> Replace(TStateKey name) => CacheReplace[name];
        public ExecuteTransition<TStateKey, TTransitionKey> None() => CachedNone;
        public ExecuteTransition<TStateKey, TTransitionKey> Trigger(TTransitionKey transitionKey) => CacheTransition[transitionKey];
            
        public float Delta { get; internal set; }
    }

    public class TriggerContext<TStateKey> {
        private static readonly TriggerTransition<TStateKey> CachePop = new TriggerTransition<TStateKey>(TransitionType.Pop);
        
        private static readonly Dictionary<TStateKey, TriggerTransition<TStateKey>> CachePush =
            Enum.GetValues(typeof(TStateKey)).Cast<TStateKey>().ToDictionary(s => s,
                s => new TriggerTransition<TStateKey>(s, TransitionType.Push));

        private static readonly Dictionary<TStateKey, TriggerTransition<TStateKey>> CachePopPush =
            Enum.GetValues(typeof(TStateKey)).Cast<TStateKey>().ToDictionary(s => s,
                s => new TriggerTransition<TStateKey>(s, TransitionType.PopPush));

        private static readonly Dictionary<TStateKey, TriggerTransition<TStateKey>> CacheReplace =
            Enum.GetValues(typeof(TStateKey)).Cast<TStateKey>().ToDictionary(s => s,
                s => new TriggerTransition<TStateKey>(s, TransitionType.Replace));

        public TriggerTransition<TStateKey> Push(TStateKey name) => CachePush[name];
        public TriggerTransition<TStateKey> PopPush(TStateKey name) => CachePopPush[name];
        public TriggerTransition<TStateKey> Pop() => CachePop;
        public TriggerTransition<TStateKey> Replace(TStateKey name) => CacheReplace[name];

    }

    public class Transition {
        public readonly TransitionType Type;

        public Transition(TransitionType type) {
            Type = type;
        }

        public bool IsNone() => Type == TransitionType.None;
        public bool IsReplace() => Type == TransitionType.Replace;
        public bool IsPop() => Type == TransitionType.Pop;
        public bool IsPopPush() => Type == TransitionType.PopPush;
        public bool IsPush() => Type == TransitionType.Push;
    }

    public class ExecuteTransition<TStateKey, TTransitionKey> : Transition {
        public readonly TStateKey StateKey;
        public readonly TTransitionKey TransitionKey;
        
        internal ExecuteTransition(TransitionType type) : base(type) {
            if (type != TransitionType.Pop && type != TransitionType.None)
                throw new ArgumentException("ExecuteTransition without state can be only Pop or None");
        }

        internal ExecuteTransition(TTransitionKey transitionKey) : base(TransitionType.Trigger) {
            TransitionKey = transitionKey;
        }

        internal ExecuteTransition(TStateKey stateKey, TransitionType type): base(type) {
            if (type == TransitionType.Trigger)
                throw new ArgumentException("ExecuteTransition with state can't have a Trigger type");
            StateKey = stateKey;
        }

        public bool IsTrigger() => Type == TransitionType.Trigger;

        public bool IsReplace(TStateKey key) {
            return IsReplace() && EqualityComparer<TStateKey>.Default.Equals(StateKey, key);
        }

    }
    
    public class TriggerTransition<TStateKey> : Transition {
        public readonly TStateKey StateKey;

        internal TriggerTransition(TransitionType type) : base(type) {
            if (type != TransitionType.Pop && type != TransitionType.None)
                throw new ArgumentException("TriggerTransition without transition can be only Pop or None");
        }

        internal TriggerTransition(TStateKey stateKey, TransitionType type) : base(type) {
            StateKey = stateKey;
            if (type == TransitionType.Trigger)
                throw new ArgumentException("TriggerTransition type can't be a trigger");
        }

        internal ExecuteTransition<TStateKey, TTransitionKey> ToTransition<TTransitionKey>() {
            return new ExecuteTransition<TStateKey, TTransitionKey>(StateKey, Type);
        }
    }
}