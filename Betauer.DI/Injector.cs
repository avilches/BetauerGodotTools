using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Betauer.Reflection;

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
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Injector));
        private readonly Container _container;

        private const BindingFlags InjectFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public Injector(Container container) {
            _container = container;
        }

        public void InjectServices(object target, ResolveContext context) {
            if (target is Delegate) return;
#if DEBUG
            _logger.Debug("Injecting fields in " + target.GetType() + ": " + target.GetHashCode().ToString("X"));
#endif
            var members = target.GetType().GetSettersCached<InjectAttribute>(MemberTypes.Method | MemberTypes.Property, InjectFlags);
            foreach (var setter in members)
                InjectMember(target, context, setter);
        }

        private void InjectMember(object target, ResolveContext context, ISetter<InjectAttribute> setter) {
            if (setter is IGetter getter && getter.GetValue(target) != null) {
                // Ignore the already defined values
                // TODO: test
                return;
            }

            if (InjectorFunction.InjectField(_container, target, setter)) return;

            var nullable = setter.Attribute.Nullable;
            var name = setter.Attribute.Name;
            if (name != null) {
                name = name.Trim();
                if (_container.Contains(name)) {
#if DEBUG
                    _logger.Debug(target.GetType().FullName + " (" + target.GetHashCode()+") | " + setter + " | Name in [Inject(\""+name+"\")");
#endif
                    InjectFieldByName(target, context, setter, name);
                    return;
                }
                if (!nullable) {
                    throw new InjectMemberException(setter.Name, target,
                        "Service \"" + name + "\" not found when trying to inject " + setter);
                }
                return;
            } 
            
            if (_container.Contains(setter.Name)) {
#if DEBUG
                _logger.Debug(target.GetType().FullName + " (" + target.GetHashCode()+") | " + setter + " | Member name: "+setter.Name);
#endif
                InjectFieldByName(target, context, setter, setter.Name);
                return;
            }

            var realType = setter.Type.IsGenericType && setter.Type.GetGenericTypeDefinition() == typeof(Factory<>)
                ? setter.Type.GetGenericArguments()[0]
                : setter.Type;
            if (_container.CreateIfNotFound || _container.Contains(realType)) {
#if DEBUG
                _logger.Debug(target.GetType().FullName + " (" + target.GetHashCode()+") | " + setter + " | Member type: "+realType);
#endif
                InjectFieldByType(target, context, setter);
                return;
            }

            if (!nullable) {
                throw new InjectMemberException(setter.Name, target, "Not service found when trying to inject [" + setter +
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