using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.DataMath.Data;

/// <summary>
/// A unidimensional array of T values that can be accessed by float normalized coordinates (0.0..1.0), no matter of the size
/// </summary>
/// <typeparam name="T"></typeparam>
public class FloatArray<T> {
    public T[] Array { get; set; }

    public FloatArray(int size) {
        Array = new T[size];
    }

    public void Set(int x, T value) {
        Array[x] = value;
    }

    public void Set(int x, int width, T value) {
        for (var xx = 0; xx < width; xx++) {
            Array[x + xx] = value;
        }
    }

    public T Get(float x) {
        var maxValue = Array.Length - 1;
        var pos = Mathf.RoundToInt(Mathf.Lerp(0, maxValue, x));
        return Array[System.Math.Clamp(pos, 0, maxValue)];    
    }

    public static FloatArray<TT> Parse<TT>(string line, Dictionary<char, TT> mapping) {
        var width = line.Length;
        var floatArray = new FloatArray<TT>(width);
        var x = 0;
        foreach (var value in line.Select(c => mapping[c])) {
            floatArray.Set(x, value);
            x++;
        }
        return floatArray;
    }
}