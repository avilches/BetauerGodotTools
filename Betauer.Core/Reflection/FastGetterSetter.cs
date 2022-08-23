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
        private readonly IGetter _iGetter;
        private readonly ISetter _iSetter;
        public Type Type => _iGetter.Type;
        public string Name => _iGetter.Name;
        public Action<object, object> SetValue => _iSetter.SetValue;
        public Func<object, object> GetValue => _iGetter.GetValue;
        public MemberInfo MemberInfo => _iGetter.Type;
        
        public FastGetterSetter(MemberInfo memberInfo) {
            if (memberInfo is PropertyInfo propertyInfo) {
                _iGetter = new PropertyFastGetter(propertyInfo);
                _iSetter = new PropertyFastSetter(propertyInfo);
            } else if (memberInfo is FieldInfo fieldInfo) {
                _iGetter = new FieldFastGetter(fieldInfo);
                _iSetter = new FieldFastSetter(fieldInfo);
            } else {
                throw new ArgumentException("Member must be PropertyInfo or FieldInfo");
            }
        }
    }
}