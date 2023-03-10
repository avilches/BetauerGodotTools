using System;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public class FastGetterSetter : IGetterSetter {
    private readonly IGetter _iGetter;
    private readonly ISetter _iSetter;

    public FastGetterSetter(MemberInfo memberInfo) {
        if (!IsValid(memberInfo))
            throw new ArgumentException(
                "MemberInfo must be PropertyInfo or FieldInfo",
                nameof(memberInfo));
        if (memberInfo is PropertyInfo propertyInfo) {
            _iGetter = new PropertyFastGetter(propertyInfo);
            _iSetter = new PropertyFastSetter(propertyInfo);
        } else if (memberInfo is FieldInfo fieldInfo) {
            _iGetter = new FieldFastGetter(fieldInfo);
            _iSetter = new FieldFastSetter(fieldInfo);
        } else {
            throw new Exception(
                $"Cant' create {typeof(FastGetterSetter)} with MemberInfo type {memberInfo.GetType()}");
        }
    }

    public Type Type => _iGetter.Type;
    public string Name => _iGetter.Name;
    public MemberInfo MemberInfo => _iGetter.Type;

    public void SetValue(object instance, object? value) {
        _iSetter.SetValue(instance, value);
    }

    public object? GetValue(object instance) {
        return _iGetter.GetValue(instance);
    }

    public override string ToString() {
        return _iGetter.ToString();
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