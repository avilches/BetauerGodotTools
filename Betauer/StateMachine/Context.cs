namespace Betauer.StateMachine {
    public readonly struct Transition {
        public enum TransitionType {
            Push,
            Pop,
            Change,
            None
        }
        public readonly IState State;
        public readonly TransitionType Type;
        public readonly bool IsImmediate;

        public Transition(IState state, TransitionType type, bool isImmediate) {
            State = state;
            Type = type;
            IsImmediate = isImmediate;
        }
    }

    public readonly struct StateChange {
        public readonly string? Name;
        public readonly bool IsImmediate;

        public static StateChange CreatePopFrame() => new StateChange(null, false);

        public static StateChange CreatePopImmediate() => new StateChange(null, true);

        public static StateChange CreateNextFrame(string name) => new StateChange(name, false);

        public static StateChange CreateImmediate(string name) => new StateChange(name, true);

        internal StateChange(string? name, bool isImmediate) {
            Name = name;
            IsImmediate = isImmediate;
        }
    }

    public class Context {
        public readonly StateMachine StateMachine;
        public readonly Timer StateTimer;
        public IState FromState { get; private set; }
        public IState CurrentState { get; private set; }
        public int FrameCount { get; private set; }
        public float Delta { get; private set; }

        public Context(StateMachine stateMachine) {
            StateMachine = stateMachine;
            StateTimer = new AutoTimer(StateMachine.Owner).Start();
        }

        internal void Reset(IState currentState, IState fromState) {
            CurrentState = currentState;
            FromState = fromState;
            FrameCount = 0;
            Delta = 0.16f;
            StateTimer.Reset().Start();
        }

        internal void Update(float delta) {
            Delta = delta;
            FrameCount++;
        }

        public StateChange NextFrame(string name) => StateChange.CreateNextFrame(name);

        public StateChange Immediate(string name) => StateChange.CreateImmediate(name);

        public StateChange PopFrame() => StateChange.CreatePopFrame();

        public StateChange PopImmediate() => StateChange.CreatePopImmediate();

        public StateChange Repeat() {
            return new StateChange(CurrentState.Name, false);
        }

        public StateChange ImmediateIfAlarm(string name) {
            return StateTimer.IsAlarm() ? Immediate(name) : Repeat();
        }

        public StateChange ImmediateIfElapsed(float elapsed, string name) {
            return StateTimer.Elapsed > elapsed ? Immediate(name) : Repeat();
        }

        public StateChange NextFrameIfAlarm(string name) {
            return StateTimer.IsAlarm() ? NextFrame(name) : Repeat();
        }

        public StateChange NextFrameIfElapsed(float elapsed, string name) {
            return StateTimer.Elapsed > elapsed ? NextFrame(name) : Repeat();
        }
    }
}