using System;
using System.Linq;
using System.Reflection;

namespace Betauer.Tools.FastReflection; 

public static class MemberInfoExtensions {
    public static T? GetAttribute<T>(this MemberInfo member, bool inherit = false) where T : class {
        if (typeof(T).IsSubclassOf(typeof(Attribute)) || typeof(T) == typeof(Attribute)) {
            return Attribute.GetCustomAttribute(member, typeof(T), inherit) as T;
        }
        return Attribute.GetCustomAttributes(member, inherit).OfType<T>().FirstOrDefault();
    }

    public static T[] GetAttributes<T>(this MemberInfo member, bool inherit = false) {
        if (typeof(T).IsSubclassOf(typeof(Attribute)) || typeof(T) == typeof(Attribute)) {
            return Attribute.GetCustomAttributes(member, typeof(T), inherit) as T[];
        }
        return Attribute.GetCustomAttributes(member, inherit).OfType<T>().ToArray();
    }

    public static bool HasAttribute<T>(this MemberInfo member, bool inherit = false) {
        if (typeof(T).IsSubclassOf(typeof(Attribute)) || typeof(T) == typeof(Attribute)) {
            return Attribute.GetCustomAttribute(member, typeof(T), inherit) is T;
        }
        return Attribute.GetCustomAttributes(member, inherit).OfType<T>().Any();
    }
}