using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Betauer {
    public class GetterSetter {
        public readonly Type Type;
        public readonly string Name;
        public readonly Action<object?, object?> SetValue;
        public readonly Func<object?, object?> GetValue;
        
        public GetterSetter(MemberInfo member) {
            if (member is PropertyInfo property) {
                Type = property.PropertyType;
                Name = property.Name;
                SetValue = property.SetValue;
                GetValue = property.GetValue;
            } else if (member is FieldInfo field) {
                Type = field.FieldType;
                Name = field.Name;
                SetValue = field.SetValue;
                GetValue = field.GetValue;
            } else {
                throw new ArgumentException("Member must be PropertyInfo or FieldInfo");
            }
        }
    }

    public class Getter {
        public readonly Type Type;
        public readonly string Name;
        public readonly Func<object?, object?> GetValue;
        
        public Getter(MemberInfo member) {
            if (member is PropertyInfo property) {
                Type = property.PropertyType;
                Name = property.Name;
                GetValue = property.GetValue;
            } else if (member is FieldInfo field) {
                Type = field.FieldType;
                Name = field.Name;
                GetValue = field.GetValue;
            } else if (member is MethodInfo method) {
                Type = method.ReturnType;
                Name = method.Name;
                GetValue = (instance) => method.Invoke(instance, Array.Empty<object>());
            } else {
                throw new ArgumentException("Member must be PropertyInfo, FieldInfo or MethodInfo");
            }
        }
    }

    public class Getter<T> : Getter {
        public readonly T Attribute;

        public Getter(MemberInfo member, T attribute) : base(member) {
            Attribute = attribute;
        }
    }

    public class GetterSetter<T> : GetterSetter {
        public readonly T Attribute;

        public GetterSetter(MemberInfo member, T attribute) : base(member) {
            Attribute = attribute;
        }
    }

    public static class TypeExtensions {

        public static IEnumerable<GetterSetter> GetPropertiesAndFields(this Type type, BindingFlags bindingAttr) {
            LinkedList<GetterSetter> setters = new LinkedList<GetterSetter>();
            foreach (var memberInfo in type.GetProperties(bindingAttr).OfType<MemberInfo>().Concat(type.GetFields(bindingAttr))) {
                setters.AddLast(new GetterSetter(memberInfo));
            }
            return setters;
        }

        public static IEnumerable<GetterSetter<T>> GetPropertiesAndFields<T>(this Type type, BindingFlags bindingAttr) {
            LinkedList<GetterSetter<T>> setters = new LinkedList<GetterSetter<T>>();
            foreach (var memberInfo in type.GetProperties(bindingAttr).OfType<MemberInfo>().Concat(type.GetFields(bindingAttr))) {
                if (Attribute.GetCustomAttribute(memberInfo, typeof(T), false) is T attribute) {
                    setters.AddLast(new GetterSetter<T>(memberInfo, attribute));
                }
            }
            return setters;
        } 
        
        public static IEnumerable<Getter<T>> GetGetters<T>(this Type type, BindingFlags bindingAttr) {
            LinkedList<Getter<T>> setters = new LinkedList<Getter<T>>();
            foreach (var memberInfo in type.GetProperties(bindingAttr)) {
                if (Attribute.GetCustomAttribute(memberInfo, typeof(T), false) is T attribute) {
                    setters.AddLast(new Getter<T>(memberInfo, attribute));
                }
            }
            foreach (var methodInfo in type.GetMethods(bindingAttr)) {
                if (Attribute.GetCustomAttribute(methodInfo, typeof(T), false) is T attribute && methodInfo.GetParameters().Length == 0) {
                    setters.AddLast(new Getter<T>(methodInfo, attribute));
                }
            }
            return setters;
        } 
        
        public static string GetNameWithoutGenerics(this Type type) {
            var name = type.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        public static MethodInfo FindMethod(this Type type, string name, Type returnType,
            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            Type?[] parameterTypes = null) {
            var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return methods.FirstOrDefault(method =>
                method.Name == name &&
                method.ReturnType == returnType &&
                ParameterMatches(method, parameterTypes));
        }

        private static bool ParameterMatches(MethodInfo method, Type[]? parameterTypes) {
            var parameterInfos = method.GetParameters();
            if ((parameterTypes?.Length ?? 0) != parameterInfos.Length) return false;
            if (parameterInfos.Length == 0) return true;
            for (var i = 0; i < parameterInfos.Length; i++) {
                if (parameterInfos[i].ParameterType != parameterTypes[i]) {
                    return false;
                }
            }
            return true;
        }
    }
}