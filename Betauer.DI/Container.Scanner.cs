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
            ScanForServices(configuration);
        }

        internal void Scan(Type type, HashSet<Type>? stack) {
            if (!type.IsClass || type.IsAbstract) return;

            var attributes = Attribute.GetCustomAttributes(type);
            BaseProviderAttribute baseProviderAttribute = null;
            ConfigurationAttribute configurationAttribute = null;
            for (int i = 0; i < attributes.Length; i++) {
                switch (attributes[i]) {
                    case BaseProviderAttribute providerAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException(
                            $"Can't use {providerAttributeFound.FormatAttribute()} and {baseProviderAttribute.FormatAttribute()} in the same class: {type.GetTypeName()}");
                    case BaseProviderAttribute providerAttributeFound when configurationAttribute != null:
                        throw new InvalidAttributeException(
                            $"Can't use {providerAttributeFound.FormatAttribute()} and {configurationAttribute.FormatAttribute()} in the same class: {type.GetTypeName()}");
                    case BaseProviderAttribute providerAttributeFound:
                        baseProviderAttribute = providerAttributeFound;
                        break;
                    case ConfigurationAttribute when configurationAttribute != null:
                        throw new InvalidAttributeException($"Duplicate [Configuration] attribute found in class {type.FullName}");
                    case ConfigurationAttribute configurationAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException(
                            $"Can't use {configurationAttributeFound.FormatAttribute()} and {baseProviderAttribute.FormatAttribute()} in the same class: {type.GetTypeName()}");
                    case ConfigurationAttribute configurationAttributeFound:
                        configurationAttribute = configurationAttributeFound;
                        break;
                    case ScanAttribute scanAttributeFound when baseProviderAttribute != null:
                        throw new InvalidAttributeException(
                            $"Can't use {scanAttributeFound.FormatAttribute()} and {baseProviderAttribute.FormatAttribute()} in the same class: {type.GetTypeName()}");
                }
            }

            if (baseProviderAttribute is FactoryAttribute factoryAttribute) {
                RegisterCustomFactoryFromClass(type, factoryAttribute);
            } else if (baseProviderAttribute is ServiceAttribute serviceAttribute) {
                RegisterServiceFromClass(type, serviceAttribute);
            } else if (configurationAttribute != null) {
                var configuration = Activator.CreateInstance(type)!;
                ScanForScanAttributes(type, stack);
                ScanForServices(configuration);
                ScanForTemplateClass(configuration);
            } else if (type.HasAttribute<ScanAttribute>()) throw new InvalidAttributeException("[Scan] attributes are only valid with [Configuration]");
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

        private void ScanForServices(object configuration) {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            // The scan include Fields, but only the FactoryTemplateAttribute and ServiceTemplateAttribute are allowed on fields
            const MemberTypes memberFlags = MemberTypes.Method | MemberTypes.Property | MemberTypes.Field;

            // No need cache getters, this reflection scan is only done once
            var serviceGetters = configuration.GetType().GetGetters<ServiceAttribute>(memberFlags, bindingFlags);
            foreach (var getter in serviceGetters) {
                if (getter.MemberInfo.HasAttribute<FactoryAttribute>() || getter.MemberInfo.HasAttribute<MemberTemplateAttribute>()) throw new InvalidAttributeException("WRONG");
                RegisterServiceFromGetter(configuration, getter);
            }

            var factoryGetters = configuration.GetType().GetGetters<FactoryAttribute>(memberFlags, bindingFlags);
            foreach (var getter in factoryGetters) {
                if (getter.MemberInfo.HasAttribute<ServiceAttribute>() || getter.MemberInfo.HasAttribute<MemberTemplateAttribute>()) throw new InvalidAttributeException("WRONG");
                RegisterFactoryFromGetter(configuration, getter);
            }

            var templateGetters = configuration.GetType().GetGetters<MemberTemplateAttribute>(memberFlags, bindingFlags);
            foreach (var getter in templateGetters) {
                if (getter.MemberInfo.HasAttribute<FactoryAttribute>() || getter.MemberInfo.HasAttribute<ServiceAttribute>()) throw new InvalidAttributeException("WRONG");
                if (getter.GetterAttribute is ServiceTemplateAttribute) {
                    RegisterServiceTemplate(getter, configuration);
                } else if (getter.GetterAttribute is FactoryTemplateAttribute) {
                    RegisterFactoryTemplate(getter, configuration);
                }
            }
        }

        private void ScanForTemplateClass(object configuration) {
            var type = configuration.GetType();
            type.GetAttributes<ServiceTemplateClassAttribute>().ForEach(serviceTemplateAttribute => RegisterServiceTemplateClass(serviceTemplateAttribute, configuration));
            type.GetAttributes<FactoryTemplateClassAttribute>().ForEach(factoryTemplateAttribute => RegisterFactoryTemplateClass(factoryTemplateAttribute, configuration));
        }

        private void RegisterCustomFactoryFromClass(Type type, FactoryAttribute factoryAttribute) {
            var iFactoryInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFactory<>));
            if (!type.IsClass || iFactoryInterface == null) {
                throw new InvalidAttributeException(
                    $"Class {type.FullName} with factory attribute {factoryAttribute.FormatAttribute()} must be a class and implements IFactory<T>");
            }

            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterCustomFactory(type, factoryAttribute.Lifetime, Factory, factoryAttribute.Name, factoryAttribute.Primary);
        }

        private void RegisterServiceFromClass(Type type, ServiceAttribute serviceAttribute) {
            var registeredType = serviceAttribute.GetType().GetGenericArguments().FirstOrDefault() ?? type;
            var lazy = serviceAttribute is SingletonAttribute { Lazy: true };
            object Factory() => Activator.CreateInstance(type)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, serviceAttribute.Lifetime, Factory, serviceAttribute.Name, serviceAttribute.Primary,
                lazy);
        }

        private void RegisterServiceFromGetter(object configuration, IGetter<ServiceAttribute> getter) {
            var serviceAttribute = getter.GetterAttribute;
            var type = getter.Type;
            var registeredType = serviceAttribute.GetType().GetGenericArguments().FirstOrDefault() ?? type;
            var name = serviceAttribute.Name ?? getter.Name;
            var lazy = serviceAttribute is SingletonAttribute { Lazy: true };
            object Factory() => getter.GetValue(configuration)!;
            _builder.RegisterServiceAndAddFactory(registeredType, type, serviceAttribute.Lifetime, Factory, name, serviceAttribute.Primary, lazy);
        }

        private void RegisterFactoryFromGetter(object configuration, IGetter<FactoryAttribute> getter) {
            var attributeType = getter.GetterAttribute.GetType();
            if (!getter.Type.ImplementsInterface(typeof(IFactory<>)))
                throw new InvalidAttributeException($"Member {getter} with factory attribute {attributeType.FormatAttribute()} must implement IFactory<T>");

            var factoryAttribute = getter.GetterAttribute;
            var name = factoryAttribute.Name ?? getter.Name;
            object Factory() => getter.GetValue(configuration)!;
            _builder.RegisterCustomFactory(getter.Type, factoryAttribute.Lifetime, Factory, name, factoryAttribute.Primary);
        }

        private void RegisterServiceTemplateClass(ServiceTemplateClassAttribute templateClassAttribute, object configuration) {
            ServiceTemplate template = templateClassAttribute.CreateServiceTemplate(configuration);
            _builder.RegisterServiceAndAddFactory(template.RegisterType, template.ProviderType, template.Lifetime,
                template.Factory, template.Name, template.Primary, template.Lazy);
        }

        private void RegisterFactoryTemplateClass(FactoryTemplateClassAttribute templateClassAttribute, object configuration) {
            FactoryTemplate template = templateClassAttribute.CreateFactoryTemplate(configuration);
            _builder.RegisterCustomFactory(template.FactoryType, template.Lifetime, template.Factory, template.Name, template.Primary);
        }

        private void RegisterServiceTemplate(IGetter<MemberTemplateAttribute> getter, object configuration) {
            var templateAttribute = (ServiceTemplateAttribute)getter.GetterAttribute;
            ServiceTemplate template = templateAttribute.CreateServiceTemplate(configuration, getter);
            _builder.RegisterServiceAndAddFactory(template.RegisterType, template.ProviderType, template.Lifetime,
                template.Factory, template.Name, template.Primary, template.Lazy);
        }

        private void RegisterFactoryTemplate(IGetter<MemberTemplateAttribute> getter, object configuration) {
            var templateAttribute = (FactoryTemplateAttribute)getter.GetterAttribute;
            FactoryTemplate template = templateAttribute.CreateFactoryTemplate(configuration, getter);
            _builder.RegisterCustomFactory(template.FactoryType, template.Lifetime, template.Factory, template.Name, template.Primary);
        }
    }
}