using System;

namespace Betauer.Core;

public class FloatRange<T> {
    public float[] Floats { get; }
    public T[] Values { get; }

    public FloatRange(float[] floats, T[] values) {
        if (floats.Length != values.Length) {
            throw new ArgumentException("Both arrays should have the same length");
        }
        Floats = floats;
        Values = values;
    }

    public T GetValue(float point) {
        var index = Array.BinarySearch(Floats, point);
        if (index < 0) {
            // If the value is not found, BinarySearch returns the complement of the index of the next smallest value.
            index = ~index;
        }
        if (index <= 0) return Values[0];
        if (index >= Floats.Length) return Values[Floats.Length - 1];
        return Values[index];
    }
}