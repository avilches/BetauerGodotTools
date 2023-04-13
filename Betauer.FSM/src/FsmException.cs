using System;

namespace Betauer.FSM;

public abstract class FsmException : Exception {
    protected FsmException(string message) : base(message) {
    }
}

public class InvalidStateException : FsmException {
    public InvalidStateException(string message) : base(message) {
    }
}

public class DuplicateStateException : FsmException {
    public DuplicateStateException(string message) : base(message) {
    }
}
public class StateNotFoundException : FsmException {
    public StateNotFoundException(string message) : base(message) {
    }
}

public class EventNotFoundException : FsmException {
    public EventNotFoundException(string message) : base(message) {
    }
}