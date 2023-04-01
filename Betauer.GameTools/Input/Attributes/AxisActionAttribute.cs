using System;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.Input.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class AxisActionAttribute : Attribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }

    public AxisActionAttribute() {
    }

    public AxisActionAttribute(string name) {
        Name = name;
    }

    public void CreateProvider(object configuration, IGetter getter, Container.Builder builder) {
        AttributeTools.ValidateDuplicates<IConfigurationMemberAttribute>(getter, this);
        var inputActionContainer = configuration.GetType().GetAttribute<InputActionsContainerAttribute>()!;
        if (inputActionContainer == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(AxisActionAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(InputActionsContainerAttribute).FormatAttribute()}");
        }
        var name = Name ?? getter.Name;
        Func<AxisAction> factory = () => {
            AxisAction axisAction = (AxisAction)getter.GetValue(configuration)!;
            axisAction.PreInject(name, inputActionContainer.Name);
            return axisAction;
        };
        builder.RegisterServiceAndAddFactory(typeof(AxisAction), typeof(AxisAction), Lifetime.Singleton, factory, name, false, false);
    }
}