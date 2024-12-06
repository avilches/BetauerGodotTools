using System.Collections.Generic;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazePerZoneConstraints(int maxTotalNodes = -1) : IMazeZonedConstraints {
    public List<ZoneConfig> Zones { get; set; } = [];

    public int MaxTotalNodes { get; set; } = maxTotalNodes;

    public int MaxZones => Zones.Count;

    public MazePerZoneConstraints SetMaxTotalNodes(int maxTotalNodes) {
        MaxTotalNodes = maxTotalNodes;
        return this;
    }

    public MazePerZoneConstraints Zone(ZoneConfig zoneConfig) {
        Zones.Add(zoneConfig);
        return this;
    }

    public MazePerZoneConstraints Zone(int nodes, int parts = 1, int maxDoorsOut = -1, bool corridor = false) {
        Zone(new ZoneConfig(nodes, parts, maxDoorsOut, corridor));
        return this;
    }

    public int GetNodesPerZone(int zoneId) {
        return Zones[zoneId].Nodes;
    }

    public int GetParts(int zoneId) {
        return Zones[zoneId].Parts;
    }

    public int GetMaxDoorsOut(int zoneId) {
        return Zones[zoneId].MaxDoorsOut;
    }

    public bool IsCorridor(int zoneId) {
        return Zones[zoneId].Corridor;
    }
}