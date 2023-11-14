using System;

namespace Betauer.Core.Data;

public class IslandGenerator : NormalizedDataGrid {
    public enum OverlapType {
        Simple,
        MaxHeight,
    }

    public IslandGenerator(int width, int height) : base(width, height) {
    }

    public void Clear() {
        SetAll(0);
    }

    public void AddIsland(int cx, int cy, int rx, int ry, OverlapType overlap) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var distance = (float)Math.Sqrt(Math.Pow((cx - x) / (double)rx, 2) + Math.Pow((cy - y) / (double)ry, 2));
                var heightValue = distance <= 1 ? 1 - distance : 0;

                if (overlap == OverlapType.Simple) {
                    Data[x, y] += heightValue;
                    
                } else if (overlap == OverlapType.MaxHeight) {
                    Data[x, y] = Math.Max(Data[x, y], heightValue);
                }
            }
        }
    }
}