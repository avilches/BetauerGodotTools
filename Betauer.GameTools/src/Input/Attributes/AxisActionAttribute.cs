using System;
using Betauer.Application.Settings;
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
    public string? Name { get; init; }
    public string? SaveAs { get; init; }
    public bool AutoSave { get; init; } = true;

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
        AxisAction axisAction = (AxisAction)getter.GetValue(configuration)!;
        // It's ok to change the name here, the name is not used for anything (yet!)
        if (axisAction.Name == null) axisAction.Name = name;
        builder.Register(Provider.Static<AxisAction, AxisAction>(axisAction, name));
        
        builder.OnBuildFinished += () => {
            var inputActionsContainer = builder.Container.Resolve<InputActionsContainer>(inputActionContainer.Name);
            axisAction.SetInputActionsContainer(inputActionsContainer);
            
            if (settingsContainerName != null) {
                var settingsContainer = builder.Container.Resolve<SettingsContainer>(settingsContainerName);
                axisAction.CreateSaveSetting(settingsContainer, SaveAs!, AutoSave, true);
            }
        };
    }
}