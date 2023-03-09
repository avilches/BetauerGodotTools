using System;
using System.Linq;
using System.Reflection;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.Tools.Reflection;

namespace Betauer.DI;

public partial class Container {
    public class Injector {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Injector));
        private readonly Container _container;

        private const BindingFlags InjectFlags =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        public Injector(Container container) {
            _container = container;
        }

        public void InjectServices(object target, ResolveContext context) {
            if (target is Delegate) return;
            Logger.Debug($"Injecting fields in {target.GetType()}: {target.GetHashCode():x8}");
            var members = target.GetType()
                .GetSettersCached<InjectAttribute>(MemberTypes.Method | MemberTypes.Property, InjectFlags);
            foreach (var setter in members) {
                if (setter is not IGetter getter || // no getter, like methods 
                    getter.GetValue(target) is null) { // getter (properties and fields) returned null
                    InjectMember(target, context, setter);
                }
            }
        }

        private void InjectMember(object target, ResolveContext context, ISetter<InjectAttribute> setter) {
            var nullable = setter.SetterAttribute.Nullable;
            var name = setter.SetterAttribute.Name;
            if (name != null) {
                // Explicit name with [Inject(Name = "")]
                if (TryInjectFieldByName(target, context, setter, name)) {
                    Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {setter} | Name taken from [Inject(\"{name}\")");
                    return;
                }
                if (!nullable) {
                    throw new InjectMemberException(setter.Name, target, $"Service Name=\"{name}\" not found when trying to inject {setter}");
                }
            } else {
                // Implicit name (from variable, [Inject] Node pepe, so "pepe" is the name).
                if (TryInjectFieldByName(target, context, setter, setter.Name)) {
                    Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {setter} | Name taken from member: {setter.Name}");
                    return;
                }
            }
            if (TryInjectFieldByType(target, context, setter)) {
                Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {setter} | Type: {setter.Type}");
                return;
            }
            if (_container.CreateIfNotFound) {
                setter.SetValue(target, _container.Resolve(setter.Type));
                Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {setter} | Auto created. Type: {setter.Type}");
                return;
            }
            if (!nullable) {
                throw new InjectMemberException(setter.Name, target, $"Service type {target.GetType().FullName} not found when trying to inject {setter}");
            }
        }

        // [Service] Node Node1 => new Node();
        //
        // [Inject] Node Node1
        // [Inject(Name="Node1")] Node _pepe

        // [Service] IFactory<Node> NodeFactory => new MyNodeFactory();
        //
        // [Inject] IFactory<Node> NodeFactory
        // [Inject(Name="NodeFactory")] IFactory<Node> _nodeFactory
        private bool TryInjectFieldByName(object target, ResolveContext context, ISetter setter, string name) {
            if (_container.TryGetProvider(name, out var provider)) {
                if (setter.CanAssign(provider.ProviderType)) {
                    var service = provider.Get(context);
                    setter.SetValue(target, service);
                    return true;
                }
            }
            return false;
        }

        private bool TryInjectFieldByType(object target, ResolveContext context, ISetter setter) {
            if (_container.TryGetProvider(setter.Type, out var provider)) {
                var service = provider.Get(context);
                setter.SetValue(target, service);
                return true;
            }
            return false;

        }
    }
}