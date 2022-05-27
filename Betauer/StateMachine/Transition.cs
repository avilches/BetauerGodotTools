namespace Betauer.StateMachine {
    public readonly struct Transition {
        public enum TransitionType {
            Push,
            PopPush,
            Pop,
            Change,
            None
        }

        private static readonly Transition TransitionPop = new Transition(TransitionType.Pop);
        private static readonly Transition TransitionNone = new Transition(TransitionType.None);

        internal readonly IState? State;
        public readonly string Name;
        public readonly TransitionType Type;

        public static Transition Push(string name) => new Transition(name, TransitionType.Push);
        public static Transition PopPush(string name) => new Transition(name, TransitionType.PopPush);
        public static Transition Pop() => TransitionPop;
        public static Transition Set(string name) => new Transition(name, TransitionType.Change);
        public static Transition None() => TransitionNone;

        private Transition(TransitionType type) {
            State = null;
            Name = null;
            Type = type;
        }

        private Transition(string name, TransitionType type) {
            State = null;
            Name = name;
            Type = type;
        }

        internal Transition(IState state, TransitionType type) {
            State = state;
            Name = state.Name;
            Type = type;
        }

        internal Transition WithState(IState state) {
            return new Transition(state, Type);
        }
    }
}