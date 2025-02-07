using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.GridTemplate;

[TestFixture]
public class DirectionTransformationsTests {
    [Test]
    public void TestRotate90() {
        var type = (byte)(DirectionFlag.Up | DirectionFlag.Right);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.Rotate90);
        Assert.That(result, Is.EqualTo((byte)(DirectionFlag.Right | DirectionFlag.Down)));
    }

    [Test]
    public void TestRotate180() {
        var type = (byte)(DirectionFlag.Up | DirectionFlag.Right);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.Rotate180);
        Assert.That(result, Is.EqualTo((byte)(DirectionFlag.Down | DirectionFlag.Left)));
    }

    [Test]
    public void TestRotateMinus90() {
        var type = (byte)(DirectionFlag.Up | DirectionFlag.Right);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.RotateMinus90);
        Assert.That(result, Is.EqualTo((byte)(DirectionFlag.Left | DirectionFlag.Up)));
    }

    [Test]
    public void TestFlipH() {
        var type = (byte)(DirectionFlag.Right | DirectionFlag.UpRight);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.FlipH);
        Assert.That(result, Is.EqualTo((byte)(DirectionFlag.Left | DirectionFlag.UpLeft)));
    }

    [Test]
    public void TestFlipV() {
        var type = (byte)(DirectionFlag.Up | DirectionFlag.UpRight);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.FlipV);
        Assert.That(result, Is.EqualTo((byte)(DirectionFlag.Down | DirectionFlag.DownRight)));
    }

    [Test]
    public void TestMirrorLR() {
        // L side: Up, Left, UpLeft, DownLeft
        // R side should mirror L side
        var type = (byte)(DirectionFlag.Up | DirectionFlag.Left | DirectionFlag.UpLeft | DirectionFlag.DownLeft);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.MirrorLR);
        var expected = type | (byte)(DirectionFlag.Right | DirectionFlag.UpRight | DirectionFlag.DownRight);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestMirrorRL() {
        // R side: Up, Right, UpRight, DownRight
        // L side should mirror R side
        var type = (byte)(DirectionFlag.Up | DirectionFlag.Right | DirectionFlag.UpRight | DirectionFlag.DownRight);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.MirrorRL);
        var expected = type | (byte)(DirectionFlag.Left | DirectionFlag.UpLeft | DirectionFlag.DownLeft);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestMirrorTB() {
        // T side: Up, Left, Right, UpLeft, UpRight
        // B side should mirror T side
        var type = (byte)(DirectionFlag.Up | DirectionFlag.Left | DirectionFlag.Right | 
                        DirectionFlag.UpLeft | DirectionFlag.UpRight);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.MirrorTB);
        var expected = type | (byte)(DirectionFlag.Down | DirectionFlag.DownLeft | DirectionFlag.DownRight);
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestMirrorBT() {
        // B side: Down, Left, Right, DownLeft, DownRight
        // T side should mirror B side
        var type = (byte)(DirectionFlag.Down | DirectionFlag.Left | DirectionFlag.Right | 
                        DirectionFlag.DownLeft | DirectionFlag.DownRight);
        var result = DirectionTransformations.TransformFlags(type, Transformations.Type.MirrorBT);
        var expected = type | (byte)(DirectionFlag.Up | DirectionFlag.UpLeft | DirectionFlag.UpRight);
        Assert.That(result, Is.EqualTo(expected));
    }
}