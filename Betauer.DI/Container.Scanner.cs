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
            
            var attributes = Attribute.GetCustomAttributes(type);
            BaseProviderAttribute baseProviderAttribute = null; 
            ConfigurationAttribute configurationAttribute = null;
            for (int i = 0; i < attributes.Length; i++) {
                switch (attributes[i]) {
                    case BaseProviderAttribute providerAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException($"Can't use {FormatAttribute(providerAttributeFound)} and {FormatAttribute(baseProviderAttribute)} in the same class: {type.Name}");
                    case BaseProviderAttribute providerAttributeFound when configurationAttribute != null:
                        throw new InvalidAttributeException($"Can't use {FormatAttribute(providerAttributeFound)} and {FormatAttribute(configurationAttribute)} in the same class: {type.Name}");
                    case BaseProviderAttribute providerAttributeFound:
                        baseProviderAttribute = providerAttributeFound;
                        break;
                    case ConfigurationAttribute when configurationAttribute != null:
                        throw new InvalidAttributeException($"Duplicate [Configuration] attribute found in class {type.FullName}");
                    case ConfigurationAttribute configurationAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException($"Can't use {FormatAttribute(configurationAttributeFound)} and {FormatAttribute(baseProviderAttribute)} in the same class: {type.Name}");
                    case ConfigurationAttribute configurationAttributeFound:
                        configurationAttribute = configurationAttributeFound;
                        break;
                    case ScanAttribute scanAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException($"Can't use {FormatAttribute(scanAttributeFound)} and {FormatAttribute(baseProviderAttribute)} in the same class: {type.Name}");
                }
            }
    
            if (baseProviderAttribute is FactoryAttribute factoryAttribute) {
                RegisterCustomFactoryFromClass(type, factoryAttribute);
                
            } else if (baseProviderAttribute is ServiceAttribute serviceAttribute) {
                RegisterServiceFromClass(type, serviceAttribute);
                
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
                throw new InvalidAttributeException($"Class {type.FullName} with factory attribute {FormatAttribute(factoryAttribute)} must be a class and implements IFactory<T>");
            }
            // var genericType = iFactoryInterface.GetGenericArguments()[0];
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterCustomFactory(type, factoryAttribute.Lifetime, Factory, factoryAttribute.Name, factoryAttribute.Primary);
        }

        private void RegisterServiceFromClass(Type type, ServiceAttribute serviceAttribute) {
            var registeredType = serviceAttribute.GetType().GetGenericArguments().FirstOrDefault() ?? type;
            var lazy = serviceAttribute is SingletonAttribute { Lazy: true };
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, serviceAttribute.Lifetime, Factory, serviceAttribute.Name, serviceAttribute.Primary, lazy);
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
            if (attributeType.IsAssignableTo(typeof(ServiceAttribute))) {
                ServiceAttribute serviceAttribute = (ServiceAttribute)getter.GetterAttribute;
                var type = getter.Type;
                var registeredType = serviceAttribute.GetType().GetGenericArguments().FirstOrDefault() ?? type;
                var name = serviceAttribute.Name ?? getter.Name;
                var lazy = serviceAttribute is SingletonAttribute { Lazy: true };
                object Factory() => getter.GetValue(configuration)!;
                _builder.RegisterServiceAndAddFactory(registeredType, type, serviceAttribute.Lifetime, Factory, name, serviceAttribute.Primary, lazy);
                
            } else if (attributeType.IsAssignableTo(typeof(ServiceTemplateAttribute))) {
                if (getter.GetValue(configuration) != null) throw new InvalidAttributeException($"Member {getter} with factory attribute {FormatAttribute(attributeType)} is a field and must be null or not specified");
                ServiceTemplateAttribute templateAttribute = (ServiceTemplateAttribute)getter.GetterAttribute;
                ProviderTemplate template = templateAttribute.CreateProviderTemplate((FieldInfo)getter.MemberInfo);
                _builder.RegisterServiceAndAddFactory(template.RegisterType, template.ProviderType, template.Lifetime, template.Factory, template.Name, template.Primary);

            } else {
                throw new InvalidAttributeException($"Member {getter} with unknown attribute {FormatAttribute(attributeType)}");
            }
        }

        private void RegisterFactoryFromGetter(object configuration, IGetter<BaseFactoryAttribute> getter) {
            var attributeType = getter.GetterAttribute.GetType();
            if (!getter.Type.ImplementsInterface(typeof(IFactory<>))) throw new InvalidAttributeException($"Member {getter} with factory attribute {FormatAttribute(attributeType)} must implement IFactory<T>");
            
            if (attributeType.IsAssignableTo(typeof(FactoryAttribute))) {
                FactoryAttribute factoryAttribute = (FactoryAttribute)getter.GetterAttribute;
                var name = factoryAttribute.Name ?? getter.Name;
                object Factory() => getter.GetValue(configuration)!;
                _builder.RegisterCustomFactory(getter.Type, factoryAttribute.Lifetime, Factory, name, factoryAttribute.Primary);
                
            } else if (attributeType.IsAssignableTo(typeof(FactoryTemplateAttribute))) {
                if (getter.GetValue(configuration) != null) throw new InvalidAttributeException($"Member {getter} with factory attribute {FormatAttribute(attributeType)} is a field and must be null or not specified");
                FactoryTemplateAttribute templateAttribute = (FactoryTemplateAttribute)getter.GetterAttribute;
                FactoryTemplate template = templateAttribute.CreateFactoryTemplate((FieldInfo)getter.MemberInfo);
                _builder.RegisterCustomFactory(template.FactoryType, template.Lifetime, template.Factory, template.Name, template.Primary);
                
            } else {
                throw new InvalidAttributeException($"Member {getter} with unknown attribute {FormatAttribute(attributeType)}");
            }
        }

        private static string FormatAttribute(Attribute att) {
            return FormatAttribute(att.GetType());
        }

        private static string FormatAttribute(Type type) {
            var name = type.Name;
            return $"[{name.Remove(name.LastIndexOf("Attribute", StringComparison.Ordinal))}]";
        }
    }
}