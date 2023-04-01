using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection.FastImpl; 

public class PropertyFastGetter : IGetter {
    private readonly Func<object, object> _getValue;
    private readonly string? _toString;

    public PropertyFastGetter(PropertyInfo propertyInfo) {
        if (!IsValid(propertyInfo))
            throw new ArgumentException($"PropertyInfo {propertyInfo.Name} doesn't have get", nameof(propertyInfo));
        PropertyInfo = propertyInfo;
        MemberInfo = propertyInfo;
        Type = propertyInfo.PropertyType;
        Name = propertyInfo.Name;
        DeclaringType = propertyInfo.DeclaringType;
        _getValue = CreateLambdaGetter(propertyInfo);
#if DEBUG
        _toString = "Property: " + Type.GetTypeName() + " " + Name +
                    (propertyInfo.GetMethod.IsPrivate ? " { private" : " { public") + " get; " +
                    (propertyInfo.SetMethod != null
                        ? propertyInfo.SetMethod.IsPrivate ? "private set; }" : "public set; }"
                        : "}");
#endif
    }

    public Type Type { get; }
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
        var body = Expression.Call(
            Expression.Convert(instanceParam, propertyInfo.DeclaringType),
            propertyInfo.GetMethod);
        return (Func<object, object>)Expression.Lambda(body, instanceParam).Compile();
    }
}