using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public class FieldFastSetter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
        private readonly Action<object, object?> _setValue;
        private readonly string? _toString;

        public void SetValue(object instance, object? value) => _setValue(instance, value);

        public FieldFastSetter(FieldInfo fieldInfo) {
            if (!IsValid(fieldInfo))
                throw new ArgumentException($"FieldInfo {fieldInfo.Name} can't be readonly", nameof(fieldInfo));
            MemberInfo = fieldInfo;
            Type = fieldInfo.FieldType;
            Name = fieldInfo.Name;
            _setValue = CreateLambdaSetter(fieldInfo);
            #if DEBUG
                _toString = $"Field {(fieldInfo.IsPrivate ? "private " : "public ")}{Type.Name} {Name}";
            #endif                
        }

        public override string ToString() => _toString ?? base.ToString();

        public static bool IsValid(MemberInfo memberInfo) =>
            memberInfo is FieldInfo { IsInitOnly: false };

        public static Action<object, object> CreateLambdaSetter(FieldInfo fieldInfo) {
            var sourceParam = Expression.Parameter(typeof(object));
            var valueParam = Expression.Parameter(typeof(object));
            var convertedValueExpr = Expression.Convert(valueParam, fieldInfo.FieldType);
            Expression returnExpression = Expression.Assign(Expression.Field
                (Expression.Convert(sourceParam, fieldInfo.DeclaringType), fieldInfo), convertedValueExpr);
            if (!fieldInfo.FieldType.IsClass) {
                returnExpression = Expression.Convert(returnExpression, typeof(object));
            }
            var lambda = Expression.Lambda(typeof(Action<object, object>),
                returnExpression, sourceParam, valueParam);
            return (Action<object, object>)lambda.Compile();
        }
    }
}