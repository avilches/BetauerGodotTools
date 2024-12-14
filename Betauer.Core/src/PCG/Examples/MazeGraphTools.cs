using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core.PCG.Maze;
using Betauer.Core.PCG.Maze.Zoned;

namespace Betauer.Core.PCG.Examples;

public static class MazeGraphTools {
    /// <summary>
    /// Connects nodes within the same zone using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCyclesInZone(MazeGraph maze, int zoneId, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZone(zoneId)
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes within the same zone using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCyclesInZone(MazeGraph maze, int zoneId, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZone(zoneId)
            .OrderByLongestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes between two different zones using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCyclesBetweenZones(MazeGraph maze, int zoneA, int zoneB, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZones(zoneA, zoneB)
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes between two different zones using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCyclesBetweenZones(MazeGraph maze, int zoneA, int zoneB, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInZones(zoneA, zoneB)
            .OrderByLongestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes that are in different zones (any zones) using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCyclesAcrossZones(MazeGraph maze, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInDifferentZones()
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes that are in different zones (any zones) using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCyclesAcrossZones(MazeGraph maze, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .WhereNodesInDifferentZones()
            .OrderByLongestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes globally (ignoring zones) using the shortest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectShortestCycles(MazeGraph maze, int maxCycles = 1) {
        return maze.GetPotentialCycles().Query()
            .OrderByShortestDistance()
            .Connect(maxCycles);
    }

    /// <summary>
    /// Connects nodes globally (ignoring zones) using the longest possible cycles
    /// </summary>
    public static List<(MazeNode nodeA, MazeNode nodeB, int distance)> ConnectLongestCycles(MazeGraph maze, int maxCycles = 1) {
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
    public static void NeverConnectZone(MazeGraph maze, int zoneId) {
        maze.AddEdgeValidator((from, to) => from.ZoneId != zoneId && to.ZoneId != zoneId);
    }
}