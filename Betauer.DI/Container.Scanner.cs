using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.Tools.Logging;
using Betauer.Tools.Reflection;

namespace Betauer.DI;

public partial class Container {
    public class Scanner {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Scanner));
        private readonly Builder _builder;
        private readonly Container _container;

        internal Scanner(Builder builder, Container container) {
            _builder = builder;
            _container = container;
        }

        internal void ScanConfiguration(object configuration) {
            var attributes = Attribute.GetCustomAttributes(configuration.GetType());
            ScanConfigurationScanAttributes(configuration.GetType(), attributes, null);
            ScanConfigurationAttributes(configuration, attributes);
            ScanConfigurationMembers(configuration);
        }

        internal void Scan(Type type, HashSet<Type>? stack) {
            if (!type.IsClass || type.IsAbstract) return;

            var attributes = Attribute.GetCustomAttributes(type);
            if (type.HasAttribute<ConfigurationAttribute>()) {
                if (attributes.OfType<IClassAttribute>().FirstOrDefault() is Attribute classAttribute) {
                    throw new InvalidAttributeException(
                        $"Can't use {classAttribute.FormatAttribute()} and {typeof(ConfigurationAttribute).FormatAttribute()} in the same class: {type.GetTypeName()}");
                }
                Logger.Debug($"{typeof(ConfigurationAttribute).FormatAttribute()} class {type.GetTypeName()}");
                var configuration = Activator.CreateInstance(type)!;
                ScanConfigurationScanAttributes(type, attributes, stack);
                ScanConfigurationAttributes(configuration, attributes);
                ScanConfigurationMembers(configuration);
            } else {
                if (attributes.OfType<IConfigurationClassAttribute>().FirstOrDefault() is Attribute configurationClassAttribute) {
                    throw new InvalidAttributeException(
                        $"Can't use {configurationClassAttribute.FormatAttribute()} without {typeof(ConfigurationAttribute).FormatAttribute()} in the same class: {type.GetTypeName()}");
                }

                if (type.GetAttributes<ScanAttribute>().FirstOrDefault() is Attribute scanAttribute) {
                    throw new InvalidAttributeException(
                        $"Can't use {scanAttribute.FormatAttribute()} without {typeof(ConfigurationAttribute).FormatAttribute()} in the same class: {type.GetTypeName()}");
                }
                ScanClassHeaderAttributes(type, attributes);
            }
        }

        private void ScanConfigurationScanAttributes(Type type, IEnumerable<Attribute> attributes, HashSet<Type>? stack) {
            foreach (var scanAttribute in attributes.OfType<ScanAttribute>()) {
                Logger.Debug($"{scanAttribute.FormatAttribute()} class {type.GetTypeName()}");
                stack ??= new HashSet<Type>();
                stack.Add(type);
                scanAttribute.GetType().GetGenericArguments()
                    .Where(typeToImport => !stack.Contains(typeToImport))
                    .ForEach(typeToImport => Scan(typeToImport, stack));
            }
        }

        private void ScanConfigurationAttributes(object configuration, IEnumerable<Attribute> attributes) {
            foreach (var configurationClassAttributes in attributes.OfType<IConfigurationClassAttribute>()) {
                Logger.Debug($"{(configurationClassAttributes as Attribute).FormatAttribute()} class {configuration.GetType().GetTypeName()}");
                configurationClassAttributes.CreateProvider(configuration, _builder);
            }
        }

        private void ScanConfigurationMembers(object configuration) {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            const MemberTypes memberFlags = MemberTypes.Method | MemberTypes.Property | MemberTypes.Field;

            var configurationMemberAttributes = configuration.GetType().GetGetters<IConfigurationMemberAttribute>(memberFlags, bindingFlags);
            foreach (var getter in configurationMemberAttributes) {
                Logger.Debug($"class {configuration.GetType().GetTypeName()}: {getter}");
                getter.GetterAttribute.CreateProvider(configuration, getter, _builder);
            }
        }

        private void ScanClassHeaderAttributes(Type type, IEnumerable<Attribute> attributes) {
            foreach (var classAttribute in attributes.OfType<IClassAttribute>()) {
                Logger.Debug($"{(classAttribute as Attribute).FormatAttribute()} class {type.GetTypeName()}");
                classAttribute.CreateProvider(type, _builder);
            }
        }
    }
}