using System;
using System.Reflection;

namespace Betauer.DI {
    public class Injector {
        private readonly Logger _logger = LoggerFactory.GetLogger(typeof(Injector));
        private readonly Container _container;

        private const BindingFlags InjectFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public Injector(Container container) {
            _container = container;
        }

        public void InjectAllFields(object target, ResolveContext context) {
            if (target is Delegate) return;
            _logger.Debug("Injecting fields in " + target.GetType() + ": " + target.GetHashCode().ToString("X"));
            foreach (var setter in target.GetType().GetPropertiesAndFields<InjectAttribute>(InjectFlags))
                InjectField(target, context, setter);
        }

        private void InjectField(object target, ResolveContext context, GetterSetter<InjectAttribute> getterSetter) {
            if (getterSetter.GetValue(target) != null) {
                // Ignore the already defined values
                // TODO: test
                return;
            }

            if (InjectorFunction.InjectField(_container, target, getterSetter)) return;

            var nullable = getterSetter.Attribute.Nullable;
            var name = getterSetter.Attribute.Name;
            if (name != null) {
                name = name.Trim();
                if (_container.Contains(name)) {
                    // There is a provider for the name
                    _logger.Debug("Injecting field name '" + name + "' " + getterSetter.Name + " " + getterSetter.Type.Name +
                                  " in " + target.GetType() + "(" + target.GetHashCode() + ")");
                    var service = _container.Resolve(name, context);
                    getterSetter.SetValue(target, service);
                    return;
                }
                if (!nullable) {
                    throw new InjectFieldException(getterSetter.Name, target,
                        "Injectable property [Inject(Name=\""+name+"\")] " + getterSetter.Type.Name + " " + getterSetter.Name +
                        " not found while injecting fields in " + target.GetType().Name);
                }
                return;
            } 
            
            if (_container.Contains(getterSetter.Name)) {
                // There is a provider for the name
                _logger.Debug("Injecting field name '" + getterSetter.Name + "' " + getterSetter.Name + " " + getterSetter.Type.Name +
                              " in " + target.GetType() + "(" + target.GetHashCode() + ")");
                var service = _container.Resolve(getterSetter.Name, context);
                getterSetter.SetValue(target, service);
                return;
            }

            if (_container.CreateIfNotFound || _container.Contains(getterSetter.Type)) {
                // There is a provider for the field type
                _logger.Debug("Injecting field " + getterSetter.Name + " " + getterSetter.Type.Name + " in " + target.GetType() +
                              "(" + target.GetHashCode() + ")");
                var service = _container.Resolve(getterSetter.Type, context);
                getterSetter.SetValue(target, service);
                return;
            }

            if (!nullable) {
                throw new InjectFieldException(getterSetter.Name, target,
                    "Injectable property [Inject] " + getterSetter.Type.Name + " " + getterSetter.Name +
                    " not found while injecting fields in " + target.GetType().Name);
            }
        }

    }
}