using System;

namespace Betauer.StateMachine;

public abstract class StateMachineException : Exception {
    protected StateMachineException(string message) : base(message) {
    }
}

public class InvalidStateException : StateMachineException {
    public InvalidStateException(string message) : base(message) {
    }
}

public class DuplicateStateException : StateMachineException {
    public DuplicateStateException(string message) : base(message) {
    }
}
public class StateNotFoundException : StateMachineException {
    public StateNotFoundException(string message) : base(message) {
    }
}

public class EventNotFoundException : StateMachineException {
    public EventNotFoundException(string message) : base(message) {
    }
}