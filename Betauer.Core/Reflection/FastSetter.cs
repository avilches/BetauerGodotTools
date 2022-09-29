using System;
using System.Reflection;

namespace Betauer.Reflection {
    public class FastSetter : ISetter {
        private readonly ISetter _iSetter;

        public Type Type => _iSetter.Type;
        public string Name => _iSetter.Name;
        public MemberInfo MemberInfo => _iSetter.MemberInfo;
        public void SetValue(object instance, object? value) => _iSetter.SetValue(instance, value);
        
        public FastSetter(MemberInfo memberInfo) {
            if (!IsValid(memberInfo)) {
                throw new ArgumentException(
                    "MemberInfo must be PropertyInfo, FieldInfo or MethodInfo (void method with 1 parameter",
                    nameof(memberInfo));
            }
            _iSetter = memberInfo switch {
                PropertyInfo propertyInfo => new PropertyFastSetter(propertyInfo),
                FieldInfo fieldInfo => new FieldFastSetter(fieldInfo),
                MethodInfo methodInfo => new MethodFastSetter(methodInfo),
            };
        }
        public override string ToString() => _iSetter.ToString();

        public static bool IsValid(MemberInfo memberInfo) =>
            PropertyFastSetter.IsValid(memberInfo) ||
            FieldFastSetter.IsValid(memberInfo) ||
            MethodFastSetter.IsValid(memberInfo);
    }

    public class FastSetter<T> : FastSetter, ISetter<T> where T : Attribute {
        public T SetterAttribute { get; }

        public FastSetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            SetterAttribute = attribute;
        }
    }
}