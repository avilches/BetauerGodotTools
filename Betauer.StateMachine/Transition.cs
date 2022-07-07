using System;
using System.Collections.Generic;

namespace Betauer.StateMachine {
    public enum TransitionType {
        Push,
        PopPush,
        Pop,
        Change,
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
            new Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>();

        private static readonly Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>> CachePopPush =
            new Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>();

        private static readonly Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>> CacheChange =
            new Dictionary<TStateKey, ExecuteTransition<TStateKey, TTransitionKey>>();

        private static readonly Dictionary<TTransitionKey, ExecuteTransition<TStateKey, TTransitionKey>> CacheTransition =
            new Dictionary<TTransitionKey, ExecuteTransition<TStateKey, TTransitionKey>>();

        static ExecuteContext() {
            foreach (var stateKey in (TStateKey[])Enum.GetValues(typeof(TStateKey))) {
                CachePush[stateKey] = new ExecuteTransition<TStateKey, TTransitionKey>(stateKey, TransitionType.Push);
                CachePopPush[stateKey] = new ExecuteTransition<TStateKey, TTransitionKey>(stateKey, TransitionType.PopPush);
                CacheChange[stateKey] = new ExecuteTransition<TStateKey, TTransitionKey>(stateKey, TransitionType.Change);
            }
            foreach (var transitionKey in (TTransitionKey[])Enum.GetValues(typeof(TTransitionKey))) {
                CacheTransition[transitionKey] = new ExecuteTransition<TStateKey, TTransitionKey>(transitionKey);; 
            }
        }

        public ExecuteTransition<TStateKey, TTransitionKey> Push(TStateKey name) => CachePush[name];
        public ExecuteTransition<TStateKey, TTransitionKey> PopPush(TStateKey name) => CachePopPush[name];
        public ExecuteTransition<TStateKey, TTransitionKey> Pop() => CachePop;
        public ExecuteTransition<TStateKey, TTransitionKey> Set(TStateKey name) => CacheChange[name];
        public ExecuteTransition<TStateKey, TTransitionKey> None() => CachedNone;
        public ExecuteTransition<TStateKey, TTransitionKey> Trigger(TTransitionKey transitionKey) => CacheTransition[transitionKey];
            

        public float Delta { get; internal set; }
    }

    public class TriggerContext<TStateKey> {
        private static readonly TriggerTransition<TStateKey> CachePop = new TriggerTransition<TStateKey>(TransitionType.Pop);
        
        private static readonly Dictionary<TStateKey, TriggerTransition<TStateKey>> CachePush =
            new Dictionary<TStateKey, TriggerTransition<TStateKey>>();

        private static readonly Dictionary<TStateKey, TriggerTransition<TStateKey>> CachePopPush =
            new Dictionary<TStateKey, TriggerTransition<TStateKey>>();

        private static readonly Dictionary<TStateKey, TriggerTransition<TStateKey>> CacheChange =
            new Dictionary<TStateKey, TriggerTransition<TStateKey>>();

        static TriggerContext() {
            foreach (var stateKey in (TStateKey[])Enum.GetValues(typeof(TStateKey))) {
                CachePush[stateKey] = new TriggerTransition<TStateKey>(stateKey, TransitionType.Push);
                CachePopPush[stateKey] = new TriggerTransition<TStateKey>(stateKey, TransitionType.PopPush);
                CacheChange[stateKey] = new TriggerTransition<TStateKey>(stateKey, TransitionType.Change);
            }
        }

        public TriggerTransition<TStateKey> Push(TStateKey name) => CachePush[name];

        public TriggerTransition<TStateKey> PopPush(TStateKey name) => CachePopPush[name];

        public TriggerTransition<TStateKey> Pop() => CachePop;

        public TriggerTransition<TStateKey> Set(TStateKey name) => CacheChange[name];

    }

    public class Transition {
        public readonly TransitionType Type;

        public Transition(TransitionType type) {
            Type = type;
        }

        public bool IsNone() => Type == TransitionType.None;
        public bool IsChange() => Type == TransitionType.Change;
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

        public bool IsChange(TStateKey key) {
            return IsChange() && EqualityComparer<TStateKey>.Default.Equals(StateKey, key);
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