using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Betauer.StateMachine {

    public interface IStateMachine<TStateKey, TTransitionKey> where TStateKey : Enum where TTransitionKey : Enum {
        public void AddState(IState<TStateKey, TTransitionKey> state);
        public StateBuilder<IStateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateState(TStateKey stateKey);
        public void AddListener(IStateMachineListener<TStateKey> machineListener);
        public void On(TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public void On(TStateKey stateKey, TTransitionKey transitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition);
        public bool IsState(TStateKey state);
        public IState<TStateKey, TTransitionKey> CurrentState { get; }
        public void Enqueue(TTransitionKey name);
        public Task Execute(float delta);
        public bool Available { get; }
    }

    public abstract class StateMachine {
        protected static readonly Logger StaticLogger = LoggerFactory.GetLogger(typeof(StateMachine));
    }

    public class StateMachine<TStateKey, TTransitionKey> : StateMachine, IStateMachine<TStateKey, TTransitionKey> where TStateKey : Enum where TTransitionKey : Enum {
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
            internal bool IsSet() => Type == TransitionType.Set;
            internal bool IsChange(TStateKey key) {
                return Type == TransitionType.Set && State != null && EqualityComparer<TStateKey>.Default.Equals(State.Key, key);
            }
            internal bool IsNone() => Type == TransitionType.None;
        }

        private readonly Stack<IState<TStateKey, TTransitionKey>> _stack = new Stack<IState<TStateKey, TTransitionKey>>();
        private readonly ExecuteContext<TStateKey, TTransitionKey> _executeContext = new ExecuteContext<TStateKey, TTransitionKey>();
        private readonly TriggerContext<TStateKey> _triggerContext = new TriggerContext<TStateKey>();
        private Dictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _events;
        private Dictionary<Tuple<TStateKey, TTransitionKey>, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>? _stateEvents;
        private List<IStateMachineListener<TStateKey>>? _listeners;
        private Change _nextChange;
        private readonly TStateKey _initialState;
        private bool _disposed = false;
        private TTransitionKey _nextTransition;
        private bool _nextTransitionDefined = false;
        private readonly object _lockObject = new object();

        public readonly Logger Logger;
        public readonly string? Name;
        public readonly Dictionary<TStateKey, IState<TStateKey, TTransitionKey>> States = new Dictionary<TStateKey, IState<TStateKey, TTransitionKey>>();
        public TStateKey[] GetStack() => _stack.Reverse().Select(e => e.Key).ToArray();
        public IState<TStateKey, TTransitionKey> CurrentState { get; private set; }
        public bool IsState(TStateKey state) => CurrentState != null && state.Equals(CurrentState.Key);
        public bool Available { get; private set; } = true;

        public StateMachine(TStateKey initialState, string? name = null) {
            _initialState = initialState;
            Name = name;
            Logger = name == null ? StaticLogger : StaticLogger.GetSubLogger(name);
        }

        public void On(TTransitionKey transitionKey, 
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _events ??= new Dictionary<TTransitionKey, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>();
            _events[transitionKey] = transition;
        }

        public void On(TStateKey stateKey, TTransitionKey transitionKey,
            Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>> transition) {
            _stateEvents ??= new Dictionary<Tuple<TStateKey, TTransitionKey>, Func<TriggerContext<TStateKey>, TriggerTransition<TStateKey>>>();
            _stateEvents[new Tuple<TStateKey, TTransitionKey>(stateKey, transitionKey)] = transition;
        }

        public void AddListener(IStateMachineListener<TStateKey> machineListener) {
            _listeners ??= new List<IStateMachineListener<TStateKey>>();
            _listeners.Add(machineListener);
        }

        public StateBuilder<IStateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey> CreateState(
            TStateKey stateKey) {
            return new StateBuilder<IStateMachine<TStateKey, TTransitionKey>, TStateKey, TTransitionKey>(this, stateKey);
        }

        public void AddState(IState<TStateKey, TTransitionKey> state) {
            if (States.ContainsKey(state.Key)) throw new DuplicateNameException();
            States[state.Key] = state;
        }

        public void Enqueue(TTransitionKey name) {
            lock (this) {
                _nextTransition = name;
                _nextTransitionDefined = true;
            }
        }
        
        public async Task Execute(float delta) {
            if (_disposed) return;
            if (!Available) return;
            lock (_lockObject) {
                if (!Available) return;
                Available = false;
            }
            try {
                if (_nextTransitionDefined) {
                    _nextTransitionDefined = false;
                    var triggerTransition = GetTransitionFromTrigger(_nextTransition);
                    _nextChange = CreateChange(triggerTransition);
                }
                var change = CurrentState == null && _nextChange.State == null
                    ? new Change(States[_initialState], TransitionType.Set)
                    : _nextChange;
                if (change.Type == TransitionType.Pop) await DoPop(change);
                else if (change.Type == TransitionType.Push) await DoPush(change);
                else if (change.Type == TransitionType.PopPush) await DoPopPush(change);
                else if (change.Type == TransitionType.Set) await DoSet(change);

                var transition = await DoExecute(delta);
                _nextChange = CreateChange(transition);
            } finally {
                lock (_lockObject) {
                    Available = true;
                }
            }
        }

        private async Task DoPop(Change change) {
            if (CurrentState != null) await Exit(_stack.Pop(), change.State.Key);
            var newState = TransitionTo(change, out var oldState);
            await Awake(newState, oldState.Key);
        }

        private async Task DoPopPush(Change change) {
            if (CurrentState != null) await Exit(_stack.Pop(), change.State.Key);
            var newState = TransitionTo(change, out var oldState);
            _stack.Push(newState);
            await Enter(CurrentState, oldState.Key);
        }

        private async Task DoPush(Change change) {
            if (CurrentState != null) await Suspend(CurrentState, change.State!.Key);
            var newState = TransitionTo(change, out var oldState);
            _stack.Push(newState);
            await Enter(CurrentState, oldState.Key);
        }

        private async Task DoSet(Change change) {
            if (_stack.Count == 1) {
                await Exit(_stack.Pop(), change.State.Key);
            } else {
                // Special case: 
                // Exit from all the states from the stack, in order
                while (_stack.Count > 0) {
                    var exitingState = _stack.Pop();
                    var to = _stack.Count > 0 ? _stack.Peek().Key : change.State.Key;
                    await Exit(exitingState, to);
                }
            } 
            var newState = TransitionTo(change, out var oldState);
            _stack.Push(newState);
            await Enter(CurrentState, oldState.Key);
        }

        private async Task<ExecuteTransition<TStateKey, TTransitionKey>> DoExecute(float delta) {
            _listeners?.ForEach(listener => listener.OnExecuteStart(delta, CurrentState.Key));
            _executeContext.Delta = delta;
            var transition = await CurrentState.Execute(_executeContext);
            _listeners?.ForEach(listener => listener.OnExecuteEnd(CurrentState.Key));
            return transition;
        }

        private IState<TStateKey, TTransitionKey> TransitionTo(Change change, out IState<TStateKey, TTransitionKey> oldState) {
            var newState = change.State;
            oldState = CurrentState ?? newState;
            CurrentState = newState;
            Transition(change, oldState, newState);
            return newState;
        }

        private Change CreateChange(ExecuteTransition<TStateKey, TTransitionKey> candidate) {
            if (candidate.IsTrigger() ) {
                candidate = GetTransitionFromTrigger(candidate.TransitionKey);
            }
            if (CurrentState != null && candidate.IsSet(CurrentState.Key)) {
                return new Change(CurrentState, TransitionType.None);
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
                return new Change(CurrentState, TransitionType.None);
            }
            IState<TStateKey, TTransitionKey> newState = States[candidate.StateKey];
            return new Change(newState, candidate.Type);
        }


        private ExecuteTransition<TStateKey, TTransitionKey> GetTransitionFromTrigger(TTransitionKey name) {
            TriggerTransition<TStateKey> triggerTransition = default;
            var found = false;
            if (CurrentState != null && _stateEvents != null) {
                var key = new Tuple<TStateKey, TTransitionKey>(CurrentState.Key, name);
                if (_stateEvents != null && _stateEvents.ContainsKey(key)) {
                    triggerTransition = _stateEvents[key].Invoke(_triggerContext);
                    found = true;
                }
            }
            if (!found && _events != null && _events.ContainsKey(name)) {
                triggerTransition = _events[name].Invoke(_triggerContext);
                found = true;
            }
            if (!found) {
                throw new KeyNotFoundException("Transition " + name + " not found");
            }
            var transition = triggerTransition.ToTransition<TTransitionKey>();
            return transition;
        }

        private void Transition(Change change, IState<TStateKey, TTransitionKey> from, IState<TStateKey, TTransitionKey> to) {
            try {
                _listeners?.ForEach(listener => listener.OnTransition(from.Key, to.Key));
            } catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            Logger.Debug($"> {change.Type} State: \"{to.Key}\"(from:{from.Key}");
        }

        private async Task Exit(IState<TStateKey, TTransitionKey> state, TStateKey to) {
            _listeners?.ForEach(listener => listener.OnExit(state.Key, to));
            Logger.Debug($"Exit: \"{state.Key}\"(to:{to})\"");
            await state.Exit(to);
        }

        private async Task Suspend(IState<TStateKey, TTransitionKey> state, TStateKey to) {
            _listeners?.ForEach(listener => listener.OnSuspend(state.Key, to));
            Logger.Debug($"Suspend: \"{state.Key}\"(to:{to})");
            await state.Suspend(to);
        }

        private async Task Awake(IState<TStateKey, TTransitionKey> state, TStateKey from) {
            _listeners?.ForEach(listener => listener.OnAwake(state.Key, from));
            Logger.Debug($"Awake: \"{state.Key}\"(from:{from})");
            await state.Awake(from);
        }

        private async Task Enter(IState<TStateKey, TTransitionKey> state, TStateKey from) {
            _listeners?.ForEach(listener => listener.OnEnter(state.Key, from));
            Logger.Debug($"Enter: \"{state.Key}\"(from:{from})");
            await state.Enter(from);
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}