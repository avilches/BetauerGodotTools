using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tools.Statemachine {
    public class StateMachine {
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();
        private readonly StateMachineDebugConfig _stateMachineDebugConfig;

        private State _currentState;
        private State _nextState;
        private Dictionary<string, object> _nextConfig;

        private bool _nextStateImmediate;
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

        public void SetNextState(Type nextStateType, bool immediate = false) {
            State nextState = _states[nextStateType];
            if (_nextState == null) {
                DebugStateFlow(
                    $"#{Frame}: {_currentState?.GetType().Name} | Next State = {nextState.GetType().Name}{(immediate ? " (immediate)" : " (next frame)")}");
            } else {
                DebugStateFlow(
                    $"#{Frame}: {_currentState?.GetType().Name} | Next State = {nextState.GetType().Name}{(immediate ? " (immediate)" : " (next frame)")} (replaced old {_nextState.GetType().Name})");
            }

            _nextState = nextState;
            _nextStateImmediate = immediate;
        }

        public void SetNextConfig(Dictionary<string, object> config) {
            _nextConfig = config;
        }

        public void SetNextConfig(string key, object value) {
            if (_nextConfig == null) {
                _nextConfig = new Dictionary<string, object>();
            }
            _nextConfig[key] = value;
        }

        public Dictionary<string, object> GetNextConfig() {
            return _nextConfig;
        }


        private const int MAX_CHANGES = 2;

        public void Execute() {
            var stateChanges = 0;
            CheckNextState(_nextState);
            if (_currentState == null) return;
            var immediateChanges = new List<string>(MAX_CHANGES+1);
            do {
                immediateChanges.Add(_currentState.GetType().Name);
                _currentState?.Execute();
                stateChanges++;
                if (stateChanges > MAX_CHANGES) {
                    if (immediateChanges.Count != immediateChanges.Distinct().Count()) {
                        throw new Exception(
                            $"{stateChanges} > {MAX_CHANGES} immediate changes in the same frame: {string.Join(", ", immediateChanges)}");
                    } else {
                        GD.Print("mmmmmmmmmm");
                    }
                }
            } while (CheckNextState(_nextState != null && _nextStateImmediate ? _nextState : null));
            // if (stateChanges > 1) {
                // GD.Print($"{stateChanges} immediate: {string.Join(", ", immediateChanges)}");
            // }
        }

        private bool CheckNextState(State newState) {
            if (newState == null) return false;
            _nextState = null;

            if (_stateMachineDebugConfig.DEBUG_STATEMACHINE_CHANGE || _stateMachineDebugConfig.DEBUG_STATEMACHINE_FLOW) {
                GD.Print($"#{Frame}: {_currentState?.GetType().Name} -> {newState.GetType().Name}");
            }

            EndState();
            _currentState = newState;
            StartState();
            return true;
        }

        public void _UnhandledInput(InputEvent @event) {
            _currentState?._UnhandledInput(@event);
        }

        private void DebugStateFlow(string message) {
            if (!_stateMachineDebugConfig.DEBUG_STATEMACHINE_FLOW) return;
            GD.Print(message);
        }

        private void StartState() {
            if (_currentState == null) return;
            DebugStateFlow($"#{Frame}: {_currentState.GetType().Name}.Start()");
            if (_nextConfig != null) _currentState.Configure(_nextConfig);
            _nextConfig = null;
            _currentState.Start();
        }

        private void EndState() {
            if (_currentState == null) return;
            DebugStateFlow($"#{Frame}: {_currentState.GetType().Name}.End()");
            _currentState.End();
        }
    }

    public interface StateMachineDebugConfig {
        bool DEBUG_STATEMACHINE_FLOW { get; }
        bool DEBUG_STATEMACHINE_CHANGE { get; }
    }
}