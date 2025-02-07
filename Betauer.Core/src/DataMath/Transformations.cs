using System;

namespace Betauer.Core.DataMath;

public static partial class Transformations {

    public enum Type {
        // Non destructive
        Rotate90,
        Rotate180,
        RotateMinus90,
        FlipH,
        FlipV,

        // Destructive
        MirrorLR,
        MirrorRL,
        MirrorTB,
        MirrorBT
    }

    public static T[,] Transform<T>(this T[,] source, Type type) {
        return type switch {
            Type.Rotate90 => source.Rotate90(),
            Type.Rotate180 => source.Rotate180(),
            Type.RotateMinus90 => source.RotateMinus90(),
            Type.FlipH => source.FlipH(),
            Type.FlipV => source.FlipV(),
            Type.MirrorLR => source.MirrorLeftToRight(),
            Type.MirrorRL => source.MirrorRightToLeft(),
            Type.MirrorTB => source.MirrorTopToBottom(),
            Type.MirrorBT => source.MirrorBottomToTop(),
        };
    }


    public static T[,] Clone<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var clone = new T[height, width];
        for (var x = 0; x < height; x++) {
            for (var y = 0; y < width; y++) {
                clone[x, y] = source[x, y];
            }
        }
        return clone;
    }
    
    public static T[,] Rotate90<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var temp = new T[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[x, y] = source[height - 1 - y, x];
            }
        }
        return temp;
    }
    
    public static T[,] Rotate180<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var temp = new T[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source[height - 1 - y, width - 1 - x];
            }
        }
        return temp;
    }
    
    public static T[,] RotateMinus90<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var temp = new T[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[width - 1 - x, y] = source[y, x];
            }
        }
        return temp;
    }
    
    public static T[,] FlipH<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var temp = new T[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source[y, width - 1 - x];
            }
        }
        return temp;
    }
    
    public static T[,] MirrorLeftToRight<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var halfWidth = width / 2;
        if (width % 2 != 0) halfWidth++;
        var temp = new T[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < halfWidth; x++) {
                temp[y, x] = source[y, x];
                temp[y, width - 1 - x] = source[y, x];
            }
        }
        return temp;
    }
    
    public static T[,] MirrorRightToLeft<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var halfWidth = width / 2;
        if (width % 2 != 0) halfWidth++;
        var temp = new T[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < halfWidth; x++) {
                temp[y, x] = source[y, width - 1 - x];
                temp[y, width - 1 - x] = source[y, width - 1 - x];
            }
        }
        return temp;
    }

    public static T[,] FlipV<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var temp = new T[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source[height - 1 - y, x];
            }
        }
        return temp;
    }
    
    public static T[,] MirrorTopToBottom<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var halfHeight = height / 2;
        if (height % 2 != 0) halfHeight++;
        var temp = new T[height, width];
        for (var y = 0; y < halfHeight; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source[y, x];
                temp[height - 1 - y, x] = source[y, x];
            }
        }
        return temp;
    }

    public static T[,] MirrorBottomToTop<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var halfHeight = height / 2;
        if (height % 2 != 0) halfHeight++;
        var temp = new T[height, width];
        for (var y = 0; y < halfHeight; y++) {
            for (var x = 0; x < width; x++) {
                temp[y, x] = source[height - 1 - y, x];
                temp[height - 1 - y, x] = source[height - 1 - y, x];
            }
        }
        return temp;
    }

    /// <summary>
    /// Rotate by primary diagonal from up,left -> down,right
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[,] FlipDiagonal<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var temp = new T[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[x, y] = source[y, x];
            }
        }
        return temp;
    }

    /// <summary>
    /// Rotate by secondary diagonal from up,right -> down,left
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[,] FlipDiagonalSecondary<T>(this T[,] source) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        var temp = new T[width, height];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                temp[width - 1 - x, height - 1 - y] = source[y, x];
            }
        }
        return temp;
    }

    public static T[,] Resize<T>(this T[,] source, int newWidth, int newHeight, T defaultValue = default) {
        var height = source.GetLength(0);
        var width = source.GetLength(1);
        if (newWidth == width && newHeight == height) {
            return source;
        }
        var newGrid = new T[newHeight, newWidth];
        for (var y = 0; y < newHeight; y++) {
            for (var x = 0; x < newWidth; x++) {
                newGrid[y, x] = x < width && y < height ? source[y, x] : defaultValue;
            }
        }
        return newGrid;
    }
}