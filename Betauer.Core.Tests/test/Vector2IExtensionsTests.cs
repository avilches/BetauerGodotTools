using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class Vector2IExtensionsTests {
    [Test]
    public void Inverse_ReturnsNegatedVector() {
        // Arrange
        var vector = new Vector2I(3, -5);

        // Act
        var result = vector.Inverse();

        // Assert
        Assert.AreEqual(new Vector2I(-3, 5), result);
    }

    [Test]
    public void ToDirectionString_CardinalDirections_ReturnsCorrectString() {
        // Arrange & Act & Assert
        Assert.AreEqual("Up", Vector2I.Up.ToDirectionString());
        Assert.AreEqual("Right", Vector2I.Right.ToDirectionString());
        Assert.AreEqual("Down", Vector2I.Down.ToDirectionString());
        Assert.AreEqual("Left", Vector2I.Left.ToDirectionString());
    }

    [Test]
    public void ToDirectionString_NonCardinalDirection_ReturnsVectorString() {
        // Arrange
        var vector = new Vector2I(1, 1);

        // Act
        var result = vector.ToDirectionString();

        // Assert
        Assert.AreEqual(vector.ToString(), result);
    }

    [Test]
    public void SameDirection_WithDirection_PointsInLine_ReturnsTrue() {
        // Arrange
        var start = new Vector2I(5, 5);
        var direction = Vector2I.Right;
        var end = new Vector2I(10, 5);

        // Act & Assert
        Assert.IsTrue(start.IsSameDirection(direction, end));
    }

    [Test]
    public void SameDirection_WithDirection_PointsNotInLine_ReturnsFalse() {
        // Arrange
        var start = new Vector2I(5, 5);
        var direction = Vector2I.Right;
        var end = new Vector2I(10, 6);

        // Act & Assert
        Assert.IsFalse(start.IsSameDirection(direction, end));
    }

    [Test]
    public void SameDirection_WithDirection_OppositeDirection_ReturnsFalse() {
        // Arrange
        var start = new Vector2I(5, 5);
        var direction = Vector2I.Right;
        var end = new Vector2I(3, 5);

        // Act & Assert
        Assert.IsFalse(start.IsSameDirection(direction, end));
    }

    [Test]
    public void SameDirection_PointsInHorizontalLine_ReturnsTrue() {
        // Arrange
        var start = new Vector2I(5, 5);
        var end = new Vector2I(10, 5);

        // Act & Assert
        Assert.IsTrue(start.IsOrthogonal(end));
    }

    [Test]
    public void SameDirection_PointsInVerticalLine_ReturnsTrue() {
        // Arrange
        var start = new Vector2I(5, 5);
        var end = new Vector2I(5, 10);

        // Act & Assert
        Assert.IsTrue(start.IsOrthogonal(end));
    }

    [Test]
    public void SameDirection_PointsNotInStraightLine_ReturnsFalse() {
        // Arrange
        var start = new Vector2I(5, 5);
        var end = new Vector2I(10, 8);

        // Act & Assert
        Assert.IsFalse(start.IsOrthogonal(end));
    }

    [Test]
    public void GetPositions_WithDirectionAndLength_ReturnsCorrectPositions() {
        // Arrange
        var start = new Vector2I(5, 5);
        var direction = Vector2I.Right;
        var length = 3;
        var expected = new List<Vector2I> {
            new(5, 5),
            new(6, 5),
            new(7, 5),
            new(8, 5)
        };

        // Act
        var positions = start.GetPositions(direction, length).ToList();

        // Assert
        Assert.AreEqual(expected.Count, positions.Count);
        for (int i = 0; i < expected.Count; i++) {
            Assert.AreEqual(expected[i], positions[i]);
        }
    }

    [Test]
    public void GetPositions_WithStartAndEnd_HorizontalLine_ReturnsCorrectPositions() {
        // Arrange
        var start = new Vector2I(5, 5);
        var end = new Vector2I(8, 5);
        var expected = new List<Vector2I> {
            new(5, 5),
            new(6, 5),
            new(7, 5),
            new(8, 5)
        };

        // Act
        var positions = start.GetPositions(end).ToList();

        // Assert
        Assert.AreEqual(expected.Count, positions.Count);
        for (int i = 0; i < expected.Count; i++) {
            Assert.AreEqual(expected[i], positions[i]);
        }
    }

    [Test]
    public void GetPositions_WithStartAndEnd_VerticalLine_ReturnsCorrectPositions() {
        // Arrange
        var start = new Vector2I(5, 5);
        var end = new Vector2I(5, 8);
        var expected = new List<Vector2I> {
            new(5, 5),
            new(5, 6),
            new(5, 7),
            new(5, 8)
        };

        // Act
        var positions = start.GetPositions(end).ToList();

        // Assert
        Assert.AreEqual(expected.Count, positions.Count);
        for (int i = 0; i < expected.Count; i++) {
            Assert.AreEqual(expected[i], positions[i]);
        }
    }

    [Test]
    public void GetPositions_WithStartAndEnd_NotAligned_ThrowsException() {
        // Arrange
        var start = new Vector2I(5, 5);
        var end = new Vector2I(8, 7);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => start.GetPositions(end).ToList());
    }

    [Test]
    public void DirectionTo_HorizontalPositions_ReturnsCorrectDirection() {
        // Arrange
        var start = new Vector2I(5, 5);
        var endRight = new Vector2I(10, 5);
        var endLeft = new Vector2I(2, 5);

        // Act & Assert
        Assert.AreEqual(Vector2I.Right, start.GetOrthogonalDirectionTo(endRight));
        Assert.AreEqual(Vector2I.Left, start.GetOrthogonalDirectionTo(endLeft));
    }

    [Test]
    public void DirectionTo_VerticalPositions_ReturnsCorrectDirection() {
        // Arrange
        var start = new Vector2I(5, 5);
        var endUp = new Vector2I(5, 2);
        var endDown = new Vector2I(5, 10);

        // Act & Assert
        Assert.AreEqual(Vector2I.Up, start.GetOrthogonalDirectionTo(endUp));
        Assert.AreEqual(Vector2I.Down, start.GetOrthogonalDirectionTo(endDown));
    }

    [Test]
    public void DirectionTo_DiagonalPositions_ThrowsException() {
        // Arrange
        var start = new Vector2I(5, 5);
        var end = new Vector2I(7, 8);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => start.GetOrthogonalDirectionTo(end));
    }

    [Test]
    public void IsValidDirection_CardinalDirections_ReturnsTrue() {
        // Act & Assert
        Assert.IsTrue(Vector2I.Up.IsOrthogonalDirection());
        Assert.IsTrue(Vector2I.Right.IsOrthogonalDirection());
        Assert.IsTrue(Vector2I.Down.IsOrthogonalDirection());
        Assert.IsTrue(Vector2I.Left.IsOrthogonalDirection());
    }

    [Test]
    public void IsValidDirection_NonCardinalDirections_ReturnsFalse() {
        // Arrange
        var diag = new Vector2I(1, 1);
        var zero = Vector2I.Zero;
        var big = new Vector2I(2, 0);

        // Act & Assert
        Assert.IsFalse(diag.IsOrthogonalDirection());
        Assert.IsFalse(zero.IsOrthogonalDirection());
        Assert.IsFalse(big.IsOrthogonalDirection());
    }

    [Test]
    public void IsHorizontal_HorizontalDirections_ReturnsTrue() {
        // Act & Assert
        Assert.IsTrue(Vector2I.Right.IsHorizontal());
        Assert.IsTrue(Vector2I.Left.IsHorizontal());
        Assert.IsTrue(new Vector2I(5, 0).IsHorizontal());
    }

    [Test]
    public void IsHorizontal_NonHorizontalDirections_ReturnsFalse() {
        // Act & Assert
        Assert.IsFalse(Vector2I.Up.IsHorizontal());
        Assert.IsFalse(Vector2I.Down.IsHorizontal());
        Assert.IsFalse(new Vector2I(1, 1).IsHorizontal());
    }

    [Test]
    public void IsVertical_VerticalDirections_ReturnsTrue() {
        // Act & Assert
        Assert.IsTrue(Vector2I.Up.IsVertical());
        Assert.IsTrue(Vector2I.Down.IsVertical());
        Assert.IsTrue(new Vector2I(0, 5).IsVertical());
    }

    [Test]
    public void IsVertical_NonVerticalDirections_ReturnsFalse() {
        // Act & Assert
        Assert.IsFalse(Vector2I.Right.IsVertical());
        Assert.IsFalse(Vector2I.Left.IsVertical());
        Assert.IsFalse(new Vector2I(1, 1).IsVertical());
    }

    [Test]
    public void IsPerpendicular_PerpendicularDirections_ReturnsTrue() {
        // Act & Assert
        Assert.IsTrue(Vector2I.Up.IsPerpendicular(Vector2I.Right));
        Assert.IsTrue(Vector2I.Right.IsPerpendicular(Vector2I.Up));
        Assert.IsTrue(Vector2I.Down.IsPerpendicular(Vector2I.Left));
        Assert.IsTrue(Vector2I.Left.IsPerpendicular(Vector2I.Down));
    }

    [Test]
    public void IsPerpendicular_NonPerpendicularDirections_ReturnsFalse() {
        // Act & Assert
        Assert.IsFalse(Vector2I.Up.IsPerpendicular(Vector2I.Down));
        Assert.IsFalse(Vector2I.Right.IsPerpendicular(Vector2I.Left));
        Assert.IsFalse(Vector2I.Up.IsPerpendicular(new Vector2I(1, 1)));
    }

    [Test]
    public void IsParallel_ParallelDirections_ReturnsTrue() {
        // Act & Assert
        Assert.IsTrue(Vector2I.Up.IsParallel(Vector2I.Up));
        Assert.IsTrue(Vector2I.Right.IsParallel(Vector2I.Right));
        Assert.IsTrue(Vector2I.Up.IsParallel(Vector2I.Down));
        Assert.IsTrue(Vector2I.Right.IsParallel(Vector2I.Left));
        Assert.IsTrue(new Vector2I(2, 0).IsParallel(Vector2I.Right));
    }

    [Test]
    public void IsParallel_NonParallelDirections_ReturnsFalse() {
        // Act & Assert
        Assert.IsFalse(Vector2I.Up.IsParallel(Vector2I.Right));
        Assert.IsFalse(Vector2I.Right.IsParallel(Vector2I.Up));
        Assert.IsFalse(new Vector2I(1, 1).IsParallel(Vector2I.Up));
    }

    [Test]
    public void Rotate180_ReturnsCorrectVector() {
        // Act & Assert
        Assert.AreEqual(Vector2I.Down, Vector2I.Up.Rotate180());
        Assert.AreEqual(Vector2I.Left, Vector2I.Right.Rotate180());
        Assert.AreEqual(Vector2I.Up, Vector2I.Down.Rotate180());
        Assert.AreEqual(Vector2I.Right, Vector2I.Left.Rotate180());
        Assert.AreEqual(new Vector2I(-2, -3), new Vector2I(2, 3).Rotate180());
    }

    [Test]
    public void Rotate90Left_ReturnsCorrectVector() {
        // Act & Assert
        Assert.AreEqual(Vector2I.Left, Vector2I.Up.Rotate90Left());
        Assert.AreEqual(Vector2I.Up, Vector2I.Right.Rotate90Left());
        Assert.AreEqual(Vector2I.Right, Vector2I.Down.Rotate90Left());
        Assert.AreEqual(Vector2I.Down, Vector2I.Left.Rotate90Left());
        Assert.AreEqual(new Vector2I(3, -2), new Vector2I(2, 3).Rotate90Left());
    }

    [Test]
    public void Rotate90Right_ReturnsCorrectVector() {
        // Act & Assert
        Assert.AreEqual(Vector2I.Right, Vector2I.Up.Rotate90Right());
        Assert.AreEqual(Vector2I.Down, Vector2I.Right.Rotate90Right());
        Assert.AreEqual(Vector2I.Left, Vector2I.Down.Rotate90Right());
        Assert.AreEqual(Vector2I.Up, Vector2I.Left.Rotate90Right());
        Assert.AreEqual(new Vector2I(-3, 2), new Vector2I(2, 3).Rotate90Right());
    }

    [Test]
    public void PositionHelpers_ReturnCorrectPositions() {
        // Arrange
        var pos = new Vector2I(5, 5);

        // Act & Assert
        Assert.AreEqual(new Vector2I(4, 4), pos.UpLeftPos());
        Assert.AreEqual(new Vector2I(6, 4), pos.UpRightPos());
        Assert.AreEqual(new Vector2I(4, 6), pos.DownLeftPos());
        Assert.AreEqual(new Vector2I(6, 6), pos.DownRightPos());
        Assert.AreEqual(new Vector2I(4, 5), pos.LeftPos());
        Assert.AreEqual(new Vector2I(6, 5), pos.RightPos());
        Assert.AreEqual(new Vector2I(5, 4), pos.UpPos());
        Assert.AreEqual(new Vector2I(5, 6), pos.DownPos());
    }

    [Test]
    public void Clockwise_ReturnsCorrectDirection() {
        // Act & Assert
        Assert.AreEqual(Vector2I.Right, Vector2I.Up.Clockwise());
        Assert.AreEqual(Vector2I.Down, Vector2I.Right.Clockwise());
        Assert.AreEqual(Vector2I.Left, Vector2I.Down.Clockwise());
        Assert.AreEqual(Vector2I.Up, Vector2I.Left.Clockwise());
    }

    [Test]
    public void CounterClockwise_ReturnsCorrectDirection() {
        // Act & Assert
        Assert.AreEqual(Vector2I.Left, Vector2I.Up.CounterClockwise());
        Assert.AreEqual(Vector2I.Up, Vector2I.Right.CounterClockwise());
        Assert.AreEqual(Vector2I.Right, Vector2I.Down.CounterClockwise());
        Assert.AreEqual(Vector2I.Down, Vector2I.Left.CounterClockwise());
    }

    [Test]
    public void DistanceTo_ReturnsCorrectDistance() {
        // Arrange
        var pos1 = new Vector2I(0, 0);
        var pos2 = new Vector2I(3, 4);
        var expectedDistance = 5f; // 3-4-5 triangle

        // Act
        var distance = pos1.DistanceTo(pos2);

        // Assert
        Assert.AreEqual(expectedDistance, distance);
    }

    // Add to Vector2IExtensionsTests.cs
    [Test]
    public void ManhattanDistanceTo_ReturnsCorrectDistance() {
        // Arrange
        var pos1 = new Vector2I(0, 0);
        var pos2 = new Vector2I(3, 4);
        var expectedDistance = 7f; // |3-0| + |4-0| = 3 + 4 = 7

        // Act
        var distance = pos1.ManhattanDistanceTo(pos2);

        // Assert
        Assert.AreEqual(expectedDistance, distance);
    }

    [Test]
    public void EuclideanSquaredDistanceTo_ReturnsCorrectDistance() {
        // Arrange
        var pos1 = new Vector2I(0, 0);
        var pos2 = new Vector2I(3, 4);
        var expectedDistance = 25f; // (3-0)² + (4-0)² = 9 + 16 = 25

        // Act
        var distance = pos1.DistanceSquaredTo(pos2);

        // Assert
        Assert.AreEqual(expectedDistance, distance);
    }

    [Test]
    public void ChebyshevDistanceTo_ReturnsCorrectDistance() {
        // Arrange
        var pos1 = new Vector2I(0, 0);
        var pos2 = new Vector2I(3, 4);
        var expectedDistance = 4f; // max(|3-0|, |4-0|) = max(3, 4) = 4

        // Act
        var distance = pos1.ChebyshevDistanceTo(pos2);

        // Assert
        Assert.AreEqual(expectedDistance, distance);
    }

    [Test]
    public void IsSameLine_SameLinePoints_ReturnsTrue() {
        // Arrange
        var point1 = new Vector2I(5, 5);
        var direction1 = Vector2I.Right;
        var point2 = new Vector2I(10, 5);
        var direction2 = Vector2I.Right;

        // Act & Assert
        Assert.IsTrue(point1.IsSameLine(direction1, point2, direction2));
    }

    [Test]
    public void IsSameLine_SameLineOppositeDirections_ReturnsTrue() {
        // Arrange
        var point1 = new Vector2I(5, 5);
        var direction1 = Vector2I.Right;
        var point2 = new Vector2I(10, 5);
        var direction2 = Vector2I.Left;

        // Act & Assert
        Assert.IsTrue(point1.IsSameLine(direction1, point2, direction2));
    }

    [Test]
    public void IsSameLine_DifferentLines_ReturnsFalse() {
        // Arrange
        var point1 = new Vector2I(5, 5);
        var direction1 = Vector2I.Right;
        var point2 = new Vector2I(5, 10);
        var direction2 = Vector2I.Right;

        // Act & Assert
        Assert.IsFalse(point1.IsSameLine(direction1, point2, direction2));
    }

    [Test]
    public void IsSameLine_DiagonalSameLines_ReturnsTrue() {
        // Arrange
        var point1 = new Vector2I(0, 0);
        var direction1 = new Vector2I(1, 1);
        var point2 = new Vector2I(5, 5);
        var direction2 = new Vector2I(2, 2);

        // Act & Assert
        Assert.IsTrue(point1.IsSameLine(direction1, point2, direction2));
    }

    [Test]
    public void SameDirection_DiagonalDirections_PointsInLine_ReturnsTrue() {
        // Arrange
        var start = new Vector2I(0, 0);
        var direction = new Vector2I(1, 1);
        var end = new Vector2I(5, 5);

        // Act & Assert
        Assert.IsTrue(start.IsSameDirection(direction, end));
    }

    [Test]
    public void SameDirection_DiagonalDirections_PointsNotInLine_ReturnsFalse() {
        // Arrange
        var start = new Vector2I(0, 0);
        var direction = new Vector2I(1, 1);
        var end = new Vector2I(5, 6);

        // Act & Assert
        Assert.IsFalse(start.IsSameDirection(direction, end));
    }

    [Test]
    public void SameDirection_ZeroDirection_SamePoint_ReturnsTrue() {
        // Arrange
        var start = new Vector2I(5, 5);
        var direction = Vector2I.Zero;
        var end = new Vector2I(5, 5);

        // Act & Assert
        Assert.IsTrue(start.IsSameDirection(direction, end));
    }

    [Test]
    public void SameDirection_ZeroDirection_DifferentPoint_ReturnsFalse() {
        // Arrange
        var start = new Vector2I(5, 5);
        var direction = Vector2I.Zero;
        var end = new Vector2I(6, 5);

        // Act & Assert
        Assert.IsFalse(start.IsSameDirection(direction, end));
    }

    [Test]
    public void SameDirection_NonMultiplePoints_SameDirection_ReturnsTrue() {
        // Arrange
        var start = new Vector2I(0, 0);
        var direction = new Vector2I(2, 2);
        var end = new Vector2I(5, 5); // Not a multiple of (2,2)

        // Act & Assert
        Assert.IsTrue(start.IsSameDirection(direction, end));
    }
}