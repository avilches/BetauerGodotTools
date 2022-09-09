using System;
using System.Reflection;
using Godot;

namespace Betauer.Reflection {
    public interface ISetter {
        public Type Type { get; }
        public string Name { get; }
        public void SetValue(object instance, object value);
        public MemberInfo MemberInfo { get; }
    }

    public interface ISetter<out T> : ISetter where T : Attribute {
        public T Attribute { get;  }
    }

    public class FastSetter<T> : FastSetter, ISetter<T> where T : Attribute {
        public T Attribute { get; }

        public FastSetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            Attribute = attribute;
        }
    }

    public class FastSetter : ISetter {
        private readonly ISetter _iSetter;

        public Type Type => _iSetter.Type;
        public string Name => _iSetter.Name;
        public MemberInfo MemberInfo => _iSetter.MemberInfo;
        public void SetValue(object instance, object value) => _iSetter.SetValue(instance, value);
        
        public FastSetter(MemberInfo memberInfo) {
            _iSetter = memberInfo switch {
                PropertyInfo propertyInfo => new PropertyFastSetter(propertyInfo),
                FieldInfo fieldInfo => new FieldFastSetter(fieldInfo),
                MethodInfo methodInfo => new MethodFastSetter(methodInfo),
                _ => throw new ArgumentException("Member must be PropertyInfo, FieldInfo or MethodInfo")
            };
        }
        public override string ToString() => _iSetter.ToString();
    }
}