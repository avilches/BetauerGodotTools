namespace Betauer.FSM;

public readonly struct TransitionArgs<TStateKey> {
    public readonly TStateKey From;
    public readonly TStateKey To;
    public readonly CommandType Type;

    internal TransitionArgs(TStateKey from, TStateKey to, CommandType type) {
        From = from;
        To = to;
        Type = type;
    }
}