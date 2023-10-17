using System;
using Godot;

namespace Betauer.TileSet.Image;

public class TileSetLayoutBuilder : TileSetLayout {
    
    public TileSetLayoutBuilder(ITileSetLayout tileSetLayout) {
        Height = tileSetLayout.Height;
        Width = tileSetLayout.Width;
        _Load(tileSetLayout.Export());
    }

    public TileSetLayoutBuilder(int width, int height) {
        Height = height;
        Width = width;
        Positions = new int[height, width];
        Clear();
    }

    public TileSetLayoutBuilder Clear() {
        Tiles.Clear();
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                Positions[y, x] = -1;
            }
        }
        return this;
    }

    public TileSetLayoutBuilder Load(int[,] layout) {
        _Load(layout);
        return this;
    }

    public TileSetLayoutBuilder RemoveTile(int x, int y) {
        var tileId = Positions[y, x];
        if (tileId > -1) {
            Tiles.Remove(tileId);
        }
        Positions[y, x] = -1;
        return this;
    }

    public TileSetLayoutBuilder RemoveTile(int tileId) {
        if (tileId < 0) throw new Exception($"Invalid tile id {tileId}");
        if (Tiles.TryGetValue(tileId, out var position)) {
            Positions[position.Y, position.X] = -1;
            Tiles.Remove(tileId);
        }
        return this;
    }

    public TileSetLayoutBuilder AddTile(int tileId, int x, int y) {
        if (tileId < 0) throw new Exception($"Invalid tile id {tileId}");
        Positions[y, x] = tileId;
        Tiles[tileId] = new Vector2I(x, y);
        return this;
    }
}

