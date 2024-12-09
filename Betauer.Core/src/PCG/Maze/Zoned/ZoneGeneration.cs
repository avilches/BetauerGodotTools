using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

internal class ZoneGeneration(MazeGraph graphZoned, IMazeZonedConstraints constraints, int zoneId) {
    internal int ZoneId { get; } = zoneId;
    internal int NodesCreated = 0;
    internal int DoorsOutCreated = 0;
    internal bool IsCorridor => constraints.IsCorridor(ZoneId);

    internal List<ZonePartGeneration> Parts { get; } = [];

    // These are the nodes with free adjacent positions after the GrowZoned method has finished
    internal List<MazeNode> AvailableNodes { get; set; } = [];

    internal int MaxDoorsOut {
        get {
            var maxDoorsOut = constraints.GetMaxDoorsOut(ZoneId);
            return maxDoorsOut == -1 ? NodesCreated * NodesCreated : maxDoorsOut;
        }
    }

    internal void AddNodeToSamePart(MazeNode currentNode, MazeNode newNode) {
        var part = Parts.First(p => p.Nodes.Contains(currentNode));
        part.Nodes.Add(newNode);
        newNode.PartId = part.PartId;
    }

    internal ZonePartGeneration CreateNewPart(MazeNode startNode) {
        var part = new ZonePartGeneration(Parts.Count, startNode);
        Parts.Add(part);
        part.Nodes.Add(startNode);
        startNode.PartId = part.PartId;
        return part;
    }

    internal Zone ToZone() {
        var zone = new Zone(ZoneId);
        Parts.Select(part => new ZonePart(zone, part.PartId, part.StartNode, part.Nodes)).ForEach(zone.Parts.Add);
        return zone;
    }
}