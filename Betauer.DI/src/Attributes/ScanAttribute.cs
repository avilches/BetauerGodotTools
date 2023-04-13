using System;

namespace Betauer.DI.Attributes;

public abstract class ScanAttribute : Attribute {
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScanAttribute<T> : ScanAttribute {
}
