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

        public void InjectServices(object target, ResolveContext context) {
            if (target is Delegate) return;
            _logger.Debug("Injecting fields in " + target.GetType() + ": " + target.GetHashCode().ToString("X"));
            foreach (var setter in target.GetType().GetSetters<InjectAttribute>(MemberTypes.Method | MemberTypes.Property, InjectFlags))
                InjectField(target, context, setter);
        }

        private void InjectField(object target, ResolveContext context, ISetter<InjectAttribute> setter) {
            if (setter is IGetter getter && getter.GetValue(target) != null) {
                // Ignore the already defined values
                // TODO: test
                return;
            }

            if (InjectorFunction.InjectField(_container, target, setter)) return;

            var nullable = setter.Attribute.Nullable;
            var name = setter.Attribute.Name;
            if (name != null) {
                name = name.Trim();
                if (_container.Contains(name)) {
                    // There is a provider for the name
                    _logger.Debug("Injecting field name '" + name + "' " + setter.Name + " " + setter.Type.Name +
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
            
            if (_container.Contains(setter.Name)) {
                // There is a provider for the name
                _logger.Debug("Injecting field name '" + setter.Name + "' " + setter.Name + " " + setter.Type.Name +
                              " in " + target.GetType() + "(" + target.GetHashCode() + ")");
                var service = _container.Resolve(setter.Name, context);
                setter.SetValue(target, service);
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

            if (!nullable) {
                throw new InjectFieldException(setter.Name, target,
                    "Injectable property [Inject] " + setter.Type.Name + " " + setter.Name +
                    " not found while injecting fields in " + target.GetType().Name);
            }
        }

    }
}