using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.Attributes;
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
            BaseProviderAttribute baseProviderAttribute = null; 
            ConfigurationAttribute configurationAttribute = null;
            for (int i = 0; i < attributes.Length; i++) {
                switch (attributes[i]) {
                    case BaseProviderAttribute providerAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(providerAttributeFound)}] and [{FormatAttribute(baseProviderAttribute)}] in the same class: {type.Name}");
                    case BaseProviderAttribute providerAttributeFound when configurationAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(providerAttributeFound)}] and [{FormatAttribute(configurationAttribute)}] in the same class: {type.Name}");
                    case BaseProviderAttribute providerAttributeFound:
                        baseProviderAttribute = providerAttributeFound;
                        break;
                    case ConfigurationAttribute when configurationAttribute != null:
                        throw new InvalidAttributeException($"Duplicate [Configuration] attribute found in class {type.FullName}");
                    case ConfigurationAttribute configurationAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(configurationAttributeFound)}] and [{FormatAttribute(baseProviderAttribute)}] in the same class: {type.Name}");
                    case ConfigurationAttribute configurationAttributeFound:
                        configurationAttribute = configurationAttributeFound;
                        break;
                    case ScanAttribute scanAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException($"Can't use [{FormatAttribute(scanAttributeFound)}] and [{FormatAttribute(baseProviderAttribute)}] in the same class: {type.Name}");
                }
            }
    
            if (baseProviderAttribute is FactoryAttribute factoryAttribute) {
                RegisterCustomFactoryFromClass(type, factoryAttribute);
                
            } else if (baseProviderAttribute is ServiceAttribute serviceAttr) {
                RegisterServiceFromClass(type, serviceAttr);
                
            } else if (configurationAttribute != null) {
                var configuration = Activator.CreateInstance(type);
                ScanServicesFromConfigurationInstance(configuration!);
                ScanForScanAttributes(type, stack);
                
            } else if (type.HasAttribute<ScanAttribute>()) throw new InvalidAttributeException("[Scan] attributes are only valid with [Configuration]");
        }

        private void RegisterCustomFactoryFromClass(Type type, FactoryAttribute factoryAttribute) {
            var iFactoryInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFactory<>));
            if (!type.IsClass || iFactoryInterface == null) {
                throw new InvalidAttributeException($"Class {type.FullName} with factory attribute must be a class and implement IFactory<T>");
            }
            // var genericType = iFactoryInterface.GetGenericArguments()[0];
            var primary = factoryAttribute.Primary;
            var lifetime = factoryAttribute.Lifetime;
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterCustomFactory(type, lifetime, Factory, factoryAttribute.Name, primary);
        }

        private void RegisterServiceFromClass(Type type, ServiceAttribute serviceAttr) {
            var registeredType = serviceAttr.GetType().GetGenericArguments().FirstOrDefault() ?? type;
            var name = serviceAttr.Name;
            var primary = serviceAttr.Primary;
            var lazy = serviceAttr is SingletonAttribute { Lazy: true };
            var lifetime = serviceAttr.Lifetime;
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, lifetime, Factory, name, primary, lazy);
        }

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
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            // The scan include Fields, but only the FactoryTemplateAttribute and ServiceTemplateAttribute are allowed on fields
            const MemberTypes memberFlags = MemberTypes.Method | MemberTypes.Property | MemberTypes.Field;
            
            // No need cache getters, this reflection scan is only done once
            var serviceGetters = configuration.GetType().GetGetters<BaseServiceAttribute>(memberFlags, bindingFlags);
            foreach (var getter in serviceGetters) RegisterServiceFromGetter(configuration, getter);
            
            var factoryGetters = configuration.GetType().GetGetters<BaseFactoryAttribute>(memberFlags, bindingFlags);
            foreach (var getter in factoryGetters) RegisterFactoryFromGetter(configuration, getter);
        }

        private void RegisterServiceFromGetter(object configuration, IGetter<BaseServiceAttribute> getter) {
            
            var attributeType = getter.GetterAttribute.GetType();
            if (attributeType.ImplementsInterface(typeof(ServiceTemplateAttribute))) {
                if (getter.GetValue(configuration) != null)
                    throw new InvalidAttributeException($"Member {getter} with factory attribute is a field and must be null or not specified");
                ServiceTemplateAttribute templateAttribute = (ServiceTemplateAttribute)getter.GetterAttribute;
                ProviderTemplate template = templateAttribute.CreateProviderTemplate((FieldInfo)getter.MemberInfo);
                _builder.RegisterServiceAndAddFactory(template.RegisterType, template.ProviderType, template.Lifetime, template.Factory, template.Name, template.Primary);

            } else if (attributeType.ImplementsInterface(typeof(ServiceAttribute))) {
                ServiceAttribute serviceAttr = (ServiceAttribute)getter.GetterAttribute;
                var type = getter.Type;
                var registeredType = serviceAttr.GetType().GetGenericArguments().FirstOrDefault() ?? type;
                var name = serviceAttr.Name ?? getter.Name;
                var primary = serviceAttr.Primary;
                var lazy = serviceAttr is SingletonAttribute { Lazy: true };
                var lifetime = serviceAttr.Lifetime;
                object Factory() => getter.GetValue(configuration)!;
                _builder.RegisterServiceAndAddFactory(registeredType, type, lifetime, Factory, name, primary, lazy);
            }
        }

        private void RegisterFactoryFromGetter(object configuration, IGetter<BaseFactoryAttribute> getter) {
            if (!getter.Type.ImplementsInterface(typeof(IFactory<>))) {
                throw new InvalidAttributeException($"Member {getter} with factory attribute must implement IFactory<T>");
            }
            var attributeType = getter.GetterAttribute.GetType();
            if (attributeType.ImplementsInterface(typeof(FactoryAttribute))) {
                FactoryAttribute factoryAttribute = (FactoryAttribute)getter.GetterAttribute;
                var name = factoryAttribute.Name ?? getter.Name;
                object Factory() => getter.GetValue(configuration)!;
                _builder.RegisterCustomFactory(getter.Type, factoryAttribute.Lifetime, Factory, name, factoryAttribute.Primary);
                
            } else if (attributeType.ImplementsInterface(typeof(FactoryTemplateAttribute))) {
                if (getter.GetValue(configuration) != null)
                    throw new InvalidAttributeException($"Member {getter} with factory attribute is a field and must be null or not specified");
                FactoryTemplateAttribute templateAttribute = (FactoryTemplateAttribute)getter.GetterAttribute;
                FactoryTemplate template = templateAttribute.CreateFactoryTemplate((FieldInfo)getter.MemberInfo);
                _builder.RegisterCustomFactory(template.FactoryType, template.Lifetime, template.Factory, template.Name, template.Primary);
            }
        }
    }
}