using System;

namespace Betauer.Core.PCG.GridTemplate;

[Flags]
public enum DirectionFlags {
    None = 0,
    North = 1 << 0, // 00000001
    East = 1 << 2, // 00000100
    South = 1 << 4, // 00010000
    West = 1 << 6, // 10000000

    // Máscara útil
    All = North | East | South | West // 1111
}