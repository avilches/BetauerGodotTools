using Betauer.Core.DataMath;
using Godot;

namespace Betauer.Core.PCG.Maze;

public class MazeCarver : BaseMazeCarver {
    public Array2D<MazeCarveType> Grid { get; init; }

    public MazeCarver(int width, int height) : this(new Array2D<MazeCarveType>(width, height)) {
    }

    public MazeCarver(Array2D<MazeCarveType> grid) : base(grid.Width, grid.Height) {
        Grid = grid;
    }

    public override bool IsCarved(Vector2I pos) {
        return Grid[pos] != MazeCarveType.Empty;
    }

    public override void UnCarve(Vector2I pos) {
        var dataCells = Grid;
        dataCells[pos] = MazeCarveType.Empty;
    }

    public override void Carve(Vector2I pos, MazeCarveType type) {
        var dataCells = Grid;
        dataCells[pos] = type;
    }

    public override bool CanCarve(Vector2I pos, Vector2I direction) {
        var vector2I = pos + direction * 3;
        return DataMath.Geometry.Geometry.IsPointInRectangle(vector2I.X, vector2I.Y, 0f, 0f, Grid.Width, Grid.Height) &&
               !IsCarved(pos + direction * 2);
    }
}