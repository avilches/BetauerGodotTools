using Godot;
using NUnit.Framework;

namespace Veronenger.Tests.StructTests {
    [TestFixture]
    [Ignore("")]
    public class BasisTests {
        const float marginOfError = 0.01f;
        Basis b;


        [SetUp]
        public void SetUp() {
            b = new Basis(Quat.Identity);
        }

        // Z value after Rotations
        [Test]
        public void ZShouldBeAtZeroZeroOneWhenFacingForward() {
            CompareVector3s(b.z, new Vector3(0, 0, 1), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_NegativePointSeven_Zero_PointSeven_WhenRotated45Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi / 4.0f);
            CompareVector3s(b.z, new Vector3(0.7f, 0, 0.7f), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_One_Zero_Zero_WhenRotated90Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi / 2.0f);
            CompareVector3s(b.z, new Vector3(1, 0, 0), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_PointSeven_Zero_NegativePointSeven_WhenRotated135Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi * (3.0f / 4.0f));
            CompareVector3s(b.z, new Vector3(0.7f, 0, -0.7f), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_Zero_Zero_NegativeOne_WhenRotated180Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi);
            CompareVector3s(b.z, new Vector3(0, 0, -1), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_NegativePointSeven_Zero_NegativePointSeven_WhenRotated225Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi + Mathf.Pi / 4.0f);
            CompareVector3s(b.z, new Vector3(-0.7f, 0, -0.7f), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_NegativeOne_Zero_Zero_WhenRotated270Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi + Mathf.Pi / 2.0f);
            CompareVector3s(b.z, new Vector3(-1, 0, 0), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_PointSeven_Zero_NegativePointSeven_WhenRotated315Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi + Mathf.Pi * (3.0f / 4.0f));
            CompareVector3s(b.z, new Vector3(-0.7f, 0, 0.7f), marginOfError);
        }

        [Test]
        public void ZShouldBeAt_Zero_Zero_One_WhenRotated360Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi * 2.0f);
            CompareVector3s(b.z, new Vector3(0, 0, 1), marginOfError);
        }

        // X value after Y Rotations

        [Test]
        public void XShouldBeAt_PointSeven_Zero_NegativePointSeven_WhenRotated45Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi / 4.0f);
            CompareVector3s(b.x, new Vector3(0.7f, 0, -0.7f), marginOfError);
        }

        [Test]
        public void XShouldBeAt_NegativePointSeven_Zero_NegativePointSeven_WhenRotated135Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi * (3.0f / 4.0f));
            CompareVector3s(b.x, new Vector3(-0.7f, 0, -0.7f), marginOfError);
        }

        [Test]
        public void XShouldBeAt_NegativePointSeven_Zero_PointSeven_WhenRotated225Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi + Mathf.Pi / 4.0f);
            CompareVector3s(b.x, new Vector3(-0.7f, 0, 0.7f), marginOfError);
        }

        [Test]
        public void XShouldBeAt_PointSeven_Zero_PointSeven_WhenRotated315Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi + Mathf.Pi * (3.0f / 4.0f));
            CompareVector3s(b.x, new Vector3(0.7f, 0, 0.7f), marginOfError);
        }

        // Y value after Y Rotations

        [Test]
        public void YShouldBeAt_Zero_One_Zero_WhenRotated45Around_Y() {
            b = b.Rotated(new Vector3(0, 1, 0), Mathf.Pi / 4.0f);
            CompareVector3s(b.y, new Vector3(0, 1, 0), marginOfError);
        }

        [Test]
        public void YShouldBeAt_Zero_PointSeven_PointSeven_WhenRotated45Around_X() {
            b = b.Rotated(new Vector3(1, 0, 0), Mathf.Pi / 4.0f);
            CompareVector3s(b.y, new Vector3(0, 0.7f, 0.7f), marginOfError);
        }

        [Test]
        public void YShouldBeAt_Zero_NegativePointSeven_PointSeven_WhenRotated135Around_X() {
            b = b.Rotated(new Vector3(1, 0, 0), Mathf.Pi * (3.0f / 4.0f));
            CompareVector3s(b.y, new Vector3(0, -0.7f, 0.7f), marginOfError);
        }

        [Test]
        public void YShouldBeAt_Zero_NegativePointSeven_NegativePointSeven_WhenRotated225Around_X() {
            b = b.Rotated(new Vector3(1, 0, 0), Mathf.Pi + Mathf.Pi / 4.0f);
            CompareVector3s(b.y, new Vector3(0, -0.7f, -0.7f), marginOfError);
        }

        [Test]
        public void YShouldBeAt_Zero_PointSeven_NegativePointSeven_WhenRotated315Around_X() {
            b = b.Rotated(new Vector3(1, 0, 0), Mathf.Pi + Mathf.Pi * (3.0f / 4.0f));
            CompareVector3s(b.y, new Vector3(0, 0.7f, -0.7f), marginOfError);
        }


        void CompareVector3s(Vector3 actual, Vector3 expected, float within = 0) {
            Assert.That(actual.x, Is.EqualTo(expected.x).Within(within));
            Assert.That(actual.y, Is.EqualTo(expected.y).Within(within));
            Assert.That(actual.z, Is.EqualTo(expected.z).Within(within));
        }
    }
}