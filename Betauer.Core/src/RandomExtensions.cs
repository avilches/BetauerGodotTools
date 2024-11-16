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
    public static Vector2 Next(this Random rng, Vector2 center, int radius) {
        var angle = rng.NextDouble() * 2 * Math.PI; // Angle in radians
        var distance = Math.Sqrt(rng.NextDouble()) * radius;
        return center + Vector2.FromAngle((float)angle) * (float)distance;
    }
    
    /// <summary> Returns a random point inside the circle</summary>
    public static Vector2I Next(this Random rng, Vector2I center, int radius) {
        var angle = rng.NextDouble() * 2 * Math.PI; // Angle in radians
        var distance = Math.Sqrt(rng.NextDouble()) * radius;
        return center + new Vector2I(Mathf.RoundToInt(distance * Math.Cos(angle)), Mathf.RoundToInt(distance * Math.Sin(angle)));
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
    /// Returns a uniformly random element from the array 
    /// in the enum.
    /// </summary>
    public static T Next<T>(this Random random, T[] values) {
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
    /// Selects `n` unique random elements from an array `items`.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="random">The instance of the `Random` class.</param>
    /// <param name="items">The array from which elements are to be selected.</param>
    /// <param name="n">The number of elements to select.</param>
    /// <returns>An enumerable containing `n` unique random elements from the input array `items`.</returns>
    public static IEnumerable<T> Extract<T>(this Random random, T[] items, int n) {
        var selectedCount = 0;
        double needed = n;
        double available = items.Length;
        while (selectedCount < n) {
            if( random.NextDouble() < needed / available ) {
                yield return items[Mathf.RoundToInt(available) - 1];
                selectedCount++;
                needed--;
            }
            available--;
        }
    }
    /// <summary>
    /// Selects `n` unique random elements from an IList `items`.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="random">The instance of the `Random` class.</param>
    /// <param name="items">The list from which elements are to be selected.</param>
    /// <param name="n">The number of elements to select.</param>
    /// <returns>An enumerator containing `n` unique random elements from the input list `items`.</returns>
    public static IEnumerable<T> Extract<T>(this Random random, IList<T> items, int n) {
        var selectedCount = 0;
        double needed = n;
        double available = items.Count;
        while (selectedCount < n) {
            if( random.NextDouble() < needed / available ) {
                yield return items[Mathf.RoundToInt(available) - 1];
                selectedCount++;
                needed--;
            }
            available--;
        }
    }
    
    /// <summary>
    /// Shuffle the array in place using the Fisher-Yates algorithm.
    /// https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle
    /// </summary>
    /// <param name="rng"></param>
    /// <param name="array"></param>
    /// <typeparam name="T"></typeparam>
    public static void Shuffle<T>(this Random rng, T[] array) {
        var n = array.Length;
        while (n > 1) {
            var k = rng.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }
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

public class Producer<T> : IEnumerable<T> {
    private readonly Func<T> _producer;

    public Producer(Func<T> producer) {
        _producer = producer;
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public IEnumerator<T> GetEnumerator() {
        while (true) yield return _producer();
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
}