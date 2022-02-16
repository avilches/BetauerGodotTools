namespace Betauer.StateMachine {
    public readonly struct NextState {
        public readonly string Name;
        public readonly bool IsImmediate;

        internal NextState(string name, bool isImmediate) {
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

        public static NextState NextFrame(string name) {
            // IState state = StateMachine.GetState(name);
            return new NextState(name, false);
        }

        public static NextState Immediate(string name) {
            // IState state = StateMachine.GetState(name);
            return new NextState(name, true);
        }

        public NextState Current() {
            return new NextState(CurrentState.Name, false);
        }

        public NextState ImmediateIfAlarm(string name) {
            return StateTimer.IsAlarm() ? Immediate(name) : Current();
        }

        public NextState ImmediateIfElapsed(float elapsed, string name) {
            return StateTimer.Elapsed > elapsed ? Immediate(name) : Current();
        }

        public NextState NextFrameIfAlarm(string name) {
            return StateTimer.IsAlarm() ? NextFrame(name) : Current();
        }

        public NextState NextFrameIfElapsed(float elapsed, string name) {
            return StateTimer.Elapsed > elapsed ? NextFrame(name) : Current();
        }
    }
}