using System;
using System.Reflection;
using Betauer.Core;
using Betauer.Tools.FastReflection.FastImpl;

namespace Betauer.Tools.FastReflection; 

public class FastSetter : ISetter {
    public ISetter Setter { get; }

    public FastSetter(MemberInfo memberInfo) {
        if (!IsValid(memberInfo))
            throw new ArgumentException("MemberInfo must be PropertyInfo, FieldInfo or MethodInfo (void method with 1 parameter)", nameof(memberInfo));
        Setter = memberInfo switch {
            PropertyInfo propertyInfo => new PropertyFastSetter(propertyInfo),
            FieldInfo fieldInfo => new FieldFastSetter(fieldInfo),
            MethodInfo methodInfo => new MethodFastSetter(methodInfo)
        };
    }

    public Type MemberType => Setter.MemberType;
    public string Name => Setter.Name;
    public MemberInfo MemberInfo => Setter.MemberInfo;
    public Type DeclaringType => Setter.DeclaringType;

    public void SetValue(object instance, object? value) {
        Setter.SetValue(instance, value);
    }

    public override string ToString() {
        return Setter.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return PropertyFastSetter.IsValid(memberInfo) ||
               FieldFastSetter.IsValid(memberInfo) ||
               MethodFastSetter.IsValid(memberInfo);
    }
}

public class FastSetter<T> : FastSetter, ISetter<T> {
    public FastSetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
        SetterAttribute = attribute;
    }

    public T SetterAttribute { get; }
    
    public override string ToString() {
        return $"{SetterAttribute.GetType().FormatAttribute()} {Setter.ToString()}";
    }
}