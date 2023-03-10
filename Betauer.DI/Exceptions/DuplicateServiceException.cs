using System;

namespace Betauer.DI.Exceptions;

public class DuplicateServiceException : Exception {
    public DuplicateServiceException(Type type) : base($"Service already registered. Type: {type.Name}") {
    }
        
    public DuplicateServiceException(string name) : base($"Service already registered. Name: {name}") {
    }
}