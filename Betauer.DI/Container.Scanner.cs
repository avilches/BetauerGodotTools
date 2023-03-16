using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.Exceptions;
using Betauer.DI.Factory;
using Betauer.Tools.Reflection;

namespace Betauer.DI;

public partial class Container {
    private class Scanner {
        private readonly Builder _builder;
        private readonly Container _container;

        internal Scanner(Builder builder, Container container) {
            _builder = builder;
            _container = container;
        }

        internal void ScanConfiguration(object configuration) {
            ScanForScanAttributes(configuration.GetType(), null);
            ScanServicesFromConfigurationInstance(configuration);
        }

        internal void Scan(Type type, HashSet<Type>? stack) {
            if (!type.IsClass || type.IsAbstract) return;
            
            string FormatAttribute(Attribute att) {
                var name = att.GetType().Name;
                return name.Remove(name.LastIndexOf("Attribute", StringComparison.Ordinal));
            }

            var attributes = Attribute.GetCustomAttributes(type);
            BaseServiceAttribute baseServiceAttribute = null; 
            ConfigurationAttribute configurationAttribute = null;
            for (int i = 0; i < attributes.Length; i++) {
                switch (attributes[i]) {
                    case BaseServiceAttribute serviceAttributeFound when baseServiceAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(serviceAttributeFound)}] and [{FormatAttribute(baseServiceAttribute)}] in the same class: {type.Name}");
                    case BaseServiceAttribute serviceAttributeFound when configurationAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(serviceAttributeFound)}] and [{FormatAttribute(configurationAttribute)}] in the same class: {type.Name}");
                    case BaseServiceAttribute serviceAttributeFound:
                        baseServiceAttribute = serviceAttributeFound;
                        break;
                    case ConfigurationAttribute when configurationAttribute != null:
                        throw new InvalidAttributeException($"Duplicate [Configuration] attribute found in class {type.FullName}");
                    case ConfigurationAttribute configurationAttributeFound when baseServiceAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(configurationAttributeFound)}] and [{FormatAttribute(baseServiceAttribute)}] in the same class: {type.Name}");
                    case ConfigurationAttribute configurationAttributeFound:
                        configurationAttribute = configurationAttributeFound;
                        break;
                    case ScanAttribute scanAttributeFound when baseServiceAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(scanAttributeFound)}] and [{FormatAttribute(baseServiceAttribute)}] in the same class: {type.Name}");
                }
            }
    
            if (baseServiceAttribute is FactoryAttribute factoryAttribute) {
                RegisterCustomFactoryFromClass(type, factoryAttribute);
                
            } else if (baseServiceAttribute is ServiceAttribute serviceAttr) {
                RegisterServiceFromClass(type, serviceAttr);
                
            } else if (configurationAttribute != null) {
                var configuration = Activator.CreateInstance(type);
                ScanServicesFromConfigurationInstance(configuration!);
                ScanForScanAttributes(type, stack);
            } else if (type.HasAttribute<ScanAttribute>()) throw new InvalidAttributeException("[Scan] attributes are only valid with [Configuration]");
        }
    
        private const BindingFlags ScanMemberFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    
        private void ScanForScanAttributes(Type type, HashSet<Type>? stack) {
            foreach (var scanAttribute in type.GetAttributes<ScanAttribute>()) {
                stack ??= new HashSet<Type>();
                stack.Add(type);
                scanAttribute.GetType().GetGenericArguments()
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
            var registeredType = serviceAttr.GetType().GetGenericArguments().FirstOrDefault() ?? type;
            var name = serviceAttr.Name;
            var primary = serviceAttr.Primary;
            var lazy = serviceAttr is SingletonAttribute { Lazy: true };
            var lifetime = serviceAttr.Lifetime;
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, Factory, lifetime, name, primary, lazy);
        }
    
        private void RegisterServiceFromGetter(object configuration, IGetter<ServiceAttribute> getter) {
            var serviceAttr = getter.GetterAttribute;
            var type = getter.Type;
            var registeredType = serviceAttr.GetType().GetGenericArguments().FirstOrDefault() ?? type;
            var name = serviceAttr.Name ?? getter.Name;
            var primary = serviceAttr.Primary;
            var lazy = serviceAttr is SingletonAttribute { Lazy: true };
            var lifetime = serviceAttr.Lifetime;
            object Factory() => getter.GetValue(configuration)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, Factory, lifetime, name, primary, lazy);
        }
    
        private void RegisterCustomFactoryFromClass(Type type, FactoryAttribute factoryAttribute) {
            var iFactoryInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFactory<>));
            if (iFactoryInterface == null) {
                throw new InvalidAttributeException($"Class {type.FullName} with [SingletonFactory] attribute must implement IFactory<T>");
            }
            // var genericType = iFactoryInterface.GetGenericArguments()[0];
            var primary = factoryAttribute.Primary;
            var lifetime = factoryAttribute.Lifetime;
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterCustomFactory(type, Factory, factoryAttribute.Name, lifetime, primary, true);
        }
    
        private void RegisterCustomFactoryFromGetter(object configuration, IGetter<FactoryAttribute> getter) {
            if (!getter.Type.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidAttributeException("Member " + getter + " with [SingletonFactory] attribute must implement IFactory<T>");
            }
            var factoryAttribute = getter.GetterAttribute;
            var primary = factoryAttribute.Primary;
            var name = factoryAttribute.Name ?? getter.Name;
            var lifetime = factoryAttribute.Lifetime;
            object Factory() => getter.GetValue(configuration)!;
            _builder.RegisterCustomFactory(getter.Type, Factory, name, lifetime, primary, true);
        }
    }
}