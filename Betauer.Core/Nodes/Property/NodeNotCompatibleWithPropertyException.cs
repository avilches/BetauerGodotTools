using System;
using System.Runtime.Serialization;

namespace Betauer.Core.Nodes.Property; 

public class NodeNotCompatibleWithPropertyException : Exception {
    public NodeNotCompatibleWithPropertyException() {
    }

    public NodeNotCompatibleWithPropertyException(string message) : base(message) {
    }
}