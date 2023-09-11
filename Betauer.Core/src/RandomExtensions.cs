using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Betauer.Core;

public static partial class RandomExtensions {
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
    /// Returns a DateTime in the range [min, max] (both inclusive, within milliseconds)
    /// </summary>
    public static DateTime Range(this Random random, DateTime start, DateTime end) {
        if (start > end) (start, end) = (end, start);
        var diff = (end - start).TotalMilliseconds;
        var rn = random.Next((int)diff + 1000);
        return start.AddMilliseconds(rn);        
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
    /// Returns true with a specific chance. So, NextBool(0.7f) will be true the 70% 
    /// </summary>
    /// <returns></returns>
    public static bool NextBool(this Random random, double chance) {
        var rn = random.NextDouble();
        return rn <= chance;
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