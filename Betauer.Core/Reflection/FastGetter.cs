using System;
using System.Reflection;

namespace Betauer.Reflection {
    public interface IGetter {
        public Type Type { get; }
        public string Name { get; }
        public Func<object, object> GetValue { get; }
        public MemberInfo MemberInfo { get; }
    }

    public class FastGetter : IGetter {
        private readonly IGetter _iGetter;

        public Type Type => _iGetter.Type;
        public string Name => _iGetter.Name;
        public Func<object, object> GetValue => _iGetter.GetValue;
        public MemberInfo MemberInfo => _iGetter.MemberInfo;
        
        public FastGetter(MemberInfo memberInfo) {
            _iGetter = memberInfo switch {
                PropertyInfo propertyInfo => new PropertyFastGetter(propertyInfo),
                FieldInfo fieldInfo => new FieldFastGetter(fieldInfo),
                MethodInfo methodInfo => new MethodFastGetter(methodInfo),
                _ => throw new ArgumentException("Member must be PropertyInfo, FieldInfo or MethodInfo")
            };
        }
    }

    public interface IGetter<T> : IGetter {
        public T Attribute { get;  }
    }

    public class FastGetter<T> : FastGetter, IGetter<T> {
        public T Attribute { get; }

        public FastGetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            Attribute = attribute;
        }
    }
}