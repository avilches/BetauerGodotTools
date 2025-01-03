using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Betauer.Tools.FastReflection.FastImpl;

namespace Betauer.Tools.FastReflection; 

public static class TypeExtensions {
    private static readonly Dictionary<(Type, Type, MemberTypes, BindingFlags), object> Cache = new();

    public static IEnumerable<FastMethodInfo> GetMethods<T>(this Type type, BindingFlags bindingAttr, Predicate<MethodInfo> filter) {
        return type.GetMethods(bindingAttr)
            .Where(info => filter(info) && info.HasAttribute<T>())
            .Select(info => new FastMethodInfo(info));
    }

    public static List<ISetter<T>> GetSettersCached<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : class {
        var key = (typeof(ISetter<T>), type, memberFlags, bindingAttr);
        if (Cache.TryGetValue(key, out var result)) return (List<ISetter<T>>)result;
        return (List<ISetter<T>>)(Cache[key] = type.GetSetters<T>(memberFlags, bindingAttr));
    }

    public static List<ISetter<T>> GetGettersCached<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : class {
        var key = (typeof(ISetter<T>), type, memberFlags, bindingAttr);
        if (Cache.TryGetValue(key, out var result)) return (List<ISetter<T>>)result;
        return (List<ISetter<T>>)(Cache[key] = type.GetGetters<T>(memberFlags, bindingAttr));
    }

    public static List<ISetter<T>> GetSetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : class {
        var e = Enumerable.Empty<MemberInfo>();
        e = ConcatFields(e, type, memberFlags, bindingAttr);
        e = ConcatProperties(e, type, memberFlags, bindingAttr);
        e = ConcatMethods(e, type, memberFlags, bindingAttr, FastSetter.IsValid);

        var setters = new List<ISetter<T>>();
        foreach (var memberInfo in e)
            if (memberInfo.GetAttribute<T>() is T attribute) {
                if (FastSetter.IsValid(memberInfo)) setters.Add(new FastSetter<T>(memberInfo, attribute));
            }
        return setters;
    }

    public static List<IGetter<T>> GetGetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : class {
        var e = Enumerable.Empty<MemberInfo>();
        e = ConcatFields(e, type, memberFlags, bindingAttr);
        e = ConcatProperties(e, type, memberFlags, bindingAttr);
        e = ConcatMethods(e, type, memberFlags, bindingAttr, FastGetter.IsValid);
        var getters = new List<IGetter<T>>();
        foreach (var memberInfo in e)
            if (memberInfo.GetAttribute<T>() is T attribute) {
                if (FastGetter.IsValid(memberInfo)) getters.Add(new FastGetter<T>(memberInfo, attribute));
            }
        return getters;
    }

    private static IEnumerable<MemberInfo> ConcatMethods(IEnumerable<MemberInfo> e, Type type, MemberTypes memberFlags, BindingFlags bindingAttr, Predicate<MethodInfo> filter) {
        if (memberFlags.HasFlag(MemberTypes.Method)) e = e.Concat(type.GetMethods(bindingAttr).Where(methodInfo => filter(methodInfo)));
        return e;
    }

    private static IEnumerable<MemberInfo> ConcatProperties(IEnumerable<MemberInfo> e, Type type, MemberTypes memberFlags, BindingFlags bindingAttr) {
        if (memberFlags.HasFlag(MemberTypes.Property)) e = e.Concat(type.GetProperties(bindingAttr));
        return e;
    }

    private static IEnumerable<MemberInfo> ConcatFields(IEnumerable<MemberInfo> e, Type type, MemberTypes memberFlags, BindingFlags bindingAttr) {
        if (memberFlags.HasFlag(MemberTypes.Field)) e = e.Concat(type.GetFields(bindingAttr));
        return e;
    }
}