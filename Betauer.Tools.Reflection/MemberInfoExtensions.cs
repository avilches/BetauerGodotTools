using System;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public static class MemberInfoExtensions {
    public static T? GetAttribute<T>(this MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) as T;
    }

    public static T[] GetAttributes<T>(this MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttributes(member, typeof(T), inherit) as T[];
    }

    public static bool HasAttribute<T>(this MemberInfo member, bool inherit = false) where T : Attribute {
        return Attribute.GetCustomAttribute(member, typeof(T), inherit) is T;
    }
}