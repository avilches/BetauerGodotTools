using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.Tools.Logging;
using Betauer.Tools.FastReflection;

namespace Betauer.DI;

public partial class Container {
    public class Scanner {
        private static readonly Logger Logger = LoggerFactory.GetLogger<Scanner>();
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
                        $"Can't use {classAttribute.FormatAttribute()} and {typeof(ConfigurationAttribute).FormatAttribute()} attribute in the same class. Type: {type}");
                }
                Logger.Debug("{0} class {1}", typeof(ConfigurationAttribute).FormatAttribute(), type.GetTypeName());
                var configuration = Activator.CreateInstance(type)!;
                ScanConfigurationScanAttributes(type, attributes, stack);
                ScanConfigurationAttributes(configuration, attributes);
                ScanConfigurationMembers(configuration);
            } else {
                if (attributes.OfType<IConfigurationClassAttribute>().FirstOrDefault() is Attribute configurationClassAttribute) {
                    throw new InvalidAttributeException(
                        $"The attribute {configurationClassAttribute.FormatAttribute()} needs a {typeof(ConfigurationAttribute).FormatAttribute()} attribute in the same class. Type: {type}");
                }

                if (type.GetAttributes<ScanAttribute>().FirstOrDefault() is Attribute scanAttribute) {
                    throw new InvalidAttributeException(
                        $"The attribute {scanAttribute.FormatAttribute()} needs a {typeof(ConfigurationAttribute).FormatAttribute()} attribute in the same class: Type: {type}");
                }
                ScanClassHeaderAttributes(type, attributes);
            }
        }

        private void ScanConfigurationScanAttributes(Type type, IEnumerable<Attribute> attributes, HashSet<Type>? stack) {
            foreach (var scanAttribute in attributes.OfType<ScanAttribute>()) {
                Logger.Debug("{0} class {1}", scanAttribute.FormatAttribute(), type.GetTypeName());
                stack ??= new HashSet<Type>();
                stack.Add(type);
                scanAttribute.GetType().GetGenericArguments()
                    .Where(typeToImport => !stack.Contains(typeToImport))
                    .ForEach(typeToImport => Scan(typeToImport, stack));
            }
        }

        private void ScanConfigurationAttributes(object configuration, IEnumerable<Attribute> attributes) {
            foreach (var configurationClassAttributes in attributes.OfType<IConfigurationClassAttribute>()) {
                Logger.Debug("{0} class {1}", (configurationClassAttributes as Attribute).FormatAttribute(),
                    configuration.GetType().GetTypeName());
                configurationClassAttributes.Apply(configuration, _builder);
            }
        }

        private void ScanConfigurationMembers(object configuration) {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            const MemberTypes memberFlags = MemberTypes.Method | MemberTypes.Property | MemberTypes.Field;

            var configurationMemberAttributes = configuration.GetType().GetGetters<IConfigurationMemberAttribute>(memberFlags, bindingFlags);
            foreach (var getter in configurationMemberAttributes) {
                Logger.Debug("class {0}: {1}", configuration.GetType().GetTypeName(), getter);
                getter.GetterAttribute.Apply(configuration, getter, _builder);
            }
        }

        private void ScanClassHeaderAttributes(Type type, IEnumerable<Attribute> attributes) {
            foreach (var classAttribute in attributes.OfType<IClassAttribute>()) {
                Logger.Debug("{0} class {1}", (classAttribute as Attribute)!.FormatAttribute(), type.GetTypeName());
                classAttribute.Apply(type, _builder);
            }
        }
    }
}