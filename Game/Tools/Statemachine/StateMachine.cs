using System;
using System.Collections.Generic;
using System.Linq;
using Veronenger.Game.Tools.Character;
using Godot;

namespace Veronenger.Game.Tools.Statemachine {
    public class StateMachine {
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();
        private readonly CharacterConfig _characterConfig;

        private State _currentState;
        private State _nextState;
        private Dictionary<string, object> _nextConfig;

        private bool _nextStateImmediate;
        private readonly IFrameAware _frameAware;

        public State CurrenState => _currentState;

        public StateMachine(CharacterConfig characterConfig, IFrameAware frameAware) {
            _characterConfig = characterConfig;
            _frameAware = frameAware;
        }

        private long Frame => _frameAware.GetFrame();

        public StateMachine AddState(State state) {
            _states[state.GetType()] = state;
            return this;
        }

        public void SetNextState(Type nextStateType, bool immediate = false) {
            var nextState = _states[nextStateType];
            if (_nextState == null) {
                DebugStateFlow("#" + Frame + ": " + _currentState?.GetType().Name + " | Next State = " +
                               nextState.GetType().Name + (immediate ? " (immediate)" : " (next frame)"));
            } else {
                DebugStateFlow("#" + Frame + ": " + _currentState?.GetType().Name + " | Next State = " +
                               nextState.GetType().Name + (immediate ? " (immediate)" : " (next frame)")+" (replaced old " +
                                   _nextState.GetType().Name+")");
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


        public void Execute() {
            var stateChanges = 0;
            var list = new List<string>();
            CheckNextState(_nextState);
            do {
                list.Add(_currentState.GetType().Name);
                ExecuteState();
                stateChanges++;
                if (stateChanges > 10) {
                    list.ForEach(state=> GD.Print(state));
                    throw new Exception("State has been changed too many times in the same frame: " + stateChanges);
                }
            } while (CheckNextState(_nextState != null && _nextStateImmediate ? _nextState : null));
        }

        private bool CheckNextState(State newState) {
            if (newState == null) return false;
            _nextState = null;

            if (_characterConfig.DEBUG_STATE_CHANGE || _characterConfig.DEBUG_STATE_FLOW) {
                GD.Print($"#{Frame}: {_currentState?.GetType().Name} > {newState.GetType().Name}");
            }

            EndState();
            _currentState = newState;
            StartState();
            return true;
        }

        private void ExecuteState() {
            _currentState?.Execute();
        }

        public void _UnhandledInput(InputEvent @event) {
            _currentState?._UnhandledInput(@event);
        }

        private void DebugStateFlow(string message) {
            if (!_characterConfig.DEBUG_STATE_FLOW) return;
            GD.Print(message);
        }

        private void StartState() {
            if (_currentState == null) return;
            DebugStateFlow("#" + Frame + ": " + _currentState.GetType().Name + ".Start()");
            _currentState.Configure(_nextConfig);
            _nextConfig = null;
            _currentState.Start();
        }

        private void EndState() {
            if (_currentState == null) return;
            DebugStateFlow("#" + Frame + ": " + _currentState.GetType().Name + ".End()");
            _currentState.End();
        }
    }
}