using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Tools.Statemachine {
    public class StateMachine {
        private readonly Dictionary<Type, State> _states = new Dictionary<Type, State>();
        private readonly DebugConfig _debugConfig;

        private State _currentState;
        private State _nextState;
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

        public void SetNextState(Type nextStateType) {
            var nextState = _states[nextStateType];
            DebugStateFlow("#"+Frame+": "+_currentState?.GetType().Name+" | SetNextState "+nextState.GetType().Name+"");
            _nextState = nextState;
        }

        public void ChangeStateTo(Type newStateType) {
            var newState = _states[newStateType];

            if (_debugConfig.DEBUG_STATE_CHANGE || _debugConfig.DEBUG_STATE_FLOW) {
                GD.Print("#" + Frame + ": " + _currentState?.GetType().Name + " > " +
                         newState.GetType().Name);
            }

            CallEnd(_currentState);
            _currentState = newState;
            CallStart(_currentState);
            _currentState.Execute();
        }

        public void Execute() {
            if (_nextState != null) {
                DebugStateFlow("#"+Frame+": "+_currentState?.GetType().Name+" | _PhysicsProcess(). Detected nextState "+_nextState.GetType().Name);
                var nextState = _nextState;
                _nextState = null;
                ChangeStateTo(nextState.GetType());
            } else {
                _currentState?.Execute();
            }
        }

        public void _UnhandledInput(InputEvent @event) {
            _currentState?._UnhandledInput(@event);
        }

        private void DebugStateFlow(string message) {
            if (!_debugConfig.DEBUG_STATE_FLOW) return;
            GD.Print(message);
        }

        private void CallStart(State state) {
            if (state == null) return;
            DebugStateFlow("#" + Frame + ": " + state.GetType().Name + ".Start()");
            state.Start();
        }

        private void CallEnd(State state) {
            if (state == null) return;
            DebugStateFlow("#" + Frame + ": " + state.GetType().Name + ".End()");
            state.End();
        }

    }
}