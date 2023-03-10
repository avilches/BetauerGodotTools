using System;

namespace Betauer.DI.Exceptions;

public class ServiceNotFoundException : Exception {
    public ServiceNotFoundException(Type type) : base($"Service not found. Type: {type.Name}") {
    }
        
    public ServiceNotFoundException(string name) : base($"Service not found. Name: {name}") {
    }
}