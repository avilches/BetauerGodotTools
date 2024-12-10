using System;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeZonedConstraints(int maxZones) : IMazeZonedConstraints {
    public int MaxZones { get; set; } = maxZones;
    public int MaxTotalNodes { get; set; } = -1;
    public int PartsPerZone { get; set; } = 1;
    public int MaxExitNodes { get; set; } = int.MaxValue;
    public bool CreateMorePartsIfNotSpaceToExpand { get; set; } = true;
    public bool Corridors { get; set; } = false;
    public int[] NodesPerZone { get; set; }
    
    public MazeZonedConstraints(int maxZones, int maxTotalNodes) : this(maxZones) {
        if (maxTotalNodes < maxZones) {
            throw new ArgumentException($"Max total nodes {maxTotalNodes} must be greater than max zones {maxZones}");
        }
        MaxTotalNodes = maxTotalNodes;
        NodesPerZone = [maxTotalNodes / maxZones];
    }

    public int GetNodesPerZone(int zoneId) {
        // The array could have fewer elements than maxZones, so we use the last value for the remaining zones
        return NodesPerZone[Math.Min(zoneId, NodesPerZone.Length - 1)];
    }

    public int GetParts(int zoneId) {
        return zoneId == 0 ? 1 : PartsPerZone;
    }

    public int GetMaxExitNodes(int zoneId) {
        return zoneId == MaxZones - 1 ? 0 : MaxExitNodes;
    }

    public bool IsAutoSplitOnExpand(int zoneId) {
        return CreateMorePartsIfNotSpaceToExpand;
    }

    public bool IsCorridor(int zoneId) {
        return Corridors;
    }

    public MazeZonedConstraints SetPartsPerZone(int partsPerZone) {
        if (partsPerZone < 1) {
            throw new ArgumentException($"Wrong partsPerZone value: {partsPerZone}, it must be greater or equals than 1");
        }
        var minimumNodesPerZone = NodesPerZone.Min();
        if (partsPerZone > minimumNodesPerZone) {
            throw new ArgumentException($"Wrong partsPerZone value: {partsPerZone}, it must be greater or equals than the minimum nodes per zone {minimumNodesPerZone}");
        }
        PartsPerZone = partsPerZone;
        return this;
    }

    public MazeZonedConstraints SetMaxExitNodes(int maxExitNodes) {
        if (maxExitNodes < 1) {
            throw new ArgumentException($"Wrong maxExitNodes value: {maxExitNodes}, it must be greater or equals than 1");
        }
        MaxExitNodes = maxExitNodes;
        return this;
    }

    public MazeZonedConstraints SetAutoSplitOnExpand(bool autoSplitOnExpand) {
        CreateMorePartsIfNotSpaceToExpand = autoSplitOnExpand;
        return this;
    }

    public MazeZonedConstraints SetCorridors(bool corridors) {
        Corridors = corridors;
        return this;
    }

    private int GetTotalNodes() {
        var totalNodes = NodesPerZone.Sum();
        // The array could have fewer elements than maxZones, so we need to add the remaining nodes duplicating the last value
        totalNodes += NodesPerZone[^1] * (maxZones - NodesPerZone.Length);
        return totalNodes;
    }

    public MazeZonedConstraints SetNodesPerZones(params int[] nodesPerZone) {
        NodesPerZone = nodesPerZone;
        if (nodesPerZone.Any(v => v < 1)) {
            throw new ArgumentException("All nodes per zone values must be greater or equals than 1");
        }
        return this;
    }

    public MazeZonedConstraints SetRandomNodesPerZone(int minNodesPerZone, int maxNodesPerZoneExclusive, Random? rng = null) {
        if (minNodesPerZone < 1) {
            throw new ArgumentException($"Wrong minNodesPerZone value: {minNodesPerZone}. Must be greater or equals than 1");
        }
        if (maxNodesPerZoneExclusive <= minNodesPerZone) {
            throw new ArgumentException($"Wrong maxNodesPerZoneExclusive value: {maxNodesPerZoneExclusive}. Must be greater than minNodesPerZone {minNodesPerZone}");
        }
        rng ??= new Random();
        NodesPerZone = new int[maxZones];
        for (var i = 0; i < maxZones; i++) {
            NodesPerZone[i] = rng.Next(minNodesPerZone, maxNodesPerZoneExclusive);
        }
        return this;
    }
}