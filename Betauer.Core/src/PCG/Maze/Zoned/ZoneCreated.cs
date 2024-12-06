using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeNodePart<T>(int partId, MazeNode<T> startNode) {
    public int PartId { get; } = partId;
    public MazeNode<T> StartNode { get; } = startNode;
    public List<MazeNode<T>> Nodes { get;} = [startNode];

    public void AddNode(MazeNode<T> node) {
        Nodes.Add(node);
    }
}

public class ZoneCreated<T>(IMazeZonedConstraints constraints, int zoneId) {
    public int ZoneId { get; } = zoneId;
    public int Nodes { get; internal set; } = 0;
    public List<MazeNode<T>> AvailableNodes { get; internal set; } = new();
    public int ConfigParts => constraints.GetParts(ZoneId);
    public int DoorsOut { get; internal set; } = 0;
    public bool Corridor => constraints.IsCorridor(ZoneId);
    public List<MazeNodePart<T>> Parts { get; } = [];
    
    public int MaxDoorsOut {
        get {
            var maxDoorsOut = constraints.GetMaxDoorsOut(ZoneId);
            return maxDoorsOut == -1 ? Nodes * Nodes : maxDoorsOut;
        }
    }
    
    public MazeNodePart<T> FindPart(MazeNode<T> node) {
        return Parts.First(p => p.Nodes.Contains(node));
    }
    public void CreateNewPart(MazeNode<T> startNode) {
        var part = new MazeNodePart<T>(Parts.Count, startNode);
        Parts.Add(part);
    }
}