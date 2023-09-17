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
}