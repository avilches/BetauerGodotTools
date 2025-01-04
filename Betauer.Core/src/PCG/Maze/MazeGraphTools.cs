using System.Collections.Generic;
using Betauer.Core.PCG.Maze.Zoned;

namespace Betauer.Core.PCG.Maze;

public static class MazeGraphTools {

    
    /// <summary>
    /// Finds all potential cycle connections between adjacent nodes that are not directly connected,
    /// calculating distances either through parent relationships or shortest path through edges.
    /// 
    /// Example: Consider a maze structure where solid lines are connections only and arrows
    /// show parent+connection relationships:
    ///   A -> B -> C
    ///   ↓    |    ↓
    ///   D -> E    F
    /// 
    /// For nodes B and E:
    /// - Parent distance is 4 (B->A->D->E)
    /// - Edge distance is 1 (B-E directly)
    /// 
    /// For nodes C and E:
    /// - Parent distance is 5 (C->B->A->D->E)
    /// - Edge distance is 2 (C-B-E)
    /// 
    /// Usage:
    /// var cycles = maze.FindPotentialCycles(minDistance: 3, useParentDistance: true);
    /// // Returns list of tuples: (nodeA, nodeB, distance)
    /// </summary>
    /// <param name="useParentDistance">If true, calculates distance following parent relationships.
    /// If false, finds the shortest path using all available connections.</param>
    /// <returns>List of potential connections ordered by distance (longest paths first)</returns>
    public static PotentialCycles GetPotentialCycles(this MazeGraph maze, bool useParentDistance = false) {
        return new PotentialCycles(maze, useParentDistance);
    }

    
    /// <summary>
    /// Connects nodes within the same zone using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCyclesInZone(this MazeGraph maze, int zoneId, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZone(zoneId)
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes within the same zone using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCyclesInZone(this MazeGraph maze, int zoneId, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZone(zoneId)
            .OrderByLongestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes between two different zones using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCyclesBetweenZones(this MazeGraph maze, int zoneA, int zoneB, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZones(zoneA, zoneB)
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes between two different zones using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCyclesBetweenZones(this MazeGraph maze, int zoneA, int zoneB, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZones(zoneA, zoneB)
            .OrderByLongestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes that are in different zones (any zones) using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCyclesAcrossZones(this MazeGraph maze, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInDifferentZones()
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes that are in different zones (any zones) using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCyclesAcrossZones(this MazeGraph maze, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInDifferentZones()
            .OrderByLongestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes globally (ignoring zones) using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCycles(this MazeGraph maze, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes globally (ignoring zones) using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCycles(this MazeGraph maze, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .OrderByLongestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Creates cycles in each zone, connecting using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCyclesInAllZones(MazeZones zones, int maxCyclesPerZone = 1) {
        List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];
        foreach (var zone in zones.Zones) {
            for (var i = 0; i < zone.Parts.Count; i++) {
                cyclesAdded.AddRange(ConnectShortestCyclesInZone(zones.MazeGraph, zone.ZoneId, maxCyclesPerZone));
            }
        }
        return cyclesAdded;
    }

    /// <summary>
    /// Creates cycles in each zone, connecting using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCyclesInAllZones(MazeZones zones, int maxCyclesPerZone = 1) {
        List<(MazeNode nodeA, MazeNode nodeB, int distance)> cyclesAdded = [];
        foreach (var zone in zones.Zones) {
            for (var i = 0; i < zone.Parts.Count; i++) {
                cyclesAdded.AddRange(ConnectLongestCyclesInZone(zones.MazeGraph, zone.ZoneId, maxCyclesPerZone));
            }
        }
        return cyclesAdded;
    }

    /// <summary>
    /// Prevents any connections to or from the specified zone
    /// </summary>
    public static void NeverConnectZone(this MazeGraph maze, int zoneId) {
        maze.AddEdgeValidator((from, to) => from.ZoneId != zoneId && to.ZoneId != zoneId);
    }
}