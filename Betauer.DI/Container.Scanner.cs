using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.DI;

public partial class Container {
    private class Scanner {
        private readonly Builder _builder;

        internal Scanner(Builder builder) {
            _builder = builder;
        }

        internal void ScanConfiguration(object configuration) {
            ScanForScanAttributes(configuration.GetType(), null);
            ScanServicesFromConfigurationInstance(configuration);
        }

        internal void Scan(Type type, HashSet<Type>? stack) {
            // Look up for [Scan(typeof(...)]
            ScanForScanAttributes(type, stack);
    
            if (type.GetAttribute<FactoryAttribute>() is FactoryAttribute factoryAttribute) {
                if (type.HasAttribute<ConfigurationAttribute>()) throw new InvalidAttributeException("Can't use [Factory] and [Configuration] in the same class");
                if (type.HasAttribute<ServiceAttribute>()) throw new InvalidAttributeException("Can't use [Factory] and [Service] in the same class");
                RegisterCustomFactoryFromClass(type, factoryAttribute);
                
            } else if (type.GetAttribute<ServiceAttribute>() is ServiceAttribute serviceAttr) {
                if (type.HasAttribute<ConfigurationAttribute>()) throw new InvalidAttributeException("Can't use [Service] and [Configuration] in the same class");
                RegisterServiceFromClass(type, serviceAttr);
                
            } else if (type.GetAttribute<ConfigurationAttribute>() is ConfigurationAttribute configurationAttribute) {
                var configuration = Activator.CreateInstance(type);
                ScanServicesFromConfigurationInstance(configuration!);
            }
        }
    
        private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    
        private void ScanForScanAttributes(Type type, HashSet<Type>? stack) {
            foreach (var importAttribute in type.GetAttributes<ScanAttribute>()) {
                stack ??= new HashSet<Type>();
                stack.Add(type);
                importAttribute.GetType().GetGenericArguments()
                    .Where(typeToImport => !stack.Contains(typeToImport))
                    .ForEach(typeToImport => Scan(typeToImport, stack));
            }
        }
    
        private void ScanServicesFromConfigurationInstance(object configuration) {
            // No need to use GetGettersCached, this reflection scan is only done once
            var serviceGetters = configuration.GetType().GetGetters<ServiceAttribute>(MemberTypes.Method | MemberTypes.Property, ScanMemberFlags);
            foreach (var getter in serviceGetters) RegisterServiceFromGetter(configuration, getter);
            
            var factoryGetters = configuration.GetType().GetGetters<FactoryAttribute>(MemberTypes.Method | MemberTypes.Property, ScanMemberFlags);
            foreach (var getter in factoryGetters) RegisterCustomFactoryFromGetter(configuration, getter);
        }
    
        private void RegisterServiceFromClass(Type type, ServiceAttribute serviceAttr) {
            var registeredType = serviceAttr.Type ?? type;
            var name = serviceAttr.Name;
            var primary = serviceAttr.Primary || type.HasAttribute<PrimaryAttribute>();
            var lazy = serviceAttr.Lazy || type.HasAttribute<LazyAttribute>();
            var lifetime = serviceAttr.Lifetime;
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, Factory, lifetime, name, primary, lazy);
        }
    
        private void RegisterServiceFromGetter(object configuration, IGetter<ServiceAttribute> getter) {
            var serviceAttr = getter.GetterAttribute;
            var type = getter.Type;
            var registeredType = serviceAttr.Type ?? type;
            var name = serviceAttr.Name ?? getter.Name;
            var primary = serviceAttr.Primary || getter.MemberInfo.HasAttribute<PrimaryAttribute>();
            var lazy = serviceAttr.Lazy || getter.MemberInfo.HasAttribute<LazyAttribute>();
            var lifetime = serviceAttr.Lifetime;
            object Factory() => getter.GetValue(configuration)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, Factory, lifetime, name, primary, lazy);
        }
    
        private void RegisterCustomFactoryFromClass(Type type, FactoryAttribute factoryAttribute) {
            var iFactoryInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFactory<>));
            if (iFactoryInterface == null) {
                throw new InvalidAttributeException($"Class {type.FullName} with [Factory] attribute must implement IFactory<T>");
            }
            var genericType = iFactoryInterface.GetGenericArguments()[0];
            var primary = factoryAttribute.Primary || type.HasAttribute<PrimaryAttribute>();
            var lazy = factoryAttribute.Lazy || type.HasAttribute<LazyAttribute>();
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterCustomFactory(genericType, Factory, factoryAttribute.Name, primary, lazy);
        }
    
        private void RegisterCustomFactoryFromGetter(object configuration, IGetter<FactoryAttribute> getter) {
            if (!getter.Type.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidAttributeException("Member " + getter + " with [Factory] attribute must implement IFactory<T>");
            }
            var factoryAttribute = getter.GetterAttribute;
            var genericType = getter.Type.GetGenericArguments()[0];
            var primary = factoryAttribute.Primary || getter.MemberInfo.HasAttribute<PrimaryAttribute>();
            var name = factoryAttribute.Name ?? getter.Name;
            var lazy = factoryAttribute.Lazy || getter.MemberInfo.HasAttribute<LazyAttribute>();
            object Factory() => getter.GetValue(configuration)!;
            _builder.RegisterCustomFactory(genericType, Factory, name, primary, lazy);
        }
    
        internal class FastGetFromProvider {
            private readonly Type _type;
            private readonly IProvider _provider;
            private FastMethodInfo? _providerGetMethod;
    
            public FastGetFromProvider(Type type, IProvider provider) {
                _type = type;
                _provider = provider;
            }
    
            public object Get() {
                var factoryInstance = _provider.Get();
                _providerGetMethod ??= FindGetMethod(factoryInstance);
                return _providerGetMethod.Invoke(factoryInstance)!;
            }
    
            private FastMethodInfo FindGetMethod(object factoryInstance) {
                return new FastMethodInfo(
                    factoryInstance.GetType()
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                        .First(m =>
                            m.Name == nameof(IFactory<object>.Get) &&
                            m.GetParameters().Length == 0 &&
                            m.ReturnType == _type));
            }
        }
    
        internal abstract class CustomFactoryProvider {
            public abstract object Get();
            internal static CustomFactoryProvider Create(Type type, IProvider provider) {
                var factoryType = typeof(CustomFactoryProvider<>).MakeGenericType(type);
                CustomFactoryProvider instance = (CustomFactoryProvider)Activator.CreateInstance(factoryType, provider)!;
                return instance;
            }
        }
    
        internal class CustomFactoryProvider<T> : CustomFactoryProvider {
            private readonly IProvider _customFactory;
    
            public CustomFactoryProvider(IProvider customFactory) {
                _customFactory = customFactory;
            }
    
            public override object Get() {
                IFactory<T> factory = (IFactory<T>)_customFactory.Get();
                return factory.Get();
            }
        }
    }
}