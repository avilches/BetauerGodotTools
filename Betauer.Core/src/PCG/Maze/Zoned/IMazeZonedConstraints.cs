namespace Betauer.Core.PCG.Maze;

public interface IMazeZonedConstraints {
    int MaxZones { get; }
    int MaxTotalNodes { get; }
    int GetNodesPerZone(int currentZone);
    int GetParts(int currentZone);
    int GetMaxDoorsOut(int currentZone);
}