using System;

namespace Betauer.DI.Exceptions;

public class InjectMemberException : Exception {
    public readonly string MemberName;
    public readonly object Instance;

    public InjectMemberException(string memberName, object instance, string message) : base(message) {
        MemberName = memberName;
        Instance = instance;
    }
}