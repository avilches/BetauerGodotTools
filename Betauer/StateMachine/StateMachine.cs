using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Collections;
using Godot;

namespace Betauer.StateMachine {

    public class StateMachineAlreadyStartedException : Exception { }

    public class StateMachineNotInitializedException : Exception { }
    public interface IStateMachine {
        public void On(string on, Transition transition);
        public void InitialState(string state);
        public void AddState(IState state);
        public IState FindState(string name);
        public void Trigger(string name);
        public Task Execute(float delta);
        public IState State { get; }
        public Transition Transition { get; }

    }

    public class StateMachineBuilder<T> where T : IStateMachine {
        private readonly T _stateMachine;
        private readonly Queue<StateBuilder<T, StateMachineBuilder<T>>> _pending = new Queue<StateBuilder<T, StateMachineBuilder<T>>>();
        private Queue<Tuple<string, Transition>>? _events;

        public StateMachineBuilder(T stateMachine) {
            _stateMachine = stateMachine;
        }

        public StateMachineBuilder<T> On(string on, Transition transition) {
            if (transition.Type == Transition.TransitionType.Trigger) {
                throw new StackOverflowException("Transition " + on + " can't be other transition");
            }         
            _events ??= new Queue<Tuple<string, Transition>>();
            _events.Enqueue(new Tuple<string, Transition>(on, transition));
            return this;
        }

        public StateBuilder<T, StateMachineBuilder<T>> State(string name) {
            var stateBuilder = new StateBuilder<T, StateMachineBuilder<T>>(name, this);
            _pending.Enqueue(stateBuilder);
            return stateBuilder;
        }

        public T Build() {
            while (_pending.Count > 0) _pending.Dequeue().Build(_stateMachine);
            while (_events != null && _events.Count > 0) {
                var entry = _events.Dequeue();
                _stateMachine.On(entry.Item1, entry.Item2);
            }
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
        private Dictionary<string, Transition>? _events;

        public StateMachine(string? name = null) {
            Name = name;
            Logger = name != null ?
                LoggerFactory.GetLogger(name, "StateMachine") : 
                LoggerFactory.GetLogger("StateMachine");
        }

        public StateMachineBuilder<StateMachine> CreateBuilder() {
            return new StateMachineBuilder<StateMachine>(this);
        }

        public void On(string on, Transition transition) {
            if (transition.Type == Transition.TransitionType.Trigger) {
                throw new StackOverflowException("Transition " + on + " can't be other transition");
            }         
            _events ??= new Dictionary<string, Transition>();
            _events[on] = transition;
        }


        public void AddState(IState state) {
            if (States.ContainsKey(state.Name)) throw new DuplicateNameException();
            States[state.Name] = state;
        }

        public IState FindState(string stateTypeName) {
            return States[stateTypeName];
        }

        public void Trigger(string name) {
            Transition = GetTransitionFromTrigger(name);
        }

        private Transition GetTransitionFromTrigger(string name) {
            if (State != null && State.HasTransition(name)) {
                Transition transition = State.GetTransition(name);
                if (transition.Type == Transition.TransitionType.Trigger) {
                    throw new StackOverflowException("Transition " + name + " can't be other transition");
                }         
                return Validate(transition);
            }
            if (_events != null && _events.ContainsKey(name)) {
                Transition transition = _events[name];
                if (transition.Type == Transition.TransitionType.Trigger) {
                    throw new StackOverflowException("Transition " + name + " can't be other transition");
                }         
                return Validate(transition);
            }
            throw new KeyNotFoundException("Transition " + name + " not found");
        }

        public void InitialState(string state) {
            if (State == null) {
                Transition = Validate(Transition.Set(state));
                return;
            }
            throw new StateMachineAlreadyStartedException();
        }

        public async Task Execute(float delta) {
            if (_disposed) return;
            var transition = Transition;
            if (State == null && transition.State == null)
                throw new StateMachineNotInitializedException();
            // IState fromState = State ?? transition.State;
            await _ExitPreviousStateIfNeeded(transition);
            await _EnterNextStateIfNeeded(transition);
            Transition = Validate(await State.Execute(delta));
        }

        private Transition Validate(Transition candidate) {
            if (candidate.Type == Transition.TransitionType.Trigger ) {
                candidate = GetTransitionFromTrigger(candidate.Name);
            }
            if (candidate.Type == Transition.TransitionType.Change && State?.Name == candidate.Name) {
                return Transition.None();
            }
            if (candidate.Type == Transition.TransitionType.Pop) {
                if (_stack.Count <= 1) {
                    throw new InvalidOperationException("Pop");
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