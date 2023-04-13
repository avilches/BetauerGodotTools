using System;
using Betauer.Core;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Exceptions;

public class DuplicateServiceException : Exception {
    public DuplicateServiceException(Type type) : base($"Service already registered. Type: {type.GetTypeName()}") {
    }
        
    public DuplicateServiceException(string name) : base($"Service already registered. Name: \"{name}\"") {
    }
}