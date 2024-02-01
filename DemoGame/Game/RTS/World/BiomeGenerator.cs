using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Data;
using Betauer.Core.Easing;
using Betauer.Core.Image;
using Godot;
using FastNoiseLite = Betauer.Core.Data.FastNoiseLite;

namespace Veronenger.Game.RTS.World;

public enum BiomeType {
    None = -1,
    Glacier,
    Rock,
    
    Tundre,
    Desert,
    Plains,
    Forest,
    Dirty,

    Beach,
    Sea,
    Ocean,
}

public class BiomeGenerator {

    public string BiomeConfig;

    public readonly Dictionary<BiomeType, Biome<BiomeType>> Biomes = new();
    
    public int Width { get; private set; }
    public int Height { get; private set; }

    // Massland is not normalized
    public float MasslandBias { get; set; } = 0.5f;
    public float MasslandOffset { get; set; } = 0.5f;
    public DataGrid MassLands { get; private set; }
    // This is the noise height, creating mountains and valleys
    public FastNoiseLite HeightNoise { get; } = new();
    // public NormalizedVirtualDataGrid HeightNoiseNormalizedGrid { get; private set; }

    // Final grid with the height + optionally applied the falloff
    public bool FalloffEnabled { get; set; }
    public bool HumidityEnabled { get; set; } = true;
    public DataGrid HeightFalloffGrid { get; private set; }
    
    public FastNoiseLite HumidityNoise { get; } = new();
    public DataGrid HumidityNormalizedGrid { get; private set;}

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

        BiomeConfig = """
                      :GGGGGGGGGGGG:
                      :GGGGGGGGGGGG:
                      :RRRRRRRRRRRR:
                      :TTTRRRRRRRRR:
                      :TTTpp!!!****:
                      :DDDpp!!!****:
                      :DDDpp!!!****:
                      :DDDpp!!!****:
                      :bbbbbbbbbbbb:
                      :bbbbbbbbbbbb:
                      :............:
                      :oooooooooooo:
                      :oooooooooooo:
                      :oooooooooooo:
                      :oooooooooooo:
                      :oooooooooooo:
                      """;
        
        new Biome<BiomeType>[] { 
            new() { Char = 'G', Type = BiomeType.Glacier,  Color = Colors.White },
            new() { Char = 'R', Type = BiomeType.Rock,     Color = Color(112,112, 110)}, // gris
            new() { Char = 'T', Type = BiomeType.Tundre,   Color = Color(186,80,43)}, // rojizo
            new() { Char = 'D', Type = BiomeType.Desert,   Color = Color(231,164,	84	)}, // amarillo mas oscuro
            new() { Char = 'p', Type = BiomeType.Plains,   Color = Color(96, 163, 24	)}, // verde claro
            new() { Char = '!', Type = BiomeType.Forest,   Color = Color(59, 134, 50 )}, // verde oscuro
            new() { Char = '*', Type = BiomeType.Dirty,    Color = Color(83,	62,	26 )}, // marron
            new() { Char = 'b', Type = BiomeType.Beach,    Color = Color(255,215,104) },
            new() { Char = '.', Type = BiomeType.Sea,      Color = Color(97, 187, 221	)},
            new() { Char = 'o', Type = BiomeType.Ocean,    Color = Color(90, 150,198	)},
        }.ForEach(biome => Biomes.Add(biome.Type, biome));

        ConfigureBiomeMap(BiomeConfig);
        

        BiomeCells = new BiomeCell[height, width];
        MassLands = new DataGrid(width, height, 0f);
        MasslandBias = 0.3f;
        MasslandOffset = 0.9f;
        HumidityNormalizedGrid = new DataGrid(width, height, 0f);
        HeightFalloffGrid = new DataGrid(width, height, 0f);
        
        HeightNoise.NoiseTypeValue = FastNoiseLite.NoiseType.OpenSimplex2S;
        HeightNoise.Frequency = 0.013f;
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
        return;

        Color Color(int r, int g, int b) {
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }

    public void ConfigureBiomeMap(string biomeMap) {
        BiomeConfig = biomeMap;
        var charMapping = Biomes.ToDictionary(pair => pair.Value.Char, pair => pair.Value.Type);
        BiomeGrid = FloatGrid<BiomeType>.Parse(BiomeConfig, charMapping);
    }

    public Biome<BiomeType> FindBiome(float humidity, float height, float temperature) {
        // 1 - height because biomes are configured in the string where the glacier is on top (pos 0) but it's the highest!
        var biomeType = BiomeGrid.Get(humidity, 1 - height);
        return Biomes[biomeType];
    }

    public enum OverlapType {
        Simple,
        MaxHeight,
    }
    
    public void AddIsland(DataGrid Data, int cx, int cy, int rx, int ry, OverlapType overlap, IEasing? easing = null) {
        Draw.GradientCircle(cx, cy, Math.Max(rx, ry), (x, y, value) => {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            var heightValue = value <= 1 ? 1 - value : 0;

            if (overlap == OverlapType.Simple) {
                Data.Data[x, y] += heightValue;
            } else if (overlap == OverlapType.MaxHeight) {
                Data.Data[x, y] = Math.Max(Data.Data[x, y], heightValue);
            }
        }, easing);
    }

    public void Generate() {
        var sa = Stopwatch.StartNew();
        var s = Stopwatch.StartNew();
        var overlapType = OverlapType.MaxHeight;
        var transitionType = Easings.CreateBiasGain(MasslandBias, MasslandOffset);
        MassLands.Fill(0);
        AddIsland(MassLands, 100, 50, 90, 80, overlapType, transitionType);
        AddIsland(MassLands, 200, 100, 90, 80, overlapType, transitionType);
        AddIsland(MassLands, 300, 250, 90, 80, overlapType, transitionType);
        AddIsland(MassLands, 400, 320, 90, 80, overlapType, transitionType);
        AddIsland(MassLands, Width/2, Height/2, Width/3, Height/3, overlapType, transitionType);
        AddIsland(MassLands, Width/6 * 2, Height/4 * 3, Width/8, Height/6, overlapType, transitionType);
        AddIsland(MassLands, Width/6 * 4, Height/2 * 1, Width/8, Height/6, overlapType, transitionType);
        Console.WriteLine($"Generate1 masslands:{s.ElapsedMilliseconds}ms");
        s.Restart();
        
        HeightFalloffGrid.Load((x,y) => {
            if (FalloffEnabled) {
                var height = HeightNoise.GetNoise(x, y) / 2f + 1f;
                return height + MassLands.GetValue(x, y);
            } else {
                return HeightNoise.GetNoise(x, y);                
            }
        });
        HeightFalloffGrid.Normalize();
        Console.WriteLine($"Generate2 normalize massland:{s.ElapsedMilliseconds}ms");
        s.Restart();

        HumidityNormalizedGrid.Load((x, y) => HumidityNoise.GetNoise(x, y));
        HumidityNormalizedGrid.Normalize();
        Console.WriteLine($"Generate4:{s.ElapsedMilliseconds}ms");
        s.Restart();
                
        var list = new List<BiomeCell>();
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                BiomeCell biomeCell = new BiomeCell {
                    Height = HeightFalloffGrid.GetValue(x, y),
                    Humidity = HumidityEnabled ? HumidityNormalizedGrid.GetValue(x, y) : 0f,
                };
                biomeCell.Temp = CalculateTemperature(y, Height, biomeCell.Height);
                // Console.Write(cell.Height.ToString("0.0")+ " | ");
                BiomeCells[y, x] = biomeCell;
                biomeCell.Biome = FindBiome(biomeCell.Humidity, biomeCell.Height, biomeCell.Temp);
                list.Add(biomeCell);
            }
            // Console.WriteLine();
        }
        Console.WriteLine($"Generate6:{s.ElapsedMilliseconds}ms");
        s.Restart();
        
        var dict = list.Select(c => c.Biome.Type).GroupBy(x => x)
            .ToDictionary(g => g.Key, g => g.Count());
        dict.ForEach(pair => Console.WriteLine(pair.Key + ": " + pair.Value));
        Console.WriteLine($"Generate Total:{sa.ElapsedMilliseconds}ms");
    }

    private float CalculateTemperature(int y, int height, float heightNormalized) {
        float equatorHeat = 0.5f; // más calor en el ecuador
        float positionFactor = 1 - Math.Abs(y - height / 2f) / (height / 2f); // de 0 en los polos a 1 en el ecuador

        // La temperatura disminuye con la altitud
        float heightFactor = 1 - heightNormalized;

        return positionFactor * equatorHeat + heightFactor * (1 - equatorHeat);
    }

    public void FillMassland(FastImage fastTexture) {
        MassLands.Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }

    public void FillHeight(FastImage fastTexture) {
        HeightNoise.CreateDataGrid(Width, Height, true).Normalize().Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }

    public void FillFalloff(FastImage fastTexture) {
        var dataGrid = new DataGrid(Width, Height, (x, y) => {
            var height = HeightNoise.GetNoise(x, y) / 2f + 1f;
            return height + MassLands.GetValue(x, y);
        });
        dataGrid.Normalize();
        dataGrid.Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }

    public void FillHumidity(FastImage fastTexture) {
        HumidityNormalizedGrid.Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }
}