using Betauer.Core.DataMath;
using Betauer.TestRunner;
using NUnit.Framework;
using Betauer.Core.PCG.GridTemplate;

namespace Betauer.Core.Tests;

[TestFixture]
public class DirectionTransformationsTests {
    [Test]
    public void TestRotate90() {
        var type = (int)(DirectionFlags.Up | DirectionFlags.Right);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.Rotate90);
        Assert.That(result, Is.EqualTo((int)(DirectionFlags.Right | DirectionFlags.Down)));
    }

    [Test]
    public void TestRotate180() {
        var type = (int)(DirectionFlags.Up | DirectionFlags.Right);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.Rotate180);
        Assert.That(result, Is.EqualTo((int)(DirectionFlags.Down | DirectionFlags.Left)));
    }

    [Test]
    public void TestRotateMinus90() {
        var type = (int)(DirectionFlags.Up | DirectionFlags.Right);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.RotateMinus90);
        Assert.That(result, Is.EqualTo((int)(DirectionFlags.Left | DirectionFlags.Up)));
    }

    [Test]
    public void TestFlipH() {
        var type = (int)(DirectionFlags.Right | DirectionFlags.UpRight);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.FlipH);
        Assert.That(result, Is.EqualTo((int)(DirectionFlags.Left | DirectionFlags.UpLeft)));
    }

    [Test]
    public void TestFlipV() {
        var type = (int)(DirectionFlags.Up | DirectionFlags.UpRight);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.FlipV);
        Assert.That(result, Is.EqualTo((int)(DirectionFlags.Down | DirectionFlags.DownRight)));
    }

    [Test]
    public void TestMirrorLR() {
        // L side: Up, Left, UpLeft, DownLeft
        // R side should mirror L side
        var type = (int)(DirectionFlags.Up | DirectionFlags.Left | DirectionFlags.UpLeft | DirectionFlags.DownLeft);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.MirrorLR);
        var expected = type | (int)(DirectionFlags.Right | DirectionFlags.UpRight | DirectionFlags.DownRight);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestMirrorRL() {
        // R side: Up, Right, UpRight, DownRight
        // L side should mirror R side
        var type = (int)(DirectionFlags.Up | DirectionFlags.Right | DirectionFlags.UpRight | DirectionFlags.DownRight);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.MirrorRL);
        var expected = type | (int)(DirectionFlags.Left | DirectionFlags.UpLeft | DirectionFlags.DownLeft);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestMirrorTB() {
        // T side: Up, Left, Right, UpLeft, UpRight
        // B side should mirror T side
        var type = (int)(DirectionFlags.Up | DirectionFlags.Left | DirectionFlags.Right | 
                        DirectionFlags.UpLeft | DirectionFlags.UpRight);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.MirrorTB);
        var expected = type | (int)(DirectionFlags.Down | DirectionFlags.DownLeft | DirectionFlags.DownRight);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestMirrorBT() {
        // B side: Down, Left, Right, DownLeft, DownRight
        // T side should mirror B side
        var type = (int)(DirectionFlags.Down | DirectionFlags.Left | DirectionFlags.Right | 
                        DirectionFlags.DownLeft | DirectionFlags.DownRight);
        var result = DirectionTransformations.TransformDirections(type, Transformations.Type.MirrorBT);
        var expected = type | (int)(DirectionFlags.Up | DirectionFlags.UpLeft | DirectionFlags.UpRight);
        Assert.That(result, Is.EqualTo(expected));
    }
}