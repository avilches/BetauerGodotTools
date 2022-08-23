using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public class FieldFastSetter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object, object> SetValue { get; }
        public MemberInfo MemberInfo { get; }
        private readonly string? _toString;

        public FieldFastSetter(FieldInfo fieldInfo) {
            MemberInfo = fieldInfo;
            Type = fieldInfo.FieldType;
            Name = fieldInfo.Name;
            SetValue = CreateLambdaSetter(fieldInfo);
#if DEBUG
            _toString = "Field " + (fieldInfo.IsPrivate ? "private " : "public ") + Type.Name + " " + Name;
#endif                
        }

        public override string ToString() => _toString ?? base.ToString();

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