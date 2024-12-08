using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

/// <summary>
/// Manages scoring and analysis of zones within a maze. Used to find optimal locations and analyze 
/// node characteristics within different zones of the maze.
/// </summary>
/// <typeparam name="T">The type of data associated with maze nodes</typeparam>
public class MazeZones<T>(MazeGraphZoned<T> graphZoned, List<Zone<T>> zones) {
    public MazeGraphZoned<T> GraphZoned { get; } = graphZoned;
    public IReadOnlyList<Zone<T>> Zones => zones;

    private readonly Dictionary<int, NodeScore<T>> _scores = [];

    /// <summary>
    /// Returns all calculated node scores in the maze.
    /// </summary>
    public List<NodeScore<T>> GetScores() {
        return _scores.Values.ToList();
    }

    /// <summary>
    /// Gets the score for a specific node.
    /// </summary>
    public NodeScore<T> GetScore(MazeNode<T> node) {
        return _scores[node.Id];
    }

    /// <summary>
    /// Gets the score for a node by its ID.
    /// </summary>
    public NodeScore<T> GetScore(int id) {
        return _scores[id];
    }

    /// <summary>
    /// Returns all node scores within a specific zone.
    /// </summary>
    /// <param name="zoneId">The ID of the zone</param>
    public IEnumerable<NodeScore<T>> GetZoneScores(int zoneId) {
        return _scores.Values.Where(score => score.Node.ZoneId == zoneId);
    }

    /// <summary>
    /// Returns all node scores within a specific part of a zone.
    /// </summary>
    /// <param name="zoneId">The ID of the zone</param>
    /// <param name="partId">The ID of the part within the zone</param>
    public IEnumerable<NodeScore<T>> GetPartScores(int zoneId, int partId) {
        return _scores.Values.Where(score => score.Node.ZoneId == zoneId && score.Node.PartId == partId);
    }

    /// <summary>
    /// Calculates scores for all nodes in the maze based on three criteria:
    /// 1. Dead-end score: How close the node is to being a dead-end
    /// 2. Entry distance score: How far the node is from zone entrances
    /// 3. Exit distance score: How far the node is from zone exits
    /// </summary>
    public void CalculateNodeScores() {
        _scores.Clear();

        // Find the maximum number of edges any node has in the graph
        // Example: In a grid maze, a center node might have 4 edges, while a corner node has 2
        var maxGraphEdges = GraphZoned.GetNodes().Max(n => n.GetOutEdges().Count);

        foreach (var node in GraphZoned.GetNodes()) {
            var graphEndScore = CalculateDeadEndScore(node, maxGraphEdges);
            var entryDistanceScore = CalculateEntryDistanceScore(node);
            var exitDistanceScore = CalculateExitDistanceScore(node);

            _scores[node.Id] = new NodeScore<T>(node, graphEndScore, entryDistanceScore, exitDistanceScore);
        }
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
    private static float CalculateDeadEndScore(MazeNode<T> node, int maxEdges) {
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
    private float CalculateEntryDistanceScore(MazeNode<T> node) {
        var zone = Zones[node.ZoneId];
        var entryNodes = zone.GetDoorInNodes().ToList();

        if (entryNodes.Count == 0) return 1.0f; // If no entrances (zone 0), any position is valid

        var minDistance = entryNodes.Select(node.GetDistanceToNodeByEdges).Min();
        return (float)minDistance / zone.NodeCount;
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
    public float CalculateExitDistanceScore(MazeNode<T> node) {
        var zone = Zones[node.ZoneId];
        var exitNodes = zone.GetDoorOutNodes().ToList();

        if (exitNodes.Count == 0) return 1.0f; // No exits, any position is good

        var minDistance = exitNodes.Select(node.GetDistanceToNodeByEdges).Min();
        return (float)minDistance / zone.NodeCount;
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
    public MazeNode<T> GetBestLocation(Func<NodeScore<T>, float> scoreCalculator) {
        return _scores.Values.OrderByDescending(scoreCalculator).First().Node;
    }

    /// <summary>
    /// Returns the node with the highest score within a specific zone based on the provided scoring function.
    /// 
    /// Example usage:
    /// ```
    /// // Get the node furthest from exits in zone 1
    /// var bestNode = GetBestLocationInZone(1, score => score.ExitDistanceScore);
    /// ```
    /// </summary>
    public MazeNode<T> GetBestLocationInZone(int zoneId, Func<NodeScore<T>, float> scoreCalculator) {
        return _scores.Values.Where(score => score.Node.ZoneId == zoneId).OrderByDescending(scoreCalculator).First().Node;
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
    public MazeNode<T> GetBestLocationInZonePart(int zoneId, int partId, Func<NodeScore<T>, float> scoreCalculator) {
        return _scores.Values.Where(score => score.Node.ZoneId == zoneId && score.Node.PartId == partId).OrderByDescending(scoreCalculator).First().Node;
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
    public Dictionary<int, MazeNode<T>> GetBestLocationsByZone(Func<NodeScore<T>, float> scoreCalculator) {
        var bestLocationsPerZone = new Dictionary<int, MazeNode<T>>();
        for (var zoneId = 0; zoneId < Zones.Count; zoneId++) {
            bestLocationsPerZone[zoneId] = GetBestLocationInZone(zoneId, scoreCalculator);
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
    /// <param name="ratio">The ratio of nodes to select (0.0 to 1.0)</param>
    /// <param name="scoreCalculator">The function used to score nodes</param>
    public Dictionary<int, List<MazeNode<T>>> SpreadLocationsByZone(float ratio, Func<NodeScore<T>, float> scoreCalculator) {
        var spreadLocationsByZone = new Dictionary<int, List<MazeNode<T>>>();
        for (var zoneId = 0; zoneId < Zones.Count; zoneId++) {
            spreadLocationsByZone[zoneId] = SpreadLocationsInZone(zoneId, ratio, scoreCalculator);
        }
        return spreadLocationsByZone;
    }

    /// <summary>
    /// Spreads a proportional number of locations within a zone based on the zone's size and the provided ratio.
    /// 
    /// Example usage:
    /// ```
    /// // Select 30% of the nodes in zone 1, prioritizing nodes far from exits
    /// var locations = SpreadLocationsInZone(1, 0.3f, score => score.ExitDistanceScore);
    /// ```
    /// </summary>
    public List<MazeNode<T>> SpreadLocationsInZone(int zoneId, float ratio, Func<NodeScore<T>, float> scoreCalculator) {
        var zone = Zones[zoneId];
        var total = Mathf.RoundToInt(zone.NodeCount * ratio);
        return SpreadLocationsInZone(zoneId, total, scoreCalculator);
    }

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
    /// <param name="zoneId">The zone to place locations in</param>
    /// <param name="total">The total number of locations to place</param>
    /// <param name="scoreCalculator">The function used to score potential locations</param>
    public List<MazeNode<T>> SpreadLocationsInZone(int zoneId, int total, Func<NodeScore<T>, float> scoreCalculator) {
        var zone = Zones[zoneId];
        var locations = new List<MazeNode<T>>();
        Console.WriteLine($"Zone {zone.ZoneId} intentando colocar {total} tesoros en sus {zone.NodeCount} nodos");
        if (total == 0) return locations;

        foreach (var part in zone.Parts) {
            var partSize = part.Nodes.Count;
            var scoreNodesInPart = GetPartScores(zoneId, part.PartId).ToList();
            // Calculate how many locations should go in this part based on its size relative to the zone
            var desired = Math.Max(1, Mathf.RoundToInt(total * ((float)partSize / zone.NodeCount)));
            var minDistance = Mathf.RoundToInt((float)partSize / desired) + 1;
            while (minDistance >= 1) {
                var locationsInPart = TrySpreadLocations(scoreNodesInPart, scoreCalculator, desired, minDistance);
                Console.WriteLine($"  Zone {zone.ZoneId} Part {part.PartId} Distancia: {minDistance}. Treasures colocados {locationsInPart.Count} de {desired}");
                if (locationsInPart.Count < desired) {
                    locations.AddRange(locationsInPart);
                    break;
                }
                minDistance--;
            }

            Console.WriteLine($"Zone {zone.ZoneId} Part {part.PartId} treasures: {locations.Count(t => part.Nodes.Contains(t))}/{desired}");
        }

        Console.WriteLine($"Zone {zone.ZoneId} total treasures: {locations.Count}/{total}");
        return locations;
    }

    /// <summary>
    /// Attempts to place a specific number of locations while maintaining a minimum distance between them.
    /// Uses the scoring function to prioritize optimal locations.
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
    public static List<MazeNode<T>> TrySpreadLocations(List<NodeScore<T>> scores, Func<NodeScore<T>, float> scoreCalculator, int desired, int minDistance) {
        var locations = new List<MazeNode<T>>();
        var partCandidates = scores
            .OrderByDescending(scoreCalculator)
            .ToList();

        var pendingTreasures = desired;
        while (pendingTreasures > 0 && partCandidates.Count != 0) {
            var bestScore = partCandidates.First();
            if (locations.Count == 0 ||
                locations.All(existing => existing.GetDistanceToNodeByEdges(bestScore.Node) >= minDistance)) {
                locations.Add(bestScore.Node);
                pendingTreasures--;
            }
            partCandidates.Remove(bestScore);
        }

        return locations;
    }
}