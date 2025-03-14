using System.Collections.Generic;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

internal class ZoneGeneration(MazeGraph graphZoned, IMazeZonedConstraints constraints, int zoneId) {
    internal int ZoneId { get; } = zoneId;
    internal int NodesCreated = 0;
    internal int ExitNodesCreated = 0;
    internal bool IsCorridor => constraints.IsCorridor(ZoneId);

    internal List<ZonePartGeneration> Parts { get; } = [];

    // These are the nodes with free adjacent positions after the GrowZoned method has finished
    internal List<MazeNode> AvailableNodes { get; set; } = [];
    internal List<MazeNode> TemporarilyDiscardedNodes { get; set; } = [];

    internal int MaxExitNodes {
        get {
            var maxExitNodes = constraints.GetMaxExitNodes(ZoneId);
            return maxExitNodes == -1 ? int.MaxValue : maxExitNodes;
        }
    }

    internal void AddNode(int partId, MazeNode newNode) {
        var part = Parts[partId];
        part.Nodes.Add(newNode);
        newNode.PartId = partId;
    }

    internal ZonePartGeneration CreateNewPart(MazeNode startNode) {
        var part = new ZonePartGeneration(Parts.Count, startNode);
        startNode.PartId = part.PartId;
        Parts.Add(part);
        return part;
    }

    internal Zone ToZone() {
        var zone = new Zone(ZoneId);
        foreach (var part in Parts) {
            zone.Parts.Add(new ZonePart(zone, part.PartId, part.StartNode, part.Nodes));
        }
        return zone;
    }

    public void RecoverTemporarilyDiscardedNodes() {
        AvailableNodes.AddRange(TemporarilyDiscardedNodes);
        TemporarilyDiscardedNodes.Clear();
    }
}