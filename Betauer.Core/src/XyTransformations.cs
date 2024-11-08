using System;

namespace Betauer.Core;

public static partial class Transformations {
    public static T[,] XyRotate90<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var temp = new T[height, width];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                temp[y, x] = source[width - 1 - x, y];
            }
        }
        return temp;
    }

    public static T[,] XyRotate180<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var temp = new T[width, height];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                temp[x, y] = source[width - 1 - x, height - 1 - y];
            }
        }
        return temp;
    }

    public static T[,] XyRotateMinus90<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var temp = new T[height, width];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                temp[y, width - 1 - x] = source[x, y];
            }
        }
        return temp;
    }

    public static T[,] XyFlipH<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var temp = new T[width, height];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                temp[x, y] = source[x, height - 1 - y];
            }
        }
        return temp;
    }

    public static T[,] XyMirrorLeftToRight<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var halfWidth = width / 2;
        if (width % 2 != 0) halfWidth++;
        var temp = new T[width, height];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < halfWidth; y++) {
                temp[x, y] = source[x, y];
                temp[x, width - 1 - y] = source[x, y];
            }
        }
        return temp;
    }

    public static T[,] XyMirrorRightToLeft<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var halfWidth = width / 2;
        if (width % 2 != 0) halfWidth++;
        var temp = new T[width, height];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < halfWidth; y++) {
                temp[x, y] = source[x, width - 1 - y];
                temp[x, width - 1 - y] = source[x, width - 1 - y];
            }
        }
        return temp;
    }

    public static T[,] XyFlipV<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var temp = new T[width, height];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                temp[x, y] = source[x, height - 1 - y];
            }
        }
        return temp;
    }

    public static T[,] XyMirrorTopToBottom<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var halfHeight = height / 2;
        if (height % 2 != 0) halfHeight++;
        var temp = new T[width, height];
        for (var y = 0; y < halfHeight; y++) {
            for (var x = 0; x < width; x++) {
                temp[x, y] = source[x, y];
                temp[x, height - 1 - y] = source[x, y];
            }
        }
        return temp;
    }

    public static T[,] XyMirrorBottomToTop<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var halfHeight = height / 2;
        if (height % 2 != 0) halfHeight++;
        var temp = new T[width, height];
        for (var y = 0; y < halfHeight; y++) {
            for (var x = 0; x < width; x++) {
                temp[x, y] = source[x, height - 1 - y];
                temp[x, height - 1 - y] = source[x, height - 1 - y];
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
    public static T[,] XyFlipDiagonal<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var temp = new T[height, width];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                temp[y, x] = source[x, y];
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
    public static T[,] XyFlipDiagonalSecondary<T>(this T[,] source) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        var temp = new T[height, width];
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                temp[height - 1 - y, width - 1 - x] = source[x, y];
            }
        }
        return temp;
    }

    public static T[,] XyResize<T>(this T[,] source, int newWidth, int newHeight, T defaultValue = default) {
        var height = source.GetLength(1);
        var width = source.GetLength(0);
        if (newWidth == width && newHeight == height) {
            return source;
        }
        var newGrid = new T[newWidth, newHeight];
        for (var x = 0; x < newWidth; x++) {
            for (var y = 0; y < newHeight; y++) {
                newGrid[x, y] = x < width && y < height ? source[x, y] : defaultValue;
            }
        }
        return newGrid;
    }
    
    public static T[,] GetXyRect<T>(this T[,] source, int startX, int startY, int width, int height, T defaultValue = default) {
        var destination = new T[width, height];
        source.CopyXyRect(startX, startY, width, height, destination, value => value, defaultValue);
        return destination;
    }

    public static TDest[,] GetXyRect<TSource, TDest>(this TSource[,] source, int startX, int startY, int width, int height, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[width, height];
        source.CopyXyRect(startX, startY, width, height, destination, transformer, defaultValue);
        return destination;
    }

    public static TDest[,] GetXyRect<TSource, TDest>(this TSource[,] source, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[source.GetLength(1), source.GetLength(0)];
        source.CopyXyRect(destination, transformer, defaultValue);
        return destination;
    }

    public static T[,] GetXyRectCenter<T>(this T[,] source, int centerX, int centerY, int size, T defaultValue = default) {
        var destination = new T[size, size];
        var startX = centerX - size / 2;
        var startY = centerY - size / 2;
        source.CopyXyRect(startX, startY, size, size, destination, value => value, defaultValue);
        return destination;
    }

    public static TDest[,] GetXyRectCenter<TSource, TDest>(this TSource[,] source, int centerX, int centerY, int size, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[size, size];
        var startX = centerX - size / 2;
        var startY = centerY - size / 2;
        source.CopyXyRect(startX, startY, size, size, destination, transformer, defaultValue);
        return destination;
    }

    public static void CopyXyRect<T>(this T[,] source, int startX, int startY, int width, int height, T[,] destination, T defaultValue = default) {
        source.CopyXyRect(startX, startY, width, height, destination, value => value, defaultValue);
    }

    public static void CopyXyRect<TSource, TDest>(this TSource[,] source, TDest[,] destination, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        source.CopyXyRect(0, 0, destination.GetLength(0), destination.GetLength(1), destination, transformer, defaultValue);
    }

    public static void CopyXyRect<TSource, TDest>(this TSource[,] source, int startX, int startY, int width, int height, TDest[,] destination, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var sourceHeight = source.GetLength(1);
        var sourceWidth = source.GetLength(0);
        height = Math.Min(destination.GetLength(1), height);
        width = Math.Min(destination.GetLength(0), width);
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var sourceX = startX + x;
                var sourceY = startY + y;
                if (sourceX >= 0 && sourceX < sourceWidth &&
                    sourceY >= 0 && sourceY < sourceHeight) {
                    destination[x, y] = transformer(source[sourceX, sourceY]);
                } else {
                    destination[x, y] = defaultValue;
                }
            }
        }
    }
    
    public static void CopyXyCenterRect<T>(this T[,] source, int centerX, int centerY, T defaultValue, T[,] destination) {
        CopyXyCenterRect(source, centerX, centerY, defaultValue, destination, value => value);
    }

    public static void CopyXyCenterRect<T, TOut>(this T[,] source, int centerX, int centerY, TOut defaultValue, TOut[,] destination, Func<T, TOut> transform) {
        var sourceWidth = source.GetLength(0);
        var sourceHeight = source.GetLength(1);
        var width = destination.GetLength(0);
        var height = destination.GetLength(1);
        var startX = centerX - width / 2;
        var startY = centerY - height / 2;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var xx = startX + x;
                var yy = startY + y;
                destination[x, y] = xx < 0 || yy < 0 || xx >= sourceWidth || yy >= sourceHeight ? defaultValue : transform(source[xx, yy]);
            }
        }
    }
}