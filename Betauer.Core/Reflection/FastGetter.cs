using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public interface IGetter {
        public Type Type { get; }
        public string Name { get; }
        public Func<object, object> GetValue { get; }
        public MemberInfo MemberInfo { get; }
    }

    public interface IGetter<T> : IGetter {
        public T Attribute { get;  }
    }

    public class FastGetter<T> : FastGetter, IGetter<T> {
        public T Attribute { get; }

        public FastGetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            Attribute = attribute;
        }
    }

    public class FastGetter : IGetter {
        public Type Type { get; }
        public string Name { get; }
        public Func<object, object> GetValue { get; }
        public MemberInfo MemberInfo { get; }
        private readonly FastMethodInfo _fastMethodInfo;
        
        public FastGetter(MemberInfo memberInfo) {
            MemberInfo = memberInfo;
            if (memberInfo is PropertyInfo property) {
                Type = property.PropertyType;
                Name = property.Name;
                GetValue = CreateLambdaGetter(property);
            } else if (memberInfo is FieldInfo fieldInfo) {
                Type = fieldInfo.FieldType;
                Name = fieldInfo.Name;
                GetValue = CreateLambdaGetter(fieldInfo);
            } else if (memberInfo is MethodInfo methodInfo) {
                if (methodInfo.GetParameters().Length != 0) throw new ArgumentException("Getter method must not have parameter");
                Type = methodInfo.ReturnType;
                Name = methodInfo.Name;
                _fastMethodInfo = new FastMethodInfo(methodInfo);
                GetValue = (instance) => _fastMethodInfo.Invoke(instance);
            } else {
                throw new ArgumentException("Member must be PropertyInfo, FieldInfo or MethodInfo");
            }
        }
        
        public static Func<object, object> CreateLambdaGetter(PropertyInfo propertyInfo) {
            var instanceParam = Expression.Parameter(typeof(object));
            var body = Expression.Call(
                Expression.Convert(instanceParam, propertyInfo.DeclaringType), 
                propertyInfo.GetMethod);
            return (Func<object, object>)Expression.Lambda(body, instanceParam).Compile();
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