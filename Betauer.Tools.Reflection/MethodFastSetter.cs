using System;
using System.Reflection;

namespace Betauer.Tools.Reflection {
    public class MethodFastSetter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
        private readonly Action<object, object?> _setValue;
        private readonly FastMethodInfo _fastMethodInfo;
        private readonly string? _toString;

        public void SetValue(object instance, object? value) => _setValue(instance, value);
        
        public MethodFastSetter(MethodInfo methodInfo) {
            if (!IsValid(methodInfo)) throw new ArgumentException("Setter method must have 1 parameter only and return void");
            MemberInfo = methodInfo;
            Type = methodInfo.GetParameters()[0].ParameterType;
            Name = methodInfo.Name;
            _fastMethodInfo = new FastMethodInfo(methodInfo);
            _setValue = (instance, value) => _fastMethodInfo.Invoke(instance, value);
            #if DEBUG
                _toString = $"Method: {(methodInfo.IsPrivate ? "private" : "public")} void {Name}({Type.Name} {methodInfo.GetParameters()[0].Name})";
            #endif                
        }
        
        public override string ToString() => _toString ?? base.ToString();

        public static bool IsValid(MemberInfo memberInfo) =>
            memberInfo is MethodInfo methodInfo && methodInfo.GetParameters().Length == 1 && methodInfo.ReturnType == typeof(void);
            
    }
}