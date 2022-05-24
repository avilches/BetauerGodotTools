using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Collections;
using Godot;

namespace Betauer.StateMachine {
    public interface IStateMachine {
        public IStateMachine AddState(IState state);
        public IState FindState(string name);
        public IStateMachine SetNextState(string nextState);
        public IStateMachine PushNextState(string nextState);
        public IStateMachine PopPushNextState(string nextState);
        public IStateMachine PopNextState();
        public Task Execute(float delta);
        public IState CurrentState { get; }
        public StateChange Transition { get; }

    }

    public class StateMachineBuilder<T> where T : IStateMachine {
        private readonly T _stateMachine;
        private readonly Queue<StateBuilder<T, StateMachineBuilder<T>>> _pending = new Queue<StateBuilder<T, StateMachineBuilder<T>>>();

        public StateMachineBuilder(T stateMachine) {
            _stateMachine = stateMachine;
        }

        public StateBuilder<T, StateMachineBuilder<T>> State(string name) {
            var stateBuilder = new StateBuilder<T, StateMachineBuilder<T>>(name, this);
            _pending.Enqueue(stateBuilder);
            return stateBuilder;
        }

        public T Build() {
            while (_pending.Count > 0) {
                _pending.Dequeue().Build(_stateMachine, null);
            }
            return _stateMachine;
        }
    }

    public class StateMachine : IStateMachine {
        private const int MaxChanges = 10;

        private bool _disposed = false;
        private readonly Context _currentContext;
        private readonly Stack<IState> _stack = new Stack<IState>();

        public readonly Logger Logger;
        public readonly string Name;
        public readonly Node Owner; // Node Owner is needed to setup the state Timer
        public readonly Dictionary<string, IState> States = new Dictionary<string, IState>();
        public StateChange Transition { get; set;  }
        public string[] GetStack() => _stack.Reverse().Select(e => e.Name).ToArray();
        public IState CurrentState => _currentContext.CurrentState;

        public StateMachine(Node owner, string name) {
            Owner = owner;
            Name = name;
            _currentContext = new Context(this);
            Logger = LoggerFactory.GetLogger(name, "StateMachine");
        }

        public StateMachineBuilder<StateMachine> CreateBuilder() {
            return new StateMachineBuilder<StateMachine>(this);
        }

        public IStateMachine AddState(IState state) {
            if (States.ContainsKey(state.Name)) throw new DuplicateNameException();
            States[state.Name] = state;
            return this;
        }

        public IState FindState(string stateTypeName) {
            return States[stateTypeName];
        }

        public IStateMachine SetNextState(string nextState) {
            Transition = Validate(StateChange.CreateNextFrame(nextState));
            return this;
        }

        public IStateMachine PushNextState(string nextState) {
            Transition = Validate(StateChange.CreatePushNextFrame(nextState));
            return this;
        }

        public IStateMachine PopPushNextState(string nextState) {
            Transition = Validate(StateChange.CreatePopPushNextFrame(nextState));
            return this;
        }

        public IStateMachine PopNextState() {
            Transition = Validate(StateChange.CreatePopNextFrame());
            return this;
        }

        public async Task Execute(float delta) {
            if (_disposed) return;
            var transition = Transition;
            if (CurrentState == null && transition.State == null)
                throw new Exception("Please, initialize the state machine with a valid next state");
            await _Execute(delta, transition, CurrentState ?? transition.State);
        }

        private async Task _Execute(float delta, StateChange transition, IState initialState) {
            var execute = true;
            SimpleLinkedList<string> immediateChanges = new SimpleLinkedList<string>();
            while (execute) {
                if (_disposed) return;
                await _ExitPreviousStateIfNeeded(transition);
                await _EnterNextStateIfNeeded(transition, initialState, immediateChanges);
                _currentContext.Update(delta);
                var stateChange = await CurrentState.Execute(_currentContext);
                transition = Validate(stateChange);
                // Only if the state is different than the current state and it's immediate, will be executed right now
                execute = transition.IsImmediate;
            }
            Transition = transition;
        }

        private StateChange Validate(StateChange candidate) {
            if (candidate.Type == StateChange.TransitionType.Change && CurrentState?.Name == candidate.Name) {
                return candidate.IsImmediate ? StateChange._CreateImmediateNone() : StateChange.CreateNone();
            }
            if (candidate.Type == StateChange.TransitionType.Pop) {
                if (_stack.Count <= 1) {
                    throw new Exception("Can't pop state from a root or null state (there is no parent to go!)");
                }
                var o = _stack.Pop();
                candidate = candidate.WithState(_stack.Peek());
                _stack.Push(o);
                return candidate;
            }
            if (candidate.Type == StateChange.TransitionType.None) {
                return candidate.WithState(CurrentState);
            }
            IState newState = FindState(candidate.Name);
            return candidate.WithState(newState);
        }

        private async Task _ExitPreviousStateIfNeeded(StateChange transition) {
            if (_disposed) return;
            if (CurrentState == null ||
                transition.Type == StateChange.TransitionType.None) return;
            if (transition.Type == StateChange.TransitionType.Push) {
                Logger.Debug($"Suspend: \"{CurrentState.Name}\"");
                await CurrentState.Suspend(_currentContext);
                return;
            }
            // Exit the current state
            Logger.Debug($"Exit: \"{CurrentState.Name}\"");
            await _stack.Pop().Exit(); // TODO: enviar el contexto tambien?
            if (transition.Type == StateChange.TransitionType.Change) {
                while (_stack.Count > 0) {
                    Logger.Debug($"Exit: \"{_stack.Peek().Name}\"");
                    await _stack.Pop().Exit();                    
                }
            }
        }

        private async Task _EnterNextStateIfNeeded(StateChange transition, IState initialState, ICollection<string> immediateChanges) {
            if (_disposed) return;
            if (transition.Type == StateChange.TransitionType.None) return;
            // Change the current state
            var newState = transition.State;
            Logger.Debug($"{transition.Type} State: \"{newState.Name}\"");
            immediateChanges.Add(newState.Name);
            CheckImmediateChanges(initialState, immediateChanges);
            // To avoid having null as from state, the first execution from state = current state
            var fromState = CurrentState ?? newState;
            // TODO: Cuando hay un POP, no se ejecuta el enter pero se resetea todo (el timer por ejemplo)
            // El context deberia ser un stack tambien
            _currentContext.ChangeState(fromState, newState); // CurrentState == newState;

            if (transition.Type == StateChange.TransitionType.Pop) {
                Logger.Debug($"Awake: \"{newState.Name}\"");
                await newState.Awake(_currentContext);
                return;
            }
            _stack.Push(newState);
            // Push or Change: enter the new state
            Logger.Debug($"Enter: \"{newState.Name}\"");
            await newState.Enter(_currentContext);
        }

        private static void CheckImmediateChanges(IState initialState, ICollection<string> immediateChanges) {
            // Logger.Debug(
            // initialState?.Name + " (" + immediateChanges.Count + "):" + string.Join(", ", immediateChanges));
            if (immediateChanges.Count > MaxChanges) {
                throw new StackOverflowException(
                    $"Too many state changes detected in {initialState.Name}: {string.Join(", ", immediateChanges)}");
            }
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}