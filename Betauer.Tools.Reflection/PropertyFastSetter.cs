using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection {
    public class PropertyFastSetter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
        private readonly Action<object, object> _setValue;
        private readonly string? _toString;

        public void SetValue(object instance, object? value) => _setValue(instance, value);

        public PropertyFastSetter(PropertyInfo propertyInfo) {
            if (!IsValid(propertyInfo))
                throw new ArgumentException($"PropertyInfo {propertyInfo.Name} doesn't have set", nameof(propertyInfo));
            MemberInfo = propertyInfo;
            Type = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
            _setValue = CreateLambdaSetter(propertyInfo);
            #if DEBUG
            _toString = "Property: " + Type.Name + " " + Name +
                        (propertyInfo.GetMethod != null ? (propertyInfo.GetMethod.IsPrivate ? " { private get; " : " { public get; ") : " { ") +
                        (propertyInfo.SetMethod.IsPrivate ? "private set; }" : "public set; }");
            #endif           
        }

        public override string ToString() => _toString ?? base.ToString();

        public static bool IsValid(MemberInfo memberInfo) =>
            memberInfo is PropertyInfo { CanWrite: true } propertyInfo && propertyInfo.SetMethod != null;
        
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