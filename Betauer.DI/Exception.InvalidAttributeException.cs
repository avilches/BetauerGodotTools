using System;

namespace Betauer.DI;

public class InvalidAttributeException : Exception {
    public InvalidAttributeException(string message) : base(message) {
    }
}