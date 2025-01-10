using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class CombinationToolsTest {
    [Test]
    public void TestCalculate() {
        // Test basic combinations
        Assert.That(CombinationTools.Calculate(4, 2), Is.EqualTo(6)); // 4C2 = 6
        Assert.That(CombinationTools.Calculate(5, 3), Is.EqualTo(10)); // 5C3 = 10

        // Test edge cases
        Assert.That(CombinationTools.Calculate(5, 0), Is.EqualTo(1)); // nC0 = 1
        Assert.That(CombinationTools.Calculate(5, 5), Is.EqualTo(1)); // nCn = 1
        Assert.That(CombinationTools.Calculate(5, 6), Is.EqualTo(0)); // n < r = 0

        // Test cache
        Assert.That(CombinationTools.Calculate(10, 5, true), Is.EqualTo(252)); // First call
        Assert.That(CombinationTools.Calculate(10, 5, true), Is.EqualTo(252)); // Cached call
        Assert.That(CombinationTools.Calculate(10, 5, false), Is.EqualTo(252)); // Non-cached call
    }

    [Test]
    public void TestCombinations() {
        var source = new[] { 1, 2, 3, 4 };

        // Test size 2 combinations
        var combinations2 = source.Combinations(2).ToList();
        Assert.That(combinations2.Count, Is.EqualTo(6)); // 4C2 = 6

        // Convertir cada combinación a una lista ordenada para comparar
        var combinationsAsStrings = combinations2
            .Select(combo => string.Join(",", combo.OrderBy(x => x)))
            .ToList();

        Assert.That(combinationsAsStrings, Contains.Item("1,2"));
        Assert.That(combinationsAsStrings, Contains.Item("1,3"));
        Assert.That(combinationsAsStrings, Contains.Item("1,4"));
        Assert.That(combinationsAsStrings, Contains.Item("2,3"));
        Assert.That(combinationsAsStrings, Contains.Item("2,4"));
        Assert.That(combinationsAsStrings, Contains.Item("3,4"));

        // Test edge cases
        var combinationsEmpty = source.Combinations(0).ToList();
        Assert.That(combinationsEmpty.Count, Is.EqualTo(1));
        Assert.That(combinationsEmpty.First(), Is.Empty);

        var combinationsAll = source.Combinations(4).ToList();
        Assert.That(combinationsAll.Count, Is.EqualTo(1));
        Assert.That(combinationsAll.First().OrderBy(x => x), Is.EqualTo(source.OrderBy(x => x)));

        var combinationsInvalid = source.Combinations(5).ToList();
        Assert.That(combinationsInvalid, Is.Empty);
    }

    [Test]
    public void TestRandomCombinationsWithSimulations() {
        var source = new[] { 1, 2, 3, 4 };
        var random = new Random(123); // Fixed seed for reproducibility

        // Test with specific number of simulations
        var randomCombos = source.RandomCombinations(2, 3, random).ToList();
        Assert.That(randomCombos.Count, Is.EqualTo(3)); // Requested 3 combinations

        // Verify each combination is valid
        foreach (var combo in randomCombos) {
            Assert.That(combo.Count(), Is.EqualTo(2));
            Assert.That(combo.All(x => source.Contains(x)), Is.True);
            // Verificar que no hay elementos repetidos dentro de cada combinación
            Assert.That(combo.Distinct().Count(), Is.EqualTo(2));
        }

        // Verify uniqueness of combinations
        var uniqueCombos = new HashSet<string>(
            randomCombos.Select(combo =>
                string.Join(",", combo.OrderBy(x => x))
            )
        );
        Assert.That(uniqueCombos.Count, Is.EqualTo(randomCombos.Count),
            "RandomCombinations returned duplicate combinations");

        // Test edge cases
        var emptyCombo = source.RandomCombinations(0, 1, random).ToList();
        Assert.That(emptyCombo.Count, Is.EqualTo(1));
        Assert.That(emptyCombo.First(), Is.Empty);

        var invalidCombo = source.RandomCombinations(5, 1, random).ToList();
        Assert.That(invalidCombo, Is.Empty);
    }

    [Test]
    public void TestRandomCombinationsUniqueness() {
        var source = new[] { 1, 2, 3, 4 };
        var random = new Random(123);

        // Generate multiple sets of combinations
        for (int test = 0; test < 10; test++) {
            // Calculamos el número total de combinaciones posibles
            var totalPossible = CombinationTools.Calculate(source.Length, 2);
            var simulations = totalPossible - 1; // Pedimos una menos que el total posible

            var randomCombos = source.RandomCombinations(2, simulations, random).ToList();
            Assert.That(randomCombos.Count, Is.EqualTo(simulations),
                $"Should generate exactly {simulations} combinations");

            // Convert combinations to strings for easy comparison
            var combosAsStrings = randomCombos
                .Select(combo => string.Join(",", combo.OrderBy(x => x)))
                .ToList();

            // Check for duplicates
            var uniqueCombos = new HashSet<string>(combosAsStrings);
            Assert.That(uniqueCombos.Count, Is.EqualTo(randomCombos.Count),
                $"Found duplicate combinations in test {test}");

            // Verify each combination is valid
            foreach (var combo in randomCombos) {
                // Check size
                Assert.That(combo.Count(), Is.EqualTo(2),
                    "Each combination should have exactly 2 elements");

                // Check for duplicates within combination
                Assert.That(combo.Distinct().Count(), Is.EqualTo(2),
                    "Found duplicate elements within a combination");

                // Check all elements are from source
                Assert.That(combo.All(x => source.Contains(x)), Is.True,
                    "Combination contains elements not in source");
            }
        }
    }


    [Test]
    public void TestRandomCombinationsException() {
        var source = new[] { 1, 2, 3, 4 };
        var random = new Random(123);
        var totalPossible = CombinationTools.Calculate(source.Length, 2);
    
        // Verificar que se lanza excepción cuando pedimos más combinaciones que las posibles
        var ex = Assert.Throws<ArgumentException>(() => 
            source.RandomCombinations(2, totalPossible + 1, random).ToList());
    
        Assert.That(ex.Message, Does.Contain("Requested"));
        Assert.That(ex.Message, Does.Contain("combinations but only"));
        Assert.That(ex.ParamName, Is.EqualTo("simulations"));
    }
    
    [Test]
    public void TestRandomCombinationsFullSet() {
        int[] source = new[] { 1, 2, 3, 4 };
        var random = new Random(123);

        // Generate all possible combinations (6 for 4C2)
        var totalCombinations = CombinationTools.Calculate(source.Length, 2);
        var randomCombos = source.RandomCombinations(2, totalCombinations, random).ToList();

        // Convert to strings for comparison
        var combosAsStrings = randomCombos
            .Select(combo => string.Join(",", combo.OrderBy(x => x)))
            .ToList();

        // Verify we got all possible combinations
        var uniqueCombos = new HashSet<string>(combosAsStrings);
        Assert.That(uniqueCombos.Count, Is.EqualTo(totalCombinations),
            "Should generate all possible combinations without duplicates");

        // Verify all expected combinations are present
        var expectedCombos = new[] { "1,2", "1,3", "1,4", "2,3", "2,4", "3,4" };
        foreach (var expected in expectedCombos) {
            Assert.That(uniqueCombos, Contains.Item(expected),
                $"Missing combination {expected}");
        }
    }

    [Test]
    public void TestRandomCombinationsDistribution() {
        var source = new[] { 1, 2, 3 };
        var random = new Random(123);
        var combinations = new Dictionary<string, int>();

        // Generate many random combinations and count their occurrences
        const int simulations = 3000;
        for (int i = 0; i < simulations; i++) {
            var combo = source.RandomCombinations(2, 1, random)
                .Single();

            // Verificar que no hay elementos repetidos en la combinación
            Assert.That(combo.Distinct().Count(), Is.EqualTo(2));

            var key = string.Join(",", combo.OrderBy(x => x));
            combinations[key] = combinations.GetValueOrDefault(key) + 1;
        }

        // Verify all possible combinations appear and their distribution is roughly uniform
        Assert.That(combinations.Keys.Count, Is.EqualTo(3)); // 3C2 = 3 possible combinations
        var expectedCount = simulations / 3;
        var tolerance = expectedCount * 0.2; // Allow 20% deviation

        foreach (var count in combinations.Values) {
            Assert.That(count, Is.InRange(expectedCount - tolerance, expectedCount + tolerance));
        }
    }

    [Test]
    public void TestArgumentNullExceptions() {
        var source = new[] { 1, 2, 3 };
        var random = new Random();

        // Usar Assert.Throws con lambda para capturar las excepciones correctamente
        Assert.Throws<ArgumentNullException>(() => ((IList<int>)null).Combinations(2).ToList());
        Assert.Throws<ArgumentNullException>(() => source.RandomCombinations(2, 1, null).ToList());
        Assert.Throws<ArgumentNullException>(() => ((IList<int>)null).RandomCombinations(2, 1, random).ToList());
    }
}