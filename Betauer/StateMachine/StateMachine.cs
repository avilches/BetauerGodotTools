using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Betauer.Collections;
using Godot;

namespace Betauer.StateMachine {
    public interface IStateMachine {
        public IStateMachine AddState(IState state);
        public IState GetState(string name);
        public IStateMachine SetNextState(string nextState);
        public void Execute(float delta);
    }

    public class StateMachineBuilder<T> where T : IStateMachine {
        private readonly T _stateMachine;
        private readonly Queue<StateBuilder> _pending = new Queue<StateBuilder>();

        public StateMachineBuilder(T stateMachine) {
            _stateMachine = stateMachine;
        }

        public StateBuilder CreateState(string name) {
            var stateBuilder = new StateBuilder(name);
            _pending.Enqueue(stateBuilder);
            return stateBuilder;
        }

        public T Build() {
            while (_pending.Count > 0) {
                _stateMachine.AddState(_pending.Dequeue().Build());
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

        private NextState NextState { get; set; }
        private readonly Context _currentContext;

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

        public IState GetState(string stateTypeName) {
            return States[stateTypeName];
        }

        public IStateMachine SetNextState(string nextState) {
            NextState = Context.NextFrame(nextState);
            return this;
        }

        public void Execute(float delta) {
            if (_disposed) return;
            if (NextState.Name == null) throw new Exception("Please, initialize the state machine with the next state");
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
                NextState = _currentContext.CurrentState.Execute(_currentContext);
                // Only if the state is different than the current state and it's immediate, will be executed right now
                execute = NextState.IsImmediate;
            }
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
            var state = GetState(nextState.Name);
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