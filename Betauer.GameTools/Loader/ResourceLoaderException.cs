using System;

namespace Betauer.Loader; 

public class ResourceLoaderException : Exception {
    public ResourceLoaderException(string message) : base(message) {
    }
}