using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public class FieldFastGetter : IGetter {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
        private readonly Func<object, object> _getValue;
        
        public object GetValue(object instance) => _getValue(instance);

        public FieldFastGetter(FieldInfo fieldInfo) {
            MemberInfo = fieldInfo;
            Type = fieldInfo.FieldType;
            Name = fieldInfo.Name;
            _getValue = CreateLambdaGetter(fieldInfo);
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