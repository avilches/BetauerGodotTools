using System;
using Godot;

namespace Betauer.TileSet.TileMap;

public class TileRegionHandler<TTile> : ITileHandler<TTile> where TTile : struct, Enum {
    public int SourceId { get; init; }
    public Rect2I Region { get; init; }
    public bool Randomize { get; init; }

    public Texture2D GetTexture(Godot.TileMap godotTileMap) {
        var t = godotTileMap.TileSet.GetSource(SourceId);
        return ((TileSetAtlasSource)t).Texture;
    }

    /// <summary>
    /// Create an image from source using the AtlasCoords
    /// </summary>
    /// <param name="godotTileMap"></param>
    /// <returns></returns>
    public Godot.Image GetAtlasClip(Godot.TileMap godotTileMap) {
        var texture = GetTexture(godotTileMap);
        var image = texture.GetImage();
        var cellSize = new Vector2I(godotTileMap.TileSet.TileSize.X, godotTileMap.TileSet.TileSize.Y);
        var rect = new Rect2I(Region.Position * cellSize, Region.Size * cellSize);
        var newImage = Godot.Image.Create(rect.Size.X, rect.Size.Y, false, image.GetFormat());
        newImage.BlitRect(image, rect, Vector2I.Zero);
        return newImage;
    }

    public void Apply(TileMap<TTile> tileMap, int layer, int x, int y) {
        var atlasCoords = Randomize
            ? TileTools.GetRandomAtlasCoords(Region)
            : TileTools.GetAtlasCoords(Region, x, y);
        tileMap.SetAtlasCoords(layer, x, y, atlasCoords);
    }
}