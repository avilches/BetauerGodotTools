using System;

namespace Betauer.Application.Lifecycle.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class LoaderAttribute : Attribute {
    public string? Tag { get; set; }
    public string Name { get; set; }

    public LoaderAttribute(string name) {
        Name = name;
    }

    public LoaderAttribute(string name, string tag) {
        Tag = tag;
        Name = name;
    }
}