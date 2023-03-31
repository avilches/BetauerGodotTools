using System;

namespace Betauer.Input;

public class InvalidAxisConfigurationException : Exception {
    public InvalidAxisConfigurationException(string? message) : base(message) {
    }
}