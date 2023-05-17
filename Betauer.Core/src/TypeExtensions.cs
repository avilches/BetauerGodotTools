using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Betauer.Core; 

public static class TypeExtensions {
    
    /// <summary>
    /// A better version of type.IsAssignableTo(interfaceType) that also works with generic types, such as:
    ///
    /// - typeof(List<string>).ImplementsInterface(typeof(IList<>)) returns true
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static bool ImplementsInterface(this Type type, Type interfaceType) {
        if (type == null) throw new ArgumentNullException(nameof(type));
        if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));
    
        if (type.IsAssignableTo(interfaceType)) return true;
    
        // interfaceType is a GenericTypeDefinition (that means is something like List<> instead of List<string>)
        return (type.IsGenericType && interfaceType.IsGenericTypeDefinition && interfaceType == type.GetGenericTypeDefinition()) ||
               type.GetInterfaces().Any(implementedInterface =>
                   implementedInterface == interfaceType ||
                   implementedInterface.IsGenericType &&
                   implementedInterface.GetGenericTypeDefinition() == interfaceType);
    }
    
    public static Type[] FindGenericsFromBaseType(this Type from, Type typeToFind) {
        if (!typeToFind.IsGenericTypeDefinition) throw new Exception("TypeToFind must be a generic type definition");
        var type = from.GetTypeInfo();
        while (!type.IsGenericType || type.GetGenericTypeDefinition() != typeToFind) {
            type = type.BaseType.GetTypeInfo();
        }
        return type.GetGenericArguments();
    }
    
    public static bool IsGenericSubclassOf(this Type from, Type typeToFind) {
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
    public static string FormatAttribute(this Type type) {
        var name = type.GetTypeName();
        var lastIndexOf = name.LastIndexOf("Attribute", StringComparison.Ordinal);
        if (lastIndexOf > 0) name = name.Remove(lastIndexOf);
        return $"[{name}]";
    }

    
}