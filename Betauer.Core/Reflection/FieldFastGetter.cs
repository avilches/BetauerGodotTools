using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public class FieldFastGetter : IGetter {
        public Type Type { get; }
        public string Name { get; }
        public Func<object, object> GetValue { get; }
        public MemberInfo MemberInfo { get; }

        public FieldFastGetter(FieldInfo fieldInfo) {
            MemberInfo = fieldInfo;
            Type = fieldInfo.FieldType;
            Name = fieldInfo.Name;
            GetValue = CreateLambdaGetter(fieldInfo);
        }

        public static Func<object, object> CreateLambdaGetter(FieldInfo fieldInfo) {
            var instanceParam = Expression.Parameter(typeof(object));
            Expression body = Expression.Field
                (Expression.Convert(instanceParam, fieldInfo.DeclaringType), fieldInfo);
            if (!fieldInfo.FieldType.IsClass) {
                body = Expression.Convert(body, typeof(object));
            }
            return (Func<object, object>)Expression.Lambda(body, instanceParam).Compile();
        }
    }
}