using System;

namespace Betauer.Input.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InputActionsContainerAttribute : Attribute {
    public string Name { get; set; }

    public InputActionsContainerAttribute(string name) {
        Name = name;
    }
}