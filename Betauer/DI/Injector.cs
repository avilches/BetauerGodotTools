using System;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    public class Injector {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Injector));
        private readonly Container _container;

        private const BindingFlags InjectFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        private const BindingFlags OnReadyFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public Injector(Container container) {
            _container = container;
        }

        public void InjectAllFields(object target, ResolveContext context) {
            if (target is Delegate) return;
            _logger.Debug("Injecting fields in " + target.GetType() + ": " + target.GetHashCode().ToString("X"));
            var fields = target.GetType().GetFields(InjectFlags);

            foreach (var field in fields) {
                if (!(Attribute.GetCustomAttribute(field, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                InjectField(target, context, new Setter(field), inject.Nullable);
            }
            var properties = target.GetType().GetProperties(InjectFlags);

            foreach (var property in properties) {
                if (!(Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                InjectField(target, context, new Setter(property), inject.Nullable);
            }
        }

        private void InjectField(object target, ResolveContext context, Setter field, bool nullable) {
            if (field.GetValue(target) != null) {
                // Ignore the already defined values
                // TODO: test
                return;
            }
            if (_container.Contains(field.Type)) {
                // There is a provider for the field type
                _logger.Debug("Injecting field " + field.Name + " " + field.Type.Name + " in " + target.GetType() +
                              "(" + target.GetHashCode() + ")");
                var service = _container.Resolve(field.Type, context);
                field.SetValue(target, service);
            } else {
                /*
                if (IsZeroParametersFunction(field.FieldType)) {
                    var outType = field.FieldType.GetGenericArguments()[0];
                    // [Inject] private Func<TOut>
                    // Find a Func<TIn, TOut> where TIn is the current instance Type or any of its base classes
                    Delegate function = _container.ResolveAndCreateClosure(target, outType);
                    // TODO: test when function is not found, it should throw InjectFieldException
                    field.SetValue(target, function);
                    return;
                }
                */

                if (!nullable) {
                    throw new InjectFieldException(field.Name, target,
                        "Injectable property [" + field.Type.Name + " " + field.Name +
                        "] not found while injecting fields in " + target.GetType().Name);
                }
            }
        }

        public void LoadOnReadyNodes(Node target) {
            foreach (var field in target.GetType().GetFields(OnReadyFlags)) {
                if (Attribute.GetCustomAttribute(field, typeof(OnReadyAttribute), false) is OnReadyAttribute
                    onReady) {
                    LoadOnReadyField(target, onReady, new Setter(field));
                }
            }

            foreach (var property in target.GetType().GetProperties(OnReadyFlags)) {
                if (Attribute.GetCustomAttribute(property,
                        typeof(OnReadyAttribute), false) is OnReadyAttribute onReady) {
                    LoadOnReadyField(target, onReady, new Setter(property));
                }
            }
        }

        private void LoadOnReadyField(Node target, OnReadyAttribute onReady, Setter property) {
            if (onReady.Path != null) {
                var node = target.GetNode(onReady.Path);
                var fieldInfo = "[OnReady(\"" + onReady.Path + "\")] " + property.Type.Name + " " +
                                property.Name;

                if (node == null) {
                    if (onReady.Nullable) return;
                    throw new OnReadyFieldException(property.Name, target,
                        "Path returns a null value for field " + fieldInfo + ", class " + target.GetType().Name);
                }
                if (!property.Type.IsInstanceOfType(node)) {
                    throw new OnReadyFieldException(property.Name, target,
                        "Path returns an incompatible type " + node.GetType().Name + " for field " + fieldInfo +
                        ", class " + target.GetType().Name);
                }
                property.SetValue(target, node);
            } else {
                /*
                Delegate func =
                    _container.ResolveCompatibleFunction(target.GetType(), property.FieldType);
                var value = func.DynamicInvoke(target);
                property.SetValue(target, value);
            */
            }
        }

        private static bool IsZeroParametersFunction(Type type) =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Func<>) &&
            type.GetGenericArguments().Length == 1;

        private class Setter {
            internal Type Type;
            internal string Name;
            internal Action<object, object> SetValue;
            internal Func<object, object> GetValue;

            public Setter(PropertyInfo property) {
                Type = property.PropertyType;
                Name = property.Name;
                SetValue = property.SetValue;
                GetValue = property.GetValue;
            }

            public Setter(FieldInfo property) {
                Type = property.FieldType;
                Name = property.Name;
                SetValue = property.SetValue;
                GetValue = property.GetValue;
            }
        }
    }
}