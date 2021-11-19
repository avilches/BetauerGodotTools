using System;
using System.Linq;
using System.Reflection;

namespace Tools {
    public static class ReflectionTools {
        public static string GetTypeWithoutGenerics(Type type) {
            var name = type.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }

        public static string GetTypeWithGenerics(Type type) {
            var name = type.Name;
            var index = name.IndexOf('`');
            if (index == -1) {
                return name;
            }
            var nameWithoutGenerics = name.Substring(0, index);
            var generics = string.Join(",", type.GetGenericArguments().Select(x => x.Name).ToList());
            return $"{nameWithoutGenerics}<{generics}>";
        }

        public static MethodInfo FindMethod(object o, string name, Type returnType, Type[] parameterTypes = null) {
            var methods = o.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            return methods.FirstOrDefault(method =>
                method.Name == name && method.ReturnType == returnType && ParameterMatches(method, parameterTypes));
        }

        private static bool ParameterMatches(MethodInfo method, Type[] parameterTypes) {
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