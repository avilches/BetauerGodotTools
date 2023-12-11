using System;
using Betauer.Core;
using Betauer.Tools.FastReflection;

namespace Betauer.DI.Exceptions;

public class DuplicateServiceException : Exception {
    public DuplicateServiceException(Type type) : base($"Service already registered. Type: {type.GetTypeName()}") {
    }

    public DuplicateServiceException(Type type, string name) : base($"Service already registered. Type: {type.GetTypeName()} Name: \"{name}\"") {
    }
}