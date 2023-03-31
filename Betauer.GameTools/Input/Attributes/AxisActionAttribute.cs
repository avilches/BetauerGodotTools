using System;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.Input.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class AxisActionAttribute : ServiceTemplateAttribute {
    public string? Name { get; set; }

    public AxisActionAttribute() {
    }

    public AxisActionAttribute(string name) {
        Name = name;
    }

    public override ServiceTemplate CreateServiceTemplate(object configuration, IGetter getter) {
        var inputActionContainer = configuration.GetType().GetAttribute<InputActionsContainerAttribute>()!;
        if (inputActionContainer == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(AxisActionAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(InputActionsContainerAttribute).FormatAttribute()}");
        }
        var name = Name ?? getter.Name;
        return new ServiceTemplate {
            Lifetime = Lifetime.Singleton,
            ProviderType = typeof(AxisAction),
            RegisterType = typeof(AxisAction),
            Factory = () => {
                AxisAction axisAction = (AxisAction)getter.GetValue(configuration)!;
                axisAction.PreInject(name, inputActionContainer.Name);
                return axisAction;
            },
            Name = name,
            Primary = false,
            Lazy = false,
        };
    }
}