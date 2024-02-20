using System;
using Godot;

namespace Veronenger.Game.RTS.World;

public class Biome {
    public Color Color;
    public char Char { get; set; }
    public BiomeType Type { get; set; }
    public (int Min, int Max) Temperature { get; set; }
    public (float Min, float Max) Height { get; set; }
    public (float Min, float Max) Humidity { get; set; }
}