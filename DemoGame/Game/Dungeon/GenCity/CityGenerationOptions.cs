using System;
using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.Dungeon.GenCity;

public class CityGenerationOptions {

    public Action<Vector2I>? OnUpdate;

    /// <summary>
    /// Generation seed
    /// </summary>
    public int Seed { get; set; } = 1;

    /// <summary>
    /// Start generation position.
    /// Default: Center of map size
    /// </summary>
    public Vector2I? StartPosition { get; set; }

    /// <summary>
    /// Start generation directions.
    /// Default: Left, Right, Top, Bottom
    /// </summary>
    public List<Vector2I> StartDirections { get; set; } = [Vector2I.Right, Vector2I.Down, Vector2I.Left, Vector2I.Up];

    /// <summary>
    /// Street length before generating an intersection or turn.
    /// Default: 10
    /// </summary>
    public int StreetMinLength { get; set; } = 10;

    /// <summary>
    /// Probability of continue generating the same path when the minimum length is reached.
    ///
    /// The probability of make a turn, cross, extend or fork is (1 - ProbabilityExtend), so a 80% of extending means a 20% chance of a turn, cross, fork
    /// Default: 80%
    ///
    /// 0% means a homogenous grid of StreetMinLength cells.
    /// 100% no intersections, only straight lines.
    /// </summary>
    public float ProbabilityExtend { get; set; } = 0.80f;

    /// <summary>
    /// Probability of generating cross intersection +
    /// Default: 42%
    /// </summary>
    public float ProbabilityCross { get; set; } = 0.42f;

    /// <summary>
    /// Probability of generating fork intersection T
    /// The fork could be a T or just a straight line with a path on the side.
    /// Default: 42%
    /// </summary>
    public float ProbabilityFork { get; set; } = 0.42f;

    /// <summary>
    /// Probability of generating turn.
    /// Default: 12%
    /// </summary>
    public float ProbabilityTurn { get; set; } = 0.12f;

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