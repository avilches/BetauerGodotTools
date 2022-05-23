namespace Betauer.StateMachine {
    public readonly struct StateChange {
        public enum TransitionType {
            Push,
            PopPush,
            Pop,
            Change,
            None
        }
        internal readonly IState? State;
        public readonly string Name;
        public readonly TransitionType Type;
        public readonly bool IsImmediate;

        public static StateChange CreatePushNextFrame(string name) => new StateChange(name, TransitionType.Push, false);
        public static StateChange CreatePushImmediate(string name) => new StateChange(name, TransitionType.Push, true);
        public static StateChange CreatePopPushNextFrame(string name) => new StateChange(name, TransitionType.PopPush, false);
        public static StateChange CreatePopPushImmediate(string name) => new StateChange(name, TransitionType.PopPush, true);
        public static StateChange CreatePopNextFrame() => new StateChange(TransitionType.Pop, false);
        public static StateChange CreatePopImmediate() => new StateChange(TransitionType.Pop, true);
        public static StateChange CreateNextFrame(string name) => new StateChange(name, TransitionType.Change, false);
        public static StateChange CreateImmediate(string name) => new StateChange(name, TransitionType.Change, true);
        public static StateChange CreateNone() => new StateChange(TransitionType.None, false);
        internal static StateChange _CreateImmediateNone() => new StateChange(TransitionType.None, true);

        private StateChange(TransitionType type, bool isImmediate) {
            State = null;
            Name = null;
            Type = type;
            IsImmediate = isImmediate;
        }
        public StateChange(string name, TransitionType type, bool isImmediate) {
            State = null;
            Name = name;
            Type = type;
            IsImmediate = isImmediate;
        }

        internal StateChange(IState state, TransitionType type, bool isImmediate) {
            State = state;
            Name = state.Name;
            Type = type;
            IsImmediate = isImmediate;
        }

        internal StateChange WithState(IState state) {
            return new StateChange(state, Type, IsImmediate);
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

        public StateChange PushNextFrame(string name) => StateChange.CreatePushNextFrame(name);

        public StateChange PushImmediate(string name) => StateChange.CreatePushImmediate(name);

        public StateChange PopPushNextFrame(string name) => StateChange.CreatePopPushNextFrame(name);

        public StateChange PopPushImmediate(string name) => StateChange.CreatePopPushImmediate(name);

        public StateChange PopFrame() => StateChange.CreatePopNextFrame();

        public StateChange PopImmediate() => StateChange.CreatePopImmediate();

        public StateChange Repeat() {
            return StateChange.CreateNone();
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