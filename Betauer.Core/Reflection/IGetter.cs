using System;

namespace Betauer.Reflection {
    public interface IGetter : IMember {
        public object? GetValue(object instance);
    }

    public interface IGetter<out T> : IGetter where T : Attribute {
        public T GetterAttribute { get; }
    }
}