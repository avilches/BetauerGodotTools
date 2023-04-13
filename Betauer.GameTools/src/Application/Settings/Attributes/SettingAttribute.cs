using System;
using Betauer.Core;
using Betauer.DI;
using Betauer.DI.Attributes;
using Betauer.DI.Exceptions;
using Betauer.DI.ServiceProvider;
using Betauer.Tools.FastReflection;

namespace Betauer.Application.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SettingAttribute<T> : Attribute, IConfigurationClassAttribute {
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

    public void CreateProvider(object configuration, Container.Builder builder) {
        var settingConfiguration = configuration.GetType().GetAttribute<SettingsContainerAttribute>();
        if (settingConfiguration == null) {
            throw new InvalidAttributeException(
                $"Attribute {typeof(SettingAttribute<T>).FormatAttribute()} needs to be used in a class with attribute {typeof(SettingsContainerAttribute).FormatAttribute()}");
        }
        Func<SaveSetting<T>> factory = () => {
            if (DefaultAsString != null) {
                Default = (T)Transformers.FromString(typeof(T), DefaultAsString);
            }
            SaveSetting<T> setting = Setting.Create(SaveAs ?? Name, Default, AutoSave, Enabled);
            setting.PreInject(settingConfiguration.Name);
            return setting;
        };
        builder.RegisterServiceAndAddFactory(typeof(SaveSetting<T>), typeof(SaveSetting<T>), Lifetime.Singleton, factory, Name, false, false);
    }
}