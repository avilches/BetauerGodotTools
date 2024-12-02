using System;
using System.Linq;

namespace Betauer.Core.PCG.Maze;

public class MazeZonedConstraints(int maxZones) : IMazeZonedConstraints {
    public int MaxZones { get; set; } = maxZones;
    public int MaxTotalNodes { get; set; } = -1;
    public int PartsPerZone { get; set; } = 1;
    public int MaxDoorsOut { get; set; } = int.MaxValue;
    public int[] NodesPerZone { get; set; }

    public MazeZonedConstraints(int maxZones, int maxTotalNodes) : this(maxZones) {
        if (maxTotalNodes < maxZones) {
            throw new ArgumentException($"Max total nodes {maxTotalNodes} must be greater than max zones {maxZones}");
        }
        MaxTotalNodes = maxTotalNodes;
        NodesPerZone = [maxTotalNodes / maxZones];
    }

    public int GetNodesPerZone(int zone) {
        // The array could have fewer elements than maxZones, so we use the last value for the remaining zones
        return NodesPerZone[Math.Min(zone, NodesPerZone.Length - 1)];
    }

    public int GetParts(int zone) {
        return zone == 0 ? 1 : PartsPerZone;
    }

    public int GetMaxDoorsOut(int zone) {
        return zone == MaxZones - 1 ? 0 : MaxDoorsOut;
    }

    public MazeZonedConstraints SetPartsPerZone(int partsPerZone) {
        if (partsPerZone < 1) {
            throw new ArgumentException($"Wrong partsPerZone value: {partsPerZone}, it must be greater or equals than 1");
        }
        var minimumNodesPerZone = NodesPerZone.Min();
        if (partsPerZone > minimumNodesPerZone) {
            throw new ArgumentException($"Wrong partsPerZone value: {partsPerZone}, it must be greater or equals than the minimum nodes per zone {minimumNodesPerZone}");
        }
        return this;
    }

    public MazeZonedConstraints SetMaxDoorsOut(int maxDoorsOut) {
        if (maxDoorsOut < 1) {
            throw new ArgumentException($"Wrong maxDoorsOut value: {maxDoorsOut}, it must be greater or equals than 1");
        }
        MaxDoorsOut = maxDoorsOut;
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