using System;

namespace Betauer.NodePath; 

public abstract class NodePathException : Exception {
    public readonly object Instance;

    protected NodePathException(string message, object instance) : base(message) {
        Instance = instance;
    }
}

public class NodePathFieldException : NodePathException {
    public readonly string Name;

    public NodePathFieldException(string name, object instance, string message) : base(message, instance) {
        Name = name;
    }
}