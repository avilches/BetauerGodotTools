using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public interface ISetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object, object> SetValue { get; }
        public MemberInfo MemberInfo { get; }
    }

    public interface ISetter<T> : ISetter {
        public T Attribute { get;  }
    }

    public class FastSetter<T> : FastSetter, ISetter<T> {
        public T Attribute { get; }

        public FastSetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            Attribute = attribute;
        }
    }

    public class FastSetter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object, object> SetValue { get; }
        public MemberInfo MemberInfo { get; }
        private readonly FastMethodInfo _fastMethodInfo;
#if DEBUG
        private readonly string _toString;
#endif                

        public FastSetter(MemberInfo memberInfo) {
            MemberInfo = memberInfo;
            if (memberInfo is PropertyInfo propertyInfo) {
                Type = propertyInfo.PropertyType;
                Name = propertyInfo.Name;
                SetValue = CreateLambdaSetter(propertyInfo); // This is the slow version of property.SetValue;
#if DEBUG
                _toString = "Property " + Type.Name + " " + Name + " { " +
                            (propertyInfo.GetMethod.IsPrivate ? "private" : "public") + " get; " +
                            (propertyInfo.SetMethod.IsPrivate ? "private" : "public") + " set; }";
#endif           
            } else if (memberInfo is FieldInfo fieldInfo) {
                Type = fieldInfo.FieldType;
                Name = fieldInfo.Name;
                SetValue = CreateLambdaSetter(fieldInfo);
#if DEBUG
                _toString = "Field " + (fieldInfo.IsPrivate ? "private " : "public ") + Type.Name + " " + Name;
#endif                
            } else if (memberInfo is MethodInfo methodInfo) {
                if (methodInfo.GetParameters().Length != 1) throw new ArgumentException("Setter method must have 1 parameter only");
                Type = methodInfo.GetParameters()[0].ParameterType;
                Name = methodInfo.Name;
                _fastMethodInfo = new FastMethodInfo(methodInfo);
                SetValue = (instance, value) => _fastMethodInfo.Invoke(instance, value);
#if DEBUG
                _toString = "Method " + (methodInfo.IsPrivate ? "private " : "public ") + Name + "(" + Type.Name + " " + methodInfo.GetParameters()[0].Name + ")";
#endif                
            } else {
                throw new ArgumentException("Member must be PropertyInfo or FieldInfo or MethodInfo");
            }
        }

#if DEBUG
        public override string ToString() => _toString;
#endif

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

        public static Action<object, object> CreateLambdaSetter(PropertyInfo propertyInfo) {
            var instanceParam = Expression.Parameter(typeof(object));
            var valueParam = Expression.Parameter(typeof(object));
            var body = Expression.Call
            (Expression.Convert(instanceParam, propertyInfo.DeclaringType),
                propertyInfo.SetMethod,
                Expression.Convert(valueParam, propertyInfo.PropertyType));
            return (Action<object, object>)Expression.Lambda(body, instanceParam, valueParam).Compile();
        }
        
    }
}