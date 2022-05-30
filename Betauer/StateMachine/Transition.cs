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

    public class ExecuteContext<TStateKey, TTransitionKey> {
        public ExecuteTransition<TStateKey, TTransitionKey> Push(TStateKey name) =>
            new ExecuteTransition<TStateKey, TTransitionKey>(name, TransitionType.Push);

        public ExecuteTransition<TStateKey, TTransitionKey> PopPush(TStateKey name) =>
            new ExecuteTransition<TStateKey, TTransitionKey>(name, TransitionType.PopPush);

        public ExecuteTransition<TStateKey, TTransitionKey> Pop() =>
            new ExecuteTransition<TStateKey, TTransitionKey>(TransitionType.Pop);

        public ExecuteTransition<TStateKey, TTransitionKey> Set(TStateKey name) =>
            new ExecuteTransition<TStateKey, TTransitionKey>(name, TransitionType.Change);

        public ExecuteTransition<TStateKey, TTransitionKey> None() =>
            new ExecuteTransition<TStateKey, TTransitionKey>(TransitionType.None);

        public ExecuteTransition<TStateKey, TTransitionKey> Trigger(TTransitionKey on) =>
            new ExecuteTransition<TStateKey, TTransitionKey>(on);

        public float Delta { get; internal set; }
    }

    public class TriggerContext<TStateKey> {
        public TriggerTransition<TStateKey> Push(TStateKey name) =>
            new TriggerTransition<TStateKey>(name, TransitionType.Push);

        public TriggerTransition<TStateKey> PopPush(TStateKey name) =>
            new TriggerTransition<TStateKey>(name, TransitionType.PopPush);

        public TriggerTransition<TStateKey> Pop() =>
            new TriggerTransition<TStateKey>(TransitionType.Pop);

        public TriggerTransition<TStateKey> Set(TStateKey name) =>
            new TriggerTransition<TStateKey>(name, TransitionType.Change);

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