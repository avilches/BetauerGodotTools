using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PreloadAttribute<T> : ServiceTemplateAttribute where T : Resource {
    public string? Name { get; set; }
    public string Path { get; set; }
    public bool Lazy { get; set; }

    public PreloadAttribute() {
    }

    public override ProviderTemplate CreateProviderTemplate(MemberInfo memberInfo) {
        return new ProviderTemplate {
            Lifetime = Lifetime.Singleton,
            ProviderType = typeof(T),
            RegisterType = typeof(T),
            Factory = () => ResourceLoader.Load<T>(Path),
            Name = memberInfo.Name,
            Primary = false,
            Lazy = false,
        };
    }
}