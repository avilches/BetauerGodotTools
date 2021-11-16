using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Tools.Statemachine {
    public readonly struct Context {
        internal readonly State State;

        public readonly StateMachine StateMachine;
        public readonly Timer StateTimer;
        private readonly long _frameStart;
        public long StateFrame => StateMachine.Frame - _frameStart;
        public readonly State FromState;

        public Context(StateMachine stateMachine, State state, State fromState, long frameStart) : this() {
            StateMachine = stateMachine;
            State = state;
            FromState = fromState;
            StateTimer = new Timer().Start();
            _frameStart = frameStart;
        }

        public float Delta => StateMachine.Delta;

        public NextState NextFrame(Type state, StateConfig config = null) {
            return new NextState(StateMachine, state, false, config);
        }

        public NextState Immediate(Type state, StateConfig config = null) {
            return new NextState(StateMachine, state, true, config);
        }

        public NextState Current() {
            return new NextState(StateMachine, null, true);
        }

        public NextState ImmediateIfAlarm(Type type, StateConfig config = null) {
            return StateTimer.IsAlarm() ? Immediate(type, config) : Current();
        }

        public NextState IfAlarmNextFrame(Type type, StateConfig config = null) {
            return StateTimer.IsAlarm() ? NextFrame(type, config) : Current();
        }
    }

    public readonly struct NextState {
        public readonly Type StateType;
        public readonly bool IsImmediate;
        public readonly StateConfig Config;
        public readonly StateMachine StateMachine;

        public NextState(StateMachine stateMachine, Type stateType, bool isImmediate) {
            StateType = stateType;
            IsImmediate = isImmediate;
            StateMachine = stateMachine;
            Config = null;
        }

        public NextState(StateMachine stateMachine, Type stateType, bool isImmediate, StateConfig config) {
            StateType = stateType;
            IsImmediate = isImmediate;
            StateMachine = stateMachine;
            Config = config;
        }

        public State State => StateType != null ? StateMachine.States[StateType] : null;
    }

    public class StateMachine {
        private readonly StateMachineDebugConfig _stateMachineDebugConfig;
        private readonly StateConfig _emptyConfig = new StateConfig();
        private const int MaxChanges = 2;
        private readonly IFrameDeltaAware _frameDeltaAware;
        public long Frame => _frameDeltaAware.GetFrame();
        public float Delta => _frameDeltaAware.GetDelta();

        internal readonly Dictionary<Type, State> States = new Dictionary<Type, State>();

        private NextState NextState { get; set; }
        private Context CurrentContext { get; set; }

        public StateMachine(StateMachineDebugConfig stateMachineDebugConfig, IFrameDeltaAware frameDeltaAware) {
            _stateMachineDebugConfig = stateMachineDebugConfig;
            _frameDeltaAware = frameDeltaAware;
        }

        public StateMachine AddState(State state) {
            States[state.GetType()] = state;
            return this;
        }

        public void SetNextState(Type nextState, bool immediate = false) {
            NextState = new NextState(this, nextState, immediate);
        }

        public void SetNextState(NextState nextState) {
            NextState = nextState;
        }

        public void Execute() {
            Execute(CurrentContext.State, NextState, new List<string>());
        }

        private void Execute(State initialState, NextState nextState, List<string> immediateChanges) {
            if (CurrentContext.State != nextState.State) {
                // New state!
                if (CurrentContext.State != null) { // first execution there is no current state)
                    // End the current state
                    DebugStateFlow($"#{Frame}: {CurrentContext.State.Name}.End()");
                    CurrentContext.State.End();
                }

                // Change the current state
                DebugStateFlow($"#{Frame}: {CurrentContext.State?.Name} -> {nextState.State.Name}");
                CurrentContext = new Context(this, nextState.State, CurrentContext.State ?? nextState.State, Frame);
                immediateChanges.Add(nextState.State.Name);
                CheckImmediateChanges(initialState, immediateChanges);

                // Start the state
                CurrentContext.StateTimer.Reset().Start();
                DebugStateFlow($"#{Frame}: {nextState.State.Name}.Start()");
                nextState.State.Start(CurrentContext, nextState.Config ?? _emptyConfig);
            }
            // Execute the state
            CurrentContext.StateTimer.Add(_frameDeltaAware.GetDelta());
            NextState newNextState = CurrentContext.State.Execute(CurrentContext);
            if (newNextState.StateType == null || newNextState.State == CurrentContext.State) return;
            NextState = newNextState;
            if (!newNextState.IsImmediate) return;
            Execute(initialState, NextState, immediateChanges);
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
            CurrentContext.State?._UnhandledInput(@event);
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