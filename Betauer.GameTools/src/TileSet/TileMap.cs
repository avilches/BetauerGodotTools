using Godot;

namespace Betauer.TileSet;

public class TileMap<TTileType> where TTileType : struct {
    public struct TileInfo {
        public readonly TTileType? Type;
        public readonly Vector2I? AtlasCoords;

        public TileInfo(TTileType? type, Vector2I? atlasCoords) {
            Type = type;
            AtlasCoords = atlasCoords;
        }
    }

    public TileInfo[,,] InternalData { get; }
    public int Layers { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }

    public TileMap(int width, int height) : this(1, width, height) {
    }

    public TileMap(int layers, int width, int height) {
        Width = width;
        Height = height;
        Layers = layers;
        InternalData = new TileInfo[layers, width, height];
    }

    public TileInfo GetCellInfo(int layer, int x, int y) {
        return InternalData[layer, x, y];
    }

    public bool SetCellInfo(int layer, int x, int y, TileInfo tileInfo) {
        var currentInfo = InternalData[layer, x, y];
        if (currentInfo.Type.Equals(tileInfo.Type) && currentInfo.AtlasCoords.Equals(tileInfo.AtlasCoords)) return false;
        InternalData[layer, x, y] = tileInfo;
        return true;
    }

    public bool RemoveCellInfo(int layer, int x, int y, TileInfo tileInfo) {
        return SetCellInfo(layer, x, y, new TileInfo(null, null));
    }

    public bool SetType(int layer, int x, int y, TTileType? type) {
        var currentInfo = InternalData[layer, x, y];
        if (currentInfo.Type.Equals(type)) return false;
        InternalData[layer, x, y] = new TileInfo(type, currentInfo.AtlasCoords);
        return true;
    }

    public bool RemoveType(int layer, int x, int y) {
        return SetType(layer, x, y, null);
    }

    public bool SetAtlasCoords(int layer, int x, int y, Vector2I? atlasCoords) {
        var currentInfo = InternalData[layer, x, y];
        if (currentInfo.AtlasCoords.Equals(atlasCoords)) return false;
        InternalData[layer, x, y] = new TileInfo(currentInfo.Type, atlasCoords);
        return true;
    }

    public bool RemoveAtlasCoords(int layer, int x, int y) {
        return SetAtlasCoords(layer, x, y, null);
    }
}