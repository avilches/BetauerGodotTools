using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Betauer.Core;

public static class RandomExtensions {
    // https://tryfinally.dev/distribution-sampling-trandom-statdist
    // https://statdist.com/
    // https://numerics.mathdotnet.com/Probability.html
    // https://github.com/mathnet/mathnet-numerics
    // https://www.cambiaresearch.com/articles/13/csharp-randomprovider-class
    /// <summary>
    /// Returns an int in the range [start, end] (both inclusive)
    /// </summary>
    public static int Range(this Random random, int start, int end) {
        if (start == end) return start;
        if (start > end) (start, end) = (end, start);
        return random.Next(start, end + 1);
    }

    /// <summary>
    /// Returns a long in the range [start, end] (both inclusive)
    /// </summary>
    public static long Range(this Random random, long start, long end) {
        if (start == end) return start;
        if (start > end) (start, end) = (end, start);
        return random.NextInt64(start, end + 1);
    }

    /// <summary>
    /// Returns a DateTime in the range [min, max] (both inclusive)
    /// </summary>
    public static DateTime Range(this Random random, DateTime min, DateTime max) {
        return new DateTime(random.Range(min.Ticks, max.Ticks));
    }

    /// <summary>
    /// Returns a TimeSpan in the range [min, max] (both inclusive)
    /// </summary>
    public static TimeSpan Range(this Random random, TimeSpan min, TimeSpan max) {
        return new TimeSpan(random.Range(min.Ticks, max.Ticks));
    }


    /// <summary>
    /// Returns a float (Single) in the range [start, end) (max is excluded)
    /// </summary>
    public static float Range(this Random random, float start, float endExcluded) {
        if (start > endExcluded) (start, endExcluded) = (endExcluded, start);
        var limit = endExcluded - start;
        var rn = random.NextDouble() * limit;
        return (float)rn + start;
    }

    /// <summary>
    /// Returns a double in the range [start, end) (end is excluded)
    /// </summary>
    public static double Range(this Random random, double start, double endExcluded) {
        if (start > endExcluded) (start, endExcluded) = (endExcluded, start);
        var limit = endExcluded - start;
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
    /// Returns true/false with a chance probability (0.0 to 1.0) of getting a true
    /// So, NextBool(0.7) true is (less than) 70% 
    /// </summary>
    /// <returns></returns>
    public static bool NextBool(this Random random, double chance) {
        var rn = random.NextDouble();
        return rn < chance;
    }

    /// <summary>Returns a non-negative random integer.</summary>
    /// <returns>A 32-bit signed integer that is greater than or equal to 0 and less than <see cref="int.MaxValue"/>.</returns>
    public static long NextInt(this Random random) {
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


    /// <summary>
    /// Returns a uniformly random integer representing one of the values 
    /// in the enum.
    /// </summary>
    public static int NextEnum<T>(this Random random) where T : Enum {
        var values = (int[])Enum.GetValues(typeof(T));
        var randomIndex = random.Next(0, values.Length);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns a uniformly random element from the list
    /// in the enum.
    /// </summary>
    public static T NextElement<T>(this Random random, IList<T> values) {
        var randomIndex = random.Next(0, values.Count);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns a uniformly random element from the list
    /// in the enum.
    /// </summary>
    public static T NextElement<T>(this Random random, HashSet<T> values) {
        var randomIndex = random.Next(0, values.Count);
        return values.ElementAt(randomIndex);
    }

    /// <summary>
    /// Returns a uniformly random element from the array 
    /// in the enum.
    /// </summary>
    public static T NextElement<T>(this Random random, T[] values) {
        var randomIndex = random.Next(0, values.Length);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns a random position of the array, taking into account the value is the weight
    /// so int[] { 1, 1, 2 } has 0=25%, 1=25% and 2=50%
    /// in the enum.
    /// </summary>
    public static int NextWeight(this Random random, int[] values) {
        var total = 0D;
        var span = values.AsSpan();
        for (var j = 0; j < span.Length; j++) total += span[j];
        var x = random.NextDouble() * total;
        var sum = 0D;
        var i = 0;
        while (i < span.Length) {
            sum += span[i];
            if (sum > x) break;
            i++;
        }
        return i;
    }

    /// <summary>
    /// Returns a random position of the array, taking into account the value is the weight
    /// so long[] { 1, 1, 2 } has 0=25%, 1=25% and 2=50%
    /// in the enum.
    /// </summary>
    public static int NextWeight(this Random random, long[] values) {
        var total = 0D;
        var sum = 0D;
        var position = 0;
        var span = values.AsSpan();
        for (var j = 0; j < span.Length; j++) total += span[j];
        var x = random.NextDouble() * total;
        while (position < span.Length) {
            sum += span[position];
            if (sum > x) break;
            position++;
        }
        return position;
    }

    /// <summary>
    /// Returns a random position of the array, taking into account the value is the weight
    /// so double[] { 0.25, 0.25, 0.5 } has 0=25%, 1=25% and 2=50%
    /// in the enum.
    /// </summary>
    public static int NextWeight(this Random random, float[] values) {
        var total = 0D;
        var sum = 0D;
        var position = 0;
        var span = values.AsSpan();
        for (var j = 0; j < span.Length; j++) total += span[j];
        var x = random.NextDouble() * total;
        while (position < span.Length) {
            sum += span[position];
            if (sum > x) break;
            position++;
        }
        return position;
    }

    /// <summary>
    /// Returns a random position of the array, taking into account the value is the weight
    /// so double[] { 0.25, 0.25, 0.5 } has 0=25%, 1=25% and 2=50%
    /// in the enum.
    /// </summary>
    public static int NextWeight(this Random random, double[] values) {
        var total = 0D;
        var sum = 0D;
        var position = 0;
        var span = values.AsSpan();
        for (var j = 0; j < span.Length; j++) total += span[j];
        var x = random.NextDouble() * total;
        while (position < span.Length) {
            sum += span[position];
            if (sum > x) break;
            position++;
        }
        return position;
    }

    /// <summary>
    /// Returns a random element of the array, taking into account the value is the weight
    /// so double[] { 0.25, 0.25, 0.5 } has 0=25%, 1=25% and 2=50%
    /// in the enum.
    /// </summary>
    public static int NextWeight(this Random random, IWeight[] values) {
        var total = 0D;
        var sum = 0D;
        var position = 0;
        var span = values.AsSpan();
        for (var j = 0; j < span.Length; j++) total += span[j].Weight;
        var x = random.NextDouble() * total;
        while (position < span.Length) {
            sum += span[position].Weight;
            if (sum > x) break;
            position++;
        }
        return position;
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

    public static string DiscreteHistogram(Func<int> producer, int barSize = 40, int sampleCount = 100000) {
        var dict = new Producer<int>(producer)
            .Take(sampleCount)
            .GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());
        int labelMax = dict.Keys
            .Select(x => x.ToString().Length)
            .Max();

        string ToLabel(int t) => t.ToString().PadLeft(labelMax);

        var sup = dict.Keys.OrderBy(t => t).ToList();
        int max = dict.Values.Max();
        double scale = max < barSize ? 1.0 : ((double)barSize) / max;

        Func<int, string> Bar = t => new string('#', (int)(dict[t] * scale));
        Func<int, string> Hint = t => new string('·', barSize - (int)(dict[t] * scale) + 1) + " " + dict[t];

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

public interface IWeight {
    public float Weight { get; } 
}

internal class RandomTest {
    internal enum Pepe {
        A,B,C
    }
    public static void Main() {
        var x = new Random();
        Console.WriteLine(Distribution.Histogram(-5d, 6d, () => x.Range(-5L, 5L)));
        // Console.WriteLine(Distribution.Histogram(-2.5d, 2.5d, () => Normal.Sample(0.0, 0.5), 30));
        // Console.WriteLine(Distribution.DiscreteHistogram(() => Poisson.Sample(20)));
        Console.WriteLine(Distribution.DiscreteHistogram(() => x.Range(-10, 10)));
        // Console.WriteLine(Distribution.DiscreteHistogram(() => x.NextWeight(new[] { 1, 1, 2, 10 })));
        // Console.WriteLine(Distribution.DiscreteHistogram(() => x.NextWeight(new[] { 1L, 1L, 2L, 10L })));
        // Console.WriteLine(Distribution.DiscreteHistogram(() => x.NextWeight(new[] { 0.1, 0.1, 0.2, 1 })));
        Console.WriteLine(Distribution.DiscreteHistogram(() => x.NextWeight(new[] { 0.1f, 0.1f, 0.2f, 1f })));
        Console.WriteLine(Distribution.DiscreteHistogram(() => x.NextEnum<Pepe>()));
    }
}