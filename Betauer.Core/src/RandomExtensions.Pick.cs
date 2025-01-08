using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Betauer.Core;

public static partial class RandomExtensions {

    /// <summary>
    /// Returns a random position of the array, taking into account each value is a weight
    /// new float[] { 1, 1, 2 } the chance of every position is 0=25%, 1=25% and 2=50%
    /// new float[] { 1, 2, 3 } the chance of every position is 0=16.6%, 1=33.3% and 2=50%
    /// </summary>
    public static int PickPosition(this Random random, Span<float> span) {
        var total = 0D;
        var sum = 0D;
        var position = 0;
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
    /// Returns a random position of the array, taking into account each value is a weight
    /// new List<float> { 1, 1, 2 } the chance of every position is 0=25%, 1=25% and 2=50%
    /// new List<float> { 1, 2, 3 } the chance of every position is 0=16.6%, 1=33.3% and 2=50%
    /// </summary>
    public static int PickPosition(this Random random, List<float> values) {
        return PickPosition(random, CollectionsMarshal.AsSpan(values));
    }

    /// <summary>
    /// Returns a random position of the array, taking into account each value is a weight
    /// new List<float> { 1, 1, 2 } the chance of every position is 0=25%, 1=25% and 2=50%
    /// new List<float> { 1, 2, 3 } the chance of every position is 0=16.6%, 1=33.3% and 2=50%
    /// </summary>
    public static int PickPosition(this Random random, float[] values) {
        return PickPosition(random, values.AsSpan());
    }

    /// <summary>
    /// Returns a random position of the list, taking into account each value has a weight
    /// new List<IWeight> { WeightValue.Create("A", 1f), WeightValue.Create("B", 1f), WeightValue.Create("C", 2f) } chances are A=25%, B=25% and C=50%
    /// new List<IWeight> { WeightValue.Create("A", 1f), WeightValue.Create("B", 2f), WeightValue.Create("C", 3f) } chances are A=16.6%, B=33.3% and C=50%
    /// </summary>
    public static int PickPosition<T>(this Random random, T[] values) where T : IWeight {
        return PickPosition(random, values.AsSpan());
    }

    /// <summary>
    /// Returns a random position of the list, taking into account each value has a weight
    /// new List<IWeight> { WeightValue.Create("A", 1f), WeightValue.Create("B", 1f), WeightValue.Create("C", 2f) } chances are A=25%, B=25% and C=50%
    /// new List<IWeight> { WeightValue.Create("A", 1f), WeightValue.Create("B", 2f), WeightValue.Create("C", 3f) } chances are A=16.6%, B=33.3% and C=50%
    /// </summary>
    public static int PickPosition<T>(this Random random, List<T> values) where T : IWeight {
        return PickPosition(random, CollectionsMarshal.AsSpan(values));
    }

    /// <summary>
    /// Returns a random position of the array, taking into account each value has a weight
    /// new IWeight[] { WeightValue.Create("A", 1f), WeightValue.Create("B", 1f), WeightValue.Create("C", 2f) } chances are A=25%, B=25% and C=50%
    /// new IWeight[] { WeightValue.Create("A", 1f), WeightValue.Create("B", 2f), WeightValue.Create("C", 3f) } chances are A=16.6%, B=33.3% and C=50%
    /// </summary>
    public static int PickPosition<T>(this Random random, Span<T> span) where T : IWeight {
        var total = 0D;
        var sum = 0D;
        var position = 0;
        for (var j = 0; j < span.Length; j++) total += span[j].Weight;
        var x = random.NextDouble() * total;
        while (position < span.Length) {
            sum += span[position].Weight;
            if (sum > x) break;
            position++;
        }
        return position;
    }

    /// <summary>
    /// Returns a random element of the array, taking into account each value has a weight
    /// new IWeight[] { WeightValue.Create("A", 1f), WeightValue.Create("B", 1f), WeightValue.Create("C", 2f) } chances are A=25%, B=25% and C=50%
    /// new IWeight[] { WeightValue.Create("A", 1f), WeightValue.Create("B", 2f), WeightValue.Create("C", 3f) } chances are A=16.6%, B=33.3% and C=50%
    /// </summary>
    public static T Pick<T>(this Random random, T[] values) where T : IWeight {
        var position = PickPosition(random, values);
        return values[position];
    }

    /// <summary>
    /// Returns a random element of the list, taking into account each value has a weight
    /// new List<IWeight> { WeightValue.Create("A", 1f), WeightValue.Create("B", 1f), WeightValue.Create("C", 2f) } chances are A=25%, B=25% and C=50%
    /// new List<IWeight> { WeightValue.Create("A", 1f), WeightValue.Create("B", 2f), WeightValue.Create("C", 3f) } chances are A=16.6%, B=33.3% and C=50%
    /// </summary>
    public static T Pick<T>(this Random random, List<T> values) where T : IWeight {
        var position = PickPosition(random, values);
        return values[position];
    }
}

public interface IWeight {
    public float Weight { get; } 
}

public readonly record struct WeightValue<T>(T Value, float Weight) : IWeight;
