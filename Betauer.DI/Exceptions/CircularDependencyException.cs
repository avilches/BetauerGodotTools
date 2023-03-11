using System;

namespace Betauer.DI.Exceptions;

public class CircularDependencyException : Exception {
    public CircularDependencyException(string message) : base(message) {
    }
}