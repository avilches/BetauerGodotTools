using System;
using Godot;

namespace Betauer.TileSet;

public class SingleTileHandler<TTile> : ITileHandler<TTile> where TTile : struct, Enum {
    public int SourceId { get; init; }
    public Vector2I AtlasCoords { get; init; }

    public Texture2D GetTexture(TileMap tileMap) {
        var t = tileMap.TileSet.GetSource(SourceId);
        return ((TileSetAtlasSource)t).Texture;
    }

    /// <summary>
    /// Create an image from source using the AtlasCoords
    /// </summary>
    /// <param name="tileMap"></param>
    /// <returns></returns>
    public Godot.Image GetAtlasClip(TileMap tileMap) {
        var texture = GetTexture(tileMap);
        var image = texture.GetImage();
        var cellSize = new Vector2I(tileMap.TileSet.TileSize.X, tileMap.TileSet.TileSize.Y);
        var rect = new Rect2I(AtlasCoords * cellSize, AtlasCoords * cellSize);
        var newImage = Godot.Image.Create(rect.Size.X, rect.Size.Y, false, image.GetFormat());
        newImage.BlitRect(image, rect, Vector2I.Zero);
        return newImage;
    }

    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y) {
        tileMap.SetAtlasCoords(layer, x, y, AtlasCoords);
    }
}