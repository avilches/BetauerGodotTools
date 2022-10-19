using System.Collections.Generic;
using Betauer.Reflection;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    [Only]
    public class Vector2ExtensionsTest {
        [Test]

        public void RegularTests() {
            // Ok
            Assert.That(Vector2.Up.IsUp());
            Assert.That(Vector2.Down.IsDown());
            Assert.That(Vector2.Right.IsRight());
            Assert.That(Vector2.Left.IsLeft());
            
            // No ok
            Assert.That(Vector2.Down.IsUp(), Is.False);
            Assert.That(Vector2.Right.IsUp(), Is.False);
            Assert.That(Vector2.Left.IsUp(), Is.False);

            Assert.That(Vector2.Up.IsDown(), Is.False);
            Assert.That(Vector2.Right.IsDown(), Is.False);
            Assert.That(Vector2.Left.IsDown(), Is.False);

            Assert.That(Vector2.Up.IsRight(), Is.False);
            Assert.That(Vector2.Down.IsRight(), Is.False);
            Assert.That(Vector2.Left.IsRight(), Is.False);

            Assert.That(Vector2.Up.IsLeft(), Is.False);
            Assert.That(Vector2.Down.IsLeft(), Is.False);
            Assert.That(Vector2.Right.IsLeft(), Is.False);

            // 
            Assert.That(Vector2.Up.IsUpRight(), Is.False);
            Assert.That(Vector2.Down.IsUpRight(), Is.False);
            Assert.That(Vector2.Right.IsUpRight(), Is.False);
            Assert.That(Vector2.Left.IsUpRight(), Is.False);

            Assert.That(Vector2.Up.IsUpLeft(), Is.False);
            Assert.That(Vector2.Down.IsUpLeft(), Is.False);
            Assert.That(Vector2.Right.IsUpLeft(), Is.False);
            Assert.That(Vector2.Left.IsUpLeft(), Is.False);

            Assert.That(Vector2.Up.IsDownRight(), Is.False);
            Assert.That(Vector2.Down.IsDownRight(), Is.False);
            Assert.That(Vector2.Right.IsDownRight(), Is.False);
            Assert.That(Vector2.Left.IsDownRight(), Is.False);

            Assert.That(Vector2.Up.IsDownLeft(), Is.False);
            Assert.That(Vector2.Down.IsDownLeft(), Is.False);
            Assert.That(Vector2.Right.IsDownLeft(), Is.False);
            Assert.That(Vector2.Left.IsDownLeft(), Is.False);
        }

        [Test]
        public void NonRegularTests() {

            Assert.That(45F.AngleToVector().IsRight(), Is.True);
            Assert.That(45F.AngleToVector().IsUpRight(), Is.True);
            Assert.That(45F.AngleToVector().IsUp(), Is.True);
            Assert.That(45F.AngleToVector().IsUpLeft(), Is.False);
            Assert.That(45F.AngleToVector().IsLeft(), Is.False);
            Assert.That(45F.AngleToVector().IsDownLeft(), Is.False);
            Assert.That(45F.AngleToVector().IsDown(), Is.False);
            Assert.That(45F.AngleToVector().IsDownRight(), Is.False);

            Assert.That(135F.AngleToVector().IsRight(), Is.False);
            Assert.That(135F.AngleToVector().IsUpRight(), Is.False);
            Assert.That(135F.AngleToVector().IsUp(), Is.True);
            Assert.That(135F.AngleToVector().IsUpLeft(), Is.True);
            Assert.That(135F.AngleToVector().IsLeft(), Is.True);
            Assert.That(135F.AngleToVector().IsDownLeft(), Is.False);
            Assert.That(135F.AngleToVector().IsDown(), Is.False);
            Assert.That(135F.AngleToVector().IsDownRight(), Is.False);

            Assert.That(225F.AngleToVector().IsRight(), Is.False);
            Assert.That(225F.AngleToVector().IsUpRight(), Is.False);
            Assert.That(225F.AngleToVector().IsUp(), Is.False);
            Assert.That(225F.AngleToVector().IsUpLeft(), Is.False);
            Assert.That(225F.AngleToVector().IsLeft(), Is.True);
            Assert.That(225F.AngleToVector().IsDownLeft(), Is.True);
            Assert.That(225F.AngleToVector().IsDown(), Is.True);
            Assert.That(225F.AngleToVector().IsDownRight(), Is.False);

            Assert.That(315F.AngleToVector().IsRight(), Is.True);
            Assert.That(315F.AngleToVector().IsUpRight(), Is.False);
            Assert.That(315F.AngleToVector().IsUp(), Is.False);
            Assert.That(315F.AngleToVector().IsUpLeft(), Is.False);
            Assert.That(315F.AngleToVector().IsLeft(), Is.False);
            Assert.That(315F.AngleToVector().IsDownLeft(), Is.False);
            Assert.That(315F.AngleToVector().IsDown(), Is.True);
            Assert.That(315F.AngleToVector().IsDownRight(), Is.True);
        }

        [Test]
        public void SlopeTests() {
            // The angles are normal collision, so 90º means a flat floor
            Assert.That(90F.AngleToVector().IsFloor(Vector2.Up), Is.True);
            Assert.That(45F.AngleToVector().IsFloor(Vector2.Up), Is.True);
            Assert.That(135F.AngleToVector().IsFloor(Vector2.Up), Is.True);
            
            Assert.That(47F.AngleToVector().IsFloor(Vector2.Up), Is.False);
            Assert.That(42F.AngleToVector().IsFloor(Vector2.Up), Is.False);
            Assert.That(0F.AngleToVector().IsFloor(Vector2.Up), Is.False);
            Assert.That(180F.AngleToVector().IsFloor(Vector2.Up), Is.False);
            Assert.That(270F.AngleToVector().IsFloor(Vector2.Up), Is.False);
            
            // The angles are normal collision, so 270º means a flat ceiling
            Assert.That(270F.AngleToVector().IsCeiling(Vector2.Up), Is.True);
            Assert.That(315F.AngleToVector().IsCeiling(Vector2.Up), Is.True);
            Assert.That(225F.AngleToVector().IsCeiling(Vector2.Up), Is.True);
            
            Assert.That(224F.AngleToVector().IsCeiling(Vector2.Up), Is.False);
            Assert.That(316F.AngleToVector().IsCeiling(Vector2.Up), Is.False);
            Assert.That(0F.AngleToVector().IsCeiling(Vector2.Up), Is.False);
            Assert.That(180F.AngleToVector().IsCeiling(Vector2.Up), Is.False);
            Assert.That(90F.AngleToVector().IsCeiling(Vector2.Up), Is.False);
        }

    }
}