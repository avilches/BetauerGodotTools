using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.MathTests; 

[TestRunner.Test]
[Ignore("")]
public class MathfLerpTests {
    const float marginOfError = float.Epsilon;


    [TestRunner.Test]
    public void LerpShouldReturnPointFiveWhenHalfWayBetweenZeroAndOne() {
        Assert.That(Mathf.Lerp(0, 1, 0.5f), Is.EqualTo(0.5f).Within(marginOfError));
    }


    [TestRunner.Test]
    public void LerpShouldReturnZeroWhenTheWeightIsZeroBetweenZeroAndOne() {
        Assert.That(Mathf.Lerp(0, 1, 0), Is.EqualTo(0).Within(marginOfError));
    }


    [TestRunner.Test]
    public void LerpShouldReturnOneWhenTheWeightIsOneBetweenZeroAndOne() {
        Assert.That(Mathf.Lerp(0, 1, 1), Is.EqualTo(1).Within(marginOfError));
    }


    [TestRunner.Test]
    public void LerpShouldReturnPointFiveWhenTheWeightIsPointFiveBetweenOneAndZero() {
        Assert.That(Mathf.Lerp(1, 0, 0.5f), Is.EqualTo(0.5f).Within(marginOfError));
    }


    [TestRunner.Test]
    public void LerpShouldReturnNegativePointFiveWhenWeightIsPointFiveBetweenZeroAndNegativeOne() {
        Assert.That(Mathf.Lerp(0, -1, 0.5f), Is.EqualTo(-0.5f).Within(marginOfError));
    }


    [TestRunner.Test]
    public void LerpShoulReturnTwentyWhenLerpValueIsTwo() {
        Assert.That(Mathf.Lerp(0, 10, 2), Is.EqualTo(20).Within(marginOfError));
    }


    [TestRunner.Test]
    public void InverseLerpShoulReturnPointFiveWhenHalfWayBetweenMinMaxValues() {
        Assert.That(Mathf.InverseLerp(3, 5, 4), Is.EqualTo(0.5f).Within(marginOfError));
    }


    [TestRunner.Test]
    public void InverseLerpShoulReturnNegativePointFiveWhenNegativeHalfWayBetweenMinMaxValues() {
        Assert.That(Mathf.InverseLerp(3, 5, 2), Is.EqualTo(-0.5f).Within(marginOfError));
    }
}