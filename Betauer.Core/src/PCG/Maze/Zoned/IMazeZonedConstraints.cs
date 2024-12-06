namespace Betauer.Core.PCG.Maze.Zoned;

public interface IMazeZonedConstraints {
    int MaxZones { get; }
    int MaxTotalNodes { get; }
    int GetNodesPerZone(int zoneId);
    
    /// <summary>
    /// This is the expected number of parts for the zone. The algorithm will try to
    /// create as many parts as free doors out are in still available in the previous zones.
    /// But if there are no more doors out, or there are, but there are no more free nodes to
    /// connect them, then the zone will have fewer parts than expected.
    ///
    /// On the other hand, if all the parts were created and the zone is expanding, but there are
    /// no more free nodes available to connect, the algorithm could create more parts (if there are
    /// doors out available and there are free nodes to connect them). In this case, the zone will
    /// have more parts than expected.
    /// </summary>
    /// <param name="zoneId"></param>
    /// <returns></returns>
    int GetParts(int zoneId);
    
    /// <summary>
    /// The maximum number of doors out for the zone. -1 means no limit.
    /// </summary>
    /// <param name="zoneId"></param>
    /// <returns></returns>
    int GetMaxDoorsOut(int zoneId);
    bool IsCorridor(int zoneId);
}