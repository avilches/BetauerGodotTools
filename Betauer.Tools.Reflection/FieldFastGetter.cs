using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public class FieldFastGetter : IGetter {
    private readonly Func<object?, object> _getValue;
    private readonly string? _toString;

    public FieldFastGetter(FieldInfo fieldInfo) {
        FieldInfo = fieldInfo;
        MemberInfo = fieldInfo;
        Type = fieldInfo.FieldType;
        Name = fieldInfo.Name;
        _getValue = CreateLambdaGetter(fieldInfo);
#if DEBUG
        _toString = $"Field: {(fieldInfo.IsPrivate ? "private " : "public ")}{Type.Name} {Name};";
#endif
    }

    public Type Type { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public FieldInfo FieldInfo { get; }

    public object? GetValue(object instance) {
        return _getValue(instance);
    }

    public override string ToString() {
        return _toString ?? base.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return memberInfo is FieldInfo;
    }

    public static Func<object, object> CreateLambdaGetter(FieldInfo fieldInfo) {
        var instanceParam = Expression.Parameter(typeof(object));
        Expression body = Expression.Field
            (Expression.Convert(instanceParam, fieldInfo.DeclaringType), fieldInfo);
        if (!fieldInfo.FieldType.IsClass) body = Expression.Convert(body, typeof(object));
        return (Func<object, object>)Expression.Lambda(body, instanceParam).Compile();
    }
}