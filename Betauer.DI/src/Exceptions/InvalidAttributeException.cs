using System;

namespace Betauer.DI.Exceptions;

public class InvalidAttributeException : Exception {
    public InvalidAttributeException(string message) : base(message) {
    }
}