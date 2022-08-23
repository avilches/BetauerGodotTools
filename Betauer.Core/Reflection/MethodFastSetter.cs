using System;
using System.Reflection;

namespace Betauer.Reflection {
    public class MethodFastSetter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object, object> SetValue { get; }
        public MemberInfo MemberInfo { get; }
        private readonly FastMethodInfo _fastMethodInfo;
        private readonly string? _toString;
        
        public MethodFastSetter(MethodInfo methodInfo) {
            MemberInfo = methodInfo;
            if (methodInfo.GetParameters().Length != 1) throw new ArgumentException("Setter method must have 1 parameter only");
            Type = methodInfo.GetParameters()[0].ParameterType;
            Name = methodInfo.Name;
            _fastMethodInfo = new FastMethodInfo(methodInfo);
            SetValue = (instance, value) => _fastMethodInfo.Invoke(instance, value);
#if DEBUG
            _toString = "Method " + (methodInfo.IsPrivate ? "private " : "public ") + Name + "(" + Type.Name + " " + methodInfo.GetParameters()[0].Name + ")";
#endif                
        }
        public override string ToString() => _toString ?? base.ToString();
    }
}