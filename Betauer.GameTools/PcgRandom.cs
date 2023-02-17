using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Betauer.Core;

namespace Betauer;

public class PcgRandom {
    // https://tryfinally.dev/distribution-sampling-trandom-statdist
    // https://statdist.com/
    // https://numerics.mathdotnet.com/Probability.html
    // https://github.com/mathnet/mathnet-numerics
    // https://www.cambiaresearch.com/articles/13/csharp-randomprovider-class

    public readonly Random Random;

    public PcgRandom() {
        Random = new Pcg.PcgRandom();
    }

    public PcgRandom(int seed) {
        Random = new Pcg.PcgRandom(seed);
    }

    /// <summary>
    /// Returns int in the range [start, end] (both inclusive)
    /// </summary>
    public int Range(int start, int end) {
        if (start == end) return start;
        if (start > end) (start, end) = (end, start);
        return Random.Next(start, end + 1);
    }

    /// <summary>
    /// Returns long in the range [start, end] (both inclusive)
    /// </summary>
    public long Range(long start, long end) {
        if (start == end) return start;
        if (start > end) (start, end) = (end, start);
        return Random.NextInt64(start, end + 1);
    }

    /// <summary>
    /// Returns DateTime in the range [min, max] (both inclusive)
    /// </summary>
    public DateTime Range(DateTime min, DateTime max) {
        return new DateTime(Range(min.Ticks, max.Ticks));
    }

    /// <summary>
    /// Returns DateTime in the range [min, max] (both inclusive)
    /// </summary>
    public TimeSpan Range(TimeSpan min, TimeSpan max) {
        return new TimeSpan(Range(min.Ticks, max.Ticks));
    }


    /// <summary>
    /// Returns float (Single) in the range [start, end) (max is excluded)
    /// </summary>
    public float Range(float start, float endExcluded) {
        if (start > endExcluded) (start, endExcluded) = (endExcluded, start);
        var limit = endExcluded - start;
        var rn = Random.NextDouble() * limit;
        return (float)rn + start;
    }

    /// <summary>
    /// Returns double in the range [start, end) (end is excluded)
    /// </summary>
    public double Range(double start, double endExcluded) {
        if (start > endExcluded) (start, endExcluded) = (endExcluded, start);
        var limit = endExcluded - start;
        var rn = Random.NextDouble() * limit;
        return rn + start;
    }

    public bool NextBool() {
        return Random.Next(0, 2) == 0;
    }

    public long NextInt() {
        return Random.Next();
    }

    public long NextLong() {
        return Random.NextInt64();
    }

    public double NextFloat() {
        return Random.NextSingle();
    }

    public double NextDouble() {
        return Random.NextDouble();
    }

    /// <summary>
    /// Returns a uniformly random integer representing one of the values 
    /// in the enum.
    /// </summary>
    public int NextEnum<T>() where T : Enum {
        var values = (int[])Enum.GetValues(typeof(T));
        var randomIndex = Random.Next(0, values.Length);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns a uniformly random element from the list
    /// in the enum.
    /// </summary>
    public T NextElement<T>(IList<T> values) {
        var randomIndex = Random.Next(0, values.Count);
        return values[randomIndex];
    }

    /// <summary>
    /// Returns a uniformly random element from the list
    /// in the enum.
    /// </summary>
    public T NextElement<T>(HashSet<T> values) {
        var randomIndex = Random.Next(0, values.Count);
        return values.ElementAt(randomIndex);
    }

    /// <summary>
    /// Returns a uniformly random element from the array 
    /// in the enum.
    /// </summary>
    public T NextElement<T>(T[] values) {
        var randomIndex = Random.Next(0, values.Length);
        return values[randomIndex];
    }
}

internal class PcgRandomTest {
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
        int min = dict.Values.Min();
        double scale = max < barSize ? 1.0 : ((double)barSize) / max;

        Func<int, string> Bar = t => new string('*', (int)(dict[t] * scale));
        Func<int, string> Hint = t => new string(' ', barSize - (int)(dict[t] * scale) + 1) + " " + dict[t];

        return sup.Select(t => $"{ToLabel(t)}|{Bar(t)}{Hint(t)}").Concatenated("\n") + $"\nMin: {min}, Max: {max}:\n";
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
                       var bar = new string('*', percent);
                       var hint = new string(' ', barSize - percent + 1) + buckets[r] + "\n";
                       return string.Join("", label, bar, hint);
                   })
                   .Concatenated()
               + new string('-', barSize) + "\n"
               + $"Min: {min}, Max: {max}:\n";
    }


    public static void Main() {
        var x = new PcgRandom();
        Console.WriteLine(Histogram(-5d, 6d, () => x.Range(-5L, 5L)));
        // Console.WriteLine(Histogram(-2.5d, 2.5d, () => Normal.Sample(0.0, 0.5), 30));
        // Console.WriteLine(DiscreteHistogram(() => Poisson.Sample(20)));
        Console.WriteLine(DiscreteHistogram(() => x.Range(-10, 10)));
    }
}