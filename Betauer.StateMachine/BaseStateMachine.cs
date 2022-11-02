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
    }

    public abstract class BaseStateMachine<TStateKey, TEventKey, TState> : StateMachine
        where TStateKey : Enum
        where TEventKey : Enum
        where TState : class, IState<TStateKey, TEventKey> {


        protected readonly struct Change {
            internal readonly TState? Destination;
            internal readonly CommandType Type;

            internal Change(TState? destination, CommandType type) {
                Destination = destination;
                if (type == CommandType.Trigger)
                    throw new ArgumentException("Change type can't be a trigger");
                Type = type;
            }
        }

        protected static readonly Change NoChange = new(null, CommandType.None);

        protected readonly Stack<TState> Stack = new();
        protected readonly ConditionContext<TStateKey, TEventKey> ConditionContext = new();
        protected readonly EventContext<TStateKey, TEventKey> EventContext = new();
        protected Dictionary<TEventKey, Event<TStateKey, TEventKey>>? Events;
        protected Change NextChange;
        protected readonly TStateKey InitialState;
        protected bool IsInitialized = false;
        protected bool IsDisposed = false;
        protected TEventKey PendingEvent;
        protected bool HasPendingEvent = false;

        public readonly Dictionary<TStateKey, TState> States = new();
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
        }

        protected EventBuilder<TBuilder, TStateKey, TEventKey> On<TBuilder>(TBuilder builder,
            TEventKey eventKey) where TBuilder : class {
            Events ??= new();
            return new EventBuilder<TBuilder, TStateKey, TEventKey>(
                builder, eventKey, (c) => {
                    if (c.Execute != null) {
                        AddEvent(eventKey, new Event<TStateKey, TEventKey>(c.EventKey, c.Execute));
                    } else {
                        AddEvent(eventKey, new Event<TStateKey, TEventKey>(c.EventKey, c.Result));
                    }
                });
        }

        public void AddEvent(TEventKey eventKey, Event<TStateKey, TEventKey> @event) {
            Events[eventKey] = @event;
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

        public void Enqueue(TEventKey name) {
            PendingEvent = name;
            HasPendingEvent = true;
        }

        protected TState FindState(TStateKey stateKey) {
            return States.TryGetValue(stateKey, out var state)
                ? state
                : throw new KeyNotFoundException($"State {stateKey} not found. Please add it to the StateMachine");
        }

        protected Change CreateChange(ref Command<TStateKey, TEventKey> candidate) {
            if (CurrentState != null && candidate.IsSet(CurrentState.Key)) {
                return NoChange;
            }
            if (candidate.IsPop()) {
                if (Stack.Count <= 1) {
                    throw new InvalidOperationException("Can't pop state: stack empty");
                }
                var o = Stack.Pop();
                var change = new Change(Stack.Peek(), CommandType.Pop);
                Stack.Push(o);
                return change;
            }
            if (candidate.IsNone()) {
                return NoChange;
            }
            TState newState = FindState(candidate.StateKey);
            return new Change(newState, candidate.Type);
        }


        protected void ExecuteEvent(TEventKey name, out Command<TStateKey, TEventKey> command) {
            if (CurrentState?.Events != null && CurrentState.Events.TryGetValue(name, out var @event)) {
                command = @event.GetResult(EventContext);
                return;
            }
            if (Events != null && Events.TryGetValue(name, out @event)) {
                command = @event.GetResult(EventContext);
                return;
            }
            throw new KeyNotFoundException($"Event {name} not found. Please add it to the StateMachine");
        }

        protected void TransitionEvent(TState? from, TState to) {
            if (from != null && from != to) OnTransition?.Invoke(new TransitionArgs<TStateKey>(from.Key, to.Key));
        }

        protected void ExitEvent(TState state, TStateKey to) {
            OnExit?.Invoke(new TransitionArgs<TStateKey>(state.Key, to));
        }

        protected void SuspendEvent(TState state, TStateKey to) {
            OnSuspend?.Invoke(new TransitionArgs<TStateKey>(state.Key, to));
        }

        protected void AwakeEvent(TState state, TStateKey from) {
            OnAwake?.Invoke(new TransitionArgs<TStateKey>(from, state.Key));
        }

        protected void EnterEvent(TState state, TStateKey from) {
            OnEnter?.Invoke(new TransitionArgs<TStateKey>(from, state.Key));
        }


        public void Dispose() {
            IsDisposed = true;
        }
    }
}