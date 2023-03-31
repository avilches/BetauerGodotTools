using System;
using Betauer.Tools.Reflection;

namespace Betauer.DI.Exceptions;

public class ServiceNotFoundException : Exception {
    public ServiceNotFoundException(Type type) : base($"Service not found. Type: {type.GetTypeName()}") {
    }
        
    public ServiceNotFoundException(string name) : base($"Service not found. Name: {name}") {
    }
}