using System;
using Betauer.Core.Image;

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
        Draw.GradientCircle(cx, cy, Math.Max(rx, ry), (x, y, value) => {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            var heightValue = value <= 1 ? 1 - value : 0;

            if (overlap == OverlapType.Simple) {
                Data[x, y] += heightValue;
            } else if (overlap == OverlapType.MaxHeight) {
                Data[x, y] = Math.Max(Data[x, y], heightValue);
            }
        });
    }
}