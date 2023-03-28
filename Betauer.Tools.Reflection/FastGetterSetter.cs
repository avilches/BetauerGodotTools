using System;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

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

    public Type Type => Getter.Type;
    public string Name => Getter.Name;
    public MemberInfo MemberInfo => Getter.Type;

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

public class FastGetterSetter<T> : FastGetterSetter, IGetterSetter<T> where T : Attribute {
    public FastGetterSetter(MemberInfo member, T attribute) : base(member) {
        Attribute = attribute;
        GetterAttribute = attribute;
        SetterAttribute = attribute;
    }

    public T Attribute { get; }
    public T GetterAttribute { get; }
    public T SetterAttribute { get; }

}