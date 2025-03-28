using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Godot;

namespace Betauer.Core;

public static partial class RandomExtensions {
    // https://tryfinally.dev/distribution-sampling-trandom-statdist
    // https://statdist.com/
    // https://numerics.mathdotnet.com/Probability.html
    // https://github.com/mathnet/mathnet-numerics
    // https://www.cambiaresearch.com/articles/13/csharp-randomprovider-class

    /// <summary>
    /// Returns a DateTime in the range [min, max] (both inclusive, within milliseconds)
    /// </summary>
    public static DateTime Range(this Random random, DateTime start, DateTime end) {
        if (start >= end) throw new ArgumentException("Start date must be less than end date");
        var range = end.Ticks - start.Ticks;
        var randomTicks = (long)(random.NextDouble() * range);
        return new DateTime(start.Ticks + randomTicks);
    }

    /// <summary>
    /// Returns a TimeSpan in the range [min, max] (max exclusive)
    /// </summary>
    public static TimeSpan Range(this Random random, TimeSpan min, TimeSpan max) {
        return new TimeSpan(random.NextInt64(min.Ticks, max.Ticks));
    }


    /// <summary>
    /// Returns a float (Single) in the range [start, end) (max is excluded)
    /// </summary>
    public static float Range(this Random random, float start, float end) {
        if (start >= end) throw new ArgumentException("Start value must be less than end value");
        var limit = end - start;
        var rn = random.NextDouble() * limit;
        return (float)rn + start;
    }

    /// <summary>
    /// Returns a double in the range [start, end) (end is excluded)
    /// </summary>
    public static double Range(this Random random, double start, double end) {
        if (start >= end) throw new ArgumentException("Start value must be less than end value");
        var limit = end - start;
        var rn = random.NextDouble() * limit;
        return rn + start;
    }

    /// <summary>
    /// Returns true/false with a 50%/50%
    /// </summary>
    /// <returns></returns>
    public static bool NextBool(this Random random) {
        return random.Next(0, 2) == 0;
    }

    /// <summary>
    /// Returns an index from 0 to size-1 with exponential distribution preference for lower indices.
    /// The factor controls how strong the preference is:
    /// - Lower factors (0.1-0.3) = Almost uniform distribution
    /// - Medium factors (0.5-1.0) = Moderate preference for first elements
    /// - Higher factors (2.0+) = Strong preference for first elements where last element is very rare
    /// Factor 3 in a size of 10, means this distribution of indices returned:
    ///  0) ############# (27.9%)
    ///  1) ########## (20.5%)
    ///  2) ####### (15.2%)
    ///  3) ##### (11.3%)
    ///  4) #### (8.3%)
    ///  5) ### (6.2%)
    ///  6) ## (4.6%)
    ///  7) # (3.4%)
    ///  8) # (2.5%)
    /// </summary>
    public static int NextIndexExponential(this Random random, int size, float factor) {
        if (size <= 0) throw new ArgumentException("Size must be greater than 0");
        if (factor <= 0) throw new ArgumentException("Factor must be greater than 0");

        var lambda = factor / size;
        var maxValue = 1 - Math.Exp(-lambda * (size - 1));
        var u = random.NextDouble() * maxValue;
        return (int)(-Math.Log(1 - u) / lambda);
    }

    /// <summary>
    /// Returns true with a specific chance. So, NextBool(0.7f) will be true the 70% 
    /// </summary>
    /// <returns></returns>
    public static bool NextBool(this Random random, double chance) {
        var rn = random.NextDouble();
        return rn <= chance;
    }

    /// <summary>Returns a non-negative random integer.</summary>
    /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than <see cref="int.MaxValue"/>.</returns>
    public static int NextInt(this Random random) {
        return random.Next();
    }

    /// <summary>Returns a odd random integer between limits</summary>
    public static int NextOdd(this Random random, int lowerLimit, int upperLimit) {
        if (lowerLimit % 2 == 0) throw new ArgumentException($"Lower limit is not an odd number: {lowerLimit}");
        if (upperLimit % 2 == 0) throw new ArgumentException($"Upper limit is not an odd number: {upperLimit}");
        var range = (upperLimit - lowerLimit) / 2 + 1;
        return lowerLimit + random.Next(0, range) * 2;
    }

    public static int NextEven(this Random random, int lowerLimit, int upperLimit) {
        if (lowerLimit % 2 != 0) throw new ArgumentException($"Lower limit is not an even number: {lowerLimit}");
        if (upperLimit % 2 != 0) throw new ArgumentException($"Upper limit is not an even number: {upperLimit}");
        var range = (upperLimit - lowerLimit) / 2 + 1;
        return lowerLimit + random.Next(0, range) * 2;
    }

    /// <summary>Returns a non-negative random integer.</summary>
    /// <returns>A 64-bit signed integer that is greater than or equal to 0 and less than <see cref="long.MaxValue"/>.</returns>
    public static long NextLong(this Random random) {
        return random.NextInt64();
    }

    /// <summary>Returns a random floating-point number that is greater than or equal to 0.0, and less than 1.0.</summary>
    /// <returns>A single-precision floating point number that is greater than or equal to 0.0, and less than 1.0.</returns>
    public static double NextFloat(this Random random) {
        return random.NextSingle();
    }

    /// <summary> Returns a random point inside the Rect2I</summary>
    public static Vector2I Next(this Random rng, Rect2I rect2I) {
        return new Vector2I(rng.Next(rect2I.Position.X, rect2I.End.X), rng.Next(rect2I.Position.Y, rect2I.End.Y));
    }

    /// <summary> Returns a random point inside the Rect2</summary>
    public static Vector2 Next(this Random rng, Rect2 rect2) {
        return new Vector2(rng.Range(rect2.Position.X, rect2.End.X), rng.Range(rect2.Position.Y, rect2.End.Y));
    }

    /// <summary> Returns a random point inside the circle</summary>
    public static Vector2 Next(this Random rng, Vector2 center, float radius) {
        var angle = rng.NextSingle() * 2 * Mathf.Pi; // Angle in radians
        var distance = Mathf.Sqrt(rng.NextSingle()) * radius;
        return center + Vector2.FromAngle(angle) * distance;
    }

    /// <summary> Returns a random point inside the circle</summary>
    public static Vector2I Next(this Random rng, Vector2I center, int radius) {
        var angle = rng.NextSingle() * 2 * Mathf.Pi; // Angle in radians
        var distance = Mathf.Sqrt(rng.NextSingle()) * radius;
        return center + new Vector2I(Mathf.RoundToInt(distance * Mathf.Cos(angle)), Mathf.RoundToInt(distance * Mathf.Sin(angle)));
    }

    /// <summary>
    /// Returns a random ratio in the range [minRatio, maxRatio] (both inclusive)
    /// So, NextRatio(0.5f, 2f) will return a number between 0.5 and 2 where the chances between 0.5-1 values, and 1-2 will be the same.
    /// </summary>
    public static float NextRatio(this Random rng, float minRatio, float maxRatio) {
        // Ensure minRatio <= maxRatio
        if (minRatio > maxRatio) {
            (minRatio, maxRatio) = (maxRatio, minRatio);
        }

        // Simple case: both ratios are on the same side of 1. Example: minRatio 0.5 and maxRatio 0.8, or minRatio 1.2 and maxRatio 1.5 
        if ((minRatio >= 1 && maxRatio >= 1) || (minRatio <= 1 && maxRatio <= 1)) {
            return (float)(rng.NextDouble() * (maxRatio - minRatio) + minRatio);
        }
        // Mix case: minRatio < 1, maxRatio > 1. Example: minRatio 0.5 y maxRatio 1.5 
        // First, convert min ratio to the other side of 1. That will make a bigger range of values.
        var invertedMin = 1f / minRatio;

        // Generate a random value in both ranges, with the same probability 
        var range1 = invertedMin - 1;
        var range2 = maxRatio - 1;
        var totalRange = range1 + range2;
        var randomValue = (float)(rng.NextDouble() * totalRange);
        return randomValue <= range1
            ? 1f / (randomValue + 1) // Volver al lado < 1
            : 1 + randomValue - range1; // Pertenece al lado > 1
    }

    /// <summary>
    /// Returns a uniformly random integer representing one of the values of the <T> enum 
    /// in the enum.
    /// </summary>
    public static int Next<T>(this Random random) where T : Enum {
        var values = (int[])Enum.GetValues(typeof(T));
        var randomIndex = random.Next(0, values.Length);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns a uniformly random element from the list 
    /// in the enum.
    /// </summary>
    public static T Next<T>(this Random random, IList<T> values) {
        var randomIndex = random.Next(0, values.Count);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns a uniformly random element from the list 
    /// in the enum.
    /// </summary>
    public static T Next<T>(this Random random, ReadOnlySpan<T> values) {
        var randomIndex = random.Next(0, values.Length);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns an element of the list specified with exponential distribution preference for elements in the firsts positions.
    /// The factor controls how strong the preference is:
    /// - Lower factors (0.1-0.3) = Almost uniform distribution
    /// - Medium factors (0.5-1.0) = Moderate preference for first elements
    /// - Higher factors (2.0+) = Strong preference for first elements where last element is very rare
    /// Factor 3 in a list of 10 elements, means this distribution of element indices returned:
    ///  0) ############# (27.9%)
    ///  1) ########## (20.5%)
    ///  2) ####### (15.2%)
    ///  3) ##### (11.3%)
    ///  4) #### (8.3%)
    ///  5) ### (6.2%)
    ///  6) ## (4.6%)
    ///  7) # (3.4%)
    ///  8) # (2.5%)
    /// </summary>
    public static T NextExponential<T>(this Random random, IList<T> list, float factor) {
        var index = random.NextIndexExponential(list.Count, factor);
        return list[index];
    }

    /// <summary>
    /// Returns an element of the span specified with exponential distribution preference for elements in the firsts positions.
    /// The factor controls how strong the preference is:
    /// - Lower factors (0.1-0.3) = Almost uniform distribution
    /// - Medium factors (0.5-1.0) = Moderate preference for first elements
    /// - Higher factors (2.0+) = Strong preference for first elements where last element is very rare
    /// Factor 3 in a span of 10 elements, means this distribution of element indices returned:
    ///  0) ############# (27.9%)
    ///  1) ########## (20.5%)
    ///  2) ####### (15.2%)
    ///  3) ##### (11.3%)
    ///  4) #### (8.3%)
    ///  5) ### (6.2%)
    ///  6) ## (4.6%)
    ///  7) # (3.4%)
    ///  8) # (2.5%)
    /// </summary>
    public static T NextExponential<T>(this Random random, ReadOnlySpan<T> list, float factor) {
        var index = random.NextIndexExponential(list.Length, factor);
        return list[index];
    }

    /// <summary>
    /// Returns an array with n unique random numbers between 0 (inclusive) and maxExclusive (exclusive).
    /// This implementation uses Knuth's Algorithm S (Reservoir Sampling) which is memory efficient as it only stores
    /// 'count' numbers at any time.
    /// 
    /// The algorithm works in two phases:
    /// 1. First phase: Takes the first 'count' numbers (0 to count-1) directly
    /// 2. Second phase: For each number i from count to maxExclusive-1:
    ///    - Generates a random number j in range [0,i]
    ///    - If j is less than count, replaces the number at position j with i
    /// 
    /// This ensures that:
    /// - Each number has an equal probability of being selected
    /// - Only 'count' numbers are stored in memory at any time
    /// - Numbers are generated lazily (one at a time)
    /// - The distribution is uniform
    /// 
    /// - Memory complexity is O(count) as it only stores the reservoir array
    /// - Time complexity is O(1) per element in the first phase and O(count/i) per element i in the second phase
    ///
    /// </summary>
    /// <param name="random">The Random instance to use.</param>
    /// <param name="maxExclusive">The exclusive upper bound of the random number returned.</param>
    /// <param name="count">The number of unique random numbers to generate.</param>
    /// <returns>An iterator that generates unique random numbers.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when:
    /// - count is negative
    /// - maxExclusive is not positive
    /// - count is greater than maxExclusive
    /// </exception>
    public static int[] Take(this Random random, int maxExclusive, int count) {
        if (count < 0) throw new ArgumentException("Count cannot be negative", nameof(count));
        if (maxExclusive <= 0) throw new ArgumentException("MaxExclusive must be positive", nameof(maxExclusive));
        if (count > maxExclusive) throw new ArgumentException("Count cannot be greater than maxExclusive", nameof(count));

        if (count == 0) return Array.Empty<int>();

        var reservoir = new int[count];

        // Fill the reservoir array
        for (var i = 0; i < count; i++) {
            reservoir[i] = i;
        }

        // Replace elements with gradually decreasing probability
        for (var i = count; i < maxExclusive; i++) {
            var j = random.Next(i + 1);
            if (j < count) {
                reservoir[j] = i;
            }
        }

        return reservoir;
    }


    /// <summary>
    /// Returns an enumerable that generates n random unique items from the source list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="random">The Random instance to use.</param>
    /// <param name="items">The source list to take items from.</param>
    /// <param name="n">The number of items to take.</param>
    /// <returns>An enumerable that generates n random unique items from the source list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when items is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown when:
    /// - n is negative
    /// - n is greater than the number of items
    /// </exception>
    public static T[] Take<T>(this Random random, IList<T> items, int count) {
        if (items == null) throw new ArgumentNullException(nameof(items));
        if (count < 0) throw new ArgumentException("Count cannot be negative", nameof(count));
        if (count > items.Count) throw new ArgumentException("Count cannot be greater than the number of items", nameof(count));

        if (count == 0) return Array.Empty<T>();

        var result = new T[count];

        // Fill the reservoir array
        for (var i = 0; i < count; i++) {
            result[i] = items[i];
        }

        // Replace elements with gradually decreasing probability
        for (var i = count; i < items.Count; i++) {
            var j = random.Next(i + 1);
            if (j < count) {
                result[j] = items[i];
            }
        }

        return result;
    }

    /// <summary>
    /// Shuffle the list in place using the Fisher-Yates algorithm.
    /// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="array"></param>
    /// <typeparam name="T"></typeparam>
    public static void Shuffle<T>(this Random rng, IList<T> array) {
        var n = array.Count;
        while (n > 1) {
            var k = rng.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
    }
}

public class Producer<T>(Func<T> producer) : IEnumerable<T> {
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator() {
        while (true) yield return producer();
    }
}

public static class Distribution {
    public static Dictionary<long, long> DiscreteHistogram(Func<long> producer, int sampleCount) {
        return new Producer<long>(producer)
            .Take(sampleCount)
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.LongCount());
    }

    public static Dictionary<string, long> DiscreteHistogram(Func<string> producer, int sampleCount) {
        return new Producer<string>(producer)
            .Take(sampleCount)
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.LongCount());
    }

    public static string Show<T>(Dictionary<T, long> dict, int barSize = 40) {
        int labelMax = dict.Keys
            .Select(x => x.ToString().Length)
            .Max();

        string ToLabel(T t) => t.ToString().PadLeft(labelMax);

        var sup = dict.Keys.OrderBy(t => t).ToList();
        long max = dict.Values.Max();
        double scale = max < barSize ? 1.0 : ((double)barSize) / max;

        string Bar(T t) => new('#', (int)(dict[t] * scale));
        string Hint(T t) => new string('·', barSize - (int)(dict[t] * scale) + 1) + " " + dict[t];

        return sup.Select(t => $"{ToLabel(t)}|{Bar(t)}{Hint(t)}").Concatenated("\n");
    }

    public static string Histogram(double low, double high, Func<double> f, int width = 40, int barSize = 40,
        int sampleCount = 100000) {
        var min = double.MaxValue;
        var max = double.MinValue;
        var range = high - low;
        width = width > 0 ? width : Math.Min(100, (int)range);
        var buckets = new int[width];

        for (var i = 0; i < sampleCount; i++) {
            var value = f();
            var bucket = (int)Math.Floor((value - low) * buckets.Length / range);
            if (bucket >= 0 && bucket < buckets.Length) buckets[bucket]++;
            else Console.WriteLine("Out of bounds " + value);

            min = Math.Min(min, value);
            max = Math.Max(max, value);
        }
        var biggestBucket = buckets.Max();
        return Enumerable.Range(0, width)
                   .Select(r => {
                       var labelValue = (float)r / width * range + low;
                       var percent = (int)((float)buckets[r] / biggestBucket * barSize);

                       var label = (labelValue < 0 ? "" : " ") +
                                   labelValue.ToString("00.00", CultureInfo.InvariantCulture) + "|";
                       var bar = new string('#', percent);
                       var hint = new string('·', barSize - percent + 1) + buckets[r] + "\n";
                       return string.Join("", label, bar, hint);
                   })
                   .Concatenated()
               + $"Min: {min}, Max: {max}:\n";
    }

    public static void Main() {
        var samples = 1000000;
        var size = 10;
        var factors = new[] { 0.01f, 0.5, 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 10f, 20f };
        var random = new Random();

        foreach (var factor in factors) {
            var counts = new int[size];
            for (var i = 0; i < samples; i++) {
                var index = random.NextIndexExponential(size, (float)factor);
                counts[index]++;
            }

            Console.WriteLine($"\nFactor {factor:F1} ({samples} muestras):");
            // Console.WriteLine("Índice: [0][1][2][3][4][5][6][7][8][9]");
            // Console.WriteLine("Veces:   " + string.Join(" ", counts.Select(c => c.ToString().PadLeft(3))));
            // Console.WriteLine("       " + string.Join(" ", counts.Select(c => $"{(c * 100.0 / samples):F1}%")));

            Console.WriteLine("\nVisualización (cada # = 2%):");
            for (var i = 0; i < size; i++) {
                var percentage = (counts[i] * 100.0 / samples);
                Console.WriteLine($"[{i}] {new string('#', (int)(percentage / 2))} ({percentage:F1}%)");
            }
        }
    }
}