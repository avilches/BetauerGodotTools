using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class Zone(int zoneId) {
    public MazeZones MazeZones { get; internal set; }
    public int ZoneId { get; internal set; } = zoneId;

    public List<ZonePart> Parts { get; } = [];
    
    public int NodeCount => Parts.Sum(p => p.Nodes.Count);

    public IEnumerable<MazeNode> GetNodes() => Parts.SelectMany(part => part.Nodes);
    
    /// <summary>
    /// Entry nodes are nodes with an edge from a node in a zone with a zoneId lower than the current one.
    /// This method returns all possible entry nodes in the part from a zone with a lower zoneId.
    /// </summary>
    public IEnumerable<MazeNode> GetAllEntryNodes() {
        return Parts.SelectMany(p => p.GetAllEntryNodes());
    }

    public IEnumerable<MazeNode> GetEntryNodesFromPreviousZone() {
        return Parts.SelectMany(p => p.GetEntryNodesFromPreviousZone());
    }
    
    public IEnumerable<MazeNode> GetEntryNodesFrom(int zoneId) {
        return Parts.SelectMany(p => p.GetEntryNodesFrom(zoneId));
    }
    
    /// <summary>
    /// Exit nodes are nodes with an edge to a node in a zone with a zoneId higher than the current one.
    /// This method returns all possible exit nodes in the part to a zone with a higher zoneId.
    /// </summary>
    public IEnumerable<MazeNode> GetAllExitNodes() {
        return Parts.SelectMany(p => p.GetAllExitNodes());
    }
    
    public IEnumerable<MazeNode> GetAllExitNodesToNextZone() {
        return Parts.SelectMany(p => p.GetExitNodesToNextZone());
    }
    
    public IEnumerable<MazeNode> GetExitNodesTo(int zoneId) {
        return Parts.SelectMany(p => p.GetExitNodesTo(zoneId));
    }
    
    /// <summary>
    /// Returns all node scores within a specific zone.
    /// </summary>
    public IEnumerable<NodeScore> GetScores() {
        return MazeZones.Scores.Values.Where(score => score.Node.ZoneId == zoneId);
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
    public MazeNode GetBestLocation(Func<NodeScore, float> scoreCalculator) {
        return GetScores().OrderByDescending(scoreCalculator).First().Node;
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
    public List<MazeNode> SpreadLocations(float ratio, Func<NodeScore, float> scoreCalculator) {
        var total = Mathf.RoundToInt(NodeCount * ratio);
        return SpreadLocations(total, scoreCalculator);
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
    /// <param name="total">The total number of locations to place</param>
    /// <param name="scoreCalculator">The function used to score potential locations</param>
    public List<MazeNode> SpreadLocations(int total, Func<NodeScore, float> scoreCalculator) {
        var locations = new List<MazeNode>();
        // Console.WriteLine($"Zone {ZoneId} intentando colocar {total} tesoros en sus {NodeCount} nodos");
        if (total == 0) return locations;

        foreach (var part in Parts) {
            var partSize = part.Nodes.Count;
            // Calculate how many locations should go in this part based on its size relative to the zone
            var desired = Math.Max(1, Mathf.RoundToInt(total * (partSize / (float)NodeCount)));
            locations.AddRange(part.SpreadLocations(desired, scoreCalculator));

            // Console.WriteLine($"Zone {ZoneId} Part {part.PartId} treasures: {locations.Count(t => part.Nodes.Contains(t))}/{desired}");
        }
        // Console.WriteLine($"Zone {ZoneId} total treasures: {locations.Count}/{total}");
        return locations;
    }
    
}