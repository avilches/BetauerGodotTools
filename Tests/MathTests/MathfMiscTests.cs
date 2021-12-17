using Godot;
using NUnit.Framework;

namespace GodotTests.MathTests {
    [TestFixture]
    [Ignore("")]
    public class MathfMiscTests {
        const float marginOfError = float.Epsilon;


        // Mathf consts

        [Test]
        public void MathfPIShouldEqualDotNetPI() {
            Assert.That(Mathf.Pi, Is.EqualTo(System.Math.PI).Within(0.0000001));
        }

        // Mathf.Deg2Rad

        [Test]
        public void Deg2RadOf180ShouldBePI() {
            Assert.That(Mathf.Deg2Rad(180), Is.EqualTo(Mathf.Pi).Within(marginOfError));
        }


        [Test]
        public void Deg2RadOf270ShouldBePIAndAHalf() {
            Assert.That(Mathf.Deg2Rad(270), Is.EqualTo(Mathf.Pi + Mathf.Pi / 2).Within(marginOfError));
        }


        [Test]
        public void Deg2RadOf540ShouldBeThreePI() {
            Assert.That(Mathf.Deg2Rad(540), Is.EqualTo(Mathf.Pi * 3).Within(marginOfError));
        }

        // Mathf.Rad2Deg

        [Test]
        public void Rad2DegOfPIShouldBe180() {
            Assert.That(Mathf.Rad2Deg(Mathf.Pi), Is.EqualTo(180).Within(marginOfError));
        }


        [Test]
        public void Rad2DegOfPIAndAHalfShouldBe270() {
            Assert.That(Mathf.Rad2Deg(Mathf.Pi + Mathf.Pi / 2), Is.EqualTo(270).Within(marginOfError));
        }


        [Test]
        public void Rad2DegOfThreePIShouldBe540() {
            Assert.That(Mathf.Rad2Deg(Mathf.Pi * 3), Is.EqualTo(540).Within(marginOfError));
        }

        // Mathf.Clamp

        [Test]
        public void ClampFiveBetweenZeroAndTenShouldBeFive() {
            Assert.That(Mathf.Clamp(5, 0, 10), Is.EqualTo(5));
        }


        [Test]
        public void ClampFiveBetweenZeroAndFourShouldBeFour() {
            Assert.That(Mathf.Clamp(5, 0, 4), Is.EqualTo(4));
        }


        [Test]
        public void ClampFiveBetweenSevenAndTenShouldBeSeven() {
            Assert.That(Mathf.Clamp(5, 7, 10), Is.EqualTo(7));
        }

        // Mathf.Decimals

        [Test]
        public void DecimalCountForOnePointZeroShouldBeZero() {
            Assert.That(Mathf.DecimalCount(1.0f), Is.EqualTo(0));
        }

        [Test]
        public void DecimalCountForOnePointZeroOneShouldBeTwo() {
            Assert.That(Mathf.DecimalCount(1.01f), Is.EqualTo(2));
        }

        [Test]
        public void DecimalCountForTenPointZeroShouldBeZero() {
            Assert.That(Mathf.DecimalCount(10.0f), Is.EqualTo(0));
        }
    }
}