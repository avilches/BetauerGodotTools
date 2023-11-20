using System;
using Godot;

namespace Betauer.TileSet.TileMap.Handlers.Deprecated;

public class DeprecatedSingleDeprecatedTileHandler<TTile> : IDeprecatedTileHandler<TTile> where TTile : Enum {
    public int SourceId { get; init; }
    public Vector2I AtlasCoords { get; init; }

    public Texture2D GetTexture(global::Godot.TileMap godotTileMap) {
        var t = godotTileMap.TileSet.GetSource(SourceId);
        return ((TileSetAtlasSource)t).Texture;
    }

    /// <summary>
    /// Create an image from source using the AtlasCoords
    /// </summary>
    /// <param name="godotTileMap"></param>
    /// <returns></returns>
    public global::Godot.Image GetAtlasClip(global::Godot.TileMap godotTileMap) {
        var texture = GetTexture(godotTileMap);
        var image = texture.GetImage();
        var cellSize = new Vector2I(godotTileMap.TileSet.TileSize.X, godotTileMap.TileSet.TileSize.Y);
        var rect = new Rect2I(AtlasCoords * cellSize, AtlasCoords * cellSize);
        var newImage = global::Godot.Image.Create(rect.Size.X, rect.Size.Y, false, image.GetFormat());
        newImage.BlitRect(image, rect, Vector2I.Zero);
        return newImage;
    }

    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y) {
        tileMap.SetAtlasCoords(layer, SourceId, x, y, AtlasCoords);
    }
}