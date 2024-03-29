using System;
using System.Reflection;
using Betauer.Core;

namespace Betauer.Tools.FastReflection.FastImpl; 

public class MethodFastGetter : IGetter {
    private readonly FastMethodInfo _fastMethodInfo;
    private readonly Func<object?, object> _getValue;
    private readonly string? _toString;

    public MethodFastGetter(MethodInfo methodInfo) {
        if (!IsValid(methodInfo))
            throw new ArgumentException("Getter method must have 0 parameters and return non void type");
        MethodInfo = methodInfo;
        MemberInfo = methodInfo;
        MemberType = methodInfo.ReturnType;
        Name = methodInfo.Name;
        DeclaringType = methodInfo.DeclaringType;
        _fastMethodInfo = new FastMethodInfo(methodInfo);
        _getValue = instance => _fastMethodInfo.Invoke(instance);
#if DEBUG
        _toString = $"{(methodInfo.IsPrivate ? "private" : "public")} {methodInfo.ReturnType.GetTypeName()} {Name}()";
#endif
    }

    public Type MemberType { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public MethodInfo MethodInfo { get; }
    public Type DeclaringType { get; }

    public object? GetValue(object instance) {
        return _getValue(instance);
    }

    public override string ToString() {
        return _toString ?? base.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return memberInfo is MethodInfo methodInfo && methodInfo.GetParameters().Length == 0 &&
               methodInfo.ReturnType != typeof(void);
    }
}