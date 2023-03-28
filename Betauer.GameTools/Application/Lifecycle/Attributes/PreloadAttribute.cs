using System;
using System.Reflection;
using Betauer.DI.Attributes;
using Betauer.DI.ServiceProvider;
using Godot;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class PreloadAttribute<T> : ServiceTemplateAttribute where T : Resource {
    public string Resource { get; set; }
    public bool Lazy { get; set; }

    public PreloadAttribute(string resource) {
        Resource = resource;
        Lazy = false;
    }
    
    public PreloadAttribute(string resource, bool lazy) {
        Resource = resource;
        Lazy = lazy;
    }
    
    public override ProviderTemplate CreateProviderTemplate(MemberInfo memberInfo) {
        return new ProviderTemplate {
            Lifetime = Lifetime.Singleton,
            ProviderType = typeof(T),
            RegisterType = typeof(T),
            Factory = () => ResourceLoader.Load<T>(Resource),
            Name = memberInfo.Name,
            Primary = false,
            Lazy = false,
        };
    }
}