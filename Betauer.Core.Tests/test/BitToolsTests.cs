using System;
using System.Diagnostics;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class BitToolsTests {
    [Test]
    public void EnableDisableHasTest() {
        for (var i = 1; i < 20; i++) {
            int bit = 0b00000000000000000000;
            Assert.That(BitTools.HasBit(bit, i), Is.False);
            bit = BitTools.SetBit(0, i, true);
            Assert.That(BitTools.HasBit(bit, i), Is.True);
        }

        for (var i = 1; i < 20; i++) {
            int bit = 0b11111111111111111111;
            Assert.That(BitTools.HasBit(bit, i), Is.True);
            bit = BitTools.SetBit(0, i, false);
            Assert.That(BitTools.HasBit(bit, i), Is.False);
        }
    }

    public static bool IsNotTooWide(double width, double height, double maxRatio) {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive numbers.");

        // Current ratio as long side / short side, it will return 1 or more, like 1.7777 for a 16:9 ratio
        var ratio = Math.Max(width, height) / Math.Min(width, height);

        // If ratio sent is in the range of 0 to 1, we need to invert it. So, a ratio of 0.33 (1/3) will be 3 (3:1)
        var ratioLimit = maxRatio < 1 ? 1.0 / maxRatio : maxRatio;

        // True if current ratio is less or equal to the limit ratio
        return ratio <= ratioLimit;
    }

    [Test]
    public void TestRectangleValidation() {
        Assert.IsTrue(IsNotTooWide(16, 9, 9f / 16));
        Assert.IsTrue(IsNotTooWide(16, 9, 16f / 9));
        Assert.IsTrue(IsNotTooWide(9, 16, 9f / 16));
        Assert.IsTrue(IsNotTooWide(9, 16, 16f / 9));

        Assert.IsTrue(IsNotTooWide(10, 9, 9f / 16));
        Assert.IsTrue(IsNotTooWide(10, 9, 16f / 9));
        Assert.IsTrue(IsNotTooWide(9, 10, 9f / 16));
        Assert.IsTrue(IsNotTooWide(9, 10, 16f / 9));

        // Too wide
        Assert.IsFalse(IsNotTooWide(17, 9, 9f / 16));
        Assert.IsFalse(IsNotTooWide(17, 9, 16f / 9));
        Assert.IsFalse(IsNotTooWide(9, 17, 9f / 16));
        Assert.IsFalse(IsNotTooWide(9, 17, 16f / 9));
    }
    
     [Test]
    public void GetDivisionRatio_ValidRatio_ShouldReturnExpectedValue() {
        
        RectangleDivider.GetDivisionRatioLoop(3, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(4, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(5, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(6, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(7, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(8, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(9, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(10, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(11, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(12, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(13, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(14, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(15, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(16, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(17, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(18, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(19, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(20, 5, 16f / 9);
        RectangleDivider.GetDivisionRatioLoop(21, 5, 16f / 9);
        

    }
    
}

public class RectangleDivider
{
     public static double GetDivisionRatioLoop(double width, double height, double maxAspectRatio)
    {
        if (height <= 0 || width <= 0)
            throw new ArgumentException("El ancho y la altura deben ser mayores que cero.");

        double found = -1f;

        Console.WriteLine($"Dividiendo rectángulo de {width} x {height} con ratio máximo {maxAspectRatio:0.00}");
        for (double ratio = 0.1; ratio <= 0.91; ratio += 0.01) {
            double part1Width = ratio * width;
            double part2Width = (1 - ratio) * width;
            double part1Ratio = Math.Max(part1Width, height) / Math.Min(part1Width, height);
            double part2Ratio = Math.Max(part2Width, height) / Math.Min(part2Width, height);
            if (IsNotTooWide(part1Width, height, maxAspectRatio) && IsNotTooWide(part2Width, height, maxAspectRatio)) {
                Console.WriteLine($"- Ratio: {ratio:0.00}. Partes: Rectángulo 1: {part1Width:0.00} x {height} (ratio {part1Ratio:0.00}), " +
                                  $"Rectángulo 2: {part2Width:0.00} x {height} (ratio {part2Ratio:0.00})");
                // Console.WriteLine($"- División encontrada en {ratio:0.00}");
                // if (found < 0) {
                    // found = ratio;
                    // return ratio;
                // }
                // return ratio;
            } else {
                Console.WriteLine($"- Ratio: {ratio:0.00}. Partes: Rectángulo 1: {part1Width:0.00} x {height} (ratio {part1Ratio:0.00}), " +
                                  $"Rectángulo 2: {part2Width:0.00} x {height} (ratio {part2Ratio:0.00}) EXCEDE "+maxAspectRatio);
            } 
                
        }
        Console.WriteLine(GetDivisionRatio(width, height, maxAspectRatio));
        
        return 1;
    }

    public static bool IsNotTooWide(double width, double height, double maxRatio) {
        if (width <= 0 || height <= 0)
            throw new ArgumentException("Width and height must be positive numbers.");

        // Current ratio as long side / short side, it will return 1 or more, like 1.7777 for a 16:9 ratio
        var ratio = Math.Max(width, height) / Math.Min(width, height);

        // If ratio sent is in the range of 0 to 1, we need to invert it. So, a ratio of 0.33 (1/3) will be 3 (3:1)
        var ratioLimit = maxRatio < 1 ? 1.0 / maxRatio : maxRatio;

        // True if current ratio is less or equal to the limit ratio
        return ratio <= ratioLimit;
    }
    
    public static double GetDivisionRatio(double width, double height, double maxAspectRatio)
    {
        if (height <= 0 || width <= 0)
            throw new ArgumentException("El ancho y la altura deben ser mayores que cero.");

        double ratio = 1 / (1 + maxAspectRatio);
        return ratio;
    }
}