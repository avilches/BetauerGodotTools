using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Collections;
using Godot;

namespace Betauer.StateMachine {

    public interface IStateMachine<TStateKey, TTransitionKey> {
        public void On(TTransitionKey on, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public void AddState(IState<TStateKey, TTransitionKey> state);
        public IState<TStateKey, TTransitionKey> FindState(TStateKey name);
        public void Trigger(TTransitionKey name);
        public Task Execute(float delta);
        public IState<TStateKey, TTransitionKey> State { get; }
    }

    public class StateMachineBuilder<T, TStateKey, TTransitionKey> where T : IStateMachine<TStateKey, TTransitionKey> {
        private readonly T _stateMachine;
        private readonly Queue<StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>> _pendingStateBuilders =
            new Queue<StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>>();
        private Queue<Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>>? _pendingEvents;

        public StateMachineBuilder(T stateMachine) {
            _stateMachine = stateMachine;
        }

        public StateMachineBuilder<T, TStateKey, TTransitionKey> On(TTransitionKey on, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _pendingEvents ??= new Queue<Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>>();
            _pendingEvents.Enqueue(new Tuple<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>(on, transition));
            return this;
        }

        public StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>> State(TStateKey name) {
            var stateBuilder = new StateBuilder<T, TStateKey, TTransitionKey, StateMachineBuilder<T, TStateKey, TTransitionKey>>(name, this);
            _pendingStateBuilders.Enqueue(stateBuilder);
            return stateBuilder;
        }

        public T Build() {
            while (_pendingStateBuilders.Count > 0) _pendingStateBuilders.Dequeue().Build(_stateMachine);
            while (_pendingEvents != null && _pendingEvents.Count > 0) {
                var entry = _pendingEvents.Dequeue();
                _stateMachine.On(entry.Item1, entry.Item2);
            }
            return _stateMachine;
        }
    }

    public class StateMachine<TStateKey, TTransitionKey> : IStateMachine<TStateKey, TTransitionKey> {
        internal readonly struct Change {
            internal readonly IState<TStateKey, TTransitionKey>? State;
            internal readonly TransitionType Type;
            internal Change(IState<TStateKey, TTransitionKey>? state, TransitionType type) {
                State = state;
                if (type == TransitionType.Trigger)
                    throw new ArgumentException("Change type can't be a trigger");
                Type = type;
            }
            internal bool IsPop() => Type == TransitionType.Pop;
            internal bool IsPopPush() => Type == TransitionType.PopPush;
            internal bool IsPush() => Type == TransitionType.Push;
            internal bool IsChange() => Type == TransitionType.Change;
            internal bool IsChange(TStateKey key) {
                return Type == TransitionType.Change && State != null && EqualityComparer<TStateKey>.Default.Equals(State.Key, key);
            }
            internal bool IsNone() => Type == TransitionType.None;
        }


        
        private readonly Stack<IState<TStateKey, TTransitionKey>> _stack = new Stack<IState<TStateKey, TTransitionKey>>();
        private readonly ExecuteContext<TStateKey, TTransitionKey> _executeContext = new ExecuteContext<TStateKey, TTransitionKey>();
        private readonly TriggerContext<TStateKey> _triggerContext = new TriggerContext<TStateKey>();
        private Dictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _events;
        private Change _nextChange;
        private readonly TStateKey _initialState;
        private bool _disposed = false;

        public readonly Logger Logger;
        public readonly string? Name;
        public readonly Dictionary<TStateKey, IState<TStateKey, TTransitionKey>> States = new Dictionary<TStateKey, IState<TStateKey, TTransitionKey>>();
        public TStateKey[] GetStack() => _stack.Reverse().Select(e => e.Key).ToArray();
        public IState<TStateKey, TTransitionKey> State { get; private set; }

        public StateMachine(TStateKey initialState, string? name = null) {
            _initialState = initialState;
            Name = name;
            Logger = name != null ?
                LoggerFactory.GetLogger(name, "StateMachine") : 
                LoggerFactory.GetLogger("StateMachine");
        }

        public StateMachineBuilder<StateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateBuilder() {
            return new StateMachineBuilder<StateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey>(this);
        }

        public void On(TTransitionKey on, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _events ??= new Dictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>();
            _events[on] = transition;
        }


        public void AddState(IState<TStateKey, TTransitionKey> state) {
            if (States.ContainsKey(state.Key)) throw new DuplicateNameException();
            States[state.Key] = state;
        }

        public IState<TStateKey, TTransitionKey> FindState(TStateKey stateTypeName) {
            return States[stateTypeName];
        }

        public void Trigger(TTransitionKey name) {
            var transition = GetTransitionFromTrigger(name);
            _nextChange = CreateChange(transition);
        }

        public async Task Execute(float delta) {
            if (_disposed) return;
            var change = State == null && _nextChange.State == null
                ? new Change(FindState(_initialState), TransitionType.Change)
                : _nextChange;
            await _ExitPreviousStateIfNeeded(change);
            await _EnterNextStateIfNeeded(change);
            _executeContext.Delta = delta;
            var transition = await State.Execute(_executeContext);
            _nextChange = CreateChange(transition);
        }

        private Change CreateChange(ExecuteTransition<TStateKey, TTransitionKey> candidate) {
            if (candidate.IsTrigger() ) {
                candidate = GetTransitionFromTrigger(candidate.TransitionKey);
            }
            if (State != null && candidate.IsChange(State.Key)) {
                return new Change(State, TransitionType.None);
            }
            if (candidate.IsPop()) {
                if (_stack.Count <= 1) {
                    throw new InvalidOperationException("Pop");
                }
                var o = _stack.Pop();
                var transition = new Change(_stack.Peek(), TransitionType.Pop);
                _stack.Push(o);
                return transition;
            }
            if (candidate.IsNone()) {
                return new Change(State, TransitionType.None);
            }
            IState<TStateKey, TTransitionKey> newState = FindState(candidate.StateKey);
            return new Change(newState, candidate.Type);
        }

        private ExecuteTransition<TStateKey, TTransitionKey> GetTransitionFromTrigger(TTransitionKey name) {
            TriggerTransition<TStateKey> triggerTransition = default;
            var found = false;
            if (State != null && State.HasTransition(name)) {
                triggerTransition = State.GetTransition(name)(_triggerContext);
                found = true;
            }
            if (!found && _events != null && _events.ContainsKey(name)) {
                triggerTransition = _events[name](_triggerContext);
                found = true;
            }
            if (!found) {
                throw new KeyNotFoundException("Transition " + name + " not found");
            }
            var transition = triggerTransition.ToTransition<TTransitionKey>();
            return transition;
        }

        private async Task _ExitPreviousStateIfNeeded(Change change) {
            if (_disposed) return;
            if (State == null) {
                return;
            }
            if (change.IsNone()) return;
            if (change.IsPush()) {
                Logger.Debug($"Suspend: \"{State.Key}\"");
                await State.Suspend();
                return;
            }
            // Exit the current state
            Logger.Debug($"Exit: \"{State.Key}\"");
            await _stack.Pop().Exit();
            if (change.IsChange()) {
                while (_stack.Count > 0) {
                    Logger.Debug($"Exit: \"{_stack.Peek().Key}\"");
                    await _stack.Pop().Exit();                    
                }
            }
        }

        private async Task _EnterNextStateIfNeeded(Change change) {
            if (_disposed) return;
            if (change.IsNone()) return;
            // Change the current state
            var newState = change.State;
            Logger.Debug($"{change.Type} State: \"{newState.Key}\"");
            State = newState;

            if (change.IsPop()) {
                Logger.Debug($"Awake: \"{newState.Key}\"");
                await newState.Awake();
                return;
            }
            _stack.Push(newState);
            // Push or Change: enter the new state
            Logger.Debug($"Enter: \"{newState.Key}\"");
            await newState.Enter();
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}