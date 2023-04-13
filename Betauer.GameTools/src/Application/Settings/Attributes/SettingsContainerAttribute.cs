using System;

namespace Betauer.Application.Settings.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class SettingsContainerAttribute : Attribute {
    public string Name { get; set; }

    public SettingsContainerAttribute(string name) {
        Name = name;
    }
}