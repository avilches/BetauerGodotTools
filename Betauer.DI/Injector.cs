using System;
using System.Linq;
using System.Reflection;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Logging;
using Betauer.Tools.Reflection;

namespace Betauer.DI;

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
        var members = target.GetType().GetSettersCached<InjectAttribute>(MemberTypes.Method | MemberTypes.Property, InjectFlags);
        foreach (var setter in members)
            InjectMember(target, context, setter);
    }

    private void InjectMember(object target, ResolveContext context, ISetter<InjectAttribute> getterSetter) {
        // Ignore the already defined values
        if (getterSetter is IGetter getter && getter.GetValue(target) is not null) return;
        
        if (InjectorFunction.InjectField(_container, target, getterSetter)) return;

        var nullable = getterSetter.SetterAttribute.Nullable;
        var name = getterSetter.SetterAttribute.Name;
        // Explicit name
        // [Inject(Name = ...)]
        if (name != null) {
            if (TryInjectFieldByName(target, context, getterSetter, name)) {
                Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {getterSetter} | Name taken from [Inject(\"{name}\")");
                return;
            }
            if (!nullable) {
                throw new InjectMemberException(getterSetter.Name, target, "Service Name=\"" + name + "\" not found when trying to inject " + getterSetter);
            }
        }

        var injectFactory = getterSetter.Type.IsGenericType && getterSetter.Type.ImplementsInterface(typeof(IFactory<>));
        // Implicit name (from variable). Only try if the type is not a IFactory<>
        if (!injectFactory) {
            if (TryInjectFieldByName(target, context, getterSetter, getterSetter.Name)) {
                Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {getterSetter} | Name taken from member: {getterSetter.Name}");
                return;
            }
        }

        if (TryInjectFieldByType(target, context, getterSetter)) {
            Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {getterSetter} | Type: {getterSetter.Type}");
            return;
        }
        if (_container.CreateIfNotFound) {
            getterSetter.SetValue(target, _container.Resolve(getterSetter.Type));
            Logger.Debug($"{target.GetType().FullName} ({target.GetHashCode():x8}) | {getterSetter} | Auto created. Type: {getterSetter.Type}");
            return;
        }
        if (!nullable) {
            throw new InjectMemberException(getterSetter.Name, target, "Not service found when trying to inject [" + getterSetter +
                                                                       "] in " + target.GetType().FullName);
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
            var service = provider.Get(context);
            setter.SetValue(target, service);
            return true;
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