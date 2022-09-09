using System;
using System.Reflection;

namespace Betauer.Reflection {
    public interface IGetter {
        public Type Type { get; }
        public string Name { get; }
        public object GetValue(object instance);
        public MemberInfo MemberInfo { get; }
    }

    public interface IGetter<out T> : IGetter where T : Attribute {
        public T Attribute { get;  }
    }

    public class FastGetter<T> : FastGetter, IGetter<T> where T : Attribute {
        public T Attribute { get; }

        public FastGetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            Attribute = attribute;
        }
    }

    public class FastGetter : IGetter {
        private readonly IGetter _iGetter;

        public Type Type => _iGetter.Type;
        public string Name => _iGetter.Name;
        public MemberInfo MemberInfo => _iGetter.MemberInfo;
        public object GetValue(object instance) => _iGetter.GetValue(instance);

        public FastGetter(MemberInfo memberInfo) {
            _iGetter = memberInfo switch {
                PropertyInfo propertyInfo => new PropertyFastGetter(propertyInfo),
                FieldInfo fieldInfo => new FieldFastGetter(fieldInfo),
                MethodInfo methodInfo => new MethodFastGetter(methodInfo),
                _ => throw new ArgumentException("Member must be PropertyInfo, FieldInfo or MethodInfo")
            };
        }
    }
}