namespace Veronenger.Game.Dungeon.GenCity;

public class BuildingGenerationOptions {
    /// <summary>
    /// Generation seed
    /// </summary>
    public int Seed { get; set; } = 1;

    /// <summary>
    /// Max number of buildings to be created. -1 means no limit (it will process all the paths and will
    /// try to add as many random buildings as possible per path).
    /// </summary>
    public int Total { get; set; } = -1;

    /// <summary>
    /// Minimum size of building size.
    /// Default: 3
    /// </summary>
    public int MinSize { get; set; } = 3;

    /// <summary>
    /// Maximum size of building size.
    /// Default: 6
    /// </summary>
    public int MaxSize { get; set; } = 6;

    /// <summary>
    /// Minimum distance between buildings.
    /// Default: 1
    /// </summary>
    public int MinSpace { get; set; } = 1;

    /// <summary>
    /// Maximum distance between buildings.
    /// Default: 3
    /// </summary>
    public int MaxSpace { get; set; } = 3;

    /// <summary>
    /// Distance between building and path.
    /// Default: 0
    /// </summary>
    public int Sidewalk { get; set; } = 1;
}