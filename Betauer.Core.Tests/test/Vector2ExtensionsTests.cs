using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestFixture]
public class Vector2ExtensionsTest {
    [Test]
    public void UpDownTests() {

        Assert.That(Vector2.Up.Rotate180(), Is.EqualTo(Vector2.Down));
        Assert.That(Vector2.Right.Rotate180(), Is.EqualTo(Vector2.Left));
        Assert.That(Vector2.Left.Rotate180(), Is.EqualTo(Vector2.Right));
        Assert.That(Vector2.Down.Rotate180(), Is.EqualTo(Vector2.Up));

        Assert.That(Vector2.Up.Rotate90Left(), Is.EqualTo(Vector2.Left));
        Assert.That(Vector2.Right.Rotate90Left(), Is.EqualTo(Vector2.Up));
        Assert.That(Vector2.Left.Rotate90Left(), Is.EqualTo(Vector2.Down));
        Assert.That(Vector2.Down.Rotate90Left(), Is.EqualTo(Vector2.Right));

        Assert.That(Vector2.Up.Rotate90Right(), Is.EqualTo(Vector2.Right));
        Assert.That(Vector2.Right.Rotate90Right(), Is.EqualTo(Vector2.Down));
        Assert.That(Vector2.Left.Rotate90Right(), Is.EqualTo(Vector2.Up));
        Assert.That(Vector2.Down.Rotate90Right(), Is.EqualTo(Vector2.Left));
    }


    public void RegularTests() {

        // Ok
        Assert.That(Vector2.Up.IsSameDirection(Vector2.Up));
        Assert.That(Vector2.Down.IsOppositeDirection(Vector2.Up));
        Assert.That(Vector2.Right.IsRight(Vector2.Up));
        Assert.That(Vector2.Left.IsLeft(Vector2.Up));
            
        // No ok
        Assert.That(Vector2.Down.IsSameDirection(Vector2.Up), Is.False);
        Assert.That(Vector2.Right.IsSameDirection(Vector2.Up), Is.False);
        Assert.That(Vector2.Left.IsSameDirection(Vector2.Up), Is.False);

        Assert.That(Vector2.Up.IsOppositeDirection(Vector2.Up), Is.False);
        Assert.That(Vector2.Right.IsOppositeDirection(Vector2.Up), Is.False);
        Assert.That(Vector2.Left.IsOppositeDirection(Vector2.Up), Is.False);

        Assert.That(Vector2.Up.IsRight(Vector2.Up), Is.False);
        Assert.That(Vector2.Down.IsRight(Vector2.Up), Is.False);
        Assert.That(Vector2.Left.IsRight(Vector2.Up), Is.False);

        Assert.That(Vector2.Up.IsLeft(Vector2.Up), Is.False);
        Assert.That(Vector2.Down.IsLeft(Vector2.Up), Is.False);
        Assert.That(Vector2.Right.IsLeft(Vector2.Up), Is.False);

    }

    [Test]
    public void NonRegularFacingUpTests() {

        Assert.That(45F.AngleToVector().IsRight(Vector2.Up), Is.True);
        Assert.That(45F.AngleToVector().IsSameDirection(Vector2.Up), Is.True);
        Assert.That(45F.AngleToVector().IsLeft(Vector2.Up), Is.False);
        Assert.That(45F.AngleToVector().IsOppositeDirection(Vector2.Up), Is.False);

        Assert.That(135F.AngleToVector().IsRight(Vector2.Up), Is.False);
        Assert.That(135F.AngleToVector().IsSameDirection(Vector2.Up), Is.True);
        Assert.That(135F.AngleToVector().IsLeft(Vector2.Up), Is.True);
        Assert.That(135F.AngleToVector().IsOppositeDirection(Vector2.Up), Is.False);

        Assert.That(225F.AngleToVector().IsRight(Vector2.Up), Is.False);
        Assert.That(225F.AngleToVector().IsSameDirection(Vector2.Up), Is.False);
        Assert.That(225F.AngleToVector().IsLeft(Vector2.Up), Is.True);
        Assert.That(225F.AngleToVector().IsOppositeDirection(Vector2.Up), Is.True);

        Assert.That(315F.AngleToVector().IsRight(Vector2.Up), Is.True);
        Assert.That(315F.AngleToVector().IsSameDirection(Vector2.Up), Is.False);
        Assert.That(315F.AngleToVector().IsLeft(Vector2.Up), Is.False);
        Assert.That(315F.AngleToVector().IsOppositeDirection(Vector2.Up), Is.True);
    }

    [Test]
    public void NonRegularFacingRightTests() {
        // So, if floor is facing right, a 45º angle is facing to the up-left quarter
        Assert.That(45F.AngleToVector().IsRight(Vector2.Right), Is.False);
        Assert.That(45F.AngleToVector().IsSameDirection(Vector2.Right), Is.True);
        Assert.That(45F.AngleToVector().IsLeft(Vector2.Right), Is.True);
        Assert.That(45F.AngleToVector().IsOppositeDirection(Vector2.Right), Is.False);
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

internal static class AngleExtensions {
    public static Vector2 AngleToVector(this float angle) {
        var rad = Mathf.DegToRad(angle);
        return new Vector2(Mathf.Cos(rad), -Mathf.Sin(rad));
    }
}
