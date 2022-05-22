using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Betauer.Collections;
using Godot;

namespace Betauer.StateMachine {
    public interface IStateMachine {
        public IStateMachine AddState(IState state);
        public IState FindState(string name);
        public IStateMachine SetNextState(string nextState);
        public void Execute(float delta);
        public IState CurrentState { get; }
        public NextState NextState { get; }

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

        public NextState NextState { get; set; }
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
            NextState = Validate(NextState.PopNextFrame);
            return this;
        }

        public IStateMachine SetNextState(string nextState) {
            NextState = Validate(NextState.NextFrame(nextState));
            return this;
        }

        public void Execute(float delta) {
            if (_disposed) return;
            if (CurrentState == null && NextState.Name == null) throw new Exception("Please, initialize the state machine with a valid next state");
            _Execute(delta, _currentContext.CurrentState);
        }

        private void _Execute(float delta, IState initialState) {
            var execute = true;
            SimpleLinkedList<string> immediateChanges = new SimpleLinkedList<string>();
            while (execute) {
                if (_disposed) return;
                _EndPreviousStateIfNeeded(NextState);
                _StartNextStateIfNeeded(initialState, NextState, immediateChanges);
                _currentContext.Update(delta);
                NextState = Validate(_currentContext.CurrentState.Execute(_currentContext));
                // Only if the state is different than the current state and it's immediate, will be executed right now
                execute = NextState.IsImmediate;
            }
        }

        private NextState Validate(NextState candidate) {
            if (candidate.Name == null) {
                // Pop
                if (CurrentState?.Parent == null) {
                    throw new Exception("Can't pop state from a root or null state (there is no parent to go!)");
                }
                return new NextState(CurrentState.Parent.Name, candidate.IsImmediate);
            } else {
                IState newState = FindState(candidate.Name);
                if (CurrentState == null) {
                    if (newState.Parent != null) {
                        throw new Exception("Only root (non-nested) states are allowed");
                    }
                } else {
                    if (newState.Parent != CurrentState.Parent && // sibling state 
                        newState.Parent != CurrentState && // child state
                        newState != CurrentState.Parent // parent state
                    ) {
                        throw new Exception("Only siblings or child states are allowed");
                    }
                }
            }
            return candidate;
        }

        private void _EndPreviousStateIfNeeded(NextState nextState) {
            if (_disposed) return;
            if (_currentContext.CurrentState == null || _currentContext.CurrentState.Name == nextState.Name) return;
            // End the current state
            Logger.Debug($"End: \"{_currentContext.CurrentState.Name}\"");
            _currentContext.CurrentState.End();
        }

        private void _StartNextStateIfNeeded(IState initialState, NextState nextState, ICollection<string> immediateChanges) {
            if (_disposed) return;
            if (_currentContext.CurrentState?.Name == nextState.Name) return;
            // Change the current state
            Logger.Debug($"New State: \"{nextState.Name}\"");
            immediateChanges.Add(nextState.Name);
            CheckImmediateChanges(initialState, immediateChanges);
            var state = FindState(nextState.Name);
            // To avoid having null as from state, the first execution from state = current state
            var fromState = _currentContext.CurrentState ?? state;
            _currentContext.Reset(state, fromState);

            // Start the new state
            Logger.Debug($"Start: \"{nextState.Name}\"");
            state.Start(_currentContext);
        }

        private static void CheckImmediateChanges(IState initialState, ICollection<string> immediateChanges) {
            // Logger.Debug(
            // initialState?.Name + " (" + immediateChanges.Count + "):" + string.Join(", ", immediateChanges));
            if (immediateChanges.Count > MaxChanges) {
                throw new Exception(
                    $"Too many state changes detected in {initialState.Name}: {string.Join(", ", immediateChanges)}");
            }
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}