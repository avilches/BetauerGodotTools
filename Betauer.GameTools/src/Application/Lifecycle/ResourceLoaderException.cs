using System;

namespace Betauer.Application.Lifecycle; 

public class ResourceLoaderException : Exception {
    public ResourceLoaderException(string message) : base(message) {
    }
}