using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Betauer.Core; 

public static class TypeExtensions {
    
    /// <summary>
    /// If works like type.IsSubclassOf() and type.IsAssignableTo() but it can accept any kind of type:
    /// - classes: without generic, with generic (like Class<string>) and generic type definition (like MyClass<,>)
    /// - interfaces: without generic, with generic (like MyInterface<string>) and generic type definition (like MyInterface<,>)
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="typeToFind"></param>
    /// <returns></returns>
    public static bool IsSubclassOfOrImplements(this Type from, Type typeToFind) {
        if (typeToFind.IsInterface) return from.ImplementsInterface(typeToFind);
        return from.IsGenericSubclassOf(typeToFind);
    }

    /// <summary>
    /// It works like type.IsSubclassOf() but it also works with generic type definitions, such as:
    /// - typeof(MyClass).IsSubclassOf(typeof(ParentClass<,>))
    ///
    /// Remember IsSubClassOf() works with generic types only like:
    /// - typeof(MyClass).IsSubclassOf(typeof(ParentClass<string>))
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="parentClass"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static bool IsGenericSubclassOf(this Type from, Type parentClass) {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (parentClass == null) throw new ArgumentNullException(nameof(parentClass));

        if (!parentClass.IsClass) throw new Exception("TypeToFind must be a class");
        if (!parentClass.IsGenericType || !parentClass.IsGenericTypeDefinition) return from.IsSubclassOf(parentClass);
        
        var type = from.GetTypeInfo();
        while (!type.IsGenericType || type.GetGenericTypeDefinition() != parentClass) {
            if (type.BaseType == null) return false;
            type = type.BaseType.GetTypeInfo();
        }
        return true;
    }

    /// <summary>
    /// A better version of type.IsAssignableTo(interfaceType) but it works with generic type definition too, such as:
    /// - typeof(List<string>).ImplementsInterface(typeof(IList<>))
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
    /// Returns an array of Type from a generic interface definition, such as:
    /// - typeof(List<string>).FindGenericsFromInterfaceDefinition(typeof(IList<>)) returns new Type{}[typeof(string)] 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="interfaceType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static Type[] FindGenericsFromInterfaceDefinition(this Type from, Type interfaceType) {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (interfaceType == null) throw new ArgumentNullException(nameof(interfaceType));

        if (!interfaceType.IsInterface || !interfaceType.IsGenericTypeDefinition) throw new Exception($"TypeToFind {interfaceType} must be a generic type definition interface");
        
        if (from.IsInterface && interfaceType == from.GetGenericTypeDefinition()) return from.GetGenericArguments();
        var interfaceFound = from.GetInterfaces().FirstOrDefault(implementedInterface => 
            implementedInterface.IsGenericType && implementedInterface.GetGenericTypeDefinition() == interfaceType);
        if (interfaceFound == null) throw new Exception($"Type {from} doesn't implements {interfaceType}");
        return interfaceFound.GetGenericArguments();
    }


    /// <summary>
    /// Returns an array of Type from a generic parent definition, such as:
    /// class Parent<T> {}
    /// class SubClass : Parent<string> {}
    /// - typeof(SubClass).FindGenericsFromBaseTypeDefinition(typeof(Parent<>)) returns new Type{}[typeof(string)] 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="parentType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static Type[] FindGenericsFromBaseTypeDefinition(this Type from, Type parentType) {
        if (from == null) throw new ArgumentNullException(nameof(from));
        if (parentType == null) throw new ArgumentNullException(nameof(parentType));
        
        if (!parentType.IsClass || !parentType.IsGenericTypeDefinition) throw new Exception($"TypeToFind {parentType} must be a generic type definition class");
        
        var type = from.GetTypeInfo();
        while (!type.IsGenericType || type.GetGenericTypeDefinition() != parentType) {
            if (type.BaseType == null) throw new Exception($"Type {from} is not a subclass of {parentType}");
            type = type.BaseType.GetTypeInfo();
        }
        return type.GetGenericArguments();
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