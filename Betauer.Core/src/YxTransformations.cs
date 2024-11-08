using System;

namespace Betauer.Core;

public static partial class Transformations {
    public static T[,] YxRotate90<T>(this T[,] source) {
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
    
    public static T[,] YxRotate180<T>(this T[,] source) {
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
    
    public static T[,] YxRotateMinus90<T>(this T[,] source) {
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
    
    public static T[,] YxFlipH<T>(this T[,] source) {
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
    
    public static T[,] YxMirrorLeftToRight<T>(this T[,] source) {
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
    
    public static T[,] YxMirrorRightToLeft<T>(this T[,] source) {
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

    public static T[,] YxFlipV<T>(this T[,] source) {
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
    
    public static T[,] YxMirrorTopToBottom<T>(this T[,] source) {
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

    public static T[,] YxMirrorBottomToTop<T>(this T[,] source) {
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
    public static T[,] YxFlipDiagonal<T>(this T[,] source) {
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
    public static T[,] YxFlipDiagonalSecondary<T>(this T[,] source) {
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

    public static T[,] YxResize<T>(this T[,] source, int newWidth, int newHeight, T defaultValue = default) {
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
    
    public static T[,] GetYxRect<T>(this T[,] source, int startX, int startY, int width, int height, T defaultValue = default) {
        var destination = new T[height, width];
        source.CopyYxRect(startX, startY, width, height, destination, value => value, defaultValue);
        return destination;
    }

    public static TDest[,] GetYxRect<TSource, TDest>(this TSource[,] source, int startX, int startY, int width, int height, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[height, width];
        source.CopyYxRect(startX, startY, width, height, destination, transformer, defaultValue);
        return destination;
    }

    public static TDest[,] GetYxRect<TSource, TDest>(this TSource[,] source, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[source.GetLength(0), source.GetLength(1)];
        source.CopyYxRect(destination, transformer, defaultValue);
        return destination;
    }

    public static T[,] GetYxRectCenter<T>(this T[,] source, int centerX, int centerY, int size, T defaultValue = default) {
        var destination = new T[size, size];
        var startX = centerX - size / 2;
        var startY = centerY - size / 2;
        source.CopyYxRect(startX, startY, size, size, destination, value => value, defaultValue);
        return destination;
    }

    public static TDest[,] GetYxRectCenter<TSource, TDest>(this TSource[,] source, int centerX, int centerY, int size, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        var destination = new TDest[size, size];
        var startX = centerX - size / 2;
        var startY = centerY - size / 2;
        source.CopyYxRect(startX, startY, size, size, destination, transformer, defaultValue);
        return destination;
    }

    public static void CopyYxRect<T>(this T[,] source, int startX, int startY, int width, int height, T[,] destination, T defaultValue = default) {
        source.CopyYxRect(startX, startY, width, height, destination, value => value, defaultValue);
    }

    public static void CopyYxRect<TSource, TDest>(this TSource[,] source, TDest[,] destination, Func<TSource, TDest> transformer, TDest defaultValue = default) {
        source.CopyYxRect(0, 0, destination.GetLength(1), destination.GetLength(0), destination, transformer, defaultValue);
    }

    public static void CopyYxRect<TSource, TDest>(this TSource[,] source, int startX, int startY, int width, int height, TDest[,] destination, Func<TSource, TDest> transformer, TDest defaultValue = default) {
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
    
    public static void CopyYxCenterRect<T>(this T[,] source, int centerX, int centerY, T defaultValue, T[,] destination) {
        CopyYxCenterRect(source, centerX, centerY, defaultValue, destination, value => value);
    }

    public static void CopyYxCenterRect<T, TOut>(this T[,] source, int centerX, int centerY, TOut defaultValue, TOut[,] destination, Func<T, TOut> transform) {
        var sourceWidth = source.GetLength(1);
        var sourceHeight = source.GetLength(0);
        var width = destination.GetLength(1);
        var height = destination.GetLength(0);
        var startX = centerX - width / 2;
        var startY = centerY - height / 2;
        for (var x = 0; x < width; x++) {
            for (var y = 0; y < height; y++) {
                var xx = startX + x;
                var yy = startY + y;
                destination[y, x] = xx < 0 || yy < 0 || xx >= sourceWidth || yy >= sourceHeight ? defaultValue : transform(source[yy, xx]);
            }
        }
    }
    
    public static int[,] YxSwapColumns(this int[,] source, int sourceColumn, int otherColumn, int size = 1) {
        if (sourceColumn == otherColumn) return source;
        var height = source.GetLength(0);
        for (var s = 0; s < size; ++s) {
            for (var y = 0; y < height; ++y) {
                (source[y, sourceColumn + s], source[y, otherColumn + s]) = (source[y, otherColumn + s], source[y, sourceColumn + s]);
            }
        }
        return source;
    }

    public static int[,] YxSwapRows(this int[,] source, int sourceRow, int otherRow, int size = 1) {
        if (sourceRow == otherRow) return source;
        var width = source.GetLength(1);
        for (var s = 0; s < size; ++s) {
            for (var x = 0; x < width; ++x) {
                (source[sourceRow + s, x], source[otherRow + s, x]) = (source[otherRow + s, x], source[sourceRow + s, x]);
            }
        }
        return source;
    }

}