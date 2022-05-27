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
        public IStateMachine SetState(string nextState);
        public IStateMachine PushState(string nextState);
        public IStateMachine PopPushState(string nextState);
        public IStateMachine PopState();
        public Task Execute(float delta);
        public IState State { get; }
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
            while (_pending.Count > 0) _pending.Dequeue().Build(_stateMachine);
            return _stateMachine;
        }
    }

    public class StateMachine : IStateMachine {
        private bool _disposed = false;
        private readonly Stack<IState> _stack = new Stack<IState>();

        public readonly Logger Logger;
        public readonly string? Name;
        public readonly Dictionary<string, IState> States = new Dictionary<string, IState>();
        public Transition Transition { get; private set; }
        public string[] GetStack() => _stack.Reverse().Select(e => e.Name).ToArray();
        public IState State { get; private set; }

        public StateMachine(string? name = null) {
            Name = name;
            Logger = name != null ?
                LoggerFactory.GetLogger(name, "StateMachine") : 
                LoggerFactory.GetLogger("StateMachine");
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

        public IStateMachine SetState(string nextState) {
            Transition = Validate(Transition.Set(nextState));
            return this;
        }

        public IStateMachine PushState(string nextState) {
            Transition = Validate(Transition.Push(nextState));
            return this;
        }

        public IStateMachine PopPushState(string nextState) {
            Transition = Validate(Transition.PopPush(nextState));
            return this;
        }

        public IStateMachine PopState() {
            Transition = Validate(Transition.Pop());
            return this;
        }

        public async Task Execute(float delta) {
            if (_disposed) return;
            var transition = Transition;
            if (State == null && transition.State == null)
                throw new Exception("Please, initialize the state machine with a valid next state");
            // IState fromState = State ?? transition.State;
            await _ExitPreviousStateIfNeeded(transition);
            await _EnterNextStateIfNeeded(transition);
            Transition = Validate(await State.Execute(delta));
        }

        private Transition Validate(Transition candidate) {
            if (candidate.Type == Transition.TransitionType.Change && State?.Name == candidate.Name) {
                return Transition.None();
            }
            if (candidate.Type == Transition.TransitionType.Pop) {
                if (_stack.Count <= 1) {
                    throw new Exception("Can't pop state from a root or null state (there is no parent to go!)");
                }
                var o = _stack.Pop();
                candidate = candidate.WithState(_stack.Peek());
                _stack.Push(o);
                return candidate;
            }
            if (candidate.Type == Transition.TransitionType.None) {
                return candidate.WithState(State);
            }
            IState newState = FindState(candidate.Name);
            return candidate.WithState(newState);
        }

        private async Task _ExitPreviousStateIfNeeded(Transition transition) {
            if (_disposed) return;
            if (State == null ||
                transition.Type == Transition.TransitionType.None) return;
            if (transition.Type == Transition.TransitionType.Push) {
                Logger.Debug($"Suspend: \"{State.Name}\"");
                await State.Suspend();
                return;
            }
            // Exit the current state
            Logger.Debug($"Exit: \"{State.Name}\"");
            await _stack.Pop().Exit();
            if (transition.Type == Transition.TransitionType.Change) {
                while (_stack.Count > 0) {
                    Logger.Debug($"Exit: \"{_stack.Peek().Name}\"");
                    await _stack.Pop().Exit();                    
                }
            }
        }

        private async Task _EnterNextStateIfNeeded(Transition transition) {
            if (_disposed) return;
            if (transition.Type == Transition.TransitionType.None) return;
            // Change the current state
            var newState = transition.State;
            Logger.Debug($"{transition.Type} State: \"{newState.Name}\"");
            // To avoid having null as from state, the first execution from state = current state
            // var fromState = State ?? newState;
            State = newState;

            if (transition.Type == Transition.TransitionType.Pop) {
                Logger.Debug($"Awake: \"{newState.Name}\"");
                await newState.Awake();
                return;
            }
            _stack.Push(newState);
            // Push or Change: enter the new state
            Logger.Debug($"Enter: \"{newState.Name}\"");
            await newState.Enter();
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}