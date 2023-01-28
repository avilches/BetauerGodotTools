using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Betauer.StateMachine; 

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


    protected readonly struct StateChange {
        internal readonly TState Destination;
        internal readonly CommandType Type;

        internal StateChange(TState destination, CommandType type) {
            if (type == CommandType.SendEvent)
                throw new ArgumentException($"Command Type can't be {nameof(CommandType.SendEvent)}");
            
            Destination = destination;
            Type = type;
        }
    }

    private Command<TStateKey, TEventKey> _nextCommand;
    protected readonly Stack<TState> Stack = new();
    protected readonly CommandContext<TStateKey, TEventKey> CommandContext = new();
    protected Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>? EventRules;
    protected bool IsDisposed = false;

    public readonly Dictionary<TStateKey, TState> States = new();
    public TStateKey[] GetStack() => Stack.Reverse().Select(e => e.Key).ToArray();
    public TState CurrentState { get; protected set; }

    public bool IsState(TStateKey state) {
        return EqualityComparer<TStateKey>.Default.Equals(CurrentState.Key, state);
    }

    public string? Name { get; }

    protected BaseStateMachine(TStateKey initialState, string? name = null) {
        Name = name;
        _nextCommand = Command<TStateKey, TEventKey>.CreateInit(initialState);
    }

    protected EventBuilder<TBuilder, TStateKey, TEventKey> On<TBuilder>(TBuilder builder, TEventKey eventKey) 
        where TBuilder : class {
        Action<EventBuilder<TBuilder,TStateKey,TEventKey>> onBuild = c => {
            var eventRule = c.Execute != null
                ? new EventRule<TStateKey, TEventKey>(c.Execute)
                : new EventRule<TStateKey, TEventKey>(c.Result);
            AddEventRule(eventKey, eventRule);
        };
        return new EventBuilder<TBuilder, TStateKey, TEventKey>(
            builder, eventKey, onBuild);
    }

    public void AddEventRule(TEventKey eventKey, EventRule<TStateKey, TEventKey> eventRule) {
        EventRules ??= new Dictionary<TEventKey, EventRule<TStateKey, TEventKey>>();
        EventRules[eventKey] = eventRule;
    }

    public event Action<TransitionArgs<TStateKey>>? OnEnter;
    public event Action<TransitionArgs<TStateKey>>? OnAwake;
    public event Action<TransitionArgs<TStateKey>>? OnSuspend;
    public event Action<TransitionArgs<TStateKey>>? OnExit;
    public event Action<TransitionArgs<TStateKey>>? OnTransition;
    public event Action? OnBefore;
    public event Action? OnAfter;

    public void AddState(TState state) {
        if (States.ContainsKey(state.Key)) throw new DuplicateNameException();
        States[state.Key] = state;
        if (CurrentState == null && 
            _nextCommand.IsInit() &&
            _nextCommand.IsState(state.Key)) {
            CurrentState = state;
        }
    }

    public void Send(TEventKey eventKey, int weight = 0) {
        if (_nextCommand.IsInit())
            throw new InvalidOperationException("StateMachine not initialized. Please, execute it at least once before start sending events");
        if (!_nextCommand.IsSendEvent() || weight >= _nextCommand.Weight) {
            _nextCommand = Command<TStateKey, TEventKey>.CreateSendEvent(eventKey, weight);
        }
    }

    protected TState FindState(TStateKey stateKey) {
        return States.TryGetValue(stateKey, out var state)
            ? state
            : throw new KeyNotFoundException($"State {stateKey} not found. Please add it to the StateMachine");
    }


    protected StateChange GetNextStateChange() {
        if (CurrentState == null) {
            throw new KeyNotFoundException("Initial State not found: " + _nextCommand.StateKey);
        }
        if (_nextCommand.IsSendEvent()) {
            if (CurrentState.TryGetEventRule(_nextCommand.EventKey, out var eventRule)) {
                _nextCommand = eventRule.GetResult(CommandContext);
            } else if (EventRules != null &&
                       EventRules.TryGetValue(_nextCommand.EventKey, out eventRule)) {
                _nextCommand = eventRule.GetResult(CommandContext);
            } else {
                ThrowEventNotFound(_nextCommand.EventKey);
            }
        } else if (!_nextCommand.IsInit()) {
            // Find between all If() conditions and set a new next command (or stay if no conditions) 
            CurrentState.EvaluateConditions(CommandContext, out _nextCommand);
        }
        var stateChange = CreateStateChange();
        _nextCommand = CommandContext.Stay();
        return stateChange;
    }

    private StateChange CreateStateChange() {
        if (_nextCommand.IsStayOrSet(CurrentState.Key)) {
            return new StateChange(CurrentState, CommandType.Stay);
        }

        if (_nextCommand.IsPop()) {
            if (Stack.Count <= 1) {
                throw new InvalidOperationException("Command Pop error: stack is empty");
            }
            var o = Stack.Pop();
            var change = new StateChange(Stack.Peek(), CommandType.Pop);
            Stack.Push(o);
            return change;
        }

        // Init, Push, PopPush or Set to a different state than current state 
        TState newState = FindState(_nextCommand.StateKey);
        return new StateChange(newState, _nextCommand.Type);
    }

    private static void ThrowEventNotFound(TEventKey eventKey) =>
        throw new KeyNotFoundException($"Event {eventKey} not found. Please add it to the StateMachine");


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

    protected void BeforeEvent() {
        OnBefore?.Invoke();
    }

    protected void AfterEvent() {
        OnAfter?.Invoke();
    }

    public void Dispose() {
        IsDisposed = true;
    }
}