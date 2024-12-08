using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZonePart<T>(Zone<T> zone, int partId, MazeNode<T> startNode) {
    public Zone<T> Zone { get; } = zone;
    public int PartId { get; } = partId;
    public MazeNode<T> StartNode { get; } = startNode;
    public List<MazeNode<T>> Nodes { get; } = [startNode];

    public void AddNode(MazeNode<T> node) {
        Nodes.Add(node);
        node.PartId = PartId;
    }
    
    /// <summary>
    /// A door in node is a node that has an edge to a node from the previous zone. The GrowZoned method only creates one door in
    /// to every zone, stored in the StartNode field in MazeNodePart. But it could be possible to create more connections before, even
    /// from other zones (not only the previous one), so this method will return all possible doors in the part.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorInNodes() {
        return Nodes.Where(n => n.GetInEdges().Any(edge => edge.From.ZoneId < Zone.ZoneId) || n == StartNode);
    }

    /// <summary>
    /// A doors out node is a node that has an edge to a node to next zone. The GrowZoned method only creates as many doors out
    /// as the MaxDoorsOut property of the zone, and store the number of doors out created in the DoorsOut field.
    /// But it could be possible to create more connections before, so this method will return all possible doors out.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorOutNodes() {
        return Nodes.Where(n => n.GetOutEdges().Any(edge => edge.To.ZoneId > Zone.ZoneId));
    }
    
}

public class Zone<T>(MazeGraphZoned<T> graphZoned, IMazeZonedConstraints constraints, int zoneId) {
    public MazeGraphZoned<T> GraphZoned { get; } = graphZoned;
    public int ZoneId { get; } = zoneId;

    public int NodeCount { get; internal set; } = 0;
    public int ConfigParts => constraints.GetParts(ZoneId);
    public int DoorsOut { get; internal set; } = 0;
    public bool Corridor => constraints.IsCorridor(ZoneId);
    
    public List<ZonePart<T>> Parts { get; } = [];

    public IEnumerable<MazeNode<T>> GetNodes() => Parts.SelectMany(part => part.Nodes);
    
    // These are the nodes with free adjacent positions after the GrowZoned method has finished
    public List<MazeNode<T>> AvailableNodes { get; internal set; } = [];
    
    public int MaxDoorsOut {
        get {
            var maxDoorsOut = constraints.GetMaxDoorsOut(ZoneId);
            return maxDoorsOut == -1 ? NodeCount * NodeCount : maxDoorsOut;
        }
    }

    public ZonePart<T> FindPart(MazeNode<T> node) {
        return Parts.First(p => p.Nodes.Contains(node));
    }

    public ZonePart<T> CreateNewPart(MazeNode<T> startNode) {
        var part = new ZonePart<T>(this, Parts.Count, startNode);
        Parts.Add(part);
        part.AddNode(startNode);
        return part;
    }

    /// <summary>
    /// A doors in node is a node that has an edge to a node from the previous zone. The GrowZoned method only creates one door in
    /// to every zone, stored in the StartNode field in MazeNodePart. But it could be possible to create more connections before, even
    /// from other zones (not only the previous one), so this method will return all possible doors in.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorInNodes() {
        return Parts.SelectMany(p => p.GetDoorInNodes());
    }

    /// <summary>
    /// A doors out node is a node that has an edge to a node to next zone. The GrowZoned method only creates as many doors out
    /// as the MaxDoorsOut property of the zone, and store the number of doors out created in the DoorsOut field.
    /// But it could be possible to create more connections before, so this method will return all possible doors out.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorOutNodes() {
        return Parts.SelectMany(p => p.GetDoorOutNodes());
    }
}