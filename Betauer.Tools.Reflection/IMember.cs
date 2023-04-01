using System;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public interface IMember {
    public Type Type { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public Type DeclaringType { get; }
    public string ToString();
}