using Godot;
using NUnit.Framework;
using Vector2 = Godot.Vector2;
using Vector3 = Godot.Vector3;

namespace Betauer.Tests {
    [TestFixture]
    public class GodotToolsTests {
        [Test]
        public void SumVariantInt() {
            Assert.That(VariantHelper.SumVariant(1, 2), Is.EqualTo(3));
            Assert.That(VariantHelper.SubtractVariant(1, 2), Is.EqualTo(-1));
        }

        [Test]
        public void SumVariantFloat() {
            Assert.That(VariantHelper.SumVariant(1f, 2f), Is.EqualTo(3f).Within(0.0000001f));
            Assert.That(VariantHelper.SubtractVariant(1f, 2f), Is.EqualTo(-1f).Within(0.0000001f));
        }

        [Test]
        public void SumVariantDouble() {
            Assert.That(VariantHelper.SumVariant(1d, 2d), Is.EqualTo(3d).Within(0.0000001f));
            Assert.That(VariantHelper.SubtractVariant(1d, 2d), Is.EqualTo(-1d).Within(0.0000001f));
        }

        [Test]
        public void SumVariantVector2() {
            Assert.That(VariantHelper.SumVariant(Vector2.One, Vector2.One), Is.EqualTo(new Vector2(2f, 2f)));
            Assert.That(VariantHelper.SubtractVariant(Vector2.One, Vector2.One), Is.EqualTo(Vector2.Zero));
        }

        [Test]
        public void SumVariantVector3() {
            Assert.That(VariantHelper.SumVariant(Vector3.One, Vector3.One), Is.EqualTo(new Vector3(2f, 2f, 2f)));
            Assert.That(VariantHelper.SubtractVariant(Vector3.One, Vector3.One), Is.EqualTo(Vector3.Zero));
        }

        [Test]
        public void SumVariantColor() {
            Assert.That(VariantHelper.SumVariant(
                    new Color(0.1f, 0.2f, 0.3f, 0.4f), new Color(0.1f, 0.2f, 0.3f, 0.4f)),
                Is.EqualTo(new Color(0.2f, 0.4f, 0.6f, 0.8f)));
            Assert.That(VariantHelper.SubtractVariant(
                    new Color(0.1f, 0.1f, 0.1f, 0.1f), new Color(0.1f, 0.1f, 0.1f, 0.1f)),
                Is.EqualTo(new Color(0f, 0f, 0f, 0f)));
        }
    }
}