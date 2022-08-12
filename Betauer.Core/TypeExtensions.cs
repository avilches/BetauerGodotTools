using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Betauer {
    public interface IGetter {
        public Type Type { get; }
        public string Name { get; }
        public Func<object?, object?> GetValue { get; }
        public MemberInfo MemberInfo { get; }
    }

    public interface IGetter<T> : IGetter {
        public T Attribute { get;  }
    }

    public interface ISetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object?, object?> SetValue { get; }
        public MemberInfo MemberInfo { get; }
    }

    public interface ISetter<T> : ISetter {
        public T Attribute { get;  }
    }

    public interface IGetterSetter : ISetter, IGetter {}
    public interface IGetterSetter<T> : ISetter<T>, IGetter<T> {}


    public class Getter : IGetter {
        public Type Type { get; }
        public string Name { get; }
        public Func<object?, object?> GetValue { get; }
        public MemberInfo MemberInfo { get; }
        
        public Getter(MemberInfo memberInfo) {
            MemberInfo = memberInfo;
            if (memberInfo is PropertyInfo property) {
                Type = property.PropertyType;
                Name = property.Name;
                GetValue = property.GetValue;
            } else if (memberInfo is FieldInfo field) {
                Type = field.FieldType;
                Name = field.Name;
                GetValue = field.GetValue;
            } else if (memberInfo is MethodInfo method) {
                if (method.GetParameters().Length != 0) throw new ArgumentException("Getter method must not have parameter");
                Type = method.ReturnType;
                Name = method.Name;
                GetValue = (instance) => method.Invoke(instance, Array.Empty<object>());
            } else {
                throw new ArgumentException("Member must be PropertyInfo, FieldInfo or MethodInfo");
            }
        }
    }

    public class Setter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object?, object?> SetValue { get; }
        public MemberInfo MemberInfo { get; }
        
        public Setter(MemberInfo memberInfo) {
            MemberInfo = memberInfo;
            if (memberInfo is PropertyInfo property) {
                Type = property.PropertyType;
                Name = property.Name;
                SetValue = property.SetValue;
            } else if (memberInfo is FieldInfo field) {
                Type = field.FieldType;
                Name = field.Name;
                SetValue = field.SetValue;
            } else if (memberInfo is MethodInfo method) {
                if (method.GetParameters().Length != 1) throw new ArgumentException("Setter method must have 1 parameter only");
                Type = method.GetParameters()[0].ParameterType;
                Name = method.Name;
                SetValue = (instance, value) => method.Invoke(instance, new [] { value });
            } else {
                throw new ArgumentException("Member must be PropertyInfo or FieldInfo or MethodInfo");
            }
        }
    }

    public class GetterSetter : IGetterSetter {
        public Type Type { get; }
        public string Name { get; }
        public Action<object?, object?> SetValue { get; }
        public Func<object?, object?> GetValue { get; }
        public MemberInfo MemberInfo { get; }
        
        public GetterSetter(MemberInfo memberInfo) {
            MemberInfo = memberInfo;
            if (memberInfo is PropertyInfo property) {
                Type = property.PropertyType;
                Name = property.Name;
                SetValue = property.SetValue;
                GetValue = property.GetValue;
            } else if (memberInfo is FieldInfo field) {
                Type = field.FieldType;
                Name = field.Name;
                SetValue = field.SetValue;
                GetValue = field.GetValue;
            } else {
                throw new ArgumentException("Member must be PropertyInfo or FieldInfo");
            }
        }
    }

    public class Getter<T> : Getter, IGetter<T> {
        public T Attribute { get; }

        public Getter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            Attribute = attribute;
        }
    }

    public class Setter<T> : Setter, ISetter<T> {
        public T Attribute { get; }

        public Setter(MemberInfo memberInfo, T attribute) : base(memberInfo) {
            Attribute = attribute;
        }
    }

    public class GetterSetter<T> : GetterSetter, IGetterSetter<T> {
        public T Attribute { get; }

        public GetterSetter(MemberInfo member, T attribute) : base(member) {
            Attribute = attribute;
        }
    }

    public static class TypeExtensions {
        public static List<IGetterSetter<T>> GetGetterSetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) {
            if (memberFlags.HasFlag(MemberTypes.Method)) throw new Exception("Can't create GetterSetter with methods");

            IEnumerable<MemberInfo> e = type.GetMembers(bindingAttr)
                .Where(info => (memberFlags.HasFlag(MemberTypes.Field) && info is FieldInfo) ||
                               (memberFlags.HasFlag(MemberTypes.Property) && info is PropertyInfo));
            List<IGetterSetter<T>> getterSetter = new List<IGetterSetter<T>>();
            foreach (var memberInfo in e) {
                if (Attribute.GetCustomAttribute(memberInfo, typeof(T), false) is T attribute)
                    getterSetter.Add(new GetterSetter<T>(memberInfo, attribute));
            }
            return getterSetter;
        }

        public static List<ISetter<T>> GetSetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) {
            IEnumerable<MemberInfo> e = type.GetMembers(bindingAttr)
                .Where(info => (memberFlags.HasFlag(MemberTypes.Field) && info is FieldInfo) ||
                               (memberFlags.HasFlag(MemberTypes.Method) && info is MethodInfo methodInfo && methodInfo.GetParameters().Length == 1) ||
                               (memberFlags.HasFlag(MemberTypes.Property) && info is PropertyInfo));
            List<ISetter<T>> setters = new List<ISetter<T>>();
            foreach (var memberInfo in e) {
                if (Attribute.GetCustomAttribute(memberInfo, typeof(T), false) is T attribute)
                    setters.Add(new Setter<T>(memberInfo, attribute));
            }
            return setters;
        }

        public static List<IGetter<T>> GetGetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) {
            IEnumerable<MemberInfo> e = type.GetMembers(bindingAttr)
                .Where(info => (memberFlags.HasFlag(MemberTypes.Field) && info is FieldInfo) ||
                               (memberFlags.HasFlag(MemberTypes.Method) && info is MethodInfo methodInfo && methodInfo.GetParameters().Length == 0) ||
                               (memberFlags.HasFlag(MemberTypes.Property) && info is PropertyInfo));
            List<IGetter<T>> getters = new List<IGetter<T>>();
            foreach (var memberInfo in e) {
                if (Attribute.GetCustomAttribute(memberInfo, typeof(T), false) is T attribute)
                    getters.Add(new Getter<T>(memberInfo, attribute));
            }
            return getters;
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