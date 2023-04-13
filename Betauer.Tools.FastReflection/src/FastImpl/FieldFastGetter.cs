using System;
using System.Linq.Expressions;
using System.Reflection;
using Betauer.Core;

namespace Betauer.Tools.FastReflection.FastImpl; 

public class FieldFastGetter : IGetter {
    private readonly Func<object?, object> _getValue;
    private readonly string? _toString;

    public FieldFastGetter(FieldInfo fieldInfo) {
        FieldInfo = fieldInfo;
        MemberInfo = fieldInfo;
        Type = fieldInfo.FieldType;
        Name = fieldInfo.Name;
        DeclaringType = fieldInfo.DeclaringType;
        _getValue = CreateLambdaGetter(fieldInfo);
#if DEBUG
        _toString = $"{(fieldInfo.IsPrivate ? "private " : "public ")}{Type.GetTypeName()} {Name};";
#endif
    }

    public Type Type { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public FieldInfo FieldInfo { get; }
    public Type DeclaringType { get; }

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
        var instance = Expression.Convert(instanceParam, fieldInfo.DeclaringType!);
        var fieldAccessResult = Expression.Field(instance, fieldInfo);
        if (fieldInfo.FieldType.IsClass) {
            return (Func<object, object>)Expression.Lambda(fieldAccessResult, instanceParam).Compile();
        }
        // Pay attention to the fact that we are converting to object, so the result of the conversion is boxed
        var castToObject = Expression.Convert(fieldAccessResult, typeof(object));
        return (Func<object, object>)Expression.Lambda(castToObject, instanceParam).Compile();
            
    }
}