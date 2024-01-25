using System;
using Godot;

namespace Betauer.Application;

public static class ConfigFileExtensions {
    public static void SetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string propertyWithSection, T value) {
        var (section, key) = GetSectionAndKey<T>(propertyWithSection);
        configFile.SetTypedValue(section, key, value);
    }

    public static void SetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string section, string key, T value) {
        configFile.SetValue(section, key, Variant.From(value));
    }

    public static T GetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string propertyWithSection, T @default) {
        var (section, key) = GetSectionAndKey<T>(propertyWithSection);
        return configFile.GetTypedValue(section, key, @default);
    }

    public static T GetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string section, string key, T @default) {
        var variantValue = configFile.GetValue(section, key, Variant.From(@default));
        return variantValue.As<T>();
    }

    public static (string, string) GetSectionAndKey<T>(string propertyWithSection) {
        var pos = propertyWithSection.IndexOf('/');
        if (pos <= 0) throw new ArgumentException("Property must be in the format 'Section/Key'", nameof(propertyWithSection));
        var section = propertyWithSection[..pos];
        var key = propertyWithSection[(pos + 1)..];
        return (section, key);
    }
}