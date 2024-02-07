using Godot;

namespace Veronenger.Game.RTS.World;

public class BiomeCell {
    public bool Water => Height <= SeaLevel;
    public bool Land => Height > SeaLevel;
    public float Humidity { get; set; }
    public float Temp { get; set; }
    public float TempCelsius => HumanTemperature(Temp);
    public float Height { get; set; }
    public float HeightMt => HumanHeight(Height);
    public Biome<BiomeType> Biome { get; set; }
    
    public static float SeaLevel { get; set; }

    public static float HumanTemperature(float tempNormalized) {
        var temp = Mathf.Lerp(-50, 50, tempNormalized);
        var rounded = (int)(temp * 10);
        return rounded / 10f; // just one decimal, like 36.5
    }

    public static float HumanHeight(float heightNormalized) {
        return Mathf.Lerp(0, 1000, heightNormalized - SeaLevel);
    }
}