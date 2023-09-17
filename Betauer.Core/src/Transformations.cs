namespace Betauer.Core;

public static partial class Transformations {
    public static T[,] Rotate90<T>(this T[,] source) {
        var width = source.GetLength(0);
        var height = source.GetLength(1);
        T[,] temp = new T[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                temp[i, j] = source[height - 1 - j, i];
            }
        }
        return temp;
    }

    public static T[,] Rotate180<T>(this T[,] source) {
        var width = source.GetLength(0);
        var height = source.GetLength(1);
        T[,] temp = new T[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                temp[i, j] = source[width - 1 - i, height - 1 - j];
            }
        }
        return temp;
    }

    public static T[,] RotateMinus90<T>(this T[,] source) {
        var width = source.GetLength(0);
        var height = source.GetLength(1);
        T[,] temp = new T[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                temp[i, j] = source[j, width - 1 - i];
            }
        }
        return temp;
    }

    public static T[,] FlipH<T>(this T[,] source) {
        var width = source.GetLength(0);
        var height = source.GetLength(1);
        T[,] temp = new T[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                temp[i, j] = source[i, height - 1 - j];
            }
        }
        return temp;
    }

    public static T[,] FlipV<T>(this T[,] source) {
        var width = source.GetLength(0);
        var height = source.GetLength(1);
        T[,] temp = new T[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                temp[i, j] = source[width - 1 - i, j];
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
        var width = source.GetLength(0);
        var height = source.GetLength(1);
        T[,] temp = new T[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                temp[i, j] = source[j, i];
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
        var width = source.GetLength(0);
        var height = source.GetLength(1);
        T[,] temp = new T[width, height];
        for (var i = 0; i < width; i++) {
            for (var j = 0; j < height; j++) {
                temp[i, j] = source[width - 1 - j, height - 1 - i];
            }
        }
        return temp;
    }
}