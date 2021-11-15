using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Godot;

namespace Tools.Statemachine {
    public struct NextState {
        private readonly Dictionary<Type, State> _states;
        public Type StateType { get; private set; }
        public bool IsImmediate { get; private set; }
        public StateConfig Config { get; private set; }

        public NextState(Dictionary<Type, State> states) : this() {
            _states = states;
        }

        public NextState(Dictionary<Type, State> states, StateConfig config) : this() {
            _states = states;
            Config = config;
        }

        public State State => StateType != null ? _states[StateType] : null;

        public NextState NextFrame(Type state, StateConfig config = null) {
            StateType = state;
            IsImmediate = false;
            Config = config;
            return this;
        }

        public NextState Immediate(Type state, StateConfig config = null) {
            StateType = state;
            IsImmediate = true;
            Config = config;
            return this;
        }

        public NextState Current() {
            StateType = null;
            IsImmediate = false;
            return this;
        }
    }

    public class StateMachine {
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();
        private readonly StateMachineDebugConfig _stateMachineDebugConfig;
        private readonly StateConfig EMPTY_CONFIG = new StateConfig();

        private State _currentState;

        private NextState _nextState;

        private readonly IFrameAware _frameAware;

        public State CurrenState => _currentState;

        public StateMachine(StateMachineDebugConfig stateMachineDebugConfig, IFrameAware frameAware) {
            _stateMachineDebugConfig = stateMachineDebugConfig;
            _frameAware = frameAware;
        }

        private long Frame => _frameAware.GetFrame();

        public StateMachine AddState(State state) {
            _states[state.GetType()] = state;
            return this;
        }

        public void SetNextState(Type nextState) {
            _nextState = new NextState(_states).Immediate(nextState);
        }

        public void SetNextState(NextState nextState) {
            _nextState = nextState;
        }

        private const int MAX_CHANGES = 2;

        public void Execute() {
            Execute(_currentState, _nextState, new List<string>());
        }

        public void Execute(State initialState, NextState nextState, List<string> immediateChanges) {
            if (_currentState != nextState.State) {
                // New state!
                if (_currentState != null) { // first execution there is no current state)
                    // End the current state
                    DebugStateFlow($"#{Frame}: {_currentState.Name}.End()");
                    _currentState.End();
                }

                // Change the current state
                DebugStateFlow($"#{Frame}: {_currentState?.Name} -> {nextState.State.Name}");
                _currentState = nextState.State;
                immediateChanges.Add(nextState.State.Name);
                CheckImmediateChanges(initialState, immediateChanges);

                // Start the state
                DebugStateFlow($"#{Frame}: {nextState.State.Name}.Start()");
                nextState.State.Start(nextState.Config ?? EMPTY_CONFIG);
            }
            // Execute the state
            NextState newNextState = _currentState.Execute(new NextState(_states));
            if (newNextState.StateType == null || newNextState.State == _currentState) return;
            _nextState = newNextState;
            if (!newNextState.IsImmediate) return;
            Execute(initialState, _nextState, immediateChanges);
        }

        private static void CheckImmediateChanges(State initialState, List<string> immediateChanges) {
            GD.Print(initialState?.Name + " : " + string.Join(", ", immediateChanges));
            if (immediateChanges.Count > MAX_CHANGES) {
                if (immediateChanges.Count != immediateChanges.Distinct().Count()) {
                    throw new Exception(
                        $"Circular state change detected in {initialState.Name}: {string.Join(", ", immediateChanges)}");
                }
                throw new Exception(
                    $"Too many state changes detected in {initialState.Name}: {string.Join(", ", immediateChanges)}");
            }
        }

        public void _UnhandledInput(InputEvent @event) {
            _currentState?._UnhandledInput(@event);
        }

        private void DebugStateFlow(string message) {
            if (!_stateMachineDebugConfig.DEBUG_STATEMACHINE_FLOW) return;
            GD.Print(message);
        }
    }

    public interface StateMachineDebugConfig {
        bool DEBUG_STATEMACHINE_FLOW { get; }
    }
}