using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tools.Statemachine {
    public class Context {
        public readonly StateMachine StateMachine;
        public readonly Timer StateTimer;
        public Node Owner => StateMachine.Owner;
        public State FromState { get; private set; }
        public State CurrentState { get; private set; }
        public StateConfig Config { get; private set; }
        public int FrameCount { get; private set; }
        public float Delta { get; private set; }

        public Context(StateMachine stateMachine) {
            StateMachine = stateMachine;
            StateTimer = new AutoTimer(StateMachine.Owner).Start();
        }

        internal void Reset(State currentState, State fromState, StateConfig config) {
            CurrentState = currentState;
            FromState = fromState;
            Config = config;
            FrameCount = 0;
            Delta = 0.16f;
            StateTimer.Reset().Start();
        }

        internal void Update(float delta) {
            Delta = delta;
            FrameCount++;
        }

        public NextState NextFrame(string name, StateConfig config = null) {
            State state = StateMachine.GetState(name);
            return new NextState(state, false, config);
        }

        public NextState Immediate(string name, StateConfig config = null) {
            State state = StateMachine.GetState(name);
            return new NextState(state, true, config);
        }

        public NextState Current() {
            return new NextState(CurrentState, true);
        }

        public NextState ImmediateIfAlarm(string name, StateConfig config = null) {
            return StateTimer.IsAlarm() ? Immediate(name, config) : Current();
        }

        public NextState ImmediateIfElapsed(float elapsed, string name, StateConfig config = null) {
            return StateTimer.Elapsed > elapsed ? Immediate(name, config) : Current();
        }

        public NextState NextFrameIfAlarm(string name, StateConfig config = null) {
            return StateTimer.IsAlarm() ? NextFrame(name, config) : Current();
        }

        public NextState NextFrameIfElapsed(float elapsed, string name, StateConfig config = null) {
            return StateTimer.Elapsed > elapsed ? NextFrame(name, config) : Current();
        }
    }

    public readonly struct NextState {
        public readonly State State;
        public readonly bool IsImmediate;
        public readonly StateConfig Config;

        internal NextState(State state, bool isImmediate, StateConfig config = null) {
            State = state;
            IsImmediate = isImmediate;
            Config = config;
        }
    }

    public class StateMachine {
        private readonly StateConfig _emptyConfig = new StateConfig();
        private const int MaxChanges = 2;
        public readonly Logger Logger;
        public readonly string Name;
        public readonly Node Owner; // Node Owner is needed to setup the state Timer
        private bool _disposed = false;

        internal readonly Dictionary<string, State> States = new Dictionary<string, State>();

        private NextState NextState { get; set; }
        private readonly Context _currentContext;

        public StateMachine(Node owner, string name) {
            Owner = owner;
            Name = name;
            _currentContext = new Context(this);
            Logger = LoggerFactory.GetLogger(name, "StateMachine");
        }

        public StateMachine AddState(State state) {
            States[state.Name] = state;
            state.OnAddedToStateMachine(this);
            return this;
        }

        public State GetState(string stateTypeName) {
            return States[stateTypeName];
        }

        public StateMachine SetNextState(string nextState, StateConfig config = null) {
            NextState = _currentContext.NextFrame(nextState, config);
            return this;
        }

        public void Execute(float delta) {
            if (_disposed) return;
            if (NextState.State == null)
                throw new Exception(
                    "Please, initialize the state machine with the next state");
            _Execute(delta, _currentContext.CurrentState, NextState, new List<string>());
        }

        private void _Execute(float delta, State initialState, NextState nextState,
            List<string> immediateChanges) {
            if (_disposed) return;
            _EndPreviousStateIfNeeded(nextState);
            _StartNextStateIfNeeded(initialState, nextState, immediateChanges);
            // Execute the state
            _currentContext.Update(delta);
            NextState newNextState = _currentContext.CurrentState.Execute(_currentContext);
            if (newNextState.State == _currentContext.CurrentState) return;
            NextState = newNextState;
            if (!newNextState.IsImmediate) return;
            _Execute(delta, initialState, NextState, immediateChanges);
        }

        private void _EndPreviousStateIfNeeded(NextState nextState) {
            if (_disposed) return;
            if (_currentContext.CurrentState == null || _currentContext.CurrentState == nextState.State) return;
            // End the current state
            Logger.Debug($"End: \"{_currentContext.CurrentState.Name}\"");
            _currentContext.CurrentState.End();
        }

        private void _StartNextStateIfNeeded(State initialState, NextState nextState,
            List<string> immediateChanges) {
            if (_disposed) return;
            if (_currentContext.CurrentState == nextState.State) return;
            // Change the current state
            Logger.Debug($"New State: \"{nextState.State.Name}\"");
            immediateChanges.Add(nextState.State.Name);
            CheckImmediateChanges(initialState, immediateChanges);
            var fromState =
                _currentContext.CurrentState ??
                nextState.State; // To avoid NPE in states, the first execution will use the current state as the from state
            _currentContext.Reset(nextState.State, fromState, nextState.Config ?? _emptyConfig);

            // Start the new state
            Logger.Debug($"Start: \"{nextState.State.Name}\"");
            nextState.State.Start(_currentContext);
        }

        private void CheckImmediateChanges(State initialState, List<string> immediateChanges) {
            // _logger.Debug(initialState?.Name + " : " + string.Join(", ", immediateChanges));
            if (immediateChanges.Count > MaxChanges) {
                if (immediateChanges.Count != immediateChanges.Distinct().Count()) {
                    throw new Exception(
                        $"Circular state change detected in {initialState.Name}: {string.Join(", ", immediateChanges)}");
                }
                throw new Exception(
                    $"Too many state changes detected in {initialState.Name}: {string.Join(", ", immediateChanges)}");
            }
        }

        public void _UnhandledInput(InputEvent @event) {
            if (_disposed) return;
            _currentContext?.CurrentState?._UnhandledInput(@event);
        }

        public void Dispose() {
            _disposed = true;
        }
    }
}