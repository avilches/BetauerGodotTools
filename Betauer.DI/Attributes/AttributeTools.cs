using System;
using System.Linq;
using Betauer.DI.Exceptions;
using Betauer.Tools.Reflection;

namespace Betauer.DI.Attributes; 

public class AttributeTools {
    public static void ValidateDuplicates<T>(Type type, Attribute attribute) where T : class {
        if (type.GetAttributes<T>().FirstOrDefault() is Attribute found) {
            throw new InvalidAttributeException($"Can't use {found.FormatAttribute()} and {attribute.FormatAttribute()} in the same class: {type.GetTypeName()}");
        }
    }
    
    public static void ValidateDuplicates<T1, T2>(Type type, Attribute attribute) 
        where T1 : class 
        where T2 : class {
        var attributes = type.GetCustomAttributes(false);
        for (var i = 0; i < attributes.Length; i++) {
            var found = attributes[i] as Attribute;
            ValidateAttribute<T1>(type, found, attribute);
            ValidateAttribute<T2>(type, found, attribute);
        }
    }

    public static void ValidateDuplicates<T1, T2, T3>(Type type, Attribute attribute) 
        where T1 : class 
        where T2 : class 
        where T3 : class {
        var attributes = type.GetCustomAttributes(false);
        for (var i = 0; i < attributes.Length; i++) {
            var found = attributes[i] as Attribute;
            ValidateAttribute<T1>(type, found, attribute);
            ValidateAttribute<T2>(type, found, attribute);
            ValidateAttribute<T3>(type, found, attribute);
        }
    }

    public static void ValidateDuplicates<T1, T2, T3, T4>(Type type, Attribute attribute) 
        where T1 : class 
        where T2 : class 
        where T3 : class 
        where T4 : class {
        var attributes = type.GetCustomAttributes(false);
        for (var i = 0; i < attributes.Length; i++) {
            var found = attributes[i] as Attribute;
            ValidateAttribute<T1>(type, found, attribute);
            ValidateAttribute<T2>(type, found, attribute);
            ValidateAttribute<T3>(type, found, attribute);
            ValidateAttribute<T4>(type, found, attribute);
        }
    }

    private static void ValidateAttribute<T>(Type type, Attribute found, Attribute attribute) where T : class {
        if (found.GetType().IsAssignableTo(typeof(T))) {
            throw new InvalidAttributeException(
                $"Can't use {found.FormatAttribute()} and {attribute.FormatAttribute()} in the same class: {type.GetTypeName()}");
        }
    }
    
    public static void ValidateDuplicates<T>(IGetter getter, Attribute attribute) where T : class {
        if (getter.MemberInfo.GetAttributes<T>().FirstOrDefault() is Attribute classAttribute) {
            throw new InvalidAttributeException(
                $"Can't use {classAttribute.FormatAttribute()} and {attribute.FormatAttribute()} in the same member: {getter.ToString()} (class {getter.DeclaringType.GetTypeName()})");
        }
    }
}