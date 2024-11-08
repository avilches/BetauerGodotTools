using System;
using Betauer.Core.Math.Data;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class FloatRangeTests {
    [Betauer.TestRunner.Test]
    public void Constructor_ShouldThrowException_WhenArraySizesMismatch() {
        float[] floats = { 0.5f, 2f };
        string[] values = { "A", "B", "C" };

        Assert.Throws<ArgumentException>(() => new FloatRange<string>(floats, values));
    }

    [Betauer.TestRunner.Test]
    public void GetValue_ShouldReturnCorrectValue() {
        // Arrange
        float[] floats = { 0.5f, 2f, 3f };
        string[] values = { "A", "B", "C" };

        FloatRange<string> range = new FloatRange<string>(floats, values);

        Assert.AreEqual("A", range.GetValue(0.2f));
        Assert.AreEqual("B", range.GetValue(1.5f));
        Assert.AreEqual("C", range.GetValue(3.0f));
    }

    [Betauer.TestRunner.Test]
    public void GetValue_ShouldReturnClosestValueWhenOutsideRange() {
        // Arrange
        float[] floats = { 0.5f, 2f, 3f };
        string[] values = { "A", "B", "C" };

        FloatRange<string> range = new FloatRange<string>(floats, values);

        Assert.AreEqual("A", range.GetValue(0.1f));
        Assert.AreEqual("C", range.GetValue(5.0f));
    }
}