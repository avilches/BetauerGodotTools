using System;
using Betauer.Application.Settings.Attributes;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.Input.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false)]
public class InputActionAttribute : ServiceTemplateAttribute {
    public string? Name { get; set; }
    public string? AxisName { get; set; }
    public string? SaveAs { get; set; }
    public bool AutoSave { get; set; } = false;

    public InputActionAttribute() {
    }

    public InputActionAttribute(string name) {
        Name = name;
    }

    public override ServiceTemplate CreateServiceTemplate(object configuration, IGetter getter) {
        string? settingsContainerName = null;
        if (SaveAs != null) {
            var settingContainer = configuration.GetType().GetAttribute<SettingsContainerAttribute>();
            if (settingContainer == null) {
                throw new InvalidAttributeException(
                $"Attribute {typeof(InputActionAttribute).FormatAttribute()} with SaveAs = \"{SaveAs}\" needs to be used in a class with attribute {typeof(SettingsContainerAttribute).FormatAttribute()}");
            }
            settingsContainerName = settingContainer.Name;
        }
        
        var inputActionsContainer = configuration.GetType().GetAttribute<InputActionsContainerAttribute>()!;
        if (inputActionsContainer == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(InputActionAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(InputActionsContainerAttribute).FormatAttribute()}");
        }
        var name = Name ?? getter.Name;
        return new ServiceTemplate {
            Lifetime = Lifetime.Singleton,
            ProviderType = typeof(InputAction),
            RegisterType = typeof(InputAction),
            Factory = () => {
                InputAction inputAction = (InputAction)getter.GetValue(configuration)!;
                inputAction.PreInject(name, AxisName, inputActionsContainer.Name, settingsContainerName, SaveAs, AutoSave);
                return inputAction;
            },
            Name = name,
            Primary = false,
            Lazy = false,
        };
    }
}