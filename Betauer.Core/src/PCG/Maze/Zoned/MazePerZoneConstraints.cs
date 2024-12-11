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

    public MazePerZoneConstraints Zone(int nodes, int parts = 1, int maxExitNodes = -1, bool corridor = false, 
        bool flexibleParts = true) {
        Zone(new ZoneConfig(nodes, parts, maxExitNodes, corridor, flexibleParts));
        return this;
    }

    public int GetNodesPerZone(int zoneId) => Zones[zoneId].Nodes;
    public int GetParts(int zoneId) => Zones[zoneId].Parts;
    public int GetMaxExitNodes(int zoneId) => Zones[zoneId].MaxExitNodes;
    public bool IsCorridor(int zoneId) => Zones[zoneId].Corridor;
    
    /// If true, the algorithm could create more parts (if there are no more free adjacent nodes in the zone to expand) or
    /// less parts, if there are no more exit nodes in the previous zones.
    public bool IsFlexibleParts(int zoneId) => Zones[zoneId].FlexibleParts;
}