using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

/// <summary>
/// Manages scoring and analysis of zones within a maze. Used to find optimal locations and analyze 
/// node characteristics within different zones of the maze.
/// </summary>
public class MazeZones(MazeGraph mazeGraph, MazeNode start, IMazeZonedConstraints constraints, List<Zone> zones) {
    public MazeGraph MazeGraph { get; } = mazeGraph;
    public MazeNode Start { get; } = start;
    public IMazeZonedConstraints Constraints { get; } = constraints;
    public IReadOnlyList<Zone> Zones => zones;

    public IReadOnlyCollection<MazeNode> GetNodes() => MazeGraph.GetNodes();

    public int NodeCount => MazeGraph.GetNodes().Count;

    internal readonly Dictionary<MazeNode, NodeScore> Scores = [];

    /// <summary>
    /// Returns all calculated node scores in the maze.
    /// </summary>
    public List<NodeScore> GetScores() {
        return Scores.Values.ToList();
    }

    /// <summary>
    /// Calculates scores for all nodes in the maze based on three criteria:
    /// 1. Dead-end score: How close the node is to being a dead-end
    /// 2. Entry distance score: How far the node is from zone entrances
    /// 3. Exit distance score: How far the node is from zone exits
    /// </summary>
    public void CalculateNodeScores() {
        Scores.Clear();

        // Find the maximum number of edges any node has in the graph
        // Example: In a grid maze, a center node might have 4 edges, while a corner node has 2
        var maxGraphEdges = MazeGraph.GetNodes().Max(n => n.GetOutEdges().Count);

        foreach (var zone in Zones) {
            foreach (var part in zone.Parts) {
                part.CalculateAllEntryToExitPaths();
            }
        }

        foreach (var node in MazeGraph.GetNodes()) {
            var graphEndScore = CalculateDeadEndScore(node, maxGraphEdges);
            var (belongsToPathToEntry, entryDistanceScore) = CalculateEntryDistanceScore(node);
            var (belongsToPathToExit, exitDistanceScore) = CalculateExitDistanceScore(node);

            Scores[node] = new NodeScore(node, graphEndScore, belongsToPathToEntry, entryDistanceScore, belongsToPathToExit, exitDistanceScore);
        }
    }

    /// <summary>
    /// Creates a route in the maze starting from the Root and visiting the best location in several zones, calculating the linearity of the path.
    /// There are two modes of operation:
    ///
    /// 1. Sequential order (zoneOrder is null):
    /// - Starts at Root
    /// - Visits best location of zone 0, then zone 1, then zone 2, etc
    /// - No key validation is performed
    /// 
    /// 2. Custom order with key validation (zoneOrder is provided):
    /// The zoneOrder list defines which key you get in each zone's best location. For example, zoneOrder=[1,3,2] means:
    /// - Start at Root and go to zone 0's best location to get key for zone 1
    /// - Then go to zone 1's best location to get key for zone 3
    /// - Finally go to zone 3's best location to get key for zone 2
    ///
    /// Example with zoneOrder=[1,3,2]:
    /// 1. First path: Root -> zone 0 best location
    ///    - Start with key 0 (always available)
    ///    - Can only traverse zone 0
    ///    - Get key 1 at destination in zone 0 best location
    ///
    /// 2. Second path: zone 0 best location -> zone 1 best location
    ///    - Have keys [0,1]
    ///    - Can traverse zones 0 and 1
    ///    - Get key 3 at destination in zone 1 best location
    ///
    /// 3. Third path: zone 1 best location -> zone 3 best location
    ///    - Have keys [0,1,3]
    ///    - Can traverse zones 0, 1, and 3
    ///    - Get key 2 at destination in zone 3 best location
    ///
    /// At each step, FindShortestPath is used with a predicate that only allows traversing zones for which we have keys.
    /// If no valid path exists (because we're missing a required key), an InvalidOperationException is thrown.
    ///
    /// The method calculates the linearity of the final route using MazeLinearity.CalculateLinearity.
    /// </summary>
    /// <param name="scoreCalculator">Function used to calculate scores for finding the best location in each zone</param>
    /// <param name="start">Optional. Node to start. If null, it will use the start node from the constructor.</param>
    /// <param name="zoneOrder">Optional. List defining which key is obtained in each zone's best location. If null, zones are visited sequentially</param>
    /// <exception cref="InvalidOperationException">Thrown when no valid path exists with the current keys</exception>
    public void CalculateSolution(Func<NodeScore, float> scoreCalculator, MazeNode? start = null, List<int>? zoneOrder = null) {
        start ??= Start;
        if (GetScores().Count == 0) {
            CalculateNodeScores();
        } else {
            Scores.Values.ForEach(score => score.SolutionTraversals = 0);
        }

        Dictionary<int, MazeNode> keyLocations = GetBestLocationsByZone(scoreCalculator);
        List<MazeNode> stops = [];
        List<IReadOnlyList<MazeNode>> paths = [];
        List<MazeNode> solutionPath = [];
        zoneOrder ??= Enumerable.Range(0, Zones.Count).ToList();
        if (zoneOrder[0] != 0) {
            zoneOrder = (List<int>) [0, ..zoneOrder];
        }

        // The first path starts from the node root and goes to the first key in the zone 0 (best location in 0)
        // because the first key in the zone 0 opens the next zone in the zoneOrder
        var keys = new HashSet<int> { 0 /* The key 0 is always available */ };
        stops.Add(start);
        foreach (var zoneId in zoneOrder) {
            keys.Add(zoneId);
            var pathStart = stops.Last();
            var pathEnd = keyLocations[zoneId];

            // var zone = Zones[zoneId];
            // var e = zone.GetAllExitNodes().SelectMany(n => n.GetInEdges()).Where(edge => edge.From.ZoneId > zone.ZoneId).Select(edge => edge.From.ZoneId).ToList();
            // Console.WriteLine("Zone " + zone.ZoneId + ": connects to " + string.Join(", ", e));

            var path = pathStart.FindShortestPath(pathEnd, node => keys.Contains(node.ZoneId));
            if (path.Count == 0) {
                throw new InvalidOperationException($"Cannot reach zone {zoneId} (node id {pathEnd.Id}) from zone {pathStart.ZoneId} (node id {pathStart.Id}) : no valid path exists with current keys: {string.Join(", ", keys)}");
            }
            // Every path starts in the same node than the previous zone ends, so the first needs to be ignored to avoid duplicates
            var skip = zoneId == 0 ? 0 : 1;
            paths.Add(path);
            stops.Add(pathEnd);
            solutionPath.AddRange(path.Skip(skip));
            foreach (var node in path.Skip(skip)) {
                Scores[node].SolutionTraversals++;
            }
        }

        // Calcular non-linearity: suma de todas las visitas adicionales
        // (cada habitación que se visita más de una vez suma sus visitas extras)
        var goalPath = start.FindShortestPath(keyLocations[zoneOrder.Last()]);

        Scoring = new MazeSolutionScoring(Scores, keyLocations, zoneOrder, goalPath, solutionPath);
    }

    public MazeSolutionScoring Scoring;

    /// <summary>
    /// Calculates how much a node resembles a dead-end. 
    /// - A score of 1.0 means it's a perfect dead-end (only one connection)
    /// - A score of 0.0 means it has the maximum possible connections
    /// 
    /// Example:
    /// In a grid maze where maxEdges is 4:
    /// - A node with 1 edge (dead-end) scores 1.0
    /// - A node with 2 edges scores 0.66
    /// - A node with 3 edges scores 0.33
    /// - A node with 4 edges scores 0.0
    /// </summary>
    private static float CalculateDeadEndScore(MazeNode node, int maxEdges) {
        if (maxEdges == 1) {
            // If the maximum edges in the graph is 1, all nodes are dead-ends
            // and therefore get maximum score
            return 1.0f;
        }
        var edges = node.GetOutEdges().Count;
        var deadEndScore = 1.0f - (float)(edges - 1) / (maxEdges - 1);
        return deadEndScore;
    }

    /// <summary>
    /// Calculates how far a node is from any entrance in its zone.
    /// - A score of 1.0 means it's the furthest possible from any entrance
    /// - A score of 0.0 means it's directly at an entrance
    /// 
    /// Example:
    /// In a zone with 10 nodes and one entrance:
    /// - A node 5 steps away from the entrance scores 0.5
    /// - A node 8 steps away scores 0.8
    /// - The entrance node itself scores 0.0
    /// </summary>
    private (bool belongsToEntryPath, float score) CalculateEntryDistanceScore(MazeNode node) {
        var part = Zones[node.ZoneId].Parts[node.PartId];
        var entryNodes = part.GetEntryNodesFromPreviousZone().ToList();

        if (entryNodes.Count == 0) return (false, 1.0f); // If no entrances (zone 0), any position is valid
        if (entryNodes.Contains(node)) return (true, 0.0f); // The node is an entrance

        var minDistance = entryNodes.Select(entry => node.GetGraphDistanceToNode(entry)).Min();
        var score = (float)minDistance / part.NodeCount;
        var belongsToPath = part.EntryExitPathNodes.Contains(node);
        return (belongsToPath, score);
    }

    /// <summary>
    /// Calculates how far a node is from any exit in its zone.
    /// - A score of 1.0 means it's the furthest possible from any exit
    /// - A score of 0.0 means it's directly at an exit
    /// 
    /// Example:
    /// In a zone with 10 nodes and one exit:
    /// - A node 3 steps away from the exit scores 0.3
    /// - A node 7 steps away scores 0.7
    /// - The exit node itself scores 0.0
    /// </summary>
    private (bool belongsToExitPath, float score) CalculateExitDistanceScore(MazeNode node) {
        var part = Zones[node.ZoneId].Parts[node.PartId];
        var exitNodes = part.GetExitNodesToNextZone().ToList();

        if (exitNodes.Count == 0) return (false, 1.0f); // If no entrances (zone 0), any position is valid
        if (exitNodes.Contains(node)) return (true, 0.0f); // The node is an exit

        var belongsToPath = part.EntryExitPathNodes.Contains(node);
        var minDistance = exitNodes.Select(exit => node.GetGraphDistanceToNode(exit)).Min();
        var score = (float)minDistance / part.NodeCount;
        return (belongsToPath, score);
    }

    /// <summary>
    /// Returns the node with the highest score across all zones based on the provided scoring function.
    /// 
    /// Example usage:
    /// ```
    /// // Get the most isolated dead-end in the maze
    /// var bestNode = GetBestLocation(score => score.DeadEndScore + score.EntryDistanceScore);
    /// ```
    /// </summary>
    public MazeNode GetBestLocation(Func<NodeScore, float> scoreCalculator) {
        return Scores.Values.OrderByDescending(scoreCalculator).First().Node;
    }

    /// <summary>
    /// Returns the node with the highest score within a specific part of a zone based on the provided scoring function.
    /// 
    /// Example usage:
    /// ```
    /// // Get the best dead-end in part 2 of zone 1
    /// var bestNode = GetBestLocationInZonePart(1, 2, score => score.DeadEndScore);
    /// ```
    /// </summary>
    public MazeNode GetBestLocationInZonePart(int zoneId, int partId, Func<NodeScore, float> scoreCalculator) {
        return Scores.Values.Where(score => score.Node.ZoneId == zoneId && score.Node.PartId == partId).OrderByDescending(scoreCalculator).First().Node;
    }

    /// <summary>
    /// Returns a dictionary mapping zone IDs to their best scoring nodes based on the provided scoring function.
    /// 
    /// Example usage:
    /// ```
    /// // Get the best dead-ends in each zone
    /// var bestLocations = GetBestLocationsByZone(score => score.DeadEndScore);
    /// ```
    /// </summary>
    public Dictionary<int, MazeNode> GetBestLocationsByZone(Func<NodeScore, float> scoreCalculator) {
        var bestLocationsPerZone = new Dictionary<int, MazeNode>();
        foreach (var zone in Zones) {
            bestLocationsPerZone[zone.ZoneId] = zone.GetBestLocation(scoreCalculator);
        }
        return bestLocationsPerZone;
    }

    /// <summary>
    /// Distributes locations across zones based on a ratio of nodes to select and a scoring function.
    /// 
    /// Example usage:
    /// ```
    /// // Select 20% of the nodes in each zone, prioritizing dead-ends
    /// var spreadLocations = SpreadLocationsByZone(0.2f, score => score.DeadEndScore);
    /// ```
    /// </summary>
    /// <param name="total">The number of locations</param>
    /// <param name="scoreCalculator">The function used to score nodes</param>
    public Dictionary<int, List<MazeNode>> SpreadLocationsByZone(int total, Func<NodeScore, float> scoreCalculator) {
        var spreadLocationsByZone = new Dictionary<int, List<MazeNode>>();
        foreach (var zone in Zones) {
            var totalPerZone = Mathf.RoundToInt(total / (float)zone.NodeCount);
            spreadLocationsByZone[zone.ZoneId] = zone.SpreadLocations(totalPerZone, scoreCalculator);
        }
        return spreadLocationsByZone;
    }

    public Dictionary<int, List<MazeNode>> SpreadLocationsByZone(float ratio, Func<NodeScore, float> scoreCalculator) {
        var spreadLocationsByZone = new Dictionary<int, List<MazeNode>>();
        foreach (var zone in Zones) {
            spreadLocationsByZone[zone.ZoneId] = zone.SpreadLocations(ratio, scoreCalculator);
        }
        return spreadLocationsByZone;
    }

    public List<MazeNode> SpreadLocations(float ratio, Func<NodeScore, float> scoreCalculator) {
        var total = Mathf.RoundToInt(Scores.Count * ratio);
        return SpreadLocations(total, scoreCalculator);
    }

    /// <summary>
    /// Spreads a specific number of locations, trying to maintain even distribution across zone parts.
    /// 
    /// Example usage:
    /// ```
    /// // Place 5 items in zone 2, where the nodes far from entrances has the highest chance
    /// var locations = SpreadLocationsInZone(2, 5, score => score.EntryDistanceScore);
    /// ```
    /// </summary>
    /// <param name="total">The total number of locations to place</param>
    /// <param name="scoreCalculator">The function used to score potential locations</param>
    public List<MazeNode> SpreadLocations(int total, Func<NodeScore, float> scoreCalculator) {
        return SpreadLocationsAlgorithm.GetLocations(GetScores().ToList(), total, scoreCalculator);
    }

    /// <summary>
    /// Change the parts and nodes from one zone to another.
    /// </summary>
    /// <param name="zone1Id"></param>
    /// <param name="zone2Id"></param>
    /// <exception cref="ArgumentException"></exception>
    public void SwapZoneParts(int zone1Id, int zone2Id) {
        if (zone1Id < 0 || zone1Id >= Zones.Count) throw new ArgumentException("Invalid zone1Id", nameof(zone1Id));
        if (zone2Id < 0 || zone2Id >= Zones.Count) throw new ArgumentException("Invalid zone2Id", nameof(zone2Id));
        if (zone1Id == zone2Id) return;

        var zone1 = Zones[zone1Id];
        var zone2 = Zones[zone2Id];

        var zone1Parts = zone1.Parts.ToList();
        var zone2Parts = zone2.Parts.ToList();

        zone1.Parts.Clear();
        zone1.Parts.AddRange(zone2Parts);
        zone1.Parts.ForEach(p => {
            p.Nodes.ForEach(n => n.ZoneId = zone1Id);
            p.Zone = zone1;
        });
        zone2.Parts.Clear();
        zone2.Parts.AddRange(zone1Parts);
        zone2.Parts.ForEach(p => {
            p.Nodes.ForEach(n => n.ZoneId = zone2Id);
            p.Zone = zone2;
        });

        // Swap zones in the list
    }
}

public static class SpreadLocationsAlgorithm {
    /// <summary>
    /// Spreads a specific number of locations within a zone, trying to maintain
    /// even distribution across zone parts.
    /// 
    /// Example usage:
    /// ```
    /// // Place 5 items in zone 2, where the nodes far from entrances has the highest chance
    /// var locations = SpreadLocationsInZone(2, 5, score => score.EntryDistanceScore);
    /// ```
    /// </summary>
    /// <param name="scores">The total number of locations to place</param>
    /// <param name="desired">The total number of locations to place</param>
    /// <param name="scoreCalculator">The function used to score potential locations</param>
    public static List<MazeNode> GetLocations(List<NodeScore> scores, int desired, Func<NodeScore, float> scoreCalculator) {
        var locations = new List<MazeNode>();
        if (desired == 0) return locations;
        var minDistance = Mathf.RoundToInt((float)scores.Count / desired) + 1;
        while (minDistance >= 1) {
            var locationsInPart = TrySpreadLocations(scores, scoreCalculator, desired, minDistance);
            if (locationsInPart.Count == desired) {
                locations.AddRange(locationsInPart);
                break;
            }
            minDistance--;
        }
        return locations;
    }

    /// <summary>
    /// Attempts to place a specific number of locations while maintaining a minimum distance between them.
    /// Uses the scoring function to prioritize optimal locations.
    ///
    /// The result could be fewer locations than desired if the minimum distance can't be maintained. So, calling again with a lower minimum distance
    /// could yield more locations until the desired number is reached.
    /// 
    /// Example usage:
    /// ```
    /// // Try to place 3 locations with minimum 2 steps between them
    /// var locations = TrySpreadLocations(nodeScores, score => score.DeadEndScore, 3, 2);
    /// ```
    /// </summary>
    /// <param name="scores">The list of node scores to choose from</param>
    /// <param name="scoreCalculator">The function used to score locations</param>
    /// <param name="desired">The number of locations to place</param>
    /// <param name="minDistance">The minimum distance required between locations</param>
    public static List<MazeNode> TrySpreadLocations(List<NodeScore> scores, Func<NodeScore, float> scoreCalculator, int desired, int minDistance) {
        var locations = new List<MazeNode>();
        var partCandidates = scores
            .OrderByDescending(scoreCalculator)
            .ToList();

        var pendingTreasures = desired;
        while (pendingTreasures > 0 && partCandidates.Count != 0) {
            var bestScore = partCandidates.First();
            if (locations.Count == 0 ||
                locations.All(existing => existing.GetGraphDistanceToNode(bestScore.Node) >= minDistance)) {
                locations.Add(bestScore.Node);
                pendingTreasures--;
            }
            partCandidates.Remove(bestScore);
        }

        return locations;
    }
}