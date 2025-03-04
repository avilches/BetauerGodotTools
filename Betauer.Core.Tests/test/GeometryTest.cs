using System.Linq;
using Betauer.Core.DataMath.Geometry;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class GeometryTest {
    [Test]
    public void GetPairsWithinRatioTest() {
        CollectionAssert.AreEquivalent(Geometry.GetPairsWithinRatio(3, 5, 1, 16f/9), new []{ (3, 3), (4, 3), (4, 4), (5, 3), (5, 4), (5, 5) }.Select(i=> new Vector2I(i.Item1, i.Item2)));
        CollectionAssert.AreEquivalent(Geometry.GetPairsWithinRatio(3, 5, 1, 16f/9, 2), new []{ (3, 3), (5, 3), (5, 5) }.Select(i=> new Vector2I(i.Item1, i.Item2)));
        CollectionAssert.AreEquivalent(Geometry.GetPairsWithinRatio(3, 6, 1, 16f/9, 2), new []{ (3, 3), (5, 3), (5, 5) }.Select(i=> new Vector2I(i.Item1, i.Item2)));
    }

    [Test]
    public void CreateTest() {

        Assert.That(Geometry.CreateRect2I(0.5f, 10, Geometry.RectanglePart.Landscape),Is.EqualTo(new Rect2I(0, 0, 10, 5)));
        Assert.That(Geometry.CreateRect2I(2f, 10, Geometry.RectanglePart.Landscape),Is.EqualTo(new Rect2I(0, 0, 10, 5)));

        Assert.That(Geometry.CreateRect2I(0.5f, 10, Geometry.RectanglePart.Portrait),Is.EqualTo(new Rect2I(0, 0, 5, 10)));
        Assert.That(Geometry.CreateRect2I(2f, 10, Geometry.RectanglePart.Portrait),Is.EqualTo(new Rect2I(0, 0, 5, 10)));

        Assert.That(Geometry.CreateRect2I(0.5f, 10, Geometry.RectanglePart.Ratio),Is.EqualTo(new Rect2I(0, 0, 5, 10)));
        Assert.That(Geometry.CreateRect2I(2f, 10, Geometry.RectanglePart.Ratio),Is.EqualTo(new Rect2I(0, 0, 10, 5)));

    }

    [Test]
    public void ShrinkRect2ToEnsureRatioTest() {
        // Reduce width

        // Landscape: widths are longer than 16/9, so the size is adjusted to 16/9 and the position is centered
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 30, 9, 16f / 9), Is.EqualTo(new Rect2(7, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 24, 9, 16f / 9), Is.EqualTo(new Rect2(4, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 18, 9, 16f / 9), Is.EqualTo(new Rect2(1, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 17, 9, 16f / 9), Is.EqualTo(new Rect2(0.5f, 0, 16, 9)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 30, 9, 9f / 16), Is.EqualTo(new Rect2(7, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 24, 9, 9f / 16), Is.EqualTo(new Rect2(4, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 18, 9, 9f / 16), Is.EqualTo(new Rect2(1, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 17, 9, 9f / 16), Is.EqualTo(new Rect2(0.5f, 0, 16, 9)));

        // Landscape: widths are shorter than 16/9 so the sizes don't need to be adjusted, so the rect keeps the same size and position
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 16, 9, 16f / 9), Is.EqualTo(new Rect2(0, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 15, 9, 16f / 9), Is.EqualTo(new Rect2(0, 0, 15, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 14, 9, 16f / 9), Is.EqualTo(new Rect2(0, 0, 14, 9)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 16, 9, 9f / 16), Is.EqualTo(new Rect2(0, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 15, 9, 9f / 16), Is.EqualTo(new Rect2(0, 0, 15, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 14, 9, 9f / 16), Is.EqualTo(new Rect2(0, 0, 14, 9)));

        // Portrait: heights are longer than 16/9, so the size is adjusted to 16/9 and the position is centered        
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 30, 16f / 9), Is.EqualTo(new Rect2(0, 7, 9, 16)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 24, 16f / 9), Is.EqualTo(new Rect2(0, 4, 9, 16)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 18, 16f / 9), Is.EqualTo(new Rect2(0, 1, 9, 16)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 17, 16f / 9), Is.EqualTo(new Rect2(0, 0.5f, 9, 16)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 30, 9f / 16), Is.EqualTo(new Rect2(0, 7, 9, 16)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 24, 9f / 16), Is.EqualTo(new Rect2(0, 4, 9, 16)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 18, 9f / 16), Is.EqualTo(new Rect2(0, 1, 9, 16)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 9, 17, 9f / 16), Is.EqualTo(new Rect2(0, 0.5f, 9, 16)));

        // Portrait: heights are shorter than 16/9 so the sizes don't need to be adjusted, so the rect keeps the same size and position
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 16, 9, 16f / 9), Is.EqualTo(new Rect2(0, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 15, 9, 16f / 9), Is.EqualTo(new Rect2(0, 0, 15, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 14, 9, 16f / 9), Is.EqualTo(new Rect2(0, 0, 14, 9)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 16, 9, 9f / 16), Is.EqualTo(new Rect2(0, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 15, 9, 9f / 16), Is.EqualTo(new Rect2(0, 0, 15, 9)));
        Assert.That(Geometry.ShrinkRect2ToEnsureRatio(0, 0, 14, 9, 9f / 16), Is.EqualTo(new Rect2(0, 0, 14, 9)));
    }

    [Test]
    public void ShrinkRect2IToEnsureRatioTest() {
        // Reduce width

        // Landscape: widths are longer than 16/9, so the size is adjusted to 16/9 and the position is centered
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 30, 9, 16f / 9), Is.EqualTo(new Rect2I(7, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 24, 9, 16f / 9), Is.EqualTo(new Rect2I(4, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 18, 9, 16f / 9), Is.EqualTo(new Rect2I(1, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 17, 9, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 16, 9)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 30, 9, 9f / 16), Is.EqualTo(new Rect2I(7, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 24, 9, 9f / 16), Is.EqualTo(new Rect2I(4, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 18, 9, 9f / 16), Is.EqualTo(new Rect2I(1, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 17, 9, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 16, 9)));

        // Landscape: widths are shorter than 16/9 so the sizes don't need to be adjusted, so the rect keeps the same size and position
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 16, 9, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 15, 9, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 15, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 14, 9, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 14, 9)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 16, 9, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 16, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 15, 9, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 15, 9)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 14, 9, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 14, 9)));

        // Portrait: heights are longer than 16/9, so the size is adjusted to 16/9 and the position is centered        
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 30, 16f / 9), Is.EqualTo(new Rect2I(0, 7, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 24, 16f / 9), Is.EqualTo(new Rect2I(0, 4, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 18, 16f / 9), Is.EqualTo(new Rect2I(0, 1, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 17, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 9, 16)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 30, 9f / 16), Is.EqualTo(new Rect2I(0, 7, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 24, 9f / 16), Is.EqualTo(new Rect2I(0, 4, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 18, 9f / 16), Is.EqualTo(new Rect2I(0, 1, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 17, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 9, 16)));

        // Portrait: heights are shorter than 16/9 so the sizes don't need to be adjusted, so the rect keeps the same size and position
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 16, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 15, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 9, 15)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 14, 16f / 9), Is.EqualTo(new Rect2I(0, 0, 9, 14)));
        // Extra tests with inverted ratio
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 16, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 9, 16)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 15, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 9, 15)));
        Assert.That(Geometry.ShrinkRect2IToEnsureRatio(0, 0, 9, 14, 9f / 16), Is.EqualTo(new Rect2I(0, 0, 9, 14)));
    }
}