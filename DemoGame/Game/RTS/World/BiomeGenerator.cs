using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Animation.Easing;
using Betauer.Core;
using Betauer.Core.Data;
using Godot;
using FastNoiseLite = Betauer.Core.Data.FastNoiseLite;

namespace Veronenger.Game.RTS.World;

public enum BiomeType {
    None = -1,
    Glacier,
    Mountain,
    
    Desert,
    Savannah,
    Forest,
    Jungle,

    Beach,
    Sea,
    // Ocean,
}

public class BiomeGenerator {

    public const string BiomeConfig = """
                                      :GGGGGGGGGGGG:
                                      :MMMMMMMMMMMM:
                                      :DDDss!!!JJJJ:
                                      :DDDss!!!JJJJ:
                                      :bbbbbbbbbbbb:
                                      :............:
                                      :............:
                                      :............:
                                      :............:
                                      """;
    
    public readonly Dictionary<BiomeType, Biome<BiomeType>> Biomes = new() {
        { BiomeType.Glacier,  new Biome<BiomeType> { Char = 'G', Type = BiomeType.Glacier,  } },
        { BiomeType.Mountain, new Biome<BiomeType> { Char = 'M', Type = BiomeType.Mountain, } },
    
        { BiomeType.Desert,   new Biome<BiomeType> { Char = 'D', Type = BiomeType.Desert,   } },
        { BiomeType.Savannah, new Biome<BiomeType> { Char = 's', Type = BiomeType.Savannah, } },
        { BiomeType.Forest,   new Biome<BiomeType> { Char = '!', Type = BiomeType.Forest,   } },
        { BiomeType.Jungle,   new Biome<BiomeType> { Char = 'J', Type = BiomeType.Jungle,   } },

        { BiomeType.Beach,    new Biome<BiomeType> { Char = 'b', Type = BiomeType.Beach,    } },
        { BiomeType.Sea,      new Biome<BiomeType> { Char = '.', Type = BiomeType.Sea,      } },
        // { BiomeType.Ocean,    new Biome<BiomeType> { Type = BiomeType.Ocean,    } },
    };


    public int Width { get; private set; }
    public int Height { get; private set; }

    public FastNoiseLite HeightNoise { get; } = new();
    public INormalizedDataGrid<float> HeightNormalizedGrid { get; private set; }

    public float[,] FalloffMap { get; private set; }
    public FastNoiseLite HumidityNoise { get; } = new();
    public INormalizedDataGrid<float> HumidityNormalizedGrid { get; private set;}

    public FloatGrid<BiomeType> BiomeGrid { get; private set; }
    public BiomeCell[,] BiomeCells { get; private set; }
    
    public int Seed {
        get => HumidityNoise.Seed;
        set {
            HeightNoise.Seed = value;
            HumidityNoise.Seed = value * 137712;
        }
    }

    public void Configure(int width, int height) {
        Width = width;
        Height = height;
        Seed = 123456;

        BiomeCells = new BiomeCell[height, width];
        FalloffMap = FalloffGenerator.GenerateFalloffMap(width, height);
        HeightNormalizedGrid = HeightNoise.CreateNormalizedVirtualDataGrid(width, height);
        HumidityNormalizedGrid = HumidityNoise.CreateNormalizedVirtualDataGrid(width, height);         

        var charMapping = Biomes.ToDictionary(pair => pair.Value.Char, pair => pair.Value.Type);
        BiomeGrid = FloatGrid<BiomeType>.Parse(BiomeConfig, charMapping);

        HeightNoise.NoiseTypeValue = FastNoiseLite.NoiseType.OpenSimplex2S;
        HeightNoise.Frequency = 0.02f;

        HeightNoise.FractalTypeValue = FastNoiseLite.FractalType.FBm;
        HeightNoise.FractalOctaves = 5;
        HeightNoise.FractalLacunarity = 2;
        HeightNoise.FractalGain = 0.5f;
        HeightNoise.FractalWeightedStrength = 0f;

        HumidityNoise.NoiseTypeValue = FastNoiseLite.NoiseType.OpenSimplex2S;
        HumidityNoise.Frequency = 0.005f;
        HumidityNoise.FractalTypeValue = FastNoiseLite.FractalType.FBm;
        HumidityNoise.FractalOctaves = 5;
        HumidityNoise.FractalLacunarity = 2;
        HumidityNoise.FractalGain = 0.5f;
        HumidityNoise.FractalWeightedStrength = 0f;
    }

    public Biome<BiomeType> FindBiome(float humidity, float height, float temperature) {
        var biomeType = BiomeGrid.Get(humidity, height);
        return Biomes[biomeType];
    }

    public BiomeCell[,] Generate() {
        var transitionType = Tween.TransitionType.Quad;
        // noiseMap [x, y] = Mathf.Clamp01(noiseMap [x, y] - falloffMap [x, y]);

        HeightNormalizedGrid.Load(); // v => EasingFunctions.EaseInOut(v, transitionType));
        HumidityNormalizedGrid.Load();
                
        var list = new List<BiomeCell>();
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                BiomeCell biomeCell = new BiomeCell {
                    Height = HeightNormalizedGrid.GetValue(x, y),
                    Humidity = HumidityNormalizedGrid.GetValue(x, y),
                };
                biomeCell.Temp = CalculateTemperature(y, Height, biomeCell.Height);
                // Console.Write(cell.Height.ToString("0.0")+ " | ");
                BiomeCells[y, x] = biomeCell;
                biomeCell.Biome = FindBiome(biomeCell.Humidity, biomeCell.Height, biomeCell.Temp);
                list.Add(biomeCell);
            }
            // Console.WriteLine();
        }
        
        var dict = list.Select(c => c.Biome.Type).GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());
        dict.ForEach(pair => Console.WriteLine(pair.Key + ": " + pair.Value));
        return BiomeCells;
    }
    
    private float CalculateTemperature(int y, int height, float heightNormalized) {
        float equatorHeat = 0.5f; // más calor en el ecuador
        float positionFactor = 1 - Math.Abs(y - height / 2f) / (height / 2f); // de 0 en los polos a 1 en el ecuador

        // La temperatura disminuye con la altitud
        float heightFactor = 1 - heightNormalized;

        return positionFactor * equatorHeat + heightFactor * (1 - equatorHeat);
    }    
}

public static class FalloffGenerator {
    public static float[,] GenerateFalloffMap(int width, int height) {
        var map = new float[height, width];
        for (var y = 0; y < height; y++) {
            for (var x = 0; x < width; x++) {
                var xx = y / (float)width * 2 - 1;
                var yy = x / (float)height * 2 - 1;
                var value = Mathf.Max(Mathf.Abs(xx), Mathf.Abs(yy));
                map[y, x] = Evaluate(value);
                map[y, x] = value;
            }
        }
        return map;
    }

    static float Evaluate(float value) {
        const float a = 3f;
        const float b = 2.2f;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}