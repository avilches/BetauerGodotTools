using System;
using System.Linq.Expressions;
using System.Reflection;
using Betauer.Core;

namespace Betauer.Tools.FastReflection.FastImpl; 

public class FieldFastSetter : ISetter {
    private readonly Action<object, object?> _setValue;
    private readonly string? _toString;

    public FieldFastSetter(FieldInfo fieldInfo) {
        if (!IsValid(fieldInfo))
            throw new ArgumentException($"FieldInfo {fieldInfo.Name} can't be readonly", nameof(fieldInfo));
        FieldInfo = fieldInfo;
        MemberInfo = fieldInfo;
        MemberType = fieldInfo.FieldType;
        Name = fieldInfo.Name;
        DeclaringType = fieldInfo.DeclaringType;
        _setValue = CreateLambdaSetter(fieldInfo);
#if DEBUG
        _toString = $"{(fieldInfo.IsPrivate ? "private " : "public ")}{MemberType.GetTypeName()} {Name};";
#endif
    }

    public Type MemberType { get; }
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
        var instanceParam = Expression.Parameter(typeof(object));
        var valueParam = Expression.Parameter(typeof(object));
        var value = Expression.Convert(valueParam, fieldInfo.FieldType);
        var instance = Expression.Convert(instanceParam, fieldInfo.DeclaringType!);
        var setFieldValue = Expression.Assign(Expression.Field(instance, fieldInfo), value);
        return Expression.Lambda<Action<object, object>>(setFieldValue, instanceParam, valueParam).Compile();
    }
}