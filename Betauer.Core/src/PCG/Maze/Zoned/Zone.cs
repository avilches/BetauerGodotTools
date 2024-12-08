using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class Zone<T>(MazeGraphZoned<T> graphZoned, int zoneId) {
    public MazeGraphZoned<T> GraphZoned { get; } = graphZoned;
    public int ZoneId { get; } = zoneId;

    public List<ZonePart<T>> Parts { get; } = [];
    
    public float NodeCount => Parts.Sum(p => p.Nodes.Count);

    public IEnumerable<MazeNode<T>> GetNodes() => Parts.SelectMany(part => part.Nodes);
    
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