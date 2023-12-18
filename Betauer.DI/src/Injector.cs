using System;
using System.Reflection;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Betauer.Tools.Logging;

namespace Betauer.DI;

public abstract class Injector {
    private static readonly Logger Logger = LoggerFactory.GetLogger<Injector>();

    private const BindingFlags InjectFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    internal static void InjectServices(ResolveContext context, Lifetime lifetime, object target) {
        if (target is Delegate) return;
        var members = target.GetType().GetSettersCached<InjectAttribute>(MemberTypes.Method | MemberTypes.Property, InjectFlags);
        if (members.Count == 0) return;
        Logger.Debug("Injecting fields in {0}: {1:x8}", target.GetType().GetTypeName(), target.GetHashCode());
        foreach (var setter in members) {
            if (setter is not IGetter getter || // no getter, like methods 
                getter.GetValue(target) is null) { // getter (properties and fields) returned null
                InjectMember(context, lifetime, target, setter);
            }
        }
    }

    private static void InjectMember(ResolveContext context, Lifetime lifetime, object target, ISetter<InjectAttribute> setter) {
        var nullable = setter.SetterAttribute.Nullable;
        var name = setter.SetterAttribute.Name;
        if (name != null) {
            // Explicit name with [Inject(Name = "")]
            if (TryInjectFieldByName(context, lifetime, target, setter, name)) {
                Logger.Debug("Injected {0} ({1:x8}) | {2} | Name taken from [Inject(\"{3}\")",
                    target.GetType().GetTypeName(), target.GetHashCode(), setter, name);
                return;
            }
            if (!nullable) {
                throw new InjectMemberException(setter.Name, target,
                    $"Service Name=\"{name}\" not found when trying to inject {setter} in {target.GetType().GetTypeName()} ({target.GetHashCode():x8})");
            }
        } else {
            // Implicit name (from variable, [Inject] Node pepe, so "pepe" is the name).
            name = setter.Name;
            if (TryInjectFieldByName(context, lifetime, target, setter, name)) {
                Logger.Debug("Injected {0} ({1:x8}) | {2} | Name taken from member: {3} ({4})",
                    target.GetType().GetTypeName(), target.GetHashCode(), setter, setter.Name, name);
                return;
            }
        }
        if (TryInjectFieldByType(context, lifetime, target, setter)) {
            Logger.Debug("Injected {0} ({1:x8}) | {2} | Type: {3}", target.GetType().GetTypeName(), target.GetHashCode(), setter, setter.MemberType);
            return;
        }
        if (TryCreateAndInject(context, lifetime, target, setter)) {
            Logger.Debug("Injected {0} ({1:x8}) | {2} | Auto created. Type: {3}", target.GetType().GetTypeName(), target.GetHashCode(),
                setter, setter.MemberType);
            return;
        }
        if (!nullable) {
            throw new InjectMemberException(setter.Name, target,
                $"Service not found when trying to inject {setter} in {target.GetType().GetTypeName()} ({target.GetHashCode():x8})");
        }
    }

    private static bool TryInjectFieldByName(ResolveContext context, Lifetime lifetime, object target, ISetter setter, string name) {
        if (!context.Container.TryGetProvider(name, out var provider, setter.MemberType)) return false;
        CheckLifetimeMismatch(lifetime, target, setter, setter.MemberType, provider.Lifetime);
        var service = provider.Resolve(context);
        setter.SetValue(target, service);
        return true;
    }

    private static bool TryInjectFieldByType(ResolveContext context, Lifetime lifetime, object target, ISetter setter) {
        if (!context.Container.TryGetProvider(setter.MemberType, out var provider)) return false;
        CheckLifetimeMismatch(lifetime, target, setter, setter.MemberType, provider!.Lifetime);
        var service = provider.Resolve(context);
        setter.SetValue(target, service);
        return true;
    }

    private static bool TryCreateAndInject(ResolveContext context, Lifetime lifetime, object target, ISetter setter) {
        if (!context.Container.CreateIfNotFound) return false;
        CheckLifetimeMismatch(lifetime, target, setter, setter.MemberType, Lifetime.Transient);
        var service = context.Container.TryCreateTransientFromInjector(setter.MemberType, context);
        setter.SetValue(target, service);
        return true;
    }

    private static void CheckLifetimeMismatch(Lifetime actual, object target, ISetter setter, Type injectType, Lifetime inject) {
        if (actual == Lifetime.Transient) return; // Transient allows to inject any lifetime
        if (inject == Lifetime.Singleton) return; // Singleton allows to inject only other singleton
        throw new InjectMemberException(setter.Name, target,
            $"Lifetime mismatch: can not inject a Transient {injectType.GetTypeName()} dependency into a Singleton {target.GetType().GetTypeName()} instance: {setter}");
    }
}