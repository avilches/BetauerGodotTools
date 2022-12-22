using System;

namespace Betauer.OnReady; 

public abstract class InjectException : Exception {
    public readonly object Instance;

    public InjectException(string message, object instance) : base(message) {
        Instance = instance;
    }
}

public class InjectFieldException : InjectException {
    public readonly string Name;

    public InjectFieldException(string name, object instance, string message) : base(message, instance) {
        Name = name;
    }
}

public abstract class OnReadyException : Exception {
    public readonly object Instance;

    public OnReadyException(string message, object instance) : base(message) {
        Instance = instance;
    }
}

public class OnReadyFieldException : OnReadyException {
    public readonly string Name;

    public OnReadyFieldException(string name, object instance, string message) : base(message, instance) {
        Name = name;
    }
}