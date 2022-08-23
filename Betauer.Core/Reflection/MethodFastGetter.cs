using System;
using System.Reflection;

namespace Betauer.Reflection {
    public class MethodFastGetter : IGetter {
        public Type Type { get; }
        public string Name { get; }
        public Func<object, object> GetValue { get; }
        public MemberInfo MemberInfo { get; }
        private readonly FastMethodInfo _fastMethodInfo;

        public MethodFastGetter(MethodInfo methodInfo) {
            if (methodInfo.GetParameters().Length != 0) throw new ArgumentException("Getter method must not have parameter");
            MemberInfo = methodInfo;
            Type = methodInfo.ReturnType;
            Name = methodInfo.Name;
            _fastMethodInfo = new FastMethodInfo(methodInfo);
            GetValue = (instance) => _fastMethodInfo.Invoke(instance);
        }
    }
}