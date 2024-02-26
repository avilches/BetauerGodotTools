using System;

namespace Betauer.Core;

public static partial class Transformations {
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
        var destination = new TDest[source.GetLength(0), source.GetLength(1)];
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
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
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
    
    public static T[,] CopyXyCenterRect<T>(this T[,] source, int centerX, int centerY, T defaultValue, T[,] destination) {
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
                destination[x, y] = xx < 0 || yy < 0 || xx >= sourceWidth || yy >= sourceHeight ? defaultValue : source[xx, yy];
            }
        }
        return destination;
    }

    public static TOut[,] CopyXyCenterRect<T, TOut>(this T[,] source, int centerX, int centerY, TOut defaultValue, TOut[,] destination, Func<T, TOut> transform) {
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
                destination[x, y] = xx < 0 || yy < 0 || xx >= sourceWidth || yy >= sourceHeight ? defaultValue :  transform(source[xx, yy]);
            }
        }
        return destination;
    }

}