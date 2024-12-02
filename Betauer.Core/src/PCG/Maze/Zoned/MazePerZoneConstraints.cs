using System;
using System.Collections.Generic;
using Betauer.Core.PCG.Maze.Zoned;

namespace Betauer.Core.PCG.Maze;

public class MazePerZoneConstraints(int maxTotalNodes = -1) : IMazeZonedConstraints {
    public List<MazeZone> Zones { get; set; } = [];

    public int MaxTotalNodes { get; set; } = maxTotalNodes;

    public int MaxZones => Zones.Count;

    public MazePerZoneConstraints Zone(int zone, int nodes, int parts = 1, int maxDoorsOut = -1) {
        if (maxDoorsOut == -1) {
            maxDoorsOut = nodes * nodes;
        }
        if (nodes < 1) {
            throw new ArgumentException($"Error adding zone #{zone}: value {nodes} for nodes is wrong, it must be 1 or more");
        }
        if (zone > 0 && parts < 1) {
            throw new ArgumentException($"Error adding zone #{zone}: value {parts} for parts is wrong, it must be 1 or more");
        }
        if (zone == 0 && parts != 1) {
            throw new ArgumentException($"Error adding zone #{zone}: value {parts} for parts is wrong, it must be exactly 1 for zone 0");
        }
        if (parts > nodes) {
            throw new ArgumentException($"Error adding zone #{zone}: value {parts} for parts is wrong. It must be less or equals than {nodes} nodes");
        }
        if (zone == Zones.Count) {
            Zones.Add(new MazeZone(nodes, parts, maxDoorsOut));
        } else if (zone < Zones.Count) {
            Zones[zone] = new MazeZone(nodes, parts, maxDoorsOut);
        } else {
            throw new ArgumentException($"Zone {zone} must be added in order. Please add zone {zone - 1} first");
        }
        return this;
    }

    public int GetNodesPerZone(int zone) {
        return Zones[zone].Nodes;
    }

    public int GetParts(int zone) {
        return Zones[zone].Parts;
    }

    public int GetMaxDoorsOut(int zone) {
        return Zones[zone].MaxDoorsOut;
    }

    public MazePerZoneConstraints SetMaxTotalNodes(int maxTotalNodes) {
        MaxTotalNodes = maxTotalNodes;
        return this;
    }
}