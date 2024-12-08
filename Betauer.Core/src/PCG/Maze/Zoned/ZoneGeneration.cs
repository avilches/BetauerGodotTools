using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

internal class ZoneGeneration<T>(MazeGraphZoned<T> graphZoned, IMazeZonedConstraints constraints, int zoneId) {
    internal int ZoneId { get; } = zoneId;
    internal int NodesCreated = 0;
    internal int DoorsOutCreated = 0;
    internal bool IsCorridor => constraints.IsCorridor(ZoneId);

    internal List<ZonePartGeneration<T>> Parts { get; } = [];

    // These are the nodes with free adjacent positions after the GrowZoned method has finished
    internal List<MazeNode<T>> AvailableNodes { get; set; } = [];

    internal int MaxDoorsOut {
        get {
            var maxDoorsOut = constraints.GetMaxDoorsOut(ZoneId);
            return maxDoorsOut == -1 ? NodesCreated * NodesCreated : maxDoorsOut;
        }
    }

    internal void AddNodeToSamePart(MazeNode<T> currentNode, MazeNode<T> newNode) {
        var part = Parts.First(p => p.Nodes.Contains(currentNode));
        part.Nodes.Add(newNode);
        newNode.PartId = part.PartId;
    }

    internal ZonePartGeneration<T> CreateNewPart(MazeNode<T> startNode) {
        var part = new ZonePartGeneration<T>(Parts.Count, startNode);
        Parts.Add(part);
        part.Nodes.Add(startNode);
        startNode.PartId = part.PartId;
        return part;
    }

    internal Zone<T> ToZone() {
        var zone = new Zone<T>(ZoneId);
        Parts.Select(part => new ZonePart<T>(zone, part.PartId, part.StartNode, part.Nodes)).ForEach(zone.Parts.Add);
        return zone;
    }
}