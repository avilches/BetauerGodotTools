using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[Betauer.TestRunner.Test]
public class RandomTests {
    internal enum Pepe {
        A,
        B,
        C
    }

    public const int SampleCount = 1000000;
    public const float Tolerance = 0.02f;
    public const float NUnitTolerance = Tolerance * 100;

    [Betauer.TestRunner.Test]
    public void UniformRangeIntTest() {
        var rnd = new Random(0);
        int min = -3;
        int max = 6;
        var intHist = Distribution.DiscreteHistogram(() => rnd.Range(min, max), SampleCount);
        Console.WriteLine(Distribution.Show(intHist));

        // Both inclusive
        Assert.That(intHist.Keys.Min(), Is.EqualTo(min));
        Assert.That(intHist.Keys.Max(), Is.EqualTo(max));
        AssertUniform(intHist, Tolerance);
    }

    [Betauer.TestRunner.Test]
    public void UniformRangeLongTest() {
    // public static void Main() {
        var rnd = new Random(0);
        long min = -3L;
        long max = 6L;
        var longHist = Distribution.DiscreteHistogram(() => rnd.Range(min, max), SampleCount);
        Console.WriteLine(Distribution.Show(longHist));

        // Both inclusive
        Assert.That(longHist.Keys.Min(), Is.EqualTo(min));
        Assert.That(longHist.Keys.Max(), Is.EqualTo(max));
        AssertUniform(longHist, Tolerance);
    }

    [Betauer.TestRunner.Test]
    public void UniformRangeDateTimeTest() {
        var rnd = new Random(0);
        var from = new DateTime(2023, 12, 31, 23, 59, 55);
        var to = from.Add(TimeSpan.FromSeconds(20));
        var longHist = Distribution.DiscreteHistogram(() => rnd.Range(from, to).ToString(), SampleCount);
        Console.WriteLine(Distribution.Show(longHist));

        // Both inclusive
        var min = longHist.Keys.Select(DateTime.Parse).Min();
        Assert.That(min, Is.EqualTo(from));
        var max = longHist.Keys.Select(DateTime.Parse).Max();
        Assert.That(max, Is.EqualTo(to));
        
        AssertUniform(longHist, Tolerance);
    }
    
    [Betauer.TestRunner.Test]
    public void UniformEnumTest() {
        var rnd = new Random(0);
        var enumHist = Distribution.DiscreteHistogram(() => rnd.Next<Pepe>(), SampleCount);
        Console.WriteLine(Distribution.Show(enumHist));
        AssertUniform(enumHist, Tolerance);
    }

    [Betauer.TestRunner.Test]
    public void UniformArrayTest() {
        var rnd = new Random(0);
        var enumHist = Distribution.DiscreteHistogram(() => rnd.Next(new []{0,1,2,3}), SampleCount);
        Console.WriteLine(Distribution.Show(enumHist));
        AssertUniform(enumHist, Tolerance);
    }

    [Betauer.TestRunner.Test]
    public void UniformListTest() {
        var rnd = new Random(0);
        var enumHist = Distribution.DiscreteHistogram(() => rnd.Next(new List<int>{0,1,2,3}), SampleCount);
        Console.WriteLine(Distribution.Show(enumHist));
        AssertUniform(enumHist, Tolerance);
    }

    [Betauer.TestRunner.Test]
    public void WeightedPickFloatArrayTest() {
        var rnd = new Random(0);

        var positionHist = Distribution.DiscreteHistogram(() => rnd.PickPosition(new[] { 0.1f, 0.2f, 0.3f }), SampleCount);
        Console.WriteLine("16%/33%/50%");
        Console.WriteLine(Distribution.Show(positionHist));
        Assert.That(positionHist[0], Is.EqualTo(SampleCount * 0.16666f).Within(NUnitTolerance).Percent);
        Assert.That(positionHist[1], Is.EqualTo(SampleCount * 0.33333f).Within(NUnitTolerance).Percent);
        Assert.That(positionHist[2], Is.EqualTo(SampleCount * 0.5f).Within(NUnitTolerance).Percent);
    }
    
    [Betauer.TestRunner.Test]
    public void WeightedPickIWeightArrayTest() {
        var rnd = new Random(0);

        var weightHist = Distribution.DiscreteHistogram(() =>
            rnd.PickPosition(new IWeight[] { WeightValue.Create("A", 1f), WeightValue.Create("B", 2f), WeightValue.Create("C", 3f) }), SampleCount);
        Console.WriteLine("16%/33%/50%");
        Console.WriteLine(Distribution.Show(weightHist));
        Assert.That(weightHist[0], Is.EqualTo(SampleCount * 0.16666f).Within(NUnitTolerance).Percent);
        Assert.That(weightHist[1], Is.EqualTo(SampleCount * 0.33333f).Within(NUnitTolerance).Percent);
        Assert.That(weightHist[2], Is.EqualTo(SampleCount * 0.5f).Within(NUnitTolerance).Percent);
    }

    private static void AssertUniform<T>(Dictionary<T, long> hist, float tolerance) {
        var total = hist.Values.Sum();
        var mean = total / hist.Values.Count;
        hist.Values.ForEach(val => {
            // Console.WriteLine(val + " must be " + mean * (1f - tolerance) + " / " + mean * (1f + tolerance));
            Assert.That(val, Is.EqualTo(mean).Within(tolerance * 100).Percent);
        });
    }
}