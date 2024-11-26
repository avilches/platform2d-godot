using System.Text;

namespace Betauer.Core.DataMath;

using System;
using System.Collections;

public class BitArray2D {
    public BitArray[] Rows { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public BitArray2D(BitArray2D other) {
        Width = other.Width;
        Height = other.Height;
        Rows = new BitArray[Height];
        for (var i = 0; i < Height; i++) {
            Rows[i] = new BitArray(other.Rows[i]);
        }
    }

    public BitArray2D(int width, int height) {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

        Width = width;
        Height = height;
        Rows = new BitArray[height];
        for (var i = 0; i < height; i++) {
            Rows[i] = new BitArray(width);
        }
    }

    public bool this[int row, int col] {
        get {
            ValidateIndices(row, col);
            return Rows[row][col];
        }
        set {
            ValidateIndices(row, col);
            Rows[row][col] = value;
        }
    }

    private void ValidateIndices(int row, int col) {
        if (row < 0 || row >= Height) throw new ArgumentOutOfRangeException(nameof(row));
        if (col < 0 || col >= Width) throw new ArgumentOutOfRangeException(nameof(col));
    }

    public void Clear() {
        for (var i = 0; i < Height; i++) {
            Rows[i].SetAll(false);
        }
    }

    public void SetAll(bool value = true) {
        for (var i = 0; i < Height; i++) {
            Rows[i].SetAll(value);
        }
    }

    public string GetString(char lineSeparator = '\n') {
        return GetString(v => v ? "#" : "·", lineSeparator);
    }

    public string GetString(Func<bool, string> transform, char lineSeparator = '\n') {
        var s = new StringBuilder();
        for (var yy = 0; yy < Height; yy++) {
            for (var xx = 0; xx < Width; xx++) {
                if (yy > 0 && xx == 0) s.Append(lineSeparator);
                s.Append(transform(this[yy, xx]));
            }
        }
        return s.ToString();
    }
}