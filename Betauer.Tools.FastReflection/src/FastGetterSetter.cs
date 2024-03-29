using System;
using System.Reflection;
using Betauer.Core;
using Betauer.Tools.FastReflection.FastImpl;

namespace Betauer.Tools.FastReflection; 

public class FastGetterSetter : IGetterSetter {
    public IGetter Getter { get; }
    public ISetter Setter { get; }

    public FastGetterSetter(MemberInfo memberInfo) {
        if (!IsValid(memberInfo))
            throw new ArgumentException("MemberInfo must be PropertyInfo or FieldInfo", nameof(memberInfo));
        if (memberInfo is PropertyInfo propertyInfo) {
            Getter = new PropertyFastGetter(propertyInfo);
            Setter = new PropertyFastSetter(propertyInfo);
        } else if (memberInfo is FieldInfo fieldInfo) {
            Getter = new FieldFastGetter(fieldInfo);
            Setter = new FieldFastSetter(fieldInfo);
        } else {
            throw new Exception($"Cant' create {typeof(FastGetterSetter)} with MemberInfo type {memberInfo.GetType()}");
        }
    }

    public Type MemberType => Getter.MemberType;
    public string Name => Getter.Name;
    public MemberInfo MemberInfo => Getter.MemberType;
    public Type DeclaringType => Getter.DeclaringType;

    public void SetValue(object instance, object? value) {
        Setter.SetValue(instance, value);
    }

    public object? GetValue(object instance) {
        return Getter.GetValue(instance);
    }

    public override string ToString() {
        return Getter.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return PropertyFastGetter.IsValid(memberInfo) ||
               FieldFastGetter.IsValid(memberInfo);
    }
}

public class FastGetterSetter<T> : FastGetterSetter, IGetterSetter<T> {
    public FastGetterSetter(MemberInfo member, T attribute) : base(member) {
        Attribute = attribute;
        GetterAttribute = attribute;
        SetterAttribute = attribute;
    }

    public T Attribute { get; }
    public T GetterAttribute { get; }
    public T SetterAttribute { get; }

    public override string ToString() {
        return $"{GetterAttribute.GetType().FormatAttribute()} {Getter.ToString()}";
    }

}