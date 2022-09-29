using System;

namespace Betauer.Reflection {
    public interface IGetterSetter : IMember {
        public void SetValue(object instance, object? value);
        public object? GetValue(object instance);
    }

    public interface IGetterSetter<out T> : IGetter<T>, ISetter<T> where T : Attribute {
        public T Attribute { get; }
    }
}