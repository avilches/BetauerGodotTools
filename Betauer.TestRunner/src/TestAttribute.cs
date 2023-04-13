using System;

namespace Betauer.TestRunner;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class TestAttribute : Attribute {
    /// <summary>
    /// Descriptive text for this test
    /// </summary>
    public string? Description { get; set; }

    public bool Only { get; set; } = false;
    public bool Ignore { get; set; } = false;
}