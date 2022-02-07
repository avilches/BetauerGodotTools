using System;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property)]
    public class InjectAttribute : Attribute {
        public bool Nullable { get; set; } = false;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class OnReadyAttribute : Attribute {
        public bool Nullable { get; set; } = false;
        public readonly string? Path;

        public OnReadyAttribute() {
        }

        public OnReadyAttribute(string path) {
            Path = path;
        }
    }

    public abstract class InjectException : Exception {
        public object Instance { get; }

        public InjectException(string message, object instance) : base(message) {
            Instance = instance;
        }
    }

    public class InjectFieldException : InjectException {
        public FieldInfo FieldInfo { get; }

        public InjectFieldException(FieldInfo fieldInfo, object instance, string message) : base(message, instance) {
            FieldInfo = fieldInfo;
        }
    }

    public abstract class OnReadyException : Exception {
        public object Instance { get; }

        public OnReadyException(string message, object instance) : base(message) {
            Instance = instance;
        }
    }

    public class OnReadyFieldException : OnReadyException {
        public string Name { get; }

        public OnReadyFieldException(string name, object instance, string message) : base(message, instance) {
            Name = name;
        }
    }


    public class Injector {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Injector));
        private readonly Container _container;

        public Injector(Container container) {
            _container = container;
        }

        public void InjectAllFields(object target, ResolveContext context) {
            if (target is Delegate) return;
            _logger.Debug("Injecting fields in " + target.GetType() + ": " + target.GetHashCode().ToString("X"));
            var fields = target.GetType()
                .GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields) {
                if (!(Attribute.GetCustomAttribute(field, typeof(InjectAttribute), false) is InjectAttribute inject))
                    continue;
                InjectField(target, context, field, inject.Nullable);
            }
            // var properties = target.GetType()
            //     .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            // foreach (var field in properties) {
            //     if (!(Attribute.GetCustomAttribute(field, typeof(InjectAttribute), false) is InjectAttribute inject))
            //         continue;
            //     InjectField(target, context, field, inject.Nullable);
            // }
        }

        private static bool IsZeroParametersFunction(Type type) =>
            type.IsGenericType &&
            type.GetGenericTypeDefinition() == typeof(Func<>) &&
            type.GetGenericArguments().Length == 1;

        private void InjectField(object target, ResolveContext context, FieldInfo field, bool nullable) {
            if (field.GetValue(target) != null) {
                // Ignore the already defined values
                // TODO: test
                return;
            }
            if (_container.Contains(field.FieldType)) {
                // There is a provider for the field type
                _logger.Debug("Injecting field " + field.Name + " " + field.FieldType.Name + " in " + target.GetType() +
                              "(" + target.GetHashCode() + ")");
                var service = _container.Resolve(field.FieldType, context);
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
                    throw new InjectFieldException(field, target,
                        "Injectable property [" + field.FieldType.Name + " " + field.Name +
                        "] not found while injecting fields in " + target.GetType().Name);
                }
            }
        }

        public void LoadOnReadyNodes(Node target) {
            var fields = target.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Public |
                                                    BindingFlags.Instance);
            var properties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Public |
                                                    BindingFlags.Instance);
            foreach (var property in fields) {
                if (!(Attribute.GetCustomAttribute(property, typeof(OnReadyAttribute), false) is OnReadyAttribute
                        onReady))
                    continue;
                LoadOnReadyField(target, onReady, new Setter(property.FieldType, property.Name, property.SetValue));
            }
            foreach (var property in properties) {
                if (!(Attribute.GetCustomAttribute(property, typeof(OnReadyAttribute), false) is OnReadyAttribute
                        onReady))
                    continue;
                LoadOnReadyField(target, onReady, new Setter(property.PropertyType, property.Name, property.SetValue));
            }
        }

        private class Setter {
            internal Type Type;
            internal string Name;
            internal Action<object, object> SetValue;

            public Setter(Type type, string name, Action<object, object> setValue) {
                Type = type;
                Name = name;
                SetValue = setValue;
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
    }
}