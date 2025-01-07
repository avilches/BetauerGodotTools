namespace Betauer.Core.PCG.Maze.Zoned;

public class NodeScore(MazeNode mazeNode, float graphEndScore, bool belongsToEntryExitPath, float entryDistanceScore, float exitDistanceScore) {
    public MazeNode Node { get; } = mazeNode;

    /// <summary>
    /// 1 means the node is a dead end (only one connection)
    /// 0 means the node has the maximum number of connection.
    /// 
    /// So: if the maximum number of connections is 1, all nodes will have a score of 1.
    /// if the maximum number of connections is 3, the score will be
    ///    1 if the node is a dead end
    ///    0.5 if it has 1 connection
    ///    0 if it has 2 connection (like the maximum node)
    /// </summary>
    public float DeadEndScore { get; } = graphEndScore;

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
    public float EntryDistanceScore { get; } = entryDistanceScore;


    /// <summary>
    /// Calculates how far a node is from the closest in its zone.
    /// - A score of 1.0 means it's the furthest possible the closest exit
    /// - A score of 0.0 means it's directly at an exit
    /// 
    /// Example:
    /// In a zone with 10 nodes and one exit:
    /// - A node 5 steps away from the exit scores 0.5
    /// - A node 8 steps away scores 0.8
    /// - The exit node itself scores 0.0
    /// </summary>
    public float ExitDistanceScore { get; } = exitDistanceScore;

    public bool BelongsToEntryExitPath { get; set; } = belongsToEntryExitPath;

}