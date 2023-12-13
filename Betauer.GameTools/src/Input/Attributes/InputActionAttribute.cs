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
public class InputActionAttribute : Attribute, IConfigurationMemberAttribute {
    public string? Name { get; set; }
    public string? AxisName { get; set; }
    public string? SaveAs { get; set; }

    public InputActionAttribute() {
    }

    public InputActionAttribute(string name) {
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
                $"Attribute {typeof(InputActionAttribute).FormatAttribute()} needs to be used in a class with attribute {typeof(InputActionsContainerAttribute).FormatAttribute()}");
        }
        var name = Name ?? getter.Name;
        InputAction inputAction = (InputAction)getter.GetValue(configuration)!;
        // It's ok to change the name here, the name is not used for anything (yet!)
        if (inputAction.Name == null) inputAction.Name = name;
        if (inputAction.AxisName == null) inputAction.AxisName = AxisName;
        builder.Register(Provider.Static<InputAction, InputAction>(inputAction, name));
    
        builder.OnBuildFinished += () => {
            var inputActionsContainer = builder.Container.Resolve<InputActionsContainer>(inputActionContainer.Name);
            inputAction.SetInputActionsContainer(inputActionsContainer);
            
            if (settingsContainerName != null) {
                var settingsContainer = builder.Container.Resolve<SettingsContainer>(settingsContainerName);
                inputAction.CreateSaveSetting(settingsContainer, SaveAs!);
            }
            
            inputAction.RefreshGodotInputMap();
        };
    }
}