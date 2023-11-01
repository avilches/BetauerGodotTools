using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core;

public class FloatArray<T> {
    public T[] Grid { get; set; }
    public float Min { get; set; } = -1.0f;
    public float Max { get; set; } = 1.0f;

    public FloatArray(int size) {
        Grid = new T[size];
    }

    public FloatArray(int size, float minX, float maxX) {
        Grid = new T[size];
        Min = minX;
        Max = maxX;
    }

    public void Set(int x, T value) {
        Grid[x] = value;
    }

    public void Set(int x, int width, T biome) {
        for (var xx = 0; xx < width; xx++) {
            Grid[x + xx] = biome;
        }
    }

    public T Get(float x) {
        var maxValue = Grid.Length - 1;
        var pos = Mathf.RoundToInt(Mathf.Lerp(0, maxValue, (x - Min) / (Max - Min)));
        return Grid[Math.Clamp(pos, 0, maxValue)];
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