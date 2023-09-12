using System;
using Godot;

namespace Veronenger.Game.RTS.World;

public interface ITilePattern<TTile> where TTile : Enum {
    public TTile Key { get; init; }
    public int SourceId { get; init; }
    public void Apply(TileMap tileMap, int layer, Vector2I pos);
}