using System;
using Betauer.Core.DataMath.Data;
using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;

[Betauer.TestRunner.Test]
public class FloatRangeTests {
    [Betauer.TestRunner.Test]
    public void Constructor_ShouldThrowException_WhenArraySizesMismatch() {

        Assert.Throws<ArgumentException>(() => new FloatRange<string>(new [] { 0.5f, 2f }, new [] { "A", "B" }));
        Assert.Throws<ArgumentException>(() => new FloatRange<string>(new [] { 0.5f, 2f }, new [] { "A", "B", "C", "D" }));
        Assert.Throws<ArgumentException>(() => new FloatRange<string>(new [] { 0.5f, 2f, 3f }, new [] { "A", "B" }));
    }

    [Betauer.TestRunner.Test]
    public void GetValue_ShouldReturnCorrectValue() {
        // Arrange
        float[] floats = { /* A */ 0.5f, /* B */ 2f, /* C */ 3f /* D */ };
        string[] values = { "A", "B", "C", "D" };

        FloatRange<string> range = new FloatRange<string>(floats, values);

        for (float i = -0.5f; i < 0.50f; i += 0.1f) {
            Assert.AreEqual("A", range.GetValue(i));
        }
        for (float i = 0.5f; i < 2f; i += 0.1f) {
            Assert.AreEqual("B", range.GetValue(i));
        }
        for (float i = 2f; i < 3f; i += 0.1f) {
            Assert.AreEqual("C", range.GetValue(i));
        }
        for (float i = 3f; i < 10f; i += 0.1f) {
            Assert.AreEqual("D", range.GetValue(i));
        }
    }
}