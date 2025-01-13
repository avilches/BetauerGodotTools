using Betauer.TestRunner;

namespace Betauer.Core.Tests;

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

[TestFixture]
public class RandomExtensionsTests {
    [Test]
    public void Take_ShouldReturnUniqueNumbers() {
        // Arrange
        var random = new Random(42); // Fixed seed for reproducibility
        var maxExclusive = 10;
        var count = 5;
        var hashSet = new HashSet<int>();

        // Act
        var enumerator = random.Take(maxExclusive, count);
        while (enumerator.MoveNext()) {
            var number = enumerator.Current;
            // Assert each number is within range
            Assert.That(number, Is.GreaterThanOrEqualTo(0).And.LessThan(maxExclusive),
                $"Number {number} is out of range");
            // Assert each number is unique
            Assert.That(hashSet.Add(number), Is.True,
                $"Number {number} was already generated");
        }

        // Assert we got exactly the requested amount of numbers
        Assert.That(hashSet.Count, Is.EqualTo(count));
    }

    [Test]
    public void Take_ShouldReturnAllPossibleNumbers() {
        // Arrange
        var random = new Random(42);
        var maxExclusive = 5;
        var count = 5;
        var hashSet = new HashSet<int>();

        // Act
        var enumerator = random.Take(maxExclusive, count);
        while (enumerator.MoveNext()) {
            hashSet.Add(enumerator.Current);
        }

        // Assert
        Assert.That(hashSet.Count, Is.EqualTo(count));
        for (int i = 0; i < maxExclusive; i++) {
            Assert.That(hashSet, Does.Contain(i));
        }
    }

    [Test]
    public void Take_MultipleRuns_ShouldGenerateDifferentSequences() {
        // Arrange
        var random = new Random(42);
        var maxExclusive = 10;
        var count = 5;
        var firstRun = new List<int>();
        var secondRun = new List<int>();

        // Act
        var enumerator1 = random.Take(maxExclusive, count);
        while (enumerator1.MoveNext()) {
            firstRun.Add(enumerator1.Current);
        }

        var enumerator2 = random.Take(maxExclusive, count);
        while (enumerator2.MoveNext()) {
            secondRun.Add(enumerator2.Current);
        }

        // Assert
        Assert.That(firstRun, Is.Not.EqualTo(secondRun), "Sequences should be different");
        Assert.That(new HashSet<int>(firstRun).Count, Is.EqualTo(count), "First run should have unique numbers");
        Assert.That(new HashSet<int>(secondRun).Count, Is.EqualTo(count), "Second run should have unique numbers");
    }

    [Test]
    public void Take_MultipleTimes_ShouldAlwaysReturnUniqueNumbers() {
        // Arrange
        var random = new Random(42);
        var maxExclusive = 100;
        var count = 10;
        var iterations = 1000;

        // Act & Assert
        for (int i = 0; i < iterations; i++) {
            var hashSet = new HashSet<int>();
            var enumerator = random.Take(maxExclusive, count);
            while (enumerator.MoveNext()) {
                var number = enumerator.Current;
                Assert.That(number, Is.GreaterThanOrEqualTo(0).And.LessThan(maxExclusive),
                    $"Number {number} is out of range in iteration {i}");
                Assert.That(hashSet.Add(number), Is.True,
                    $"Duplicate number {number} found in iteration {i}");
            }
            Assert.That(hashSet.Count, Is.EqualTo(count));
        }
    }

    [Test]
    public void Take_WithNegativeCount_ShouldThrowArgumentException() {
        var random = new Random(42);
        var ex = Assert.Throws<ArgumentException>(() => random.Take(10, -1).MoveNext());
        Assert.That(ex.Message, Does.Contain("Count cannot be negative"));
    }

    [Test]
    public void Take_WithNegativeMaxExclusive_ShouldThrowArgumentException() {
        var random = new Random(42);
        var ex = Assert.Throws<ArgumentException>(() => random.Take(-1, 5).MoveNext());
        Assert.That(ex.Message, Does.Contain("MaxExclusive must be positive"));
    }

    [Test]
    public void Take_WithCountGreaterThanMaxExclusive_ShouldThrowArgumentException() {
        var random = new Random(42);
        var ex = Assert.Throws<ArgumentException>(() => random.Take(5, 10).MoveNext());
        Assert.That(ex.Message, Does.Contain("Count cannot be greater than maxExclusive"));
    }

    [Test]
    public void Take_WithMaxExclusiveZero_ShouldThrowArgumentException() {
        var random = new Random(42);
        var ex = Assert.Throws<ArgumentException>(() => random.Take(0, 0).MoveNext());
        Assert.That(ex.Message, Does.Contain("MaxExclusive must be positive"));
    }

    [Test]
    public void NextIndexExponential_DistributionTest() {
        // Arrange
        var random = new Random(42);
        var size = 5;
        var factor = 2.0f;
        var iterations = 100000;
        var counts = new int[size];

        // Act
        for (int i = 0; i < iterations; i++) {
            var index = random.NextIndexExponential(size, factor);
            counts[index]++;
        }

        // Assert
        // Verificar que los primeros índices aparecen más frecuentemente
        for (int i = 1; i < size; i++) {
            Assert.That(counts[i - 1], Is.GreaterThan(counts[i]),
                $"Index {i - 1} should appear more times than index {i}");
        }
    }

    [Test]
    public void NextIndexExponential_ValidationTest() {
        var random = new Random(42);

        Assert.Throws<ArgumentException>(() => random.NextIndexExponential(0, 1.0f));
        Assert.Throws<ArgumentException>(() => random.NextIndexExponential(5, 0f));
        Assert.Throws<ArgumentException>(() => random.NextIndexExponential(5, -1f));
    }

    [Test]
    public void NextRatio_DistributionTest() {
        // Arrange
        var random = new Random(42);
        var minRatio = 0.5f;
        var maxRatio = 2.0f;
        var iterations = 100000;
        var results = new List<float>();

        // Act
        for (int i = 0; i < iterations; i++) {
            results.Add(random.NextRatio(minRatio, maxRatio));
        }

        // Assert
        Assert.That(results.Min(), Is.GreaterThanOrEqualTo(minRatio));
        Assert.That(results.Max(), Is.LessThanOrEqualTo(maxRatio));

        // Verificar que hay una distribución similar de valores por encima y por debajo de 1
        var belowOne = results.Count(r => r < 1.0f);
        var aboveOne = results.Count(r => r > 1.0f);
        var ratio = (float)belowOne / aboveOne;
        Assert.That(ratio, Is.InRange(0.9f, 1.1f), "Distribution should be roughly equal above and below 1");
    }

    [Test]
    public void PickPosition_WithWeights_Test() {
        // Arrange
        var random = new Random(42);
        var weights = new float[] { 1f, 2f, 7f }; // 10% chance, 20% chance, 70% chance
        var iterations = 100000;
        var counts = new int[weights.Length];

        // Act
        for (int i = 0; i < iterations; i++) {
            counts[random.PickPosition(weights)]++;
        }

        // Assert
        var total = counts.Sum();
        var expectedRatios = weights.Select(w => w / weights.Sum()).ToArray();
        var actualRatios = counts.Select(c => (float)c / total).ToArray();

        for (int i = 0; i < weights.Length; i++) {
            Assert.That(actualRatios[i], Is.InRange(
                    expectedRatios[i] - 0.01, // Allow 1% deviation
                    expectedRatios[i] + 0.01
                ), $"Position {i} frequency is not within expected range");
        }
    }

    [Test]
    public void PickPosition_WithIWeight_Test() {
        // Arrange
        var random = new Random(42);
        var items = new WeightValue<string>[] {
            new("A", 1f),
            new("B", 2f),
            new("C", 7f)
        };
        var iterations = 100000;
        var counts = new int[items.Length];

        // Act
        for (int i = 0; i < iterations; i++) {
            counts[random.PickPosition(items)]++;
        }

        // Assert
        var total = counts.Sum();
        var weights = items.Select(item => item.Weight).ToArray();
        var expectedRatios = weights.Select(w => w / weights.Sum()).ToArray();
        var actualRatios = counts.Select(c => (float)c / total).ToArray();

        for (int i = 0; i < weights.Length; i++) {
            Assert.That(actualRatios[i], Is.InRange(
                    expectedRatios[i] - 0.01, // Allow 1% deviation
                    expectedRatios[i] + 0.01
                ), $"Position {i} frequency is not within expected range");
        }
    }
}