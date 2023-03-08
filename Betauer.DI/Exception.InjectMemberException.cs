namespace Betauer.DI;

public class InjectMemberException : InjectException {
    public readonly string MemberName;

    public InjectMemberException(string memberName, object instance, string message) : base(message, instance) {
        MemberName = memberName;
    }
}