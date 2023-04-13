using System;

namespace Betauer.TestRunner;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IgnoreAttribute : Attribute {
    /// <summary>
    /// Descriptive text for this test
    /// </summary>
    public string Reason { get; set; }

    public IgnoreAttribute(string reason) {
        Reason = reason;
    }
}