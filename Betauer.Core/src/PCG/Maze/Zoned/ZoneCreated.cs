using System.Collections.Generic;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZoneCreated<T>(IMazeZonedConstraints constraints, int id) {
    public int Id { get; } = id;
    public int Nodes { get; internal set; } = 0;
    public List<MazeNode<T>> AvailableNodes { get; internal set; } = new();
    public int Parts { get; internal set; } = 0;
    public int ConfigParts => constraints.GetParts(Id);
    public int DoorsOut { get; internal set; } = 0;
    public bool Corridor => constraints.IsCorridor(Id);
    
    public int MaxDoorsOut {
        get {
            var maxDoorsOut = constraints.GetMaxDoorsOut(Id);
            return maxDoorsOut == -1 ? Nodes * Nodes : maxDoorsOut;
        }
    }
}