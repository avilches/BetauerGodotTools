using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tools.Statemachine {
    public class Context {
        public readonly State CurrentState;
        public readonly StateMachine StateMachine;
        public readonly Timer StateTimer;
        public readonly State FromState;
        public int FrameCount { get; private set; }
        public float Delta { get; private set; }

        public Context(StateMachine stateMachine, State currentState, State fromState) {
            StateMachine = stateMachine;
            CurrentState = currentState;
            FromState = fromState;
            StateTimer = new Timer().Start();
            FrameCount = 0;
            Delta = 0.16f;
        }

        internal void Update(float delta) {
            Delta = delta;
            StateTimer.Update(delta);
            FrameCount++;
        }

        public NextState NextFrame(Type state, StateConfig config = null) {
            return new NextState(StateMachine, state, false, config);
        }

        public NextState Immediate(Type state, StateConfig config = null) {
            return new NextState(StateMachine, state, true, config);
        }

        public NextState Current() {
            return new NextState(StateMachine, CurrentState.GetType(), true);
        }

        public NextState ImmediateIfAlarm(Type type, StateConfig config = null) {
            return StateTimer.IsAlarm() ? Immediate(type, config) : Current();
        }

        public NextState ImmediateIfElapsed(float elapsed, Type type, StateConfig config = null) {
            return StateTimer.Elapsed > elapsed ? Immediate(type, config) : Current();
        }

        public NextState NextFrameIfAlarm(Type type, StateConfig config = null) {
            return StateTimer.IsAlarm() ? NextFrame(type, config) : Current();
        }

        public NextState NextFrameIfElapsed(float elapsed, Type type, StateConfig config = null) {
            return StateTimer.Elapsed > elapsed ? NextFrame(type, config) : Current();
        }
    }

    public readonly struct NextState {
        public readonly State State;
        public readonly bool IsImmediate;
        public readonly StateConfig Config;

        internal NextState(StateMachine stateMachine, Type stateType, bool isImmediate) {
            State = stateMachine.States[stateType];
            if (State == null) throw new Exception("State " + stateType.Name + " not found in StateMachine");
            IsImmediate = isImmediate;
            Config = null;
        }

        internal NextState(StateMachine stateMachine, Type stateType, bool isImmediate, StateConfig config) {
            State = stateMachine.States[stateType];
            if (State == null) throw new Exception("State " + stateType.Name + " not found in StateMachine");
            IsImmediate = isImmediate;
            Config = config;
        }
    }

    public class StateMachine {
        private readonly StateConfig _emptyConfig = new StateConfig();
        private const int MaxChanges = 2;
        private readonly Logger _logger;

        internal readonly Dictionary<Type, State> States = new Dictionary<Type, State>();

        private NextState NextState { get; set; }
        private Context CurrentContext { get; set; }

        public StateMachine(string name) {
            _logger = LoggerFactory.GetLogger(GetType(), name);
        }

        public StateMachine AddState(State state) {
            States[state.GetType()] = state;
            return this;
        }

        public void SetNextState(Type nextState, bool immediate = false) {
            NextState = new NextState(this, nextState, immediate);
        }

        public void Execute(float delta) {
            if (NextState.State == null)
                throw new Exception(
                    "Please, initialize the state machine with the next state, even if the next state is the same as the current one");
           _Execute(delta, CurrentContext?.CurrentState, NextState, new List<string>());
        }

        private void _Execute(float delta, State initialState, NextState nextState, List<string> immediateChanges) {
            _EndPreviousStateIfNeeded(nextState);
            _StartNextStateIfNeeded(initialState, nextState, immediateChanges);
            // Execute the state
            CurrentContext.Update(delta);
            NextState newNextState = CurrentContext.CurrentState.Execute(CurrentContext);
            if (newNextState.State == CurrentContext.CurrentState) return;
            NextState = newNextState;
            if (!newNextState.IsImmediate) return;
            _Execute(delta, initialState, NextState, immediateChanges);
        }

        private void _EndPreviousStateIfNeeded(NextState nextState) {
            if (CurrentContext == null || CurrentContext.CurrentState == nextState.State) return;
            // End the current state
            _logger.Debug($"{CurrentContext.CurrentState.Name}.End()");
            CurrentContext.CurrentState.End();
        }

        private void _StartNextStateIfNeeded(State initialState, NextState nextState, List<string> immediateChanges) {
            if (CurrentContext?.CurrentState == nextState.State) return;
            // Change the current state
            // _logger.Debug($"{CurrentContext.State?.Name} -> {nextState.State.Name}");
            var fromState =
                CurrentContext?.CurrentState ??
                nextState.State; // To avoid NPE in states, the first execution will use the current state as the from state
            CurrentContext = new Context(this, nextState.State, fromState);
            immediateChanges.Add(nextState.State.Name);
            CheckImmediateChanges(initialState, immediateChanges);

            // Start the new state
            CurrentContext.StateTimer.Reset().Start();
            _logger.Debug($"{nextState.State.Name}.Start()");
            nextState.State.Start(CurrentContext, nextState.Config ?? _emptyConfig);
        }

        private static void CheckImmediateChanges(State initialState, List<string> immediateChanges) {
            // GD.Print(initialState?.Name + " : " + string.Join(", ", immediateChanges));
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
            CurrentContext.CurrentState?._UnhandledInput(@event);
        }
    }
}