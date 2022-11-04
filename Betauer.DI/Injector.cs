using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.DI {
    public class Factory<T> {
        private readonly Container _container;
        public IProvider Provider { get; }

        public T Get() {
            return (T)Provider.Get(_container);
        }

        private Factory(Container container, IProvider provider) {
            _container = container;
            Provider = provider;
        }
    }

    public class Injector {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Injector));
        private readonly Container _container;

        private const BindingFlags InjectFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public Injector(Container container) {
            _container = container;
        }

        public void InjectServices(object target, ResolveContext context) {
            if (target is Delegate) return;
            #if DEBUG
                Logger.Debug($"Injecting fields in {target.GetType()}: {target.GetHashCode():x8}");
            #endif
            var members = target.GetType().GetSettersCached<InjectAttribute>(MemberTypes.Method | MemberTypes.Property, InjectFlags);
            foreach (var setter in members)
                InjectMember(target, context, setter);
        }

        private void InjectMember(object target, ResolveContext context, ISetter<InjectAttribute> getterSetter) {
            // Ignore the already defined values
            if (getterSetter is IGetter getter && getter.GetValue(target) is not null) return;
            
            if (InjectorFunction.InjectField(_container, target, getterSetter)) return;

            var nullable = getterSetter.SetterAttribute.Nullable;
            var name = getterSetter.SetterAttribute.Name;
            if (name != null) {
                name = name.Trim();
                if (_container.Contains(name)) {
                    #if DEBUG
                        Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {getterSetter} | Name in [Inject(\"{name}\")");
                    #endif
                    InjectFieldByName(target, context, getterSetter, name);
                    return;
                }
                if (!nullable) {
                    throw new InjectMemberException(getterSetter.Name, target,
                        "Service \"" + name + "\" not found when trying to inject " + getterSetter);
                }
                return;
            } 
            
            if (_container.Contains(getterSetter.Name)) {
                #if DEBUG
                    Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {getterSetter} | Member name: {getterSetter.Name}");
                #endif
                InjectFieldByName(target, context, getterSetter, getterSetter.Name);
                return;
            }

            var realType = getterSetter.Type.IsGenericType && getterSetter.Type.GetGenericTypeDefinition() == typeof(Factory<>)
                ? getterSetter.Type.GetGenericArguments()[0]
                : getterSetter.Type;
            if (_container.CreateIfNotFound || _container.Contains(realType)) {
                #if DEBUG
                    Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {getterSetter} | Member type: {realType}");
                #endif
                InjectFieldByType(target, context, getterSetter);
                return;
            }

            if (!nullable) {
                throw new InjectMemberException(getterSetter.Name, target, "Not service found when trying to inject [" + getterSetter +
                                                                     "] in " + target.GetType().FullName);
            }
        }

        private void InjectFieldByName(object target, ResolveContext context, ISetter setter, string name) {
            if (setter.Type.IsGenericType && setter.Type.GetGenericTypeDefinition() == typeof(Factory<>)) {
                var type = setter.Type.GetGenericArguments()[0];
                var provider = _container.GetProvider(setter.Type);
                var lazy = CreateLazyWithGeneric(_container, provider, type);
                setter.SetValue(target, lazy);
            } else {
                var service = _container.Resolve(name, context);
                setter.SetValue(target, service);
            }
        }

        private void InjectFieldByType(object target, ResolveContext context, ISetter setter) {
            if (setter.Type.IsGenericType && setter.Type.GetGenericTypeDefinition() == typeof(Factory<>)) {
                var type = setter.Type.GetGenericArguments()[0];
                var provider = _container.GetProvider(type);
                var lazy = CreateLazyWithGeneric(_container, provider, type);
                setter.SetValue(target, lazy);
            } else {
                var service = _container.Resolve(setter.Type, context);
                setter.SetValue(target, service);
            }
        }

        private static object CreateLazyWithGeneric(Container container, IProvider provider, Type genericType) {
            var type = typeof(Factory<>).MakeGenericType(genericType);
            var ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(info => info.GetParameters().Length == 2 &&
                               info.GetParameters()[0].ParameterType == typeof(Container) &&
                               info.GetParameters()[1].ParameterType == typeof(IProvider));
            return ctor.Invoke(new object[] { container, provider });
        }

    }
}