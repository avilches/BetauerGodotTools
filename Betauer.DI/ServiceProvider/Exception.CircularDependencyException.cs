using System;

namespace Betauer.DI.ServiceProvider;

public class CircularDependencyException : Exception {
    public CircularDependencyException(string message) : base(message) {
    }
}