using System;
using System.Collections.Generic;
using System.Reflection;

namespace Betauer.DI {
    public static class InjectorFunction {
        public static bool InjectField(Container container, object target, GetterSetter<InjectAttribute> field) {
            if (!IsZeroArgumentFunction(field.Type)) return false;
            var outType = field.Type.GetGenericArguments()[0];
            // [Inject] private Func<TOut> func;
            // Find a Func<T, TResult> (callable with func(instance) => returns TResult) where T is the current
            // instance Type (or any of its base classes) and convert it to a function where the current instance
            // lives inside (a closure), so the user can call it later with just func()
            try {
                Delegate function = container.ResolveAndCreateClosure(target, outType);
                field.SetValue(target, function);
            } catch (KeyNotFoundException) {
                // Ignore if not found: the Injector will take care of this field later
                return false;
            }
            return true;
        }

        private static bool IsZeroArgumentFunction(Type type) =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Func<>) &&
            type.GetGenericArguments().Length == 1;

    }

    public static class ContainerFunctionExtensions {
        public static ContainerBuilder Function<T, TResult>(this ContainerBuilder containerBuilder,
            Func<T, TResult> function) {
            containerBuilder.Static(function);
            return containerBuilder;
        }

        public static Func<T, TResult> ResolveFunction<T, TResult>(this Container container) {
            return (Func<T, TResult>)container.ResolveFunction(typeof(T), typeof(TResult));
        }

        public static Delegate ResolveFunction(this Container container, Type inputType, Type returnType) {
            var functionGenericTypes = new[] { inputType, returnType };
            var funcType = typeof(Func<,>).MakeGenericType(functionGenericTypes);

            container.TryGetProvider(funcType, out IProvider? provider);
            if (provider != null) return (Delegate)provider.Get(null);
            throw new KeyNotFoundException($"Function <{inputType},{returnType}> not found");
        }

        /*
         * Find a Func<InputType, outType>
         * ... or Func<InputType.BaseType, outType>
         * ... or Func<InputType.BaseType.BaseType, outType>
         * or throws a KeyNotFoundException
         */
        public static Func<T, TResult> ResolveCompatibleFunction<T, TResult>(this Container container) {
            return (Func<T, TResult>)container.ResolveCompatibleFunction(typeof(T), typeof(TResult));
        }

        public static Delegate ResolveCompatibleFunction(this Container container, Type inputType, Type resultType) {
            Delegate? function = null;
            var functionGenericTypes = new Type?[] { inputType, resultType };
            do {
                var funcType = typeof(Func<,>).MakeGenericType(functionGenericTypes);
                container.TryGetProvider(funcType, out IProvider? provider);
                if (provider != null) function = (Delegate)provider.Get(null);
                if (function == null) functionGenericTypes[0] = functionGenericTypes[0]?.BaseType;
            } while (function == null && functionGenericTypes[0] != null);
            if (function == null) {
                throw new KeyNotFoundException($"Function compatible with <{inputType},{resultType}> not found");
            }
            return function;
        }

        public static Func<TOut> ResolveAndCreateClosure<TOut>(this Container container, object target) {
            return (Func<TOut>)container.ResolveAndCreateClosure(target, typeof(TOut));
        }

        public static Delegate ResolveAndCreateClosure(this Container container, object target, Type returnType) {
            Delegate function = container.ResolveCompatibleFunction(target.GetType(), returnType);
            var closure = (Delegate)ComposeMethod.MakeGenericMethod(new[] { target.GetType(), returnType })
                .Invoke(null, new[] { function, target });
            return closure;
        }

        private static Func<T2> Compose<T1, T2>(Func<T1, T2> func, T1 inject) => () => func(inject);

        private static readonly MethodInfo ComposeMethod = typeof(ContainerFunctionExtensions)
            .GetMethod(nameof(Compose), BindingFlags.Static | BindingFlags.NonPublic)!;
    }
}