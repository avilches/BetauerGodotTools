using System;

namespace Betauer.Core;

public static partial class Transformations {
    public static T[,] Clone<T>(this T[,] source) {
        var d0 = source.GetLength(0);
        var d1 = source.GetLength(1);
        var temp = new T[d0, d1];
        for (var d0Pos = 0; d0Pos < d0; d0Pos++) {
            for (var d1Pos = 0; d1Pos < d1; d1Pos++) {
                temp[d0Pos, d1Pos] = source[d0Pos, d1Pos];
            }
        }
        return temp;
    }
}