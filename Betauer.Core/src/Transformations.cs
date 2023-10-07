using System;

namespace Betauer.Core;

public static partial class Transformations {
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

    public static T[,] GetSubGrid<T>(this T[,] source, int startX, int startY, int width, int height, T defaultValue = default) {
        var destination = new T[height, width];
        source.CopyGrid(startX, startY, width, height, destination, value => value, defaultValue);
        return destination;
    }

    public static TDest[,] GetSubGrid<TSource, TDest>(this TSource[,] source, int startX, int startY, int width, int height, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[height, width];
        source.CopyGrid(startX, startY, width, height, destination, transformer, defaultValue);
        return destination;
    }

    public static TDest[,] GetGrid<TSource, TDest>(this TSource[,] source, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[source.GetLength(0), source.GetLength(1)];
        source.CopyGrid(destination, transformer, defaultValue);
        return destination;
    }

    public static void CopyGrid<T>(this T[,] source, int startX, int startY, int width, int height, T[,] destination, T defaultValue = default) {
        source.CopyGrid(startX, startY, width, height, destination, value => value, defaultValue);
    }

    public static void CopyGrid<TSource, TDest>(this TSource[,] source, TDest[,] destination, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        source.CopyGrid(0, 0, destination.GetLength(1), destination.GetLength(0), destination, transformer, defaultValue);
    }

    public static void CopyGrid<TSource, TDest>(this TSource[,] source, int startX, int startY, int width, int height, TDest[,] destination, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var sourceHeight = source.GetLength(0);
        var sourceWidth = source.GetLength(1);
        height = Math.Min(destination.GetLength(0), height);
        width = Math.Min(destination.GetLength(1), width);
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var sourceX = startX + x;
                var sourceY = startY + y;
                if (sourceX >= 0 && sourceX < sourceWidth &&
                    sourceY >= 0 && sourceY < sourceHeight) {
                    destination[y, x] = transformer(source[sourceY, sourceX]);
                } else {
                    destination[y, x] = defaultValue;
                }
            }
        }
    }
}