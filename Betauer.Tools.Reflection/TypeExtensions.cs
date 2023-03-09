using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Betauer.Tools.Reflection; 

public static class TypeExtensions {
    private static readonly Dictionary<(Type, Type, MemberTypes, BindingFlags), object> Cache = new();


    public static bool ImplementsInterface(this Type type, Type interfaceType) {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
        
        if (type.IsAssignableTo(interfaceType)) return true;
        
        // interfaceType is a GenericTypeDefinition (that means is something like List<> instead of List<string>)
        return (interfaceType.IsGenericTypeDefinition && interfaceType == type.GetGenericTypeDefinition()) ||
               type.GetInterfaces().Any(implementedInterface =>
                   implementedInterface == interfaceType ||
                   implementedInterface.IsGenericType &&
                   implementedInterface.GetGenericTypeDefinition() == interfaceType);
    }

    public static List<ISetter<T>> GetSettersCached<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : Attribute {
        var key = (typeof(ISetter<T>), type, memberFlags, bindingAttr);
        if (Cache.TryGetValue(key, out var result)) return (List<ISetter<T>>)result;
        return (List<ISetter<T>>)(Cache[key] = type.GetSetters<T>(memberFlags, bindingAttr));
    }

    public static List<ISetter<T>> GetSetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr)
        where T : Attribute {
        var e = Enumerable.Empty<MemberInfo>();
        e = ConcatFields(e, type, memberFlags, bindingAttr);
        e = ConcatProperties(e, type, memberFlags, bindingAttr);
        e = ConcatMethods(e, type, memberFlags, bindingAttr, 1);

        var setters = new List<ISetter<T>>();
        foreach (var memberInfo in e)
            if (memberInfo.GetAttribute<T>() is T attribute) {
                var validSetter = FastSetter.IsValid(memberInfo);
                var validGetter = FastGetter.IsValid(memberInfo);
                if (validGetter && validSetter)
                    // fields and properties
                    setters.Add(new FastGetterSetter<T>(memberInfo, attribute));
                else if (validSetter)
                    // methods with 1 parameter and void return type
                    setters.Add(new FastSetter<T>(memberInfo, attribute));
            }
        return setters;
    }

    public static List<ISetter<T>> GetGettersCached<T>(this Type type, MemberTypes memberFlags,
        BindingFlags bindingAttr) where T : Attribute {
        var key = (typeof(ISetter<T>), type, memberFlags, bindingAttr);
        if (Cache.TryGetValue(key, out var result)) return (List<ISetter<T>>)result;
        return (List<ISetter<T>>)(Cache[key] = type.GetGetters<T>(memberFlags, bindingAttr));
    }

    public static List<IGetter<T>> GetGetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr)
        where T : Attribute {
        var e = Enumerable.Empty<MemberInfo>();
        e = ConcatFields(e, type, memberFlags, bindingAttr);
        e = ConcatProperties(e, type, memberFlags, bindingAttr);
        e = ConcatMethods(e, type, memberFlags, bindingAttr, 0);
        var getters = new List<IGetter<T>>();
        foreach (var memberInfo in e)
            if (memberInfo.GetAttribute<T>() is T attribute) {
                var validSetter = FastSetter.IsValid(memberInfo);
                var validGetter = FastGetter.IsValid(memberInfo);
                if (validGetter && validSetter)
                    // fields and properties
                    getters.Add(new FastGetterSetter<T>(memberInfo, attribute));
                else if (validGetter)
                    // methods with 1 parameter and void return type
                    getters.Add(new FastGetter<T>(memberInfo, attribute));
            }
        return getters;
    }

    private static IEnumerable<MemberInfo> ConcatMethods(IEnumerable<MemberInfo> e, Type type,
        MemberTypes memberFlags, BindingFlags bindingAttr, int parameters = -1) {
        if (memberFlags.HasFlag(MemberTypes.Method))
            e = e.Concat(type.GetMethods(bindingAttr)
                .Where(info => parameters == -1 || info.GetParameters().Length == parameters));
        return e;
    }

    private static IEnumerable<MemberInfo> ConcatProperties(IEnumerable<MemberInfo> e, Type type,
        MemberTypes memberFlags, BindingFlags bindingAttr) {
        if (memberFlags.HasFlag(MemberTypes.Property)) e = e.Concat(type.GetProperties(bindingAttr));
        return e;
    }

    private static IEnumerable<MemberInfo> ConcatFields(IEnumerable<MemberInfo> e, Type type,
        MemberTypes memberFlags, BindingFlags bindingAttr) {
        if (memberFlags.HasFlag(MemberTypes.Field)) e = e.Concat(type.GetFields(bindingAttr));
        return e;
    }

    public static void Main() {
        // False
        Console.WriteLine(typeof(List<>).ImplementsInterface(typeof(IList<string>)));
        Console.WriteLine(typeof(Dictionary<,>).ImplementsInterface(typeof(IDictionary<string,int>)));
        
        // True
        Console.WriteLine(typeof(List<string>).ImplementsInterface(typeof(IEnumerable)));
        Console.WriteLine(typeof(List<string>).ImplementsInterface(typeof(IEnumerable<>)));
        Console.WriteLine(typeof(List<string>).ImplementsInterface(typeof(IList<string>)));
        Console.WriteLine(typeof(List<string>).ImplementsInterface(typeof(IList<>)));
        Console.WriteLine(typeof(List<>).ImplementsInterface(typeof(IList<>)));
        
        Console.WriteLine(typeof(IList<string>).ImplementsInterface(typeof(IList<string>)));
        Console.WriteLine(typeof(IList<string>).ImplementsInterface(typeof(IList<>)));
        Console.WriteLine(typeof(IList<>).ImplementsInterface(typeof(IList<>)));

        Console.WriteLine(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IEnumerable)));
        Console.WriteLine(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IEnumerable<>)));
        Console.WriteLine(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IDictionary<,>)));
        Console.WriteLine(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IDictionary<string,int>)));
        Console.WriteLine(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IReadOnlyDictionary<string,int>)));
        Console.WriteLine(typeof(Dictionary<string, int>).ImplementsInterface(typeof(IReadOnlyCollection<KeyValuePair<string,int>>)));
        Console.WriteLine(typeof(Dictionary<string, int>).ImplementsInterface(typeof(ICollection<KeyValuePair<string,int>>)));
        Console.WriteLine(typeof(Dictionary<,>).ImplementsInterface(typeof(IEnumerable)));

        Console.WriteLine(typeof(IDictionary<string, int>).ImplementsInterface(typeof(IEnumerable)));
        Console.WriteLine(typeof(IDictionary<string, int>).ImplementsInterface(typeof(IEnumerable<>)));
        Console.WriteLine(typeof(IDictionary<string, int>).ImplementsInterface(typeof(ICollection<KeyValuePair<string,int>>)));
    }
}