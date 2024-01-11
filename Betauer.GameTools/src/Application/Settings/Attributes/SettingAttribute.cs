using System;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SettingAttribute<[MustBeVariant] T> : Attribute, IConfigurationClassAttribute {
    public string Name { get; set; }
    public string? SaveAs { get; set; }
    public T Default { get; set; }
    public bool AutoSave { get; set; } = false;
    
    public SettingAttribute(string name) {
        Name = name;
    }

    public SettingAttribute(string name, string saveAs) {
        Name = name;
        SaveAs = saveAs;
    }

    public void Apply(object configuration, Container.Builder builder) {
        var settingConfiguration = configuration.GetType().GetAttribute<SettingsContainerAttribute>();
        if (settingConfiguration == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(SettingAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(SettingsContainerAttribute).FormatAttribute()}");
        }

        var setting = Setting.Create(SaveAs ?? Name, Default, AutoSave);
        builder.Register(Provider.Static<SaveSetting<T>, SaveSetting<T>>(setting, Name));
        
        builder.OnBuildFinished += () => {
            var settingsContainer = builder.Container.Resolve<SettingsContainer>(settingConfiguration.Name);
            setting.SetSettingsContainer(settingsContainer);
        };
        
    }
}