using System;
using System.Reflection;

namespace Betauer.Reflection {
    public class FastGetter : IGetter {
        private readonly IGetter _iGetter;

        public Type Type => _iGetter.Type;
        public string Name => _iGetter.Name;
        public MemberInfo MemberInfo => _iGetter.MemberInfo;
        public object? GetValue(object instance) => _iGetter.GetValue(instance);

        public FastGetter(MemberInfo memberInfo) {
            if (!IsValid(memberInfo)) {
                throw new ArgumentException(
                    "MemberInfo must be PropertyInfo, FieldInfo or MethodInfo (non void return type with 0 parameters)",
                    nameof(memberInfo));
            }
            _iGetter = memberInfo switch {
                PropertyInfo propertyInfo => new PropertyFastGetter(propertyInfo),
                FieldInfo fieldInfo => new FieldFastGetter(fieldInfo),
                MethodInfo methodInfo => new MethodFastGetter(methodInfo),
            };
        }

        public override string ToString() => _iGetter.ToString();
        
        public static bool IsValid(MemberInfo memberInfo) =>
            PropertyFastGetter.IsValid(memberInfo) ||
            FieldFastGetter.IsValid(memberInfo) ||
            MethodFastGetter.IsValid(memberInfo);
    }

    public class FastGetter<T> : FastGetter, IGetter<T> where T : Attribute {
        public T GetterAttribute { get; }

        public FastGetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            GetterAttribute = attribute;
        }
    }
}