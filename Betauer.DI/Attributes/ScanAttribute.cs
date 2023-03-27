using System;

namespace Betauer.DI.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ScanAttribute : Attribute {
}

public class ScanAttribute<T> : ScanAttribute {
}
