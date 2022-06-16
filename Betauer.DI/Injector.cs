using System;
using System.Reflection;
using Godot;

namespace Betauer.DI {
    public class Setter {
        public readonly Type Type;
        public readonly string Name;
        public readonly Action<object, object> SetValue;
        public readonly Func<object, object> GetValue;

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
                if (Attribute.GetCustomAttribute(field, typeof(InjectAttribute), false) is InjectAttribute inject) {
                    InjectField(target, context, new Setter(field), inject.Nullable, inject.Name);
                }
            }
            
            var properties = target.GetType().GetProperties(InjectFlags);
            foreach (var property in properties) {
                if (Attribute.GetCustomAttribute(property, typeof(InjectAttribute), false) is InjectAttribute inject) {
                    InjectField(target, context, new Setter(property), inject.Nullable, inject.Name);
                }
            }
        }

        private void InjectField(object target, ResolveContext context, Setter setter, bool nullable, string? name) {
            if (setter.GetValue(target) != null) {
                // Ignore the already defined values
                // TODO: test
                return;
            }

            if (InjectorFunction.InjectField(_container, target, setter)) return;

            if (name != null) {
                if (_container.Contains(name)) {
                    // There is a provider for the alias
                    _logger.Debug("Injecting field alias '" + name + "' " + setter.Name + " " + setter.Type.Name +
                                  " in " + target.GetType() + "(" + target.GetHashCode() + ")");
                    var service = _container.Resolve(name, context);
                    setter.SetValue(target, service);
                    return;
                }
                if (!nullable) {
                    throw new InjectFieldException(setter.Name, target,
                        "Injectable property [Inject(Name=\""+name+"\")] " + setter.Type.Name + " " + setter.Name +
                        " not found while injecting fields in " + target.GetType().Name);
                }
                return;
            } 
            
            if (_container.CreateIfNotFound || _container.Contains(setter.Type)) {
                // There is a provider for the field type
                _logger.Debug("Injecting field " + setter.Name + " " + setter.Type.Name + " in " + target.GetType() +
                              "(" + target.GetHashCode() + ")");
                var service = _container.Resolve(setter.Type, context);
                setter.SetValue(target, service);
                return;
            }

            if (_container.Contains(setter.Name)) {
                // There is a provider for the alias
                _logger.Debug("Injecting field alias '" + setter.Name + "' " + setter.Name + " " + setter.Type.Name +
                              " in " + target.GetType() + "(" + target.GetHashCode() + ")");
                var service = _container.Resolve(setter.Name, context);
                setter.SetValue(target, service);
                return;
            }

            
            if (!nullable) {
                throw new InjectFieldException(setter.Name, target,
                    "Injectable property [Inject] " + setter.Type.Name + " " + setter.Name +
                    " not found while injecting fields in " + target.GetType().Name);
            }
        }

        public void LoadOnReadyNodes(Node target) {
            var fields = target.GetType().GetFields(OnReadyFlags);
            foreach (var field in fields) {
                if (Attribute.GetCustomAttribute(field, typeof(OnReadyAttribute), false) is OnReadyAttribute
                    onReady) {
                    LoadOnReadyField(target, onReady, new Setter(field));
                }
            }

            var properties = target.GetType().GetProperties(OnReadyFlags);
            foreach (var property in properties) {
                if (Attribute.GetCustomAttribute(property,
                        typeof(OnReadyAttribute), false) is OnReadyAttribute onReady) {
                    LoadOnReadyField(target, onReady, new Setter(property));
                }
            }
        }

        private void LoadOnReadyField(Node target, OnReadyAttribute onReady, Setter setter) {
            if (onReady.Path != null) {
                // [OnReady("path/to/node")
                // private Sprite sprite = this.GetNode<Sprite>("path/to/node");
                var node = target.GetNode(onReady.Path);
                var fieldInfo = "[OnReady(\"" + onReady.Path + "\")] " + setter.Type.Name + " " +
                                setter.Name;

                if (node == null) {
                    if (onReady.Nullable) return;
                    throw new OnReadyFieldException(setter.Name, target,
                        "Path returns a null value for field " + fieldInfo + ", class " + target.GetType().Name);
                }
                if (!setter.Type.IsInstanceOfType(node)) {
                    throw new OnReadyFieldException(setter.Name, target,
                        "Path returns an incompatible type " + node.GetType().Name + " for field " + fieldInfo +
                        ", class " + target.GetType().Name);
                }
                setter.SetValue(target, node);
                return;
            }

            InjectorFunction.OnReadyField(_container, target, setter);
        }
    }
}