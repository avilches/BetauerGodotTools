using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Betauer.StateMachine {
    public readonly struct TransitionArgs<TStateKey> {
        public readonly TStateKey From;
        public readonly TStateKey To;

        public TransitionArgs(TStateKey from, TStateKey to) {
            From = from;
            To = to;
        }
    }

    public abstract class StateMachine {
        protected static readonly Logger StaticLogger = LoggerFactory.GetLogger<StateMachine>();
    }

    public abstract class BaseStateMachine<TStateKey, TTransitionKey, TState> : StateMachine
        where TStateKey : Enum
        where TTransitionKey : Enum
        where TState : class, IState<TStateKey, TTransitionKey> {


        protected readonly struct Change {
            internal readonly TState? State;
            internal readonly CommandType Type;

            internal Change(TState? state, CommandType type) {
                State = state;
                if (type == CommandType.Trigger)
                    throw new ArgumentException("Change type can't be a trigger");
                Type = type;
            }
        }

        protected static readonly Change NoChange = new(null, CommandType.None);

        protected readonly Stack<TState> Stack = new();
        protected readonly ConditionContext<TStateKey, TTransitionKey> ConditionContext = new();
        protected readonly EventContext<TStateKey, TTransitionKey> EventContext = new();
        protected EnumDictionary<TTransitionKey, Event<TStateKey, TTransitionKey>>? Events;

        protected Func<Exception, ConditionContext<TStateKey, TTransitionKey>, Command<TStateKey, TTransitionKey>>
            OnError;

        protected Change NextChange;
        protected readonly TStateKey InitialState;
        protected bool IsInitialized = false;
        protected bool IsDisposed = false;
        protected TTransitionKey NextTransition;
        protected bool HasNextTransitionEnqueued = false;
        protected readonly object LockObject = new();

        public readonly Logger Logger;
        public readonly EnumDictionary<TStateKey, TState> States = EnumDictionary<TStateKey, TState>.Create();
        public TStateKey[] GetStack() => Stack.Reverse().Select(e => e.Key).ToArray();
        public TState CurrentState { get; protected set; }

        public bool IsState(TStateKey state) {
            return EqualityComparer<TStateKey>.Default.Equals(CurrentState.Key, state);
        }

        public string? Name { get; }

        private event Action<TransitionArgs<TStateKey>>? OnEnter;
        private event Action<TransitionArgs<TStateKey>>? OnAwake;
        private event Action<TransitionArgs<TStateKey>>? OnSuspend;
        private event Action<TransitionArgs<TStateKey>>? OnExit;
        private event Action<TransitionArgs<TStateKey>>? OnTransition;

        protected BaseStateMachine(TStateKey initialState, string? name = null) {
            InitialState = initialState;
            Name = name;
            Logger = name == null ? StaticLogger : LoggerFactory.GetLogger(name);
        }

        protected EventBuilder<TBuilder, TStateKey, TTransitionKey> On<TBuilder>(TBuilder builder,
            TTransitionKey transitionKey) where TBuilder : class {
            Events ??= EnumDictionary<TTransitionKey, Event<TStateKey, TTransitionKey>>.Create();
            return new EventBuilder<TBuilder, TStateKey, TTransitionKey>(
                builder, transitionKey, (c) => {
                    if (c.Execute != null) {
                        AddEvent(transitionKey, new Event<TStateKey, TTransitionKey>(c.TransitionKey, c.Execute));
                    } else {
                        AddEvent(transitionKey, new Event<TStateKey, TTransitionKey>(c.TransitionKey, c.Result));
                    }
                });
        }

        public void AddEvent(TTransitionKey transitionKey, Event<TStateKey, TTransitionKey> @event) {
            Events[transitionKey] = @event;
        }


    public void AddOnEnter(Action<TransitionArgs<TStateKey>> e) => OnEnter += e;
        public void AddOnAwake(Action<TransitionArgs<TStateKey>> e) => OnAwake += e;
        public void AddOnSuspend(Action<TransitionArgs<TStateKey>> e) => OnSuspend += e;
        public void AddOnExit(Action<TransitionArgs<TStateKey>> e) => OnExit += e;
        public void AddOnTransition(Action<TransitionArgs<TStateKey>> e) => OnTransition += e;

        public void RemoveOnEnter(Action<TransitionArgs<TStateKey>> e) => OnEnter -= e;
        public void RemoveOnAwake(Action<TransitionArgs<TStateKey>> e) => OnAwake -= e;
        public void RemoveOnSuspend(Action<TransitionArgs<TStateKey>> e) => OnSuspend -= e;
        public void RemoveOnExit(Action<TransitionArgs<TStateKey>> e) => OnExit -= e;
        public void RemoveOnTransition(Action<TransitionArgs<TStateKey>> e) => OnTransition -= e;

        public void AddState(TState state) {
            if (States.ContainsKey(state.Key)) throw new DuplicateNameException();
            States[state.Key] = state;
            if (EqualityComparer<TStateKey>.Default.Equals(state.Key, InitialState)) CurrentState = state;
        }

        public void Enqueue(TTransitionKey name) {
            lock (LockObject) {
                NextTransition = name;
                HasNextTransitionEnqueued = true;
            }
        }

        protected TState FindState(TStateKey stateKey) {
            return States.TryGetValue(stateKey, out var state)
                ? state
                : throw new KeyNotFoundException($"State {stateKey} not found. Please add it to the StateMachine");
        }

        protected TState TransitionTo(Change change, out TState oldState) {
            var newState = change.State;
            oldState = CurrentState ?? newState;
            TransitionEvent(change, oldState, newState);
            return newState;
        }

        protected Change CreateChange(ref Command<TStateKey, TTransitionKey> candidate) {
            if (CurrentState != null && candidate.IsSet(CurrentState.Key)) {
                return NoChange;
            }
            if (candidate.IsPop()) {
                if (Stack.Count <= 1) {
                    throw new InvalidOperationException("Can't pop state: stack empty");
                }
                var o = Stack.Pop();
                var transition = new Change(Stack.Peek(), CommandType.Pop);
                Stack.Push(o);
                return transition;
            }
            if (candidate.IsNone()) {
                return NoChange;
            }
            TState newState = FindState(candidate.StateKey);
            return new Change(newState, candidate.Type);
        }


        protected void ExecuteTransition(TTransitionKey name, out Command<TStateKey, TTransitionKey> command) {
            if (CurrentState?.Events != null && CurrentState.Events.TryGetValue(name, out var stateTransition)) {
                command = stateTransition.GetResult(EventContext);
                return;
            }
            if (Events != null && Events.TryGetValue(name, out var globalTrans)) {
                command = globalTrans.GetResult(EventContext);
                return;
            }
            throw new KeyNotFoundException($"Transition {name} not found. Please add it to the StateMachine");
        }

        protected void TransitionEvent(Change change, TState from, TState to) {
            if (from != to) OnTransition?.Invoke(new TransitionArgs<TStateKey>(from.Key, to.Key));
            #if DEBUG
            Logger.Debug($"> {change.Type} State: \"{to.Key}\"(from:{from.Key})");
            #endif
        }

        protected void ExitEvent(TState state, TStateKey to) {
            OnExit?.Invoke(new TransitionArgs<TStateKey>(state.Key, to));
            #if DEBUG
            Logger.Debug($"Exit: \"{state.Key}\"(to:{to})");
            #endif
        }

        protected void SuspendEvent(TState state, TStateKey to) {
            OnSuspend?.Invoke(new TransitionArgs<TStateKey>(state.Key, to));
            #if DEBUG
            Logger.Debug($"Suspend: \"{state.Key}\"(to:{to})");
            #endif
        }

        protected void AwakeEvent(TState state, TStateKey from) {
            OnAwake?.Invoke(new TransitionArgs<TStateKey>(from, state.Key));
            #if DEBUG
            Logger.Debug($"Awake: \"{state.Key}\"(from:{from})");
            #endif
        }

        protected void EnterEvent(TState state, TStateKey from) {
            OnEnter?.Invoke(new TransitionArgs<TStateKey>(from, state.Key));
            #if DEBUG
            Logger.Debug($"Enter: \"{state.Key}\"(from:{from})");
            #endif
        }


        public void Dispose() {
            IsDisposed = true;
        }
    }
}