using System;

namespace Betauer.TileSet.Image;

public class TileSetLayoutBuilder : TileSetLayout {
    
    public TileSetLayoutBuilder(TileSetLayout tileSetLayout) {
        Rows = tileSetLayout.Rows;
        Columns = tileSetLayout.Columns;
        _Load(tileSetLayout.Export());
    }

    public TileSetLayoutBuilder(int rows, int columns) {
        Rows = rows;
        Columns = columns;
        Clear();
    }

    public TileSetLayoutBuilder Clear() {
        Tiles.Clear();
        Positions = new int[Rows, Columns];
        for (var y = 0; y < Rows; y++) {
            for (var x = 0; x < Columns; x++) {
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
        Tiles[tileId] = new TilePosition(tileId, x, y);
        return this;
    }
}

