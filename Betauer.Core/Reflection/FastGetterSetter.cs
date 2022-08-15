using System;
using System.Reflection;

namespace Betauer.Reflection {
    public interface IGetterSetter : ISetter, IGetter {}
    public interface IGetterSetter<T> : ISetter<T>, IGetter<T> {}

    public class FastGetterSetter<T> : FastGetterSetter, IGetterSetter<T> {
        public T Attribute { get; }

        public FastGetterSetter(MemberInfo member, T attribute) : base(member) {
            Attribute = attribute;
        }
    }

    public class FastGetterSetter : IGetterSetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object, object> SetValue { get; }
        public Func<object, object> GetValue { get; }
        public MemberInfo MemberInfo { get; }
        
        public FastGetterSetter(MemberInfo memberInfo) {
            MemberInfo = memberInfo;
            if (memberInfo is PropertyInfo propertyInfo) {
                Type = propertyInfo.PropertyType;
                Name = propertyInfo.Name;
                SetValue = FastSetter.CreateLambdaSetter(propertyInfo);
                GetValue = FastGetter.CreateLambdaGetter(propertyInfo);
            } else if (memberInfo is FieldInfo fieldInfo) {
                Type = fieldInfo.FieldType;
                Name = fieldInfo.Name;
                SetValue = FastSetter.CreateLambdaSetter(fieldInfo);
                GetValue = FastGetter.CreateLambdaGetter(fieldInfo);
            } else {
                throw new ArgumentException("Member must be PropertyInfo or FieldInfo");
            }
        }
    }
}