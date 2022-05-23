using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Betauer.Collections;
using Godot;

namespace Betauer.StateMachine {
    public interface IStateMachine {
        public IStateMachine AddState(IState state);
        public IState FindState(string name);
        public IStateMachine SetNextState(string nextState);
        public Task Execute(float delta);
        public IState CurrentState { get; }
        public Transition Transition { get; }

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
        public readonly Logger Logger;
        public readonly string Name;
        public readonly Node Owner; // Node Owner is needed to setup the state Timer
        private bool _disposed = false;

        internal readonly Dictionary<string, IState> States = new Dictionary<string, IState>();
        internal readonly Stack<IState> StackState = new Stack<IState>();

        public Transition Transition { get; set;  }
        private readonly Context _currentContext;
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

        public IStateMachine PopNextState() {
            Transition = Validate(StateChange.CreatePopFrame());
            return this;
        }

        public IStateMachine SetNextState(string nextState) {
            Transition = Validate(StateChange.CreateNextFrame(nextState));
            return this;
        }


        public async Task Execute(float delta) {
            if (_disposed) return;
            if (CurrentState == null && Transition.State == null)
                throw new Exception("Please, initialize the state machine with a valid next state");
            await _Execute(delta, CurrentState ?? Transition.State);
        }

        private async Task _Execute(float delta, IState initialState) {
            var execute = true;
            SimpleLinkedList<string> immediateChanges = new SimpleLinkedList<string>();
            while (execute) {
                if (_disposed) return;
                await _ExitPreviousStateIfNeeded();
                await _EnterNextStateIfNeeded(initialState, immediateChanges);
                _currentContext.Update(delta);
                var stateChange = await _currentContext.CurrentState.Execute(_currentContext);
                Transition = Validate(stateChange);
                // Only if the state is different than the current state and it's immediate, will be executed right now
                execute = Transition.IsImmediate;
            }
        }

        private Transition Validate(StateChange candidate) {
            if (candidate.Name == null) {
                // Pop
                if (CurrentState?.Parent == null) {
                    throw new Exception("Can't pop state from a root or null state (there is no parent to go!)");
                }
                return new Transition(CurrentState.Parent, Transition.TransitionType.Pop, candidate.IsImmediate);
            }
            IState newState = FindState(candidate.Name);
            if (CurrentState == null) {
                if (newState.Parent != null) {
                    throw new Exception("Only root (non-nested) states are allowed");
                }
                return new Transition(newState, Transition.TransitionType.Push, candidate.IsImmediate);
            }
            if (newState == CurrentState) {
                return new Transition(newState, Transition.TransitionType.None, candidate.IsImmediate);
            }
            if (newState.Parent == CurrentState.Parent) {
                return new Transition(newState, Transition.TransitionType.Change, candidate.IsImmediate);
            }
            if (newState.Parent == CurrentState) {
                return new Transition(newState, Transition.TransitionType.Push, candidate.IsImmediate);
            }
            if (newState == CurrentState.Parent) {
                return new Transition(newState, Transition.TransitionType.Pop, candidate.IsImmediate);
            }
            throw new Exception("Only parent, siblings or child states are allowed");
        }

        private async Task _ExitPreviousStateIfNeeded() {
            if (_disposed) return;
            if (_currentContext.CurrentState == null ||
                Transition.Type == Transition.TransitionType.None ||
                Transition.Type == Transition.TransitionType.Push) return;
            // Exit the current state
            Logger.Debug($"Exit: \"{_currentContext.CurrentState.Name}\"");
            await _currentContext.CurrentState.Exit();
        }

        private async Task _EnterNextStateIfNeeded(IState initialState, ICollection<string> immediateChanges) {
            if (_disposed) return;
            if (Transition.Type == Transition.TransitionType.None) return;
            // Change the current state
            Logger.Debug($"{Transition.Type} State: \"{Transition.State.Name}\"");
            immediateChanges.Add(Transition.State.Name);
            CheckImmediateChanges(initialState, immediateChanges);
            // To avoid having null as from state, the first execution from state = current state
            var fromState = _currentContext.CurrentState ?? Transition.State;
            _currentContext.Reset(Transition.State, fromState);

            if (Transition.Type == Transition.TransitionType.Pop) return;
            // Push or Change: enter the new state
            Logger.Debug($"Enter: \"{Transition.State.Name}\"");
            await Transition.State.Enter(_currentContext);
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