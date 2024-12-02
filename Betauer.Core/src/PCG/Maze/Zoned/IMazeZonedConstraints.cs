namespace Betauer.Core.PCG.Maze.Zoned;

public interface IMazeZonedConstraints {
    int MaxZones { get; }
    int MaxTotalNodes { get; }
    int GetNodesPerZone(int zoneId);
    int GetParts(int zoneId);
    int GetMaxDoorsOut(int zoneId);
    bool IsAutoSplitOnExpand(int zoneId);
    bool IsCorridor(int zoneId);
}