using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Betauer.Reflection {
    public static partial class TypeExtensions {
        private static readonly Dictionary<(Type, Type, MemberTypes, BindingFlags), object> Cache = new();
        
        public static List<ISetter<T>> GetSettersCached<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : Attribute {
            var key = (typeof(ISetter<T>), type, memberFlags, bindingAttr);
            if (Cache.TryGetValue(key, out var result)) return (List<ISetter<T>>)result;
            return (List<ISetter<T>>)(Cache[key] = type.GetSetters<T>(memberFlags, bindingAttr));
        }

        public static List<ISetter<T>> GetSetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : Attribute {
            IEnumerable<MemberInfo> e = Enumerable.Empty<MemberInfo>();
            e = ConcatFields(e, type, memberFlags, bindingAttr);
            e = ConcatProperties(e, type, memberFlags, bindingAttr);
            e = ConcatMethods(e, type, memberFlags, bindingAttr, 1);
            
            List<ISetter<T>> setters = new List<ISetter<T>>();
            foreach (var memberInfo in e) {
                if (memberInfo.GetAttribute<T>() is T attribute) {
                    var validSetter = FastSetter.IsValid(memberInfo);
                    var validGetter = FastGetter.IsValid(memberInfo);
                    if (validGetter && validSetter) {
                        // fields and properties
                        setters.Add(new FastGetterSetter<T>(memberInfo, attribute));
                    } else if (validSetter) {
                        // methods with 1 parameter and void return type
                        setters.Add(new FastSetter<T>(memberInfo, attribute));
                    }
                }
            }
            return setters;
        }

        public static List<ISetter<T>> GetGettersCached<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : Attribute {
            var key = (typeof(ISetter<T>), type, memberFlags, bindingAttr);
            if (Cache.TryGetValue(key, out var result)) return (List<ISetter<T>>)result; 
            return (List<ISetter<T>>)(Cache[key] = type.GetGetters<T>(memberFlags, bindingAttr));
        }

        public static List<IGetter<T>> GetGetters<T>(this Type type, MemberTypes memberFlags, BindingFlags bindingAttr) where T : Attribute {
            IEnumerable<MemberInfo> e = Enumerable.Empty<MemberInfo>();
            e = ConcatFields(e, type, memberFlags, bindingAttr);
            e = ConcatProperties(e, type, memberFlags, bindingAttr);
            e = ConcatMethods(e, type, memberFlags, bindingAttr, 0);
            List<IGetter<T>> getters = new List<IGetter<T>>();
            foreach (var memberInfo in e) {
                if (memberInfo.GetAttribute<T>() is T attribute) {
                    var validSetter = FastSetter.IsValid(memberInfo);
                    var validGetter = FastGetter.IsValid(memberInfo);
                    if (validGetter && validSetter) {
                        // fields and properties
                        getters.Add(new FastGetterSetter<T>(memberInfo, attribute));
                    } else if (validGetter) {
                        // methods with 1 parameter and void return type
                        getters.Add(new FastGetter<T>(memberInfo, attribute));
                    }
                }
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

        public static string GetNameWithoutGenerics(this Type type) {
            var name = type.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
    }
}