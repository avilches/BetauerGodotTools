using System;

namespace Betauer.Reflection {
    public interface ISetter : IMember {
        public void SetValue(object instance, object? value);
    }

    public interface ISetter<out T> : ISetter where T : Attribute {
        public T SetterAttribute { get;  }
    }
}