namespace Betauer.Core.PCG.Maze.Zoned;

public interface IMazeZonedConstraints {
    int MaxZones { get; }
    int MaxTotalNodes { get; }
    int GetNodesPerZone(int zoneId);
    int GetMaxParts(int zoneId);
    int GetMaxDoorsOut(int zoneId);
    bool IsCorridor(int zoneId);
}