using System;
using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core;

public static class CombinationTools {
    private static readonly Dictionary<(int n, int r), int> CombinationsCache = new();

    /// <summary>
    /// Calcula el número de combinaciones de n elementos tomados de r en r.
    /// Los resultados se cachean automáticamente para futuras llamadas.
    /// </summary>
    public static int Calculate(int n, int r, bool cache = true) {
        // Casos base
        if (r == 0) return 1;
        if (r > n) return 0;
        if (r == n) return 1;
        if (r > n - r) r = n - r; // Optimización: usar el menor valor de r

        // Comprobar cache
        if (cache && CombinationsCache.TryGetValue((n, r), out int cached)) {
            return cached;
        }

        // Calcular si no está en cache usando multiplicación y división directa
        long result = 1;
        for (int i = 1; i <= r; i++) {
            result *= n - (i - 1);
            result /= i;
        }
        // Solo cachear si el resultado no es demasiado grande
        if (cache && result <= int.MaxValue) {
            CombinationsCache[(n, r)] = (int)result;
        }
        return (int)result;
    }

    /// <summary>
    /// Generates all possible combinations of the specified size from the source sequence.
    /// Uses a recursive algorithm to generate all possible combinations maintaining the original order.
    /// </summary>
    /// <example>
    /// var items = new[] { "A", "B", "C" };
    /// var combinations = items.Combinations(2);
    /// // Results in: ["A","B"], ["A","C"], ["B","C"]
    /// </example>
    /// <typeparam name="T">The type of the elements in the sequence</typeparam>
    /// <param name="source">The sequence to generate combinations from</param>
    /// <param name="size">The size of each combination</param>
    /// <returns>An enumerable of all possible combinations of the specified size</returns>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IList<T> source, int count) {
        ArgumentNullException.ThrowIfNull(source);
        if (count < 0 || count > source.Count) yield break;
        if (count == 0) {
            yield return Array.Empty<T>();
            yield break;
        }

        var indices = new int[count];
        var result = new T[count];

        // Initialize first combination
        for (int e = 0; e < count; e++) {
            indices[e] = e;
        }

        while (true) {
            // Generate current combination
            for (int f = 0; f < count; f++) {
                result[f] = source[indices[f]];
            }
            yield return result.ToArray();

            // Find rightmost index that can be incremented
            int i = count - 1;
            while (i >= 0 && indices[i] == source.Count - count + i) {
                i--;
            }

            // No more combinations
            if (i < 0) {
                break;
            }

            // Increment index and reset subsequent indices
            indices[i]++;
            for (int j = i + 1; j < count; j++) {
                indices[j] = indices[j - 1] + 1;
            }
        }
    }

    /// <summary>
    /// Generates random combinations of the given size from the source sequence.
    /// Uses Fisher-Yates shuffle algorithm for unbiased random selection.
    /// 
    /// The total number of possible combinations is calculated using the formula: n!/(r!(n-r)!)
    /// where n is the source length and r is the count.
    /// 
    /// If the requested number of simulations exceeds the total possible combinations,
    /// an ArgumentException will be thrown.
    /// </summary>
    /// <example>
    /// var source = new[] { "A", "B", "C", "D" };
    /// var random = new Random();
    /// // With size 2, there are 6 possible combinations
    /// var combos = source.RandomCombinations(2, 3, random);
    /// // Returns 3 different combinations, for example: ["A","B"], ["B","D"], ["A","C"]
    /// 
    /// // This will throw ArgumentException because there are only 6 possible combinations:
    /// var moreCombos = source.RandomCombinations(2, 7, random); // throws ArgumentException
    /// </example>
    /// <param name="source">The sequence to generate combinations from</param>
    /// <param name="count">The size of each combination</param>
    /// <param name="simulations">The number of combinations to generate (must not exceed total possible combinations)</param>
    /// <param name="random">The random number generator to use</param>
    /// <returns>An enumerable of random combinations. Each combination is guaranteed to have distinct elements.</returns>
    /// <exception cref="ArgumentNullException">If source or random is null</exception>
    /// <exception cref="ArgumentException">If simulations exceeds the total possible combinations</exception>
    /// <remarks>
    /// - Each combination will never contain repeated elements
    /// - The method uses minimal memory by reusing arrays
    /// - The Fisher-Yates shuffle ensures unbiased random selection
    /// - Uses a HashSet to track generated combinations and ensure uniqueness
    /// </remarks>
    public static IEnumerable<IEnumerable<T>> RandomCombinations<T>(this IList<T> source, int count, int simulations, Random random) {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(random);

        if (count < 0 || count > source.Count) yield break;
        if (count == 0) {
            yield return Array.Empty<T>();
            yield break;
        }

        var totalPossible = Calculate(source.Count, count);
        if (simulations > totalPossible) {
            throw new ArgumentException(
                $"Requested {simulations} combinations but only {totalPossible} are possible with {source.Count} elements taken {count} at a time",
                nameof(simulations));
        }

        var tempArray = new T[source.Count];
        var result = new T[count];
        Array.Copy(source.ToArray(), tempArray, source.Count);

        var generated = new HashSet<string>();
        var attempts = 0;
        var maxAttempts = simulations * 3;

        while (generated.Count < simulations && attempts < maxAttempts) {
            attempts++;

            for (var j = tempArray.Length - 1; j > 0; j--) {
                var k = random.Next(j + 1);
                (tempArray[j], tempArray[k]) = (tempArray[k], tempArray[j]);
            }

            Array.Copy(tempArray, result, count);
            var key = string.Join(",", result.OrderBy(x => x));

            if (generated.Add(key)) {
                yield return result.ToArray();
            }
        }
    }
}