using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public struct CityData {
    public int Width;
    public int Height;
}

public class CityGenerationParameters {

    /// <summary>
    /// Generation seed
    /// </summary>
    public int Seed { get; set; } = 1;

    /// <summary>
    /// Start generation position.
    /// Default: Center of map size
    /// </summary>
    public Vector2I StartPosition { get; set; }

    /// <summary>
    /// Start generation directions.
    /// Default: Left, Right, Top, Bottom
    /// </summary>
    public List<int> StartDirections { get; set; } = new List<int>();

    /// <summary>
    /// Street length before generating an intersection or turn.
    /// Default: 10
    /// </summary>
    public int StreetMinLength { get; set; } = 10;

    /// <summary>
    /// Probability of generating intersection.
    /// Default: 0.1
    /// </summary>
    public float ProbabilityIntersection { get; set; } = 0.1f;

    /// <summary>
    /// Probability of generating turn.
    /// Default: 0.05
    /// </summary>
    public float ProbabilityTurn { get; set; } = 0.05f;

    /// <summary>
    /// Probability of generating street end.
    /// Default: 0.001
    /// </summary>
    public float ProbabilityStreetEnd { get; set; } = 0.001f;

    /// <summary>
    /// Minimum size of building size.
    /// Default: 3
    /// </summary>
    public int BuildingMinSize { get; set; } = 3;

    /// <summary>
    /// Maximum size of building size.
    /// Default: 6
    /// </summary>
    public int BuildingMaxSize { get; set; } = 6;

    /// <summary>
    /// Minimum distance between buildings.
    /// Default: 1
    /// </summary>
    public int BuildingMinSpace { get; set; } = 1;

    /// <summary>
    /// Maximum distance between buildings.
    /// Default: 3
    /// </summary>
    public int BuildingMaxSpace { get; set; } = 3;

    /// <summary>
    /// Distance between building and path.
    /// Default: 0
    /// </summary>
    public int BuildingOffset { get; set; } = 0;
}

public struct PathData {
    public Vector2I Position;
    public int Direction;
}

public enum NodeType {
    TURN,
    CROSS,
    END
}