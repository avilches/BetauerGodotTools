using System;
using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.GridTools;

public class Array2DGraph<T>(Array2D<T> array2D, Func<Vector2I, T, bool> isBlockedFunc, Func<Vector2I, T, float>? getWeightFunc = null) :
    GridGraph(array2D.Bounds,
        CreateBlockedFunc(isBlockedFunc, array2D),
        CreateGetWeightFunc(getWeightFunc, array2D)) {
    public Array2D<T> Array2D { get; } = array2D;

    private static Func<Vector2I, bool> CreateBlockedFunc(Func<Vector2I, T, bool> isBlockedFunc, Array2D<T> array2D) {
        return pos => isBlockedFunc(pos, array2D[pos]);
    }

    private static Func<Vector2I, float> CreateGetWeightFunc(Func<Vector2I, T, float> getWeightFunc, Array2D<T> array2D) {
        return pos => getWeightFunc(pos, array2D[pos]);
    }
}