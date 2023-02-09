using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.NodeTests; 

[TestFixture]
// [Ignore("")]
public partial class SpatialTests : Node3D {
    const float marginOfError = 0.01f;

    [TearDown]
    public void TearDown() {
        //QueueFree();
    }

    // Z basis after Y Rotations
    [Test]
    public void BasisZShouldBePointSeven_Zero_PointSeven_WhenRotating45DegreesAround_Y() {
        RotateY(Mathf.Pi / 4.0f);
        CompareVector3s(Transform.Basis.Z, new Vector3(0.7f, 0, 0.7f), marginOfError);
    }

    [Test]
    public void BasisZShouldBeAt_One_Zero_Zero_WhenRotated90Around_Y() {
        RotateY(Mathf.Pi / 2.0f);
        CompareVector3s(Transform.Basis.Z, new Vector3(1, 0, 0), marginOfError);
    }

    [Test]
    public void BasisZShouldBeAt_PointSeven_Zero_NegativePointSeven_WhenRotated135Around_Y() {
        RotateY(Mathf.Pi * (3.0f / 4.0f));
        CompareVector3s(Transform.Basis.Z, new Vector3(0.7f, 0, -0.7f), marginOfError);
    }

    [Test]
    public void BasisZShouldBeAt_Zero_Zero_NegativeOne_WhenRotated180Around_Y() {
        RotateY(Mathf.Pi);
        CompareVector3s(Transform.Basis.Z, new Vector3(0, 0, -1), marginOfError);
    }

    [Test]
    public void BasisZShouldBeAt_NegativePointSeven_Zero_NegativePointSeven_WhenRotated225Around_Y() {
        RotateY(Mathf.Pi + Mathf.Pi / 4.0f);
        CompareVector3s(Transform.Basis.Z, new Vector3(-0.7f, 0, -0.7f), marginOfError);
    }

    [Test]
    public void BasisZShouldBeAt_NegativeOne_Zero_Zero_WhenRotated270Around_Y() {
        RotateY(Mathf.Pi + Mathf.Pi / 2.0f);
        CompareVector3s(Transform.Basis.Z, new Vector3(-1, 0, 0), marginOfError);
    }

    [Test]
    public void BasisZShouldBeAt_PointSeven_Zero_NegativePointSeven_WhenRotated315Around_Y() {
        RotateY(Mathf.Pi + Mathf.Pi * (3.0f / 4.0f));
        CompareVector3s(Transform.Basis.Z, new Vector3(-0.7f, 0, 0.7f), marginOfError);
    }

    [Test]
    public void BasisZShouldBeAt_Zero_Zero_One_WhenRotated360Around_Y() {
        RotateY(Mathf.Pi * 2.0f);
        CompareVector3s(Transform.Basis.Z, new Vector3(0, 0, 1), marginOfError);
    }

    // X basis after Y Rotations

    [Test]
    public void XShouldBeAt_PointSeven_Zero_NegativePointSeven_WhenRotated45Around_Y() {
        RotateY(Mathf.Pi / 4.0f);
        CompareVector3s(Transform.Basis.X, new Vector3(0.7f, 0, -0.7f), marginOfError);
    }

    [Test]
    public void XShouldBeAt_NegativePointSeven_Zero_NegativePointSeven_WhenRotated135Around_Y() {
        RotateY(Mathf.Pi * (3.0f / 4.0f));
        CompareVector3s(Transform.Basis.X, new Vector3(-0.7f, 0, -0.7f), marginOfError);
    }

    [Test]
    public void XShouldBeAt_NegativePointSeven_Zero_PointSeven_WhenRotated225Around_Y() {
        RotateY(Mathf.Pi + Mathf.Pi / 4.0f);
        CompareVector3s(Transform.Basis.X, new Vector3(-0.7f, 0, 0.7f), marginOfError);
    }

    [Test]
    public void XShouldBeAt_PointSeven_Zero_PointSeven_WhenRotated315Around_Y() {
        RotateY(Mathf.Pi + Mathf.Pi * (3.0f / 4.0f));
        CompareVector3s(Transform.Basis.X, new Vector3(0.7f, 0, 0.7f), marginOfError);
    }

    // Y basis after X Rotations

    [Test]
    public void YShouldBeAt_Zero_One_Zero_WhenRotated45Around_Y() {
        RotateY(Mathf.Pi / 4.0f);
        CompareVector3s(Transform.Basis.Y, new Vector3(0, 1, 0), marginOfError);
    }

    [Test]
    public void YShouldBeAt_Zero_PointSeven_PointSeven_WhenRotated45Around_X() {
        RotateX(Mathf.Pi / 4.0f);
        CompareVector3s(Transform.Basis.Y, new Vector3(0, 0.7f, 0.7f), marginOfError);
    }

    [Test]
    public void YShouldBeAt_Zero_NegativePointSeven_PointSeven_WhenRotated135Around_X() {
        RotateX(Mathf.Pi * (3.0f / 4.0f));
        CompareVector3s(Transform.Basis.Y, new Vector3(0, -0.7f, 0.7f), marginOfError);
    }

    [Test]
    public void YShouldBeAt_Zero_NegativePointSeven_NegativePointSeven_WhenRotated225Around_X() {
        RotateX(Mathf.Pi + Mathf.Pi / 4.0f);
        CompareVector3s(Transform.Basis.Y, new Vector3(0, -0.7f, -0.7f), marginOfError);
    }

    [Test]
    public void YShouldBeAt_Zero_PointSeven_NegativePointSeven_WhenRotated315Around_X() {
        RotateX(Mathf.Pi + Mathf.Pi * (3.0f / 4.0f));
        CompareVector3s(Transform.Basis.Y, new Vector3(0, 0.7f, -0.7f), marginOfError);
    }

    void CompareVector3s(Vector3 actual, Vector3 expected, float within = 0) {
        Assert.That(actual.X, Is.EqualTo(expected.X).Within(within));
        Assert.That(actual.Y, Is.EqualTo(expected.Y).Within(within));
        Assert.That(actual.Z, Is.EqualTo(expected.Z).Within(within));
    }
}