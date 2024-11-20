namespace Betauer.Core.PCG.Maze.ExampleDart;

public class Cell {
    public TileType Type;
    public int X;
    public int Y;
    public int Region { get; set; }

    public Cell(int x, int y, TileType type, int region) {
        X = x;
        Y = y;
        Type = type;
        Region = region;
    }

}