using System;
using Godot;

namespace Betauer.TileSet;

public class TileRegionHandler<TTile> : ITileHandler<TTile> where TTile : struct, Enum {
    public int SourceId { get; init; }
    public Rect2I Region { get; init; }
    public bool Randomize { get; init; }

    public Texture2D GetTexture(TileMap tileMap) {
        var t = tileMap.TileSet.GetSource(SourceId);
        return ((TileSetAtlasSource)t).Texture;
    }

    /// <summary>
    /// Create an image from source using the AtlasCoords
    /// </summary>
    /// <param name="tileMap"></param>
    /// <returns></returns>
    public Image GetAtlasClip(TileMap tileMap) {
        var texture = GetTexture(tileMap);
        var image = texture.GetImage();
        var cellSize = new Vector2I(tileMap.TileSet.TileSize.X, tileMap.TileSet.TileSize.Y);
        var rect = new Rect2I(Region.Position * cellSize, Region.Size * cellSize);
        var newImage = Image.Create(rect.Size.X, rect.Size.Y, false, image.GetFormat());
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