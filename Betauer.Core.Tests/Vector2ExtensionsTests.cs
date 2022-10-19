using System.Collections.Generic;
using Betauer.Reflection;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class Vector2ExtensionsTest {
        [Test]
        public void UpDownTests() {

            Assert.That(Vector2.Up.Down(), Is.EqualTo(Vector2.Down));
            Assert.That(Vector2.Right.Down(), Is.EqualTo(Vector2.Left));
            Assert.That(Vector2.Left.Down(), Is.EqualTo(Vector2.Right));
            Assert.That(Vector2.Down.Down(), Is.EqualTo(Vector2.Up));

            Assert.That(Vector2.Up.Left(), Is.EqualTo(Vector2.Left));
            Assert.That(Vector2.Right.Left(), Is.EqualTo(Vector2.Up));
            Assert.That(Vector2.Left.Left(), Is.EqualTo(Vector2.Down));
            Assert.That(Vector2.Down.Left(), Is.EqualTo(Vector2.Right));

            Assert.That(Vector2.Up.Right(), Is.EqualTo(Vector2.Right));
            Assert.That(Vector2.Right.Right(), Is.EqualTo(Vector2.Down));
            Assert.That(Vector2.Left.Right(), Is.EqualTo(Vector2.Up));
            Assert.That(Vector2.Down.Right(), Is.EqualTo(Vector2.Left));
        }


        public void RegularTests() {

            // Ok
            Assert.That(Vector2.Up.IsUp(Vector2.Up));
            Assert.That(Vector2.Down.IsDown(Vector2.Up));
            Assert.That(Vector2.Right.IsRight(Vector2.Up));
            Assert.That(Vector2.Left.IsLeft(Vector2.Up));
            
            // No ok
            Assert.That(Vector2.Down.IsUp(Vector2.Up), Is.False);
            Assert.That(Vector2.Right.IsUp(Vector2.Up), Is.False);
            Assert.That(Vector2.Left.IsUp(Vector2.Up), Is.False);

            Assert.That(Vector2.Up.IsDown(Vector2.Up), Is.False);
            Assert.That(Vector2.Right.IsDown(Vector2.Up), Is.False);
            Assert.That(Vector2.Left.IsDown(Vector2.Up), Is.False);

            Assert.That(Vector2.Up.IsRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Down.IsRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Left.IsRight(Vector2.Up), Is.False);

            Assert.That(Vector2.Up.IsLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Down.IsLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Right.IsLeft(Vector2.Up), Is.False);

            // 
            Assert.That(Vector2.Up.IsUpRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Down.IsUpRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Right.IsUpRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Left.IsUpRight(Vector2.Up), Is.False);

            Assert.That(Vector2.Up.IsUpLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Down.IsUpLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Right.IsUpLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Left.IsUpLeft(Vector2.Up), Is.False);

            Assert.That(Vector2.Up.IsDownRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Down.IsDownRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Right.IsDownRight(Vector2.Up), Is.False);
            Assert.That(Vector2.Left.IsDownRight(Vector2.Up), Is.False);

            Assert.That(Vector2.Up.IsDownLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Down.IsDownLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Right.IsDownLeft(Vector2.Up), Is.False);
            Assert.That(Vector2.Left.IsDownLeft(Vector2.Up), Is.False);
        }

        [Test]
        public void NonRegularFacingUpTests() {

            Assert.That(45F.AngleToVector().IsRight(Vector2.Up), Is.True);
            Assert.That(45F.AngleToVector().IsUpRight(Vector2.Up), Is.True);
            Assert.That(45F.AngleToVector().IsUp(Vector2.Up), Is.True);
            Assert.That(45F.AngleToVector().IsUpLeft(Vector2.Up), Is.False);
            Assert.That(45F.AngleToVector().IsLeft(Vector2.Up), Is.False);
            Assert.That(45F.AngleToVector().IsDownLeft(Vector2.Up), Is.False);
            Assert.That(45F.AngleToVector().IsDown(Vector2.Up), Is.False);
            Assert.That(45F.AngleToVector().IsDownRight(Vector2.Up), Is.False);

            Assert.That(135F.AngleToVector().IsRight(Vector2.Up), Is.False);
            Assert.That(135F.AngleToVector().IsUpRight(Vector2.Up), Is.False);
            Assert.That(135F.AngleToVector().IsUp(Vector2.Up), Is.True);
            Assert.That(135F.AngleToVector().IsUpLeft(Vector2.Up), Is.True);
            Assert.That(135F.AngleToVector().IsLeft(Vector2.Up), Is.True);
            Assert.That(135F.AngleToVector().IsDownLeft(Vector2.Up), Is.False);
            Assert.That(135F.AngleToVector().IsDown(Vector2.Up), Is.False);
            Assert.That(135F.AngleToVector().IsDownRight(Vector2.Up), Is.False);

            Assert.That(225F.AngleToVector().IsRight(Vector2.Up), Is.False);
            Assert.That(225F.AngleToVector().IsUpRight(Vector2.Up), Is.False);
            Assert.That(225F.AngleToVector().IsUp(Vector2.Up), Is.False);
            Assert.That(225F.AngleToVector().IsUpLeft(Vector2.Up), Is.False);
            Assert.That(225F.AngleToVector().IsLeft(Vector2.Up), Is.True);
            Assert.That(225F.AngleToVector().IsDownLeft(Vector2.Up), Is.True);
            Assert.That(225F.AngleToVector().IsDown(Vector2.Up), Is.True);
            Assert.That(225F.AngleToVector().IsDownRight(Vector2.Up), Is.False);

            Assert.That(315F.AngleToVector().IsRight(Vector2.Up), Is.True);
            Assert.That(315F.AngleToVector().IsUpRight(Vector2.Up), Is.False);
            Assert.That(315F.AngleToVector().IsUp(Vector2.Up), Is.False);
            Assert.That(315F.AngleToVector().IsUpLeft(Vector2.Up), Is.False);
            Assert.That(315F.AngleToVector().IsLeft(Vector2.Up), Is.False);
            Assert.That(315F.AngleToVector().IsDownLeft(Vector2.Up), Is.False);
            Assert.That(315F.AngleToVector().IsDown(Vector2.Up), Is.True);
            Assert.That(315F.AngleToVector().IsDownRight(Vector2.Up), Is.True);
        }

        [Test]
        public void NonRegularFacingRightTests() {
            // So, if floor is facing right, a 45º angle is facing to the up-left quarter
            Assert.That(45F.AngleToVector().IsRight(Vector2.Right), Is.False);
            Assert.That(45F.AngleToVector().IsUpRight(Vector2.Right), Is.False);
            Assert.That(45F.AngleToVector().IsUp(Vector2.Right), Is.True);
            Assert.That(45F.AngleToVector().IsUpLeft(Vector2.Right), Is.True);
            Assert.That(45F.AngleToVector().IsLeft(Vector2.Right), Is.True);
            Assert.That(45F.AngleToVector().IsDownLeft(Vector2.Right), Is.False);
            Assert.That(45F.AngleToVector().IsDown(Vector2.Right), Is.False);
            Assert.That(45F.AngleToVector().IsDownRight(Vector2.Right), Is.False);
        }

        [Test]
        public void SlopeFacingUpTests() {
            // The angles are normal collision, so 90º means a flat floor
            Assert.That(90F.AngleToVector().IsFloor(Vector2.Up), Is.True);
            Assert.That(45F.AngleToVector().IsFloor(Vector2.Up), Is.True);
            Assert.That(135F.AngleToVector().IsFloor(Vector2.Up), Is.True);
            
            Assert.That(137F.AngleToVector().IsFloor(Vector2.Up), Is.False);
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

        [Test]
        public void SlopeFacingRightTests() {
            // The angles are normal collision, so 90º means a flat floor. But, facing to the right, 0º means flat floor
            Assert.That(45F.AngleToVector().IsFloor(Vector2.Right), Is.True);
            Assert.That(0F.AngleToVector().IsFloor(Vector2.Right), Is.True);
            Assert.That(315F.AngleToVector().IsFloor(Vector2.Right), Is.True);

            Assert.That(314F.AngleToVector().IsFloor(Vector2.Right), Is.False);
            Assert.That(47F.AngleToVector().IsFloor(Vector2.Right), Is.False);
            Assert.That(270F.AngleToVector().IsFloor(Vector2.Right), Is.False);
            Assert.That(180F.AngleToVector().IsFloor(Vector2.Right), Is.False);
            Assert.That(90F.AngleToVector().IsFloor(Vector2.Right), Is.False);
        }
    }
}