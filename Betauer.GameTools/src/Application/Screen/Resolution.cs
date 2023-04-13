using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Application.Screen;
internal class ResolutionByHeightComparer : IComparer<Resolution> {
    /// <summary>
    /// Compare first the height. If equals, compare the width. 
    /// </summary>
    public int Compare(Resolution left, Resolution right) {
        var height = left.Y.CompareTo(right.Y);
        return height != 0 ? height : left.X.CompareTo(right.X);
    }
}

internal class ResolutionByAreaComparer : IComparer<Resolution> {
    /// <summary>
    /// Multiply the height x width to get the area
    /// </summary>
    public int Compare(Resolution left, Resolution right) => (left.X * left.Y).CompareTo(right.X * right.Y);
}

public class Resolution {
    public static readonly IComparer<Resolution> ComparerByHeight = new ResolutionByHeightComparer();
    public static readonly IComparer<Resolution> ComparerByArea = new ResolutionByAreaComparer();
    public static readonly Comparison<Resolution> ComparisonByArea = (left, right) =>
        ComparerByArea.Compare(left, right);
    public static readonly Comparison<Resolution> ComparisonByHeight = (left, right) =>
        ComparerByHeight.Compare(left, right);

    /**
     * Returns how many times can be multiplied the base resolution (size) without create a resolution bigger than maxSize
     */
    public static int CalculateMaxScale(Vector2 size, Vector2 maxSize) {
        return (int)Mathf.Max(
            Mathf.Floor(Mathf.Min(maxSize.X / size.X, maxSize.Y / size.Y)), 1);
    }

    public readonly Vector2I Size;
    public readonly AspectRatio AspectRatio;

    public Resolution(Vector2I size) {
        Size = size;
        AspectRatio = AspectRatios.Get(this);
    }

    public Resolution(int x, int y) : this(new Vector2I(x, y)) {
    }

    public int X => Size.X;
    public int Y => Size.Y;

    public override string ToString() {
        return $"{AspectRatio.Name} {X}x{Y}";
    }

    /**
     * Two Resolutions are equal if their fields Size are equal
     * Aspect ratio (inherited from parent Resolution class) is a computed value (size.X/size.Y, width/height)
     */
    public static bool operator ==(Resolution? left, Resolution? right) {
        if (ReferenceEquals(left, right)) return true; // equals or both null
        if (right is null || left is null) return false; // one of them is null
        return left.Equals(right); // not the same reference
    }

    public static bool operator !=(Resolution? left, Resolution? right) {
        if (ReferenceEquals(left, right)) return false; // equals or both null
        if (right is null || left is null) return true; // one of them is null
        return !left.Equals(right); // not the same reference
    }

    public override bool Equals(object? obj) => 
        obj is Resolution other && Equals(other);
    
    public bool Equals(Resolution? obj) => 
        ReferenceEquals(this, obj) || 
        (obj is { } && Size.Equals(obj.Size));
    
    public override int GetHashCode() => Size.GetHashCode();
}