using System;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.Reflection;

namespace Betauer.Application.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SettingAttribute<T> : ServiceTemplateClassAttribute {
    public string Name { get; set; }
    public string? SaveAs { get; set; }
    public T Default { get; set; }
    public string? DefaultAsString { get; set; }
    public bool AutoSave { get; set; } = false;
    public bool Enabled { get; set; } = true;
    
    public SettingAttribute(string name) {
        Name = name;
    }

    public SettingAttribute(string name, string saveAs) {
        Name = name;
        SaveAs = saveAs;
    }

    public override ServiceTemplate CreateServiceTemplate(object configuration) {
        var settingConfiguration = configuration.GetType().GetAttribute<SettingsContainerAttribute>();
        if (settingConfiguration == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(SettingAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(SettingsContainerAttribute).FormatAttribute()}");
        }
        return new ServiceTemplate {
            Lifetime = Lifetime.Singleton,
            ProviderType = typeof(SaveSetting<T>),
            RegisterType = typeof(SaveSetting<T>),
            Factory = () => {
                if (DefaultAsString != null) {
                    Default = (T)Transformers.FromString(typeof(T), DefaultAsString);
                }
                SaveSetting<T> setting = Setting.Create(SaveAs ?? Name, Default, AutoSave, Enabled);
                setting.PreInject(settingConfiguration.Name);
                return setting;
            },
            Name = Name,
            Primary = false,
            Lazy = false,
        };
    }
}