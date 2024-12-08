using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class ZonePart<T>(Zone<T> zone, int partId, MazeNode<T> startNode, List<MazeNode<T>> nodes) {
    public Zone<T> Zone { get; } = zone;
    public int PartId { get; } = partId;
    public MazeNode<T> StartNode { get; } = startNode;
    public List<MazeNode<T>> Nodes { get; } = nodes;

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