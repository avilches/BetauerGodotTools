using System;
using Betauer.Core;
using Godot;

namespace Betauer.Application;

public static class ConfigFileExtensions {
    public static void SetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string propertyWithSection, T value) {
        var (section, key) = GetSectionAndKey<T>(propertyWithSection);
        configFile.SetTypedValue(section, key, value);
    }

    public static void SetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string section, string key, T value) {
        var variantValue = VariantHelper.CreateFrom(value);
        configFile.SetValue(section, key, variantValue);
    }

    public static T GetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string propertyWithSection, T @default) {
        var (section, key) = GetSectionAndKey<T>(propertyWithSection);
        return configFile.GetTypedValue(section, key, @default);
    }

    public static T GetTypedValue<[MustBeVariant] T>(this ConfigFile configFile, string section, string key, T @default) {
        var variantDefault = VariantHelper.CreateFrom(@default);
        var variantValue = configFile.GetValue(section, key, variantDefault);
        return VariantHelper.ConvertTo<T>(variantValue);
    }

    public static (string, string) GetSectionAndKey<T>(string propertyWithSection) {
        var pos = propertyWithSection.IndexOf('/');
        if (pos == -1) throw new ArgumentException("Property must be in the format 'Section/Key'", nameof(propertyWithSection));
        var section = propertyWithSection.Substring(0, pos);
        var key = propertyWithSection.Substring(pos + 1);
        return (section, key);
    }
}