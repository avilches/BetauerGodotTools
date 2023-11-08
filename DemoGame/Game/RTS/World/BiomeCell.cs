using Godot;

namespace Veronenger.Game.RTS.World;

public class BiomeCell {
    public bool Water => HeightMt <= 0;
    public float Humidity { get; set; }
    public float Temp { get; set; }
    public float TempCelsius => NormalizeTemperature(Temp);
    public float Height { get; set; }
    public float HeightMt => NormalizeHeight(Height);
    public Biome<BiomeType> Biome { get; set; }

    public static float NormalizeTemperature(float tempNormalized) {
        var temp = Mathf.Lerp(-50, 50, tempNormalized);
        var rounded = (int)(temp * 10);
        return rounded / 10f; // just one decimal, like 36.5
    }

    public static float NormalizeHeight(float heightNormalized) {
        return Mathf.Lerp(-5000, 5000, heightNormalized);
    }
}