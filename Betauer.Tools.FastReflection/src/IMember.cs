using System;
using System.Reflection;

namespace Betauer.Tools.FastReflection; 

public interface IMember {
    public Type MemberType { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public Type DeclaringType { get; }
    public string ToString();
}