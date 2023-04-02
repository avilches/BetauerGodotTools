using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection.FastImpl; 

public class FieldFastSetter : ISetter {
    private readonly Action<object, object?> _setValue;
    private readonly string? _toString;

    public FieldFastSetter(FieldInfo fieldInfo) {
        if (!IsValid(fieldInfo))
            throw new ArgumentException($"FieldInfo {fieldInfo.Name} can't be readonly", nameof(fieldInfo));
        FieldInfo = fieldInfo;
        MemberInfo = fieldInfo;
        Type = fieldInfo.FieldType;
        Name = fieldInfo.Name;
        DeclaringType = fieldInfo.DeclaringType;
        _setValue = CreateLambdaSetter(fieldInfo);
#if DEBUG
        _toString = $"{(fieldInfo.IsPrivate ? "private " : "public ")}{Type.GetTypeName()} {Name};";
#endif
    }

    public Type Type { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public FieldInfo FieldInfo { get; }
    public Type DeclaringType { get; }

    public void SetValue(object instance, object? value) {
        _setValue(instance, value);
    }

    public override string ToString() {
        return _toString ?? base.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return memberInfo is FieldInfo { IsInitOnly: false };
    }

    public static Action<object, object> CreateLambdaSetter(FieldInfo fieldInfo) {
        var sourceParam = Expression.Parameter(typeof(object));
        var valueParam = Expression.Parameter(typeof(object));
        var convertedValueExpr = Expression.Convert(valueParam, fieldInfo.FieldType);
        Expression returnExpression = Expression.Assign(Expression.Field
            (Expression.Convert(sourceParam, fieldInfo.DeclaringType), fieldInfo), convertedValueExpr);
        if (!fieldInfo.FieldType.IsClass) returnExpression = Expression.Convert(returnExpression, typeof(object));
        var lambda = Expression.Lambda(typeof(Action<object, object>),
            returnExpression, sourceParam, valueParam);
        return (Action<object, object>)lambda.Compile();
    }
}