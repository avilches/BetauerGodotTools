using System;

namespace Betauer.DI; 

public abstract class InjectException : Exception {
    public readonly object Instance;

    public InjectException(string message, object instance) : base(message) {
        Instance = instance;
    }
}