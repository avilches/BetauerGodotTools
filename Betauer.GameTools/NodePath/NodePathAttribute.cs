using System;

namespace Betauer.NodePath; 

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class NodePathAttribute : Attribute {
    public bool Nullable { get; set; } = false;
    public readonly string? Path;

    public NodePathAttribute(string path) {
        Path = path;
    }
}