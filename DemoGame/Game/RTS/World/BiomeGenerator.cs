using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Collision.Spatial2D;
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
    public float MasslandBias { get; set; }
    public float MasslandOffset { get; set; }

    private IInterpolation2D Falloff;
    float MasslandFallofStart = 0f;
    float MasslandFalloffEnd = 1f;
    public Func<float, float, float> RampFunc;
    public DataGrid MassLands { get; private set; }

    // This is the noise height, creating mountains and valleys
    public FastNoiseLite HeightNoise { get; } = new();
    // public NormalizedVirtualDataGrid HeightNoiseNormalizedGrid { get; private set; }

    // Final grid with the height + optionally applied the falloff
    public bool FalloffEnabled { get; set; } = true;
    public bool HumidityEnabled { get; set; } = true;
    public DataGrid HeightFalloffGrid { get; private set; }

    public FastNoiseLite HumidityNoise { get; } = new();
    public DataGrid HumidityNormalizedGrid { get; private set; }

    public FloatGrid<BiomeType> BiomeGrid { get; private set; }
    public BiomeCell[,] BiomeCells { get; private set; }

    public int Seed {
        set {
            HeightNoise.Seed = value;
            HumidityNoise.Seed = value * 137712;
        }
    }

    public BiomeGenerator() {
        BiomeConfig = """
                      :GGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGrGGGGGGGGGGGG:
                      :GGGGGGGGrGGGGGGGGGGGG:
                      :rrrrrrrrGrrrrrrrrrrrr:
                      :rrrrrrrrGrrrrrrrrrrrr:
                      :TrTrTTrwr!!!!!rrrrrrr:
                      :TTTTTTwww!!!!!*******:
                      :TTTTTTwww!!!!!*******:
                      :TTTTTTwww!!!!!*******:
                      :TTTTTTwww!!!!!*******:
                      :TTTTTTwww!!!!!*******:
                      :TTTTTTwww!!!!!*******:
                      :DDDDDDwww!!!!!*******:
                      :DDDDDDwww!!!*!*******:
                      :bbbbbbbbbb!bb.bbb*bbb:
                      :.....................:
                      :oooo.oooooooooooooooo:
                      :ooooooooooooooooooooo:
                      :ooooooooooooooooooooo:
                      :ooooooooooooooooooooo:
                      """;

        new Biome<BiomeType>[] {
            new() { Char = 'G', Type = BiomeType.Glacier, Color = Colors.White },
            new() { Char = 'r', Type = BiomeType.Rock, Color = Color(112, 112, 110) }, // gris
            new() { Char = 'T', Type = BiomeType.Tundre, Color = Color(186, 80, 43) }, // rojizo
            new() { Char = 'D', Type = BiomeType.Desert, Color = Color(231, 164, 84) }, // amarillo mas oscuro
            new() { Char = 'w', Type = BiomeType.Plains, Color = Color(96, 163, 24) }, // verde claro
            new() { Char = '!', Type = BiomeType.Forest, Color = Color(59, 134, 50) }, // verde oscuro
            new() { Char = '*', Type = BiomeType.Dirty, Color = Color(83, 62, 26) }, // marron
            new() { Char = 'b', Type = BiomeType.Beach, Color = Color(255, 215, 104) },
            new() { Char = '.', Type = BiomeType.Sea, Color = Color(97, 187, 221) },
            new() { Char = 'o', Type = BiomeType.Ocean, Color = Color(90, 150, 198) },
        }.ForEach(biome => Biomes.Add(biome.Type, biome));

        return;

        Color Color(int r, int g, int b) {
            return new Color(r / 255f, g / 255f, b / 255f);
        }
    }

    public void Configure(int width, int height, int seed) {
        Width = width;
        Height = height;
        Seed = seed;

        ConfigureBiomeMap(BiomeConfig);

        BiomeCells = new BiomeCell[height, width];

        MassLands = new DataGrid(width, height, 0f);
        MasslandBias = 0.35f;
        MasslandOffset = 0.99f;
        MasslandFallofStart = 0.05f;
        MasslandFalloffEnd = 0.8f;
        const float seaLevel = 0.15f; // The higher, the less lakes inside the massland. The lower, more lakes and (maybe) there is no deep ocean around the massland
        RampFunc = (float h, float f) => (((h + 0.5f) / 2f) + seaLevel) * f;
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
        HumidityNoise.Frequency = 0.015f;
        HumidityNoise.FractalTypeValue = FastNoiseLite.FractalType.FBm;
        HumidityNoise.FractalOctaves = 5;
        HumidityNoise.FractalLacunarity = 2;
        HumidityNoise.FractalGain = 0.5f;
        HumidityNoise.FractalWeightedStrength = 0f;
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

    public void AddIsland(DataGrid Data, int cx, int cy, int rx, int ry, OverlapType overlap, IInterpolation? easing = null, bool up = true) {
        Draw.GradientEllipse(cx, cy, rx, ry, (x, y, value) => {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return;
            var heightValue = value;

            if (up) {
                if (overlap == OverlapType.Simple) {
                    Data.Data[x, y] += heightValue;
                } else if (overlap == OverlapType.MaxHeight) {
                    Data.Data[x, y] = Math.Max(Data.Data[x, y], heightValue);
                }
            } else {
                if (overlap == OverlapType.Simple) {
                    Data.Data[x, y] -= heightValue;
                } else if (overlap == OverlapType.MaxHeight) {
                    Data.Data[x, y] = Math.Min(Data.Data[x, y], -heightValue);
                }
            }
        }, easing);
    }

    private int gridWidth = 3;
    private int gridHeight = 3;
    private Random random;

    public void GenerateIslandsGrid(bool up, OverlapType overlap) {
        var cellWidth = Width / gridWidth;
        var borderWidth = cellWidth / 4;
        var cellHeight = Height / gridHeight;
        var borderHeight = cellHeight / 4;

        for (var i = 0; i < gridWidth; i++) {
            for (var j = 0; j < gridHeight; j++) {
                var cellXStart = i * cellWidth;
                var cellYStart = j * cellHeight;
                // Generate a random position within the cell for the circle's center
                var cx = random.Next(cellXStart + borderWidth, cellXStart + cellWidth - borderWidth);
                var cy = random.Next(cellYStart + borderWidth, cellYStart + cellHeight - borderHeight);

                // Calculate the maximum possible radius without going out of the grid
                var maxRadius = Math.Min(Math.Min(cx, Width - cx), Math.Min(cy, Height - cy));
                var rx = maxRadius * random.Range(0.8f, 1f);
                var ry = rx * random.Range(0.35f, 0.85f);
                AddIsland(MassLands, cx, cy, (int)rx, (int)ry, overlap, new BiasGainInterpolation(MasslandBias, MasslandOffset), up);
            }
        }
    }

    public int islands = 10;

    public void GenerateIslands(bool up, OverlapType overlap) {
        Random random = new Random(HeightNoise.Seed);

        // Calculate the radius of the first island
        float rx = Width / 6f;

        var spatialGrid = new SpatialGrid(rx / 2);

        for (int i = 0; i < islands; i++) {
            bool validPosition = false;
            int attempts = 0;

            while (!validPosition) {
                // Generate a random position within the grid for the circle's center
                int cx = random.Range((int)rx, Width - (int)rx);
                int cy = random.Range((int)rx, Height - (int)rx);

                // Check if the new island intersects with any existing island
                if (!spatialGrid.IntersectCircle(cx, cy, rx) || attempts > 10) {
                    // If it does not intersect, add the island to the MassLands grid and to the spatial grid
                    var ry = rx * random.Range(0.75f, 1f);
                    AddIsland(MassLands, cx, cy, (int)rx, (int)ry, overlap, new BiasGainInterpolation(MasslandBias, MasslandOffset), up);
                    spatialGrid.Add(new Circle { Position = new Vector2(cx, cy), Radius = rx });
                    validPosition = true;
                }
                attempts++;
            }
            // Reduce the radius for the next island
            rx *= 0.8f;
        }
    }

    public void Generate() {
        random = new Random(HeightNoise.Seed);
        var sa = Stopwatch.StartNew();
        var s = Stopwatch.StartNew();
        var overlapType = OverlapType.MaxHeight;
        var easing = Interpolation.Shift(new BiasGainInterpolation(MasslandBias, MasslandOffset), MasslandFallofStart, MasslandFalloffEnd);
        // falloff = new InterpolationRect2D(Width, Height, Interpolation.Shift(new BiasGainInterpolation(MasslandBias, MasslandOffset), 0.05f, MasslandOffset));
        // falloff = new InterpolationCircle(Height / 2, easing);
        Falloff = new InterpolationEllipse(Width / 2f, Height / 2f, easing);
        MassLands.Fill(0);
        MassLands.Load((x,y) => Falloff.Get(x, y));

        GenerateIslandsGrid(true, OverlapType.MaxHeight);
        // GenerateIslands(true, OverlapType.MaxHeight);
        // GenerateIslandsGrid(true, OverlapType.Simple);
        // GenerateIslands();
        // AddIsland(MassLands, 100, 50, 90, 80, overlapType, transitionType);
        // AddIsland(MassLands, 200, 100, 90, 80, overlapType, transitionType);
        // AddIsland(MassLands, 300, 250, 90, 80, overlapType, transitionType);
        // AddIsland(MassLands, 400, 320, 90, 80, overlapType, transitionType);
        // AddIsland(MassLands, Width/2, Height/2, Width/3, Height/3, overlapType, transitionType);
        // AddIsland(MassLands, Width/6 * 2, Height/4 * 3, Width/8, Height/6, overlapType, transitionType);
        // AddIsland(MassLands, Width/6 * 4, Height/2 * 1, Width/8, Height/6, overlapType, transitionType);
        Console.WriteLine($"Generate1 masslands:{s.ElapsedMilliseconds}ms");
        s.Restart();

        HeightFalloffGrid.Load((x, y) => {
            if (FalloffEnabled) {
                var height = HeightNoise.GetNoise(x, y);
                var r = MassLands.GetValue(x, y); // from 0 to 1
                return RampFunc(height, r);
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
        new DataGrid(Width, Height, (x, y) => MassLands.GetValue(x, y)).Normalize().Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }

    public void FillHeight(FastImage fastTexture) {
        HeightNoise.CreateDataGrid(Width, Height, true).Normalize().Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }

    public void FillFalloffGrid(FastImage fastTexture) {
        var dataGrid = new DataGrid(Width, Height, (x, y) => {
            var height = HeightNoise.GetNoise(x, y);
            var r = MassLands.GetValue(x, y); // from 0 to 1
            return RampFunc(height, r);
        });
        dataGrid.Normalize();
        dataGrid.Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }

    public void FillHumidityNoise(FastImage fastTexture) {
        HumidityNormalizedGrid.Loop((val, x, y) => fastTexture.SetPixel(x, y, new Color(val, val, val), false));
        fastTexture.Flush();
    }

    private int column = 400 / 2;
    public void GraphFalloff(TextureRect textureRect) {
        var f = new FastTexture(textureRect, Width, Height/4);
        f.Fill(Colors.DarkBlue);
        for (var x = 0; x < f.Width; x++) {
            var y = 1f - MassLands.GetValue((int)(x * ((float)Width/f.Width)), column);
            f.SetPixel(x, (int)(y * f.Height), Colors.White);
        }
        f.Flush();
    }

    public void GraphHeight(TextureRect textureRect) {
        var f = new FastTexture(textureRect, Width, Height/4);
        f.Fill(Colors.DarkBlue);
        for (var x = 0; x < f.Width; x++) {
            var height = 1f - HeightFalloffGrid.GetValue((int)Mathf.Round(x * ((float)Width/f.Width)), column);
            f.SetPixel(x, (int)(height * f.Height), Colors.White);
        }
        f.Flush();
    }
}