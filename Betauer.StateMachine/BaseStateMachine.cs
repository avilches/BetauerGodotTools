using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Betauer.StateMachine; 

public readonly struct TransitionArgs<TStateKey> {
    public readonly TStateKey From;
    public readonly TStateKey To;
    public readonly CommandType Type;

    internal TransitionArgs(TStateKey from, TStateKey to, CommandType type) {
        From = from;
        To = to;
        Type = type;
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
                throw new InvalidStateException($"Command Type can't be {nameof(CommandType.SendEvent)}");
            
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
    public TState CurrentState { get; private set; }

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
    public event Action<TransitionArgs<TStateKey>>? OnBefore;
    public event Action<TransitionArgs<TStateKey>>? OnAfter;

    public void AddState(TState state) {
        if (States.ContainsKey(state.Key)) throw new DuplicateStateException(state.Key.ToString());
        States[state.Key] = state;
        if (CurrentState == null && 
            _nextCommand.IsInit() &&
            _nextCommand.IsState(state.Key)) {
            CurrentState = state;
        }
    }

    public void Send(TEventKey eventKey, int weight = 0) {
        if (_nextCommand.IsInit())
            throw new InvalidStateException("StateMachine not initialized. Please, execute it at least once before start sending events");
        if (!_nextCommand.IsSendEvent() || weight >= _nextCommand.Weight) {
            _nextCommand = Command<TStateKey, TEventKey>.CreateSendEvent(eventKey, weight);
        }
    }

    protected TState FindState(TStateKey stateKey) {
        return States.TryGetValue(stateKey, out var state)
            ? state
            : throw new StateNotFoundException($"State {stateKey} not found. Please add it to the StateMachine");
    }


    protected StateChange GetNextStateChange() {
        if (CurrentState == null) {
            throw new StateNotFoundException("Initial State not found: " + _nextCommand.StateKey);
        }
        if (_nextCommand.IsSendEvent()) {
            if (CurrentState.TryGetEventRule(_nextCommand.EventKey, out var eventRule)) {
                _nextCommand = eventRule.GetResult(CommandContext);
            } else if (EventRules != null &&
                       EventRules.TryGetValue(_nextCommand.EventKey, out eventRule)) {
                _nextCommand = eventRule.GetResult(CommandContext);
            } else {
                throw new EventNotFoundException($"Event {_nextCommand.EventKey} not found. Please add it to the StateMachine");
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
                throw new InvalidStateException("Command Pop error: stack is empty");
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
    
    protected TState ChangeState(TState destination) {
        var oldState = CurrentState;
        CurrentState = destination;
        return oldState;
    }

    protected void InvokeTransitionEvent(TStateKey from, TStateKey to, CommandType type) {
        OnTransition?.Invoke(new TransitionArgs<TStateKey>(from, to, type));
    }

    protected void InvokeExitEvent(TStateKey from, TStateKey to, CommandType type) {
        OnExit?.Invoke(new TransitionArgs<TStateKey>(from, to, type));
    }

    protected void InvokeSuspendEvent(TStateKey from, TStateKey to, CommandType type) {
        OnSuspend?.Invoke(new TransitionArgs<TStateKey>(from, to, type));
    }

    protected void InvokeAwakeEvent(TStateKey from, TStateKey to, CommandType type) {
        OnAwake?.Invoke(new TransitionArgs<TStateKey>(from, to, type));
    }

    protected void InvokeEnterEvent(TStateKey from, TStateKey to, CommandType type) {
        OnEnter?.Invoke(new TransitionArgs<TStateKey>(from, to, type));
    }

    protected void InvokeBeforeEvent(TStateKey from, TStateKey to, CommandType type) {
        OnBefore?.Invoke(new TransitionArgs<TStateKey>(from, to, type));
    }

    protected void InvokeAfterEvent(TStateKey from, TStateKey to, CommandType type) {
        OnAfter?.Invoke(new TransitionArgs<TStateKey>(from, to, type));
    }

    public void Dispose() {
        IsDisposed = true;
    }
}