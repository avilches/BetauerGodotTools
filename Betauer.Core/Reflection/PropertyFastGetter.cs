using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public class PropertyFastGetter : IGetter {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
        private readonly Func<object, object> _getValue;
        
        public object GetValue(object instance) => _getValue(instance);

        public PropertyFastGetter(PropertyInfo propertyInfo) {
            MemberInfo = propertyInfo;
            Type = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
            _getValue = CreateLambdaGetter(propertyInfo);
        }
        
        public static Func<object, object> CreateLambdaGetter(PropertyInfo propertyInfo) {
            var instanceParam = Expression.Parameter(typeof(object));
            var body = Expression.Call(
                Expression.Convert(instanceParam, propertyInfo.DeclaringType), 
                propertyInfo.GetMethod);
            return (Func<object, object>)Expression.Lambda(body, instanceParam).Compile();
        }

    }
}