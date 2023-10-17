using System.Collections.Generic;

namespace Betauer.TileSet.TileMap;

public enum BasicTileType {
    Empty = -1,
    Type0 = 0,
    Type1 = 1,
    Type2 = 2,
    Type3 = 3,
    Type4 = 4,
    Type5 = 5,
    Type6 = 6,
    Type7 = 7,
    Type8 = 8,
    Type9 = 9,
}

public class BasicTileMap : TileMap<BasicTileType> {
    private static readonly Dictionary<char, BasicTileType> Defaults = new() {
        { ' ', BasicTileType.Empty },
        { '0', BasicTileType.Type0 },
        { '1', BasicTileType.Type1 },
        { '2', BasicTileType.Type2 },
        { '3', BasicTileType.Type3 },
        { '4', BasicTileType.Type4 },
        { '5', BasicTileType.Type5 },
        { '6', BasicTileType.Type6 },
        { '7', BasicTileType.Type7 },
        { '8', BasicTileType.Type8 },
        { '9', BasicTileType.Type9 },
    };

    public BasicTileMap(int width, int height) : this(1, width, height) {
    }

    public BasicTileMap(int layers, int width, int height) : base(layers, width, height, BasicTileType.Empty) {
    }

    public static TileMap<BasicTileType> Parse(string value, int layers = 1) {
        return Parse(value, Defaults, layers);
    }
}