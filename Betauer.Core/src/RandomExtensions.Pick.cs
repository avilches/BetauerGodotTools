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
        var total = 0f;
        var sum = 0f;
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
    /// Returns a random element T from the list, taking into account each value has a weight
    /// </summary>
    public static T Pick<T>(this Random random, (T, float)[] values) {
        return Pick(random, values.AsSpan());
    }

    /// <summary>
    /// Returns a random element T of the list, taking into account each value has a weight
    /// </summary>
    public static T Pick<T>(this Random random, List<(T, float)> values) {
        return Pick(random, CollectionsMarshal.AsSpan(values));
    }

    /// <summary>
    /// Returns a random element T of the array, taking into account each value has a weight
    /// </summary>
    public static T Pick<T>(this Random random, Span<(T, float)> span) {
        var total = 0f;
        var sum = 0f;
        var position = 0;
        for (var j = 0; j < span.Length; j++) total += span[j].Item2;
        var x = random.NextDouble() * total;
        while (position < span.Length) {
            sum += span[position].Item2;
            if (sum > x) break;
            position++;
        }
        return span[position].Item1;
    }
}