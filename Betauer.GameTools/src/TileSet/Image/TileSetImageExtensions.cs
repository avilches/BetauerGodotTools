using Godot;

namespace Betauer.TileSet.Image;

public static class TileSetImageExtensions {
    
    public static Color[,] CopyLH(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, 0, 0, source.CellSize / 2, source.CellSize);
    }

    public static Color[,] CopyRH(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, source.CellSize / 2, 0, source.CellSize / 2, source.CellSize);
    }

    public static Color[,] CopyTH(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, 0, 0, source.CellSize, source.CellSize / 2);
    }

    public static Color[,] CopyBH(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, 0, source.CellSize / 2, source.CellSize, source.CellSize / 2);
    }

    public static Color[,] CopyTLQ(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, 0, 0, source.CellSize / 2, source.CellSize / 2);
    }

    public static Color[,] CopyTRQ(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, source.CellSize / 2, 0, source.CellSize / 2, source.CellSize / 2);
    }

    public static Color[,] CopyBLQ(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, 0, source.CellSize / 2, source.CellSize / 2, source.CellSize / 2);
    }

    public static Color[,] CopyBRQ(this TileSetImage source, int tileId) {
        return source.CopyPart(tileId, source.CellSize / 2, source.CellSize / 2, source.CellSize / 2, source.CellSize / 2);
    }

    public static void PasteTL(this Color[,] cell, TileSetImage source, int tileId, bool blend = true) {
        source.PastePart(cell, tileId, 0, 0, blend);
    }

    public static void PasteTR(this Color[,] cell, TileSetImage source, int tileId, bool blend = true) {
        source.PastePart(cell, tileId, source.CellSize / 2, 0, blend);
    }

    public static void PasteBL(this Color[,] cell, TileSetImage source, int tileId, bool blend = true) {
        source.PastePart(cell, tileId, 0, source.CellSize / 2, blend);
    }

    public static void PasteBR(this Color[,] cell, TileSetImage source, int tileId, bool blend = true) {
        source.PastePart(cell, tileId, source.CellSize / 2, source.CellSize / 2, blend);
    }

}