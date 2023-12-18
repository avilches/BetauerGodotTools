using System;
using System.Linq.Expressions;
using System.Reflection;
using Betauer.Core;

namespace Betauer.Tools.FastReflection.FastImpl; 

public class PropertyFastGetter : IGetter {
    private readonly Func<object, object> _getValue;
    private readonly string? _toString;

    public PropertyFastGetter(PropertyInfo propertyInfo) {
        if (!IsValid(propertyInfo))
            throw new ArgumentException($"PropertyInfo {propertyInfo.Name} doesn't have get", nameof(propertyInfo));
        PropertyInfo = propertyInfo;
        MemberInfo = propertyInfo;
        MemberType = propertyInfo.PropertyType;
        Name = propertyInfo.Name;
        DeclaringType = propertyInfo.DeclaringType;
        _getValue = CreateLambdaGetter(propertyInfo);
#if DEBUG
        _toString = MemberType.GetTypeName() + " " + Name +
                    (propertyInfo.GetMethod.IsPrivate ? " { private" : " { public") + " get; " +
                    (propertyInfo.SetMethod != null
                        ? propertyInfo.SetMethod.IsPrivate ? "private set; }" : "public set; }"
                        : "}");
#endif
    }

    public Type MemberType { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public PropertyInfo PropertyInfo { get; }
    public Type DeclaringType { get; }

    public object? GetValue(object instance) {
        return _getValue(instance);
    }

    public override string ToString() {
        return _toString ?? base.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return memberInfo is PropertyInfo { CanRead: true } propertyInfo && propertyInfo.GetMethod != null;
    }

    public static Func<object, object> CreateLambdaGetter(PropertyInfo propertyInfo) {
        var instanceParam = Expression.Parameter(typeof(object));
        var instance = Expression.Convert(instanceParam, propertyInfo.DeclaringType!);
        var propertyAccessResult = Expression.Call(instance, propertyInfo.GetMethod!);
        if (propertyInfo.PropertyType.IsClass) {
            return Expression.Lambda<Func<object, object>>(propertyAccessResult, instanceParam).Compile();
        }
        // Pay attention to the fact that we are converting to object, so the result of the conversion is boxed
        var castToObject = Expression.Convert(propertyAccessResult, typeof(object));
        return Expression.Lambda<Func<object, object>>(castToObject, instanceParam).Compile();
    }
}