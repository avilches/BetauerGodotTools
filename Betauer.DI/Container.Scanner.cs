using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.Exceptions;
using Betauer.DI.Factory;
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
    
            if (type.GetAttribute<FactoryAttribute>() is FactoryAttribute factoryAttribute) {
                if (type.HasAttribute<ConfigurationAttribute>()) throw new InvalidAttributeException("Can't use [Factory] and [Configuration] in the same class");
                if (type.HasAttribute<ServiceAttribute>()) throw new InvalidAttributeException("Can't use [Factory] and [Service] in the same class");
                if (type.HasAttribute<ScanAttribute>()) throw new InvalidAttributeException("Can't use [Factory] and [Scan] in the same class");
                RegisterCustomFactoryFromClass(type, factoryAttribute);
                
            } else if (type.GetAttribute<ServiceAttribute>() is ServiceAttribute serviceAttr) {
                if (type.HasAttribute<ConfigurationAttribute>()) throw new InvalidAttributeException("Can't use [Service] and [Configuration] in the same class");
                if (type.HasAttribute<ScanAttribute>()) throw new InvalidAttributeException("Can't use [Service] and [Scan] in the same class");
                RegisterServiceFromClass(type, serviceAttr);
                
            } else if (type.GetAttribute<ConfigurationAttribute>() is ConfigurationAttribute configurationAttribute) {
                var configuration = Activator.CreateInstance(type);
                ScanServicesFromConfigurationInstance(configuration!);
                // Look up for [Scan(typeof(...)]
                ScanForScanAttributes(type, stack);
            } else if (type.HasAttribute<ScanAttribute>()) throw new InvalidAttributeException("[Scan] attributes are only valid with [Configuration]");
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
            // var genericType = iFactoryInterface.GetGenericArguments()[0];
            var primary = factoryAttribute.Primary || type.HasAttribute<PrimaryAttribute>();
            var lazy = factoryAttribute.Lazy || type.HasAttribute<LazyAttribute>();
            var lifetime = factoryAttribute.Lifetime;
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterCustomFactory(type, Factory, factoryAttribute.Name, lifetime, primary, lazy);
        }
    
        private void RegisterCustomFactoryFromGetter(object configuration, IGetter<FactoryAttribute> getter) {
            if (!getter.Type.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidAttributeException("Member " + getter + " with [Factory] attribute must implement IFactory<T>");
            }
            var factoryAttribute = getter.GetterAttribute;
            var primary = factoryAttribute.Primary || getter.MemberInfo.HasAttribute<PrimaryAttribute>();
            var name = factoryAttribute.Name ?? getter.Name;
            var lazy = factoryAttribute.Lazy || getter.MemberInfo.HasAttribute<LazyAttribute>();
            var lifetime = factoryAttribute.Lifetime;
            object Factory() => getter.GetValue(configuration)!;
            _builder.RegisterCustomFactory(getter.Type, Factory, name, lifetime, primary, lazy);
        }
    }
}