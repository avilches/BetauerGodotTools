using System;
using Godot;

namespace Veronenger.Game.RTS.World;

public class TileAtlasPattern<TTile> : ITilePattern<TTile> 
    where TTile : Enum {
    public TTile Key { get; init; }
    public int SourceId { get; init; }
    public Rect2I AtlasCoords { get; init; }
    public bool Randomize { get; init; }

    public Texture2D GetTexture(TileMap tileMap) {
        var t = tileMap.TileSet.GetSource(SourceId);
        return ((Godot.TileSetAtlasSource)t).Texture;
    }

    /// <summary>
    /// Create an image from source using the AtlasCoords
    /// </summary>
    /// <param name="tileMap"></param>
    /// <returns></returns>
    public Image GetAtlasClip(TileMap tileMap) {
        var texture = GetTexture(tileMap);
        var image = texture.GetImage();
        var cellSize  = new Vector2I(tileMap.TileSet.TileSize.X, tileMap.TileSet.TileSize.Y);
        var rect = new Rect2I(AtlasCoords.Position * cellSize, AtlasCoords.Size * cellSize);
        var newImage = Image.Create(rect.Size.X, rect.Size.Y, false, image.GetFormat());
        newImage.BlitRect(image, rect, Vector2I.Zero);
        return newImage;
    } 


    public void Apply(TileMap tileMap, int layer, Vector2I pos) {
        var x = AtlasCoords.Position.X + (Randomize ? new Random().Next(0, AtlasCoords.Size.X): pos.X % AtlasCoords.Size.X);
        var y = AtlasCoords.Position.Y + (Randomize ? new Random().Next(0, AtlasCoords.Size.Y): pos.Y % AtlasCoords.Size.Y);
        tileMap.SetCell(layer, pos, SourceId, new Vector2I(x, y));
    }
}