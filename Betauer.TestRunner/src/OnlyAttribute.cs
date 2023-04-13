using System;

namespace Betauer.TestRunner;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class OnlyAttribute : Attribute {
}