using System;

namespace Betauer.Core.DataMath.Data;

/// <summary>
/// Define a range of values that can be accessed by a float point.
/// Example:
/// F = 60)
/// D = [60-70)
/// C = [70-80)
/// B = [80-90)
/// A = [90+
///
/// <code>
/// float[] scoreRanges = { 60f, 70f, 80f, 90f };
/// string[] grades = { "F", "D", "C", "B", "A" };
///
/// var fr = new FloatRange(scoreRanges, grades);
/// fr.GetValue(2f) // "F"
/// fr.GetValue(60f) // "D"
/// fr.GetValue(75f) // "C"
/// fr.GetValue(110f) // "B"
/// </code>
/// </summary>
public class FloatRange<T> {
    public float[] Floats { get; }
    public T[] Values { get; }

    public FloatRange(float[] floats, T[] values) {
        if (floats.Length != values.Length - 1) {
            throw new ArgumentException();
        }
        Floats = floats;
        Values = values;
    }

    public T GetValue(float point) {
        var index = Array.BinarySearch(Floats, point);
        if (index < 0) {
            // If the value is not found, BinarySearch returns the complement of the index of the next smallest value.
            index = ~index;
        } else {
            index++; // This forces the index to be the next value
        }
        return Values[index];
    }
}