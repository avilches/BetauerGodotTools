using System;
using System.Reflection;

namespace Betauer.Reflection {
    public interface IMember {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
    }
}