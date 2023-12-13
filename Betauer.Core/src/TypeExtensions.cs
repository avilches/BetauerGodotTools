using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Betauer.Core; 

public static class TypeExtensions {

    public static bool IsSubclassOfOrImplements(this Type from, Type interfaceType) {
        if (interfaceType.IsInterface) return from.ImplementsInterface(interfaceType);
        return from.IsGenericSubclassOf(interfaceType);
    }

    /// <summary>
    /// A better version of type.IsAssignableTo(interfaceType) that also works with generic types, such as:
    ///
    /// - typeof(List<string>).ImplementsInterface(typeof(IList<>)) returns true
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool ImplementsInterface(this Type from, Type interfaceType) {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
        if (!interfaceType.IsInterface) throw new Exception($"TypeToFind {interfaceType} must be an interface");
    
        if (from.IsAssignableTo(interfaceType)) return true;
    
        // interfaceType is a GenericTypeDefinition (that means is something like List<> instead of List<string>)
        if (from.IsGenericType && interfaceType.IsGenericTypeDefinition && interfaceType == from.GetGenericTypeDefinition()) return true;
        return from.GetInterfaces().Any(implementedInterface =>
            implementedInterface == interfaceType ||
            (implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == interfaceType));
    }
    
    /// <summary>
    /// returns an array of Type from a generic interface definition, such as:
    /// typeof(List<string>).FindGenericsFromInterfaceDefinition(typeof(IList<>)) returns new Type{}[typeof(string)] 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static Type[] FindGenericsFromInterfaceDefinition(this Type from, Type interfaceType) {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));

        if (!interfaceType.IsInterface || !interfaceType.IsGenericTypeDefinition) throw new Exception($"TypeToFind {interfaceType} must be a generic interface definition");
        
        if (from.IsInterface && interfaceType == from.GetGenericTypeDefinition()) return from.GetGenericArguments();
        var interfaceFound = from.GetInterfaces().FirstOrDefault(implementedInterface => 
            implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == interfaceType);
        if (interfaceFound == null) throw new Exception($"Type {from} doesn't implements {interfaceType}");
        return interfaceFound.GetGenericArguments();
    }


    public static Type[] FindGenericsFromBaseTypeDefinition(this Type from, Type typeToFind) {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (typeToFind == null) throw new ArgumentNullException(nameof(typeToFind));
        
        if (!typeToFind.IsClass || !typeToFind.IsGenericTypeDefinition) throw new Exception($"TypeToFind {typeToFind} must be a generic class definition");
        
        var type = from.GetTypeInfo();
        while (!type.IsGenericType || type.GetGenericTypeDefinition() != typeToFind) {
            if (type.BaseType == null) throw new Exception($"Type {from} is not a subclass of {typeToFind}");
            type = type.BaseType.GetTypeInfo();
        }
        return type.GetGenericArguments();
    }
    
    public static bool IsGenericSubclassOf(this Type from, Type typeToFind) {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (typeToFind == null) throw new ArgumentNullException(nameof(typeToFind));

        if (!typeToFind.IsClass) throw new Exception("TypeToFind must be a class with a generic type definition");
        if (!typeToFind.IsGenericType || !typeToFind.IsGenericTypeDefinition) return from.IsSubclassOf(typeToFind);
        
        var type = from.GetTypeInfo();
        while (!type.IsGenericType || type.GetGenericTypeDefinition() != typeToFind) {
            if (type.BaseType == null) return false;
            type = type.BaseType.GetTypeInfo();
        }
        return true;
    }
    

    /// <summary>
    /// Return a string with the type name and the generic arguments (if any). So, for Dictionary<string, int> it returns "Dictionary<string,int>" instead of "Dictionary`2"
    ///
    /// Use it instead of type.Name when you need to get the full name of a generic type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetTypeName(this Type type) {
        if (!type.IsGenericType) {
            return type.Name;
        }
        var typeName = new StringBuilder(string.Concat(type.Name.AsSpan(0, type.Name.IndexOf('`')), "<"));
        var genericArguments = type.GetGenericArguments();
        for (var i = 0; i < genericArguments.Length; i++) {
            typeName.Append(GetTypeName(genericArguments[i]));
            if (i < genericArguments.Length - 1) {
                typeName.Append(',');
            }
        }
        typeName.Append('>');
        return typeName.ToString();
    }    
    
    /// <summary>
    /// If the attribute is NameAttribute type, it returns "[Name]" (it removes the final Attribute suffix and wraps it in brackets)
    /// </summary>
    /// <param name="att"></param>
    /// <returns></returns>
    public static string FormatAttribute(this Attribute att) {
        return FormatAttribute(att.GetType());
    }

    /// <summary>
    /// If the type is NameAttribute, it returns "[Name]" (it removes the final Attribute suffix and wraps it in brackets)
    /// </summary>
    /// <param name="att"></param>
    /// <returns></returns>
    public static string FormatAttribute(this Type type, Dictionary<string, object>? parameters = null) {
        var name = type.GetTypeName();
        var lastIndexOf = name.LastIndexOf("Attribute", StringComparison.Ordinal);
        if (lastIndexOf > 0) name = name.Remove(lastIndexOf);
        if (parameters != null) {
            parameters = parameters.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
            if (parameters.Count <= 0) return $"[{name}]";
            var parametersString = string.Join(", ", parameters.Select(p => $"{p.Key} = {Format(p.Value)}"));
            return $"[{name}({parametersString})]";
        }
        return $"[{name}]";

        string Format(object o) {
            if (o is string s) return $"\"{s}\"";
            return o.ToString();
        }
    }

    
}