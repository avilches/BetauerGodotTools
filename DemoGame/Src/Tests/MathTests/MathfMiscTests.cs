using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.MathTests; 

[TestRunner.Test]
[Ignore("")]
public class MathfMiscTests {
    const float marginOfError = float.Epsilon;


    // Mathf consts

    [TestRunner.Test]
    public void MathfPIShouldEqualDotNetPI() {
        Assert.That(Mathf.Pi, Is.EqualTo(System.Math.PI).Within(0.0000001));
    }

    // Mathf.DegToRad

    [TestRunner.Test]
    public void Deg2RadOf180ShouldBePI() {
        Assert.That(Mathf.DegToRad(180), Is.EqualTo(Mathf.Pi).Within(marginOfError));
    }


    [TestRunner.Test]
    public void Deg2RadOf270ShouldBePIAndAHalf() {
        Assert.That(Mathf.DegToRad(270), Is.EqualTo(Mathf.Pi + Mathf.Pi / 2).Within(marginOfError));
    }


    [TestRunner.Test]
    public void Deg2RadOf540ShouldBeThreePI() {
        Assert.That(Mathf.DegToRad(540), Is.EqualTo(Mathf.Pi * 3).Within(marginOfError));
    }

    // Mathf.RadToDeg

    [TestRunner.Test]
    public void RadToDegOfPIShouldBe180() {
        Assert.That(Mathf.RadToDeg(Mathf.Pi), Is.EqualTo(180).Within(marginOfError));
    }


    [TestRunner.Test]
    public void RadToDegOfPIAndAHalfShouldBe270() {
        Assert.That(Mathf.RadToDeg(Mathf.Pi + Mathf.Pi / 2), Is.EqualTo(270).Within(marginOfError));
    }


    [TestRunner.Test]
    public void RadToDegOfThreePIShouldBe540() {
        Assert.That(Mathf.RadToDeg(Mathf.Pi * 3), Is.EqualTo(540).Within(marginOfError));
    }

    // Mathf.Clamp

    [TestRunner.Test]
    public void ClampFiveBetweenZeroAndTenShouldBeFive() {
        Assert.That(Mathf.Clamp(5, 0, 10), Is.EqualTo(5));
    }


    [TestRunner.Test]
    public void ClampFiveBetweenZeroAndFourShouldBeFour() {
        Assert.That(Mathf.Clamp(5, 0, 4), Is.EqualTo(4));
    }


    [TestRunner.Test]
    public void ClampFiveBetweenSevenAndTenShouldBeSeven() {
        Assert.That(Mathf.Clamp(5, 7, 10), Is.EqualTo(7));
    }

    // Mathf.Decimals

    [TestRunner.Test]
    public void DecimalCountForOnePointZeroShouldBeZero() {
        Assert.That(Mathf.DecimalCount(1.0f), Is.EqualTo(0));
    }

    [TestRunner.Test]
    public void DecimalCountForOnePointZeroOneShouldBeTwo() {
        Assert.That(Mathf.DecimalCount(1.01f), Is.EqualTo(2));
    }

    [TestRunner.Test]
    public void DecimalCountForTenPointZeroShouldBeZero() {
        Assert.That(Mathf.DecimalCount(10.0f), Is.EqualTo(0));
    }
}