using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Tools.Statemachine {
    public class StateMachine {
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();
        private readonly DebugConfig _debugConfig;

        private State _currentState;
        private State _nextState;
        private bool _nextStateImmediate;
        private readonly IFrameAware _frameAware;

        public StateMachine(DebugConfig debugConfig, IFrameAware frameAware) {
            _debugConfig = debugConfig;
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

        public void Execute() {
            var stateChanges = 0;
            CheckNextState(_nextState);
            do {
                ExecuteState();
                stateChanges++;
                if (stateChanges > 10) {
                    throw new Exception("State has been changed too many times in the same frame: " + stateChanges);
                }
            } while (CheckNextState(_nextState != null && _nextStateImmediate ? _nextState : null));
        }

        private bool CheckNextState(State newState) {
            if (newState == null) return false;
            _nextState = null;

            if (_debugConfig.DEBUG_STATE_CHANGE || _debugConfig.DEBUG_STATE_FLOW) {
                GD.Print("#" + Frame + ": " + _currentState?.GetType().Name + " > " + newState.GetType().Name);
            }

            EndState(_currentState);
            _currentState = newState;
            StartState(_currentState);
            return true;
        }

        private void ExecuteState() {
            _currentState?.Execute();
        }

        public void _UnhandledInput(InputEvent @event) {
            _currentState?._UnhandledInput(@event);
        }

        private void DebugStateFlow(string message) {
            if (!_debugConfig.DEBUG_STATE_FLOW) return;
            GD.Print(message);
        }

        private void StartState(State state) {
            if (state == null) return;
            DebugStateFlow("#" + Frame + ": " + state.GetType().Name + ".Start()");
            state.Start();
        }

        private void EndState(State state) {
            if (state == null) return;
            DebugStateFlow("#" + Frame + ": " + state.GetType().Name + ".End()");
            state.End();
        }
    }
}