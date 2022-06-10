using System;
using System.Linq;
using System.Reflection;

namespace Betauer {
    public static class TypeExtensions {
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