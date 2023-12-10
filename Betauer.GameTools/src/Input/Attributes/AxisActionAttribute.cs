using System;
using Betauer.Application.Settings.Attributes;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.Input.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class AxisActionAttribute : Attribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }
    public string? SaveAs { get; set; }
    public bool AutoSave { get; set; } = false;

    public AxisActionAttribute() {
    }

    public AxisActionAttribute(string name) {
        Name = name;
    }

    public void Apply(object configuration, IGetter getter, Container.Builder builder) {
        string? settingsContainerName = null;
        if (SaveAs != null) {
            var settingContainer = configuration.GetType().GetAttribute<SettingsContainerAttribute>();
            if (settingContainer == null) {
                throw new InvalidAttributeException(
                    $"Attribute {typeof(InputActionAttribute).FormatAttribute()} with SaveAs = \"{SaveAs}\" needs to be used in a class with attribute {typeof(SettingsContainerAttribute).FormatAttribute()}");
            }
            settingsContainerName = settingContainer.Name;
        }

        var inputActionContainer = configuration.GetType().GetAttribute<InputActionsContainerAttribute>()!;
        if (inputActionContainer == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(AxisActionAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(InputActionsContainerAttribute).FormatAttribute()}");
        }
        var name = Name ?? getter.Name;
        Func<AxisAction> factory = () => {
            AxisAction axisAction = (AxisAction)getter.GetValue(configuration)!;
            axisAction.PreInject(name, inputActionContainer.Name, settingsContainerName, SaveAs, AutoSave);
            return axisAction;
        };
        var provider = Provider.Singleton<AxisAction, AxisAction>(factory, name, false);
        builder.Register(provider);
    }
}