using Godot;

namespace Veronenger.Game.RTS.World;

public class BiomeCell {
    public Vector2I Position { get; init; }
    public Biome Biome { get; set; }
    public float Humidity { get; set; }
    public float Temp { get; set; }
    public float Height { get; set; }
    public static float SeaLevel { get; set; }
    public float Water { get; set; }

    public bool River => Water > 0f;
    public bool Sea => Height <= SeaLevel;
    public bool Land => Height > SeaLevel;
    public float TempCelsius => HumanTemperature(Temp);
    public float HeightMt => HumanHeight(Height);
    public Color Color => River ? Colors.Aquamarine : (Coast ? Colors.Black : Biome.Color);
    public bool Coast { get; set; } = false;

    public static float HumanTemperature(float tempNormalized) {
        var temp = Mathf.Lerp(-50, 50, tempNormalized);
        var rounded = (int)(temp * 10);
        return rounded / 10f; // just one decimal, like 36.5
    }

    public static float HumanHeight(float heightNormalized) {
        return Mathf.Lerp(0, 1000, heightNormalized - SeaLevel);
    }
}