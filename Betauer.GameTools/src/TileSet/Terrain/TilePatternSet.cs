using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Terrain;

public class TilePatternSet<T, TT> {
    public List<(T, TilePattern<TT>)> Patterns { get; private set; }
    public Dictionary<string, Func<TT, bool>> DefaultRules { get; init; }

    public TilePatternSet<T, TT> Add(T id, TilePattern<TT> pattern) {
        if (Patterns == null) Patterns = new List<(T, TilePattern<TT>)>();
        Patterns.Add((id, pattern));
        return this;
    }
    
    public TilePatternSet<T, TT> Add(T id, string pattern) {
        return Add(id, TilePattern.Parse(pattern, DefaultRules));
    }
    
    public void Do(TT[,] data, Action<T> action) {
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) action(rule.Item1);
        }
    }

    public bool MatchAny(TT[,] data) {
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) return true;
        }
        return false;
    }

    public T? FindTilePatternId(TT[,] data, T? defaultValue = default) { 
        foreach (var rule in Patterns) {
            if (rule.Item2.Matches(data)) return rule.Item1;
        }
        return defaultValue;
    }
}