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
    public NormalizedVirtualDataGrid HeightNormalizedGrid { get; private set; }
    
    public IslandGenerator MassLands { get; private set; }
    public bool FalloffEnabled { get; set; }
    public IDataGrid<float> ContinentMap { get; private set; }
    public float FallOffMapExp { get; set; } = 1;
    public float FallOffMapOffset { get; set; } = 0.4f;
    
    public NormalizedVirtualDataGrid HeightFalloffGrid { get; private set; }
    
    public FastNoiseLite HumidityNoise { get; } = new();
    public NormalizedVirtualDataGrid HumidityNormalizedGrid { get; private set;}

    public FloatGrid<BiomeType> BiomeGrid { get; private set; }
    public BiomeCell[,] BiomeCells { get; private set; }
    
    public int Seed {
        set {
            HeightNoise.Seed = value;
            HumidityNoise.Seed = value * 137712;
        }
    }

    public void Configure(int width, int height, int seed) {
        Width = width;
        Height = height;
        Seed = seed;

        BiomeCells = new BiomeCell[height, width];

        MassLands = new IslandGenerator(width, height);
        ContinentMap = new VirtualDataGrid<float>((x, y) =>
            EasingFunctions.Logistic(MassLands.GetValue(x, y), FallOffMapExp, FallOffMapOffset));
        
        HeightNormalizedGrid = HeightNoise.CreateNormalizedVirtualDataGrid(width, height);
        HumidityNormalizedGrid = HumidityNoise.CreateNormalizedVirtualDataGrid(width, height);         
        HeightFalloffGrid = new NormalizedVirtualDataGrid(width, height, (x,y) => {
            var height = HeightNormalizedGrid.GetValue(x, y);
            if (!FalloffEnabled) return height;
            return height + ContinentMap.GetValue(x, y);
        });

        var charMapping = Biomes.ToDictionary(pair => pair.Value.Char, pair => pair.Value.Type);
        BiomeGrid = FloatGrid<BiomeType>.Parse(BiomeConfig, charMapping);

        HeightNoise.NoiseTypeValue = FastNoiseLite.NoiseType.OpenSimplex2S;
        HeightNoise.Frequency = 0.024f;

        HeightNoise.FractalTypeValue = FastNoiseLite.FractalType.FBm;
        HeightNoise.FractalOctaves = 5;
        HeightNoise.FractalLacunarity = 2;
        HeightNoise.FractalGain = 0.5f;
        HeightNoise.FractalWeightedStrength = 0f;

        HumidityNoise.NoiseTypeValue = FastNoiseLite.NoiseType.OpenSimplex2S;
        HumidityNoise.Frequency = 0.004f;
        HumidityNoise.FractalTypeValue = FastNoiseLite.FractalType.FBm;
        HumidityNoise.FractalOctaves = 5;
        HumidityNoise.FractalLacunarity = 2;
        HumidityNoise.FractalGain = 0.5f;
        HumidityNoise.FractalWeightedStrength = 0f;
    }

    public Biome<BiomeType> FindBiome(float humidity, float height, float temperature) {
        // 1 - height because biomes are configured in the string where the glacier is on top (pos 0) but it's the highest!
        var biomeType = BiomeGrid.Get(humidity, 1 - height);
        return Biomes[biomeType];
    }

    public BiomeCell[,] Generate() {
        var overlapType = IslandGenerator.OverlapType.MaxHeight;
        MassLands.Clear();
        MassLands.AddIsland(100, 50, 90, 80, overlapType);
        MassLands.AddIsland(200, 100, 90, 80, overlapType);
        MassLands.AddIsland(300, 250, 90, 80, overlapType);
        MassLands.AddIsland(400, 320, 90, 80, overlapType);
        MassLands.AddIsland(Width/2, Height/2, Width/3, Height/3, overlapType);
        MassLands.AddIsland(Width/6 * 2, Height/4 * 3, Width/8, Height/6, overlapType);
        MassLands.AddIsland(Width/6 * 4, Height/2 * 1, Width/8, Height/6, overlapType);
        MassLands.Normalize();
        
        HeightNormalizedGrid.Normalize();
        HumidityNormalizedGrid.Normalize();
        HeightFalloffGrid.Normalize();
                
        var list = new List<BiomeCell>();
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                BiomeCell biomeCell = new BiomeCell {
                    Height = HeightFalloffGrid.GetValue(x, y),
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