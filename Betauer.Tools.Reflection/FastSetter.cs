using System;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public class FastSetter : ISetter {
    private readonly ISetter _iSetter;

    public FastSetter(MemberInfo memberInfo) {
        if (!IsValid(memberInfo))
            throw new ArgumentException(
                "MemberInfo must be PropertyInfo, FieldInfo or MethodInfo (void method with 1 parameter",
                nameof(memberInfo));
        _iSetter = memberInfo switch {
            PropertyInfo propertyInfo => new PropertyFastSetter(propertyInfo),
            FieldInfo fieldInfo => new FieldFastSetter(fieldInfo),
            MethodInfo methodInfo => new MethodFastSetter(methodInfo)
        };
    }

    public Type Type => _iSetter.Type;
    public string Name => _iSetter.Name;
    public MemberInfo MemberInfo => _iSetter.MemberInfo;

    public void SetValue(object instance, object? value) {
        _iSetter.SetValue(instance, value);
    }

    public bool CanAssign(Type type) {
        return Type.IsAssignableFrom(type);
    }

    public override string ToString() {
        return _iSetter.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return PropertyFastSetter.IsValid(memberInfo) ||
               FieldFastSetter.IsValid(memberInfo) ||
               MethodFastSetter.IsValid(memberInfo);
    }
}

public class FastSetter<T> : FastSetter, ISetter<T> where T : Attribute {
    public FastSetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
        SetterAttribute = attribute;
    }

    public T SetterAttribute { get; }
}