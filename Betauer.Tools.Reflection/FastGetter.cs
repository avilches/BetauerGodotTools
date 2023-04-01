using System;
using System.Reflection;
using Betauer.Tools.Reflection.FastImpl;

namespace Betauer.Tools.Reflection; 

public class FastGetter : IGetter {
    public IGetter Getter { get; }

    public FastGetter(MemberInfo memberInfo) {
        if (!IsValid(memberInfo))
            throw new ArgumentException("MemberInfo must be PropertyInfo, FieldInfo or MethodInfo (non void return and 0 parameters)", nameof(memberInfo));
        Getter = memberInfo switch {
            PropertyInfo propertyInfo => new PropertyFastGetter(propertyInfo),
            FieldInfo fieldInfo => new FieldFastGetter(fieldInfo),
            MethodInfo methodInfo => new MethodFastGetter(methodInfo)
        };
    }

    public Type Type => Getter.Type;
    public string Name => Getter.Name;
    public MemberInfo MemberInfo => Getter.MemberInfo;
    public Type DeclaringType => Getter.DeclaringType;

    public object? GetValue(object instance) {
        return Getter.GetValue(instance);
    }

    public override string ToString() {
        return Getter.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return PropertyFastGetter.IsValid(memberInfo) ||
               FieldFastGetter.IsValid(memberInfo) ||
               MethodFastGetter.IsValid(memberInfo);
    }
}

public class FastGetter<T> : FastGetter, IGetter<T> {
    public FastGetter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
        GetterAttribute = attribute;
    }

    public T GetterAttribute { get; }
}