using System;

namespace Betauer.TestRunner;

[AttributeUsage(AttributeTargets.Method)]
public class TearDownAttribute : Attribute {
}