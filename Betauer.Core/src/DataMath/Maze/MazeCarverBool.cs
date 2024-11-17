using Godot;

namespace Betauer.Core.DataMath.Maze;

public class MazeCarverBool : BaseMazeCarver {
    public Array2D<bool> Grid { get; init; }

    public MazeCarverBool(int width, int height) : this(new Array2D<bool>(width, height)) {
    }

    public MazeCarverBool(Array2D<bool> grid) : base(grid.Width, grid.Height) {
        Grid = grid;
    }

    public override bool IsCarved(Vector2I pos) {
        return Grid[pos];
    }

    public override void UnCarve(Vector2I pos) {
        var dataCells = Grid;
        dataCells[pos] = false;
    }

    public override void Carve(Vector2I pos, MazeCarveType start, int region) {
        var dataCells = Grid;
        dataCells[pos] = true;
    }

    public override bool CanCarve(Vector2I pos, Vector2I direction) {
        var vector2I = pos + direction * 3;
        return Geometry.Geometry.IsPointInRectangle(vector2I.X, vector2I.Y, 0f, 0f, Grid.Width, Grid.Height) &&
               !IsCarved(pos + direction * 2);
    }
}