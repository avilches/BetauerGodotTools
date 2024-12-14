using System;
using System.Linq;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeZonedConstraints(int maxZones) : IMazeZonedConstraints {
    public int MaxZones { get; set; } = maxZones;
    public int MaxTotalNodes { get; set; } = -1;
    public int PartsPerZone { get; set; } = 1;
    public int MaxExitNodes { get; set; } = int.MaxValue;
    public bool FlexibleParts { get; set; } = true;
    public bool Corridors { get; set; } = false;
    public int[] NodesPerZone { get; set; }
    
    public MazeZonedConstraints(int maxZones, int maxTotalNodes) : this(maxZones) {
        if (maxTotalNodes < maxZones) {
            throw new ArgumentException($"Max total nodes {maxTotalNodes} must be greater than max zones {maxZones}");
        }
        MaxTotalNodes = maxTotalNodes;
        var baseNodesPerZone = maxTotalNodes / maxZones;
        var remaining = maxTotalNodes % maxZones;
        NodesPerZone = new int[maxZones];
        for (var i = 0; i < maxZones; i++) {
            NodesPerZone[i] = baseNodesPerZone + (i < remaining ? 1 : 0);
        }
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

    /// <summary>
    /// If true, the algorithm will expand the zone instead of create more part if there are no more exit nodes in the previous zones.
    /// In this case, the zone will have less parts, but at least it will have all the nodes needed for the zone.
    /// </summary>
    /// <param name="zoneId"></param>
    /// <returns></returns>
    public bool IsFlexibleParts(int zoneId) {
        return FlexibleParts;
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

    public MazeZonedConstraints SetFlexibleParts(bool flexibleParts) {
        FlexibleParts = flexibleParts;
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