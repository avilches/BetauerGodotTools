using System;
using System.Reflection;

namespace Betauer.Tools.Reflection.FastImpl; 

public class MethodFastSetter : ISetter {
    private readonly FastMethodInfo _fastMethodInfo;
    private readonly Action<object, object?> _setValue;
    private readonly string? _toString;

    public MethodFastSetter(MethodInfo methodInfo) {
        if (!IsValid(methodInfo))
            throw new ArgumentException("Setter method must have 1 parameter only and return void");
        MethodInfo = methodInfo;
        MemberInfo = methodInfo;
        Type = methodInfo.GetParameters()[0].ParameterType;
        Name = methodInfo.Name;
        DeclaringType = methodInfo.DeclaringType;
        _fastMethodInfo = new FastMethodInfo(methodInfo);
        _setValue = (instance, value) => _fastMethodInfo.Invoke(instance, value);
#if DEBUG
        _toString = $"{(methodInfo.IsPrivate ? "private" : "public")} void {Name}({Type.GetTypeName()} {methodInfo.GetParameters()[0].Name})";
#endif
    }

    public Type Type { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public MethodInfo MethodInfo { get; }
    public Type DeclaringType { get; }

    public void SetValue(object instance, object? value) {
        _setValue(instance, value);
    }

    public override string ToString() {
        return _toString ?? base.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return memberInfo is MethodInfo methodInfo && methodInfo.GetParameters().Length == 1 &&
               methodInfo.ReturnType == typeof(void);
    }
}