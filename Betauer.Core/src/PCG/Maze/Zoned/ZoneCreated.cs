using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.PCG.Maze.Zoned;

public class MazeNodePart<T>(int partId, MazeNode<T> startNode) {
    public int PartId { get; } = partId;
    public MazeNode<T> StartNode { get; } = startNode;
    public List<MazeNode<T>> Nodes { get; } = [startNode];

    public void AddNode(MazeNode<T> node) {
        Nodes.Add(node);
    }
}

public class NodeScore<T>(MazeNode<T> node, float deadEndScore, float entryDistanceScore, float exitDistanceScore = 0) {
    public MazeNode<T> Node { get; } = node;
    
    /// <summary>
    /// 1 means the node is a dead end (only one connection)
    /// 0 means the node has the maximum number of connection in the zone.
    /// 
    /// So: if the maximum number of connections in the zone is 1, all nodes will have a score of 1.
    /// if the maximum number of connections in the zone is 3, the score will be
    ///    1 if the node is a dead end
    ///    0.5 if it has 1 connection
    ///    0 if it has 2 connection (like the maximun node)
    /// </summary>
    public float DeadEndScore { get; } = deadEndScore;

    /// <summary>
    /// Returns a score between 0 and 1.
    /// A door in is a connection from a zone with a lower id than the current one.
    /// 
    /// 1 means the node is the farthest from any door in,
    /// 0 means the node is the closest to any door in.
    ///
    /// The bigger, the more distance to walk to from the entrance (compared with other nodes in the same zone).
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public float EntryDistanceScore { get; } = entryDistanceScore;
    
    
    /// <summary>
    /// Returns a score between 0 and 1.
    /// A door out is a connection to a zone with a higher id than the current one.
    /// 
    /// 1 means the node is the farthest from any door out,
    /// 0 means the node is the closest to any door out.
    ///
    /// The bigger, the more distance to walk to reach the exit (compared with other nodes in the same zone).
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public float ExitDistanceScore { get; } = exitDistanceScore;

    public float GetKeyScore() {
        return (DeadEndScore * 0.4f) + (EntryDistanceScore * 0.3f) + (ExitDistanceScore * 0.3f);
    }

    public float GetTreasureScore() {
        return EntryDistanceScore;
        // return (DeadEndScore * 0.6f) + (EntryDistanceScore * 0.4f);
    }
}

public class ZoneCreated<T>(IMazeZonedConstraints constraints, int zoneId) {
    public int ZoneId { get; } = zoneId;
    public int Nodes { get; internal set; } = 0;
    public List<MazeNode<T>> AvailableNodes { get; internal set; } = new();
    public int ConfigParts => constraints.GetParts(ZoneId);
    public int DoorsOut { get; internal set; } = 0;
    public bool Corridor => constraints.IsCorridor(ZoneId);
    public List<MazeNodePart<T>> Parts { get; } = [];

    public int MaxDoorsOut {
        get {
            var maxDoorsOut = constraints.GetMaxDoorsOut(ZoneId);
            return maxDoorsOut == -1 ? Nodes * Nodes : maxDoorsOut;
        }
    }

    public MazeNodePart<T> FindPart(MazeNode<T> node) {
        return Parts.First(p => p.Nodes.Contains(node));
    }

    public void CreateNewPart(MazeNode<T> startNode) {
        var part = new MazeNodePart<T>(Parts.Count, startNode);
        Parts.Add(part);
    }

    public MazeNode<T>? KeyLocation { get; private set; }
    public List<MazeNode<T>> TreasureLocations { get; } = [];

    /// <summary>
    /// A doors in node is a node that has an edge to a node from the previous zone. The GrowZoned method only creates one door in
    /// to every zone, stored in the StartNode field in MazeNodePart. But it could be possible to create more connections before, even
    /// from other zones (not only the previous one), so this method will return all possible doors in.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<MazeNode<T>> GetDoorInNodes() {
        var startNodes = Parts.Select(p => p.StartNode).ToHashSet();
        return Parts
            .SelectMany(p => p.Nodes)
            .Where(n => n.GetInEdges().Any(edge => edge.From.Zone < ZoneId) || startNodes.Contains(n));
    }

    /// <summary>
    /// A doors out node is a node that has an edge to a node to next zone. The GrowZoned method only creates as many doors out
    /// as the MaxDoorsOut property of the zone, and store the number of doors out created in the DoorsOut field.
    /// But it could be possible to create more connections before, so this method will return all possible doors out.
    /// </summary>
    /// <returns></returns>
    private IEnumerable<MazeNode<T>> GetDoorOutNodes() {
        return Parts.SelectMany(p => p.Nodes)
            .Where(n => n.GetOutEdges().Any(edge => edge.To.Zone > ZoneId));
    }

    public Dictionary<MazeNode<T>, NodeScore<T>> NodeScores = [];

    private void CalculateNodeScores() {
        NodeScores.Clear();
        var nodes = Parts.SelectMany(p => p.Nodes)
            .Where(n => n.Zone == ZoneId);

        foreach (var node in nodes) {
            var deadEndScore = CalculateDeadEndScore(node);
            var entryDistanceScore = CalculateEntryDistanceScore(node);
            var exitDistanceScore = CalculateExitDistanceScore(node);

            NodeScores[node] = new NodeScore<T>(node, deadEndScore, entryDistanceScore, exitDistanceScore);
        }
    }

    private float CalculateDeadEndScore(MazeNode<T> node) {
        var maxEdges = Parts.SelectMany(p => p.Nodes)
            .Max(n => n.GetOutEdges().Count);
        if (maxEdges == 1) {
            // Si el edge maximo de la zona es 1 es pq todos los nodos, incluido este, son un dead end, luego todos tienen
            // el score maximo de 1.0f
            return 1.0f; 
        }
        var edges = node.GetOutEdges().Count;
        // Devuelve 1 si es un dead end, y 0 si tiene el maximo numero de edges que hay en la zona
        return 1.0f - ((float)(edges - 1) / (maxEdges - 1));    }

    private float CalculateEntryDistanceScore(MazeNode<T> node) {
        var entryNodes = GetDoorInNodes();

        if (!entryNodes.Any()) return 1.0f; // Si no hay entradas (zona 0), cualquier posición es válida

        var minDistance = entryNodes.Select(node.GetDistanceToNodeByEdges).Min();
        return (float)minDistance / Nodes;
    }

    public float CalculateExitDistanceScore(MazeNode<T> node) {
        var exitNodes = GetDoorOutNodes();

        if (!exitNodes.Any()) return 1.0f; // No hay salidas, cualquier posición es buena

        var minDistance = exitNodes.Select(node.GetDistanceToNodeByEdges).Min();
        return (float)minDistance / Nodes;
    }

    public MazeNode<T> SelectKeyLocation() {
        CalculateNodeScores();
        var bestScore = NodeScores!.Values
            .OrderByDescending(score => score.GetKeyScore())
            .First();
        KeyLocation = bestScore.Node;
        return KeyLocation;
    }

    public List<MazeNode<T>> SelectTreasureLocations(float treasurePercentage) {
        CalculateNodeScores();
        TreasureLocations.Clear();
        var totalTreasures = Mathf.RoundToInt(Nodes * treasurePercentage);
        Console.WriteLine($"Zone {ZoneId} intentando colocar {totalTreasures} ({treasurePercentage * 100:0.0}%) tesoros en sus {Nodes} nodos");
        if (totalTreasures == 0) return TreasureLocations;

        foreach (var part in Parts) {
            var partSize = part.Nodes.Count;
            var desiredTreasures = Math.Max(1, Mathf.RoundToInt(totalTreasures * ((float)partSize / Nodes)));
            var minDistance = Mathf.RoundToInt((float)partSize / desiredTreasures) + 1;
            while (minDistance >= 1) {
                var success = TryPlaceTreasuresInPart(part, desiredTreasures, minDistance);
                if (success) break;
                minDistance--;
            }

            Console.WriteLine($"Zone {ZoneId} Part {part.PartId} treasures: {TreasureLocations.Count(t => part.Nodes.Contains(t))}/{desiredTreasures}");
        }

        Console.WriteLine($"Zone {ZoneId} total treasures: {TreasureLocations.Count}/{totalTreasures}");
        return TreasureLocations;
    }

    private bool TryPlaceTreasuresInPart(MazeNodePart<T> part, int desiredTreasures, int minDistance) {
        // Guardamos los tesoros existentes de otras partes
        var otherPartsTreasures = TreasureLocations.Where(t => !part.Nodes.Contains(t)).ToList();
        // Limpiamos solo los tesoros de esta parte
        TreasureLocations.RemoveAll(t => part.Nodes.Contains(t));
        // Restauramos los tesoros de otras partes
        TreasureLocations.AddRange(otherPartsTreasures);

        var partCandidates = NodeScores.Values
            .Where(score => part.Nodes.Contains(score.Node))
            .OrderByDescending(score => score.GetTreasureScore())
            .ToList();

        var pendingTreasures = desiredTreasures;
        while (pendingTreasures > 0 && partCandidates.Count != 0) {
            var bestScore = partCandidates.First();
            var existingTreasuresInPart = TreasureLocations
                .Where(t => part.Nodes.Contains(t));

            if (!existingTreasuresInPart.Any() ||
                existingTreasuresInPart.All(existing =>
                    existing.GetDistanceToNodeByEdges(bestScore.Node) >= minDistance)) {
                TreasureLocations.Add(bestScore.Node);
                pendingTreasures--;
            }
            partCandidates.Remove(bestScore);
        }

        Console.WriteLine($"  Zone {ZoneId} Part {part.PartId} Distancia: {minDistance}. Treasures colocados {desiredTreasures - pendingTreasures} de {desiredTreasures}");
        return pendingTreasures == 0; // true si colocamos todos los tesoros deseados
    }
}