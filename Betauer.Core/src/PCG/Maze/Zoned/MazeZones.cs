using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public IReadOnlyList<Zone> Zones { get; } = ValidateAndOrderZones(zones);

    private static ImmutableList<Zone> ValidateAndOrderZones(List<Zone> zones) {
        var orderedZones = zones.OrderBy(z => z.ZoneId).ToImmutableList();
        // Validate zones are sequential starting from 0
        for (var i = 0; i < orderedZones.Count; i++) {
            if (orderedZones[i].ZoneId != i) {
                throw new ArgumentException(
                    $"Zones must be sequential starting from 0. Missing or invalid ZoneId {i}. Found ZoneId {orderedZones[i].ZoneId} instead.");
            }
        }
        
        return orderedZones;
    }

    public IReadOnlyCollection<MazeNode> GetNodes() => MazeGraph.GetNodes();

    public int NodeCount => MazeGraph.GetNodes().Count;

    private readonly Dictionary<MazeNode, NodeScore> _scores = [];
    
    public NodeScore GetScore(MazeNode node) {
        if (_scores.Count != NodeCount) CalculateNodeScores();
        return _scores[node];
    }

    /// <summary>
    /// Returns all calculated node scores in the maze.
    /// </summary>
    public IReadOnlyCollection<NodeScore> GetScores() {
        if (_scores.Count != NodeCount) CalculateNodeScores();
        return _scores.Values;
    }

    /// <summary>
    /// Calculates scores for all nodes in the maze and store them in the _scores dictionary, accesible from GetScore(node) and GetScores().
    /// </summary>
    public void CalculateNodeScores() {
        _scores.Clear();

        // Find the maximum number of edges any node has in the graph
        // Example: In a grid maze, a center node might have 4 edges, while a corner node has 2
        var maxGraphEdges = MazeGraph.GetNodes().Max(n => n.GetOutEdges().Count());

        foreach (var zone in Zones) {
            foreach (var part in zone.Parts) {
                part.CalculateAllEntryToExitPaths();
            }
        }

        foreach (var node in MazeGraph.GetNodes()) {
            var graphEndScore = CalculateDeadEndScore(node, maxGraphEdges);
            var entryDistanceScore = CalculateEntryDistanceScore(node);
            var exitDistanceScore = CalculateExitDistanceScore(node);

            var belongsToEntryExitPath = Zones[node.ZoneId].Parts[node.PartId].EntryExitPathNodes.Contains(node);

            _scores[node] = new NodeScore(node, graphEndScore, belongsToEntryExitPath, entryDistanceScore, exitDistanceScore);
        }
    }

    /// <summary>
    /// Returns a MazeSolutionScoring instance, which contains the SolutionPath. This path is created visiting:
    /// - Nodes from the "start" node to keyLocations[0].
    /// - Nodes from keyLocations[0] to keyLocations[1].
    /// - And so on... until the last key location.
    ///
    /// The keyLocations size should match the number of zones in the maze.
    ///
    /// keyLocation[0] is the key to the zone 1, so the node should be part of the zone 0. The same for the rest of the other key locations.
    ///
    /// Check the MazeSolutionScoring class for more information about the results.
    /// </summary>
    /// <param name="keyScoreCalculator">Function used to calculate scores for finding the best location in each zone</param>
    /// <param name="start">Optional. Node to start. If null, it will use the start node from the constructor. The start node needs to be located in zone 0</param>
    /// <exception cref="InvalidOperationException">Thrown when no valid path exists with the current keys</exception>
    public MazeSolutionScoring CalculateSolution(Func<NodeScore, float> keyScoreCalculator, MazeNode? start = null) {
        if (_scores.Count != NodeCount) CalculateNodeScores();

        var keyLocations = GetBestLocationsByZone(keyScoreCalculator);
        return CalculateSolution(keyLocations, start);
    }
    
    /// <summary>
    /// Returns a MazeSolutionScoring instance, which contains the SolutionPath. This path is created visiting:
    /// - Nodes from the "start" node to keyLocations[0]
    /// - Nodes from keyLocations[0] to keyLocations[1]
    /// - And so on... until the last key location.
    ///
    /// The keyLocations size should match the number of zones in the maze.
    /// keyLocation[0] is the key to the zone 1, so the node should be part of the zone 0. The same for the rest of the other key locations.
    ///
    /// Check the MazeSolutionScoring class for more information about the results.
    /// </summary>
    /// <param name="keyLocations">A list with the keys of every zone</param>
    /// <param name="start">Optional. Node to start. If null, it will use the start node from the constructor. The start node needs to be located in zone 0</param>
    /// <exception cref="InvalidOperationException">Thrown when no valid path exists with the current keys</exception>
    public MazeSolutionScoring CalculateSolution(List<MazeNode> keyLocations, MazeNode? start = null) {
        start ??= Start;
        
        if (start == null) {
            throw new InvalidOperationException("A not null Start node must be provided");
        }
        
        if (start.ZoneId != 0) {
            throw new InvalidOperationException("The start node must be in zone 0");
        }
        
        return new MazeSolutionScoring(GetNodes(), keyLocations, start);
    }

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
        var edges = node.GetOutEdges().Count();
        var deadEndScore = 1.0f - (float)(edges - 1) / (maxEdges - 1);
        return deadEndScore;
    }

    /// <summary>
    /// Calculates how far a node is from the closest in its zone.
    /// - A score of 1.0 means it's the furthest possible the closest entry
    /// - A score of 0.0 means it's directly at an entry
    /// 
    /// Example:
    /// In a zone with 10 nodes and one entry:
    /// - A node 5 steps away from the entry scores 0.5
    /// - A node 8 steps away scores 0.8
    /// - The entry node itself scores 0.0
    /// </summary>
    private float CalculateEntryDistanceScore(MazeNode node) {
        var part = Zones[node.ZoneId].Parts[node.PartId];
        var entryNodes = part.GetEntryNodesFromPreviousZone().ToList();

        if (entryNodes.Count == 0) return 1.0f; // If no entry (zone 0), any position is valid
        if (entryNodes.Contains(node)) return 0.0f; // The node is an entry

        var minDistance = entryNodes.Select(entry => node.GetGraphDistanceToNode(entry)).Min();
        var score = (float)minDistance / part.NodeCount;
        return score;
    }

    /// <summary>
    /// Calculates how far a node is from the closest exit in its zone.
    /// - A score of 1.0 means it's the furthest possible from the closest exit
    /// - A score of 0.0 means it's directly at an exit
    /// 
    /// Example:
    /// In a zone with 10 nodes and one exit:
    /// - A node 3 steps away from the exit scores 0.3
    /// - A node 7 steps away scores 0.7
    /// - The exit node itself scores 0.0
    /// </summary>
    private float CalculateExitDistanceScore(MazeNode node) {
        var part = Zones[node.ZoneId].Parts[node.PartId];
        var exitNodes = part.GetExitNodesToNextZone().ToList();

        if (exitNodes.Count == 0) return 1.0f; // If no exit (zone 0), any position is valid
        if (exitNodes.Contains(node)) return 0.0f; // The node is an exit

        var minDistance = exitNodes.Select(exit => node.GetGraphDistanceToNode(exit)).Min();
        var score = (float)minDistance / part.NodeCount;
        return score;
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
        return _scores.Values.OrderByDescending(scoreCalculator).First().Node;
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
        return _scores.Values.Where(score => score.Node.ZoneId == zoneId && score.Node.PartId == partId).OrderByDescending(scoreCalculator).First().Node;
    }

    /// <summary>
    /// Returns a list mapping zone IDs to their best scoring nodes based on the provided scoring function.
    /// 
    /// Example usage:
    /// ```
    /// // Get the best dead-ends in each zone
    /// var bestLocations = GetBestLocationsByZone(score => score.DeadEndScore);
    /// ```
    /// </summary>
    public List<MazeNode> GetBestLocationsByZone(Func<NodeScore, float> scoreCalculator) {
        return Zones.Select(zone => zone.GetBestLocation(scoreCalculator)).ToList();
    }

    /// <summary>
    /// Distributes a fixed amount of locations across zones based on a scoring function.
    /// It returns a list where every element is a list of the nodes for each zone.
    /// 
    /// Example usage:
    /// ```
    /// // Select 6 the nodes in each zone, prioritizing dead-ends
    /// var spreadLocations = SpreadLocationsByZone(6, score => score.DeadEndScore);
    /// spreadLocations[0] // Nodes in zone 0
    /// ```
    ///
    /// 
    /// </summary>
    /// <param name="total">The number of locations</param>
    /// <param name="scoreCalculator">The function used to score nodes</param>
    public List<List<MazeNode>> SpreadLocationsByZone(int total, Func<NodeScore, float> scoreCalculator) {
        var spreadLocationsByZone = new List<List<MazeNode>>();
        foreach (var zone in Zones) {
            var totalPerZone = Mathf.RoundToInt(total / (float)zone.NodeCount);
            spreadLocationsByZone.Add(zone.SpreadLocations(totalPerZone, scoreCalculator));
        }
        return spreadLocationsByZone;
    }

    /// <summary>
    /// Distributes locations across zones based on a ratio of nodes to select and a scoring function.
    /// It returns a list where every element is a list of the nodes for each zone.
    /// 
    /// Example usage:
    /// ```
    /// // Select 20% of the nodes in each zone, prioritizing dead-ends
    /// var spreadLocations = SpreadLocationsByZone(0.2f, score => score.DeadEndScore);
    /// spreadLocations[0] // If the zone had 30 nodes, this list will have 6 nodes (20% of the 30 nodes in zone 0)
    /// ```
    /// </summary>
    /// <param name="ratio">The % of locations per zone</param>
    /// <param name="scoreCalculator">The function used to score nodes</param>
    /// <returns>A list of zones where every element is a list of the nodes.</returns>
    public List<List<MazeNode>> SpreadLocationsByZone(float ratio, Func<NodeScore, float> scoreCalculator) {
        return Zones.Select(zone => zone.SpreadLocations(ratio, scoreCalculator)).ToList();
    }

    /// <summary>
    /// Spreads a specific percent of locations, trying to maintain even distribution across zone parts.
    /// It returns a list of the selected nodes.
    ///
    /// Example usage:
    /// ```
    /// // Select 20% of the nodes , prioritizing dead-ends
    /// var nodes = SpreadLocations(0.2f, score => score.DeadEndScore);
    /// ```
    /// </summary>
    /// <param name="ratio">The % of locations per zone</param>
    /// <param name="scoreCalculator">The function used to score nodes</param>
    /// <returns>A list of zones where every element is a list of the nodes.</returns>
    public List<MazeNode> SpreadLocations(float ratio, Func<NodeScore, float> scoreCalculator) {
        var total = Mathf.RoundToInt(_scores.Count * ratio);
        return SpreadLocations(total, scoreCalculator);
    }

    /// <summary>
    /// Spreads a specific number of locations, trying to maintain even distribution across zone parts.
    /// 
    /// Example usage:
    /// ```
    /// // Place 5 items where the nodes far from entrances has the highest chance
    /// var nodes = SpreadLocations(5, score => score.EntryDistanceScore);
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