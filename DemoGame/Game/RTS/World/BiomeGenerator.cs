using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core;
using Betauer.Core.Data;
using Betauer.Core.Easing;
using Betauer.Core.Image;
using Betauer.TileSet.TileMap;
using Godot;
using FastNoiseLite = Betauer.Core.Data.FastNoiseLite;

namespace Veronenger.Game.RTS.World;

public enum BiomeType {
    None = -1,
    Glacier,
    Rock,

    FireDesert,
    Desert,
    Plains,
    Forest,
    Dirty,

    Beach,
    Sea,
    Ocean,
}

public class BiomeGenerator {
    public string LandBiomesConfig;
    public string SeaBiomesConfig;

    public readonly Dictionary<BiomeType, Biome<BiomeType>> Biomes = new();

    public int Width { get; private set; }
    public int Height { get; private set; }

    // Massland is not normalized
    public float MasslandBias { get; set; }
    public float MasslandOffset { get; set; }
    public Func<float, float, float> RampFunc;
    public DataGrid MassLands { get; private set; }
    public float SeaLevel { get; set; }

    // This is the noise height, creating mountains and valleys
    public FastNoiseLite HeightNoise { get; } = new();

    // Final grid with the height + optionally applied the falloff
    public bool FalloffEnabled { get; set; } = true;
    public bool HumidityEnabled { get; set; } = true;
    public DataGrid HeightFalloffGrid { get; private set; }

    public FastNoiseLite HumidityNoise { get; } = new();
    public DataGrid HumidityNormalizedGrid { get; private set; }

    public FloatGrid<BiomeType> LandBiomesGrid { get; private set; }
    public FloatGrid<BiomeType> SeaBiomesGrid { get; private set; }
    public BiomeCell[,] BiomeCells { get; private set; }

    private Random _random;

    public int Seed {
        set {
            HeightNoise.Seed = value;
            HumidityNoise.Seed = value * 137712;
        }
    }

    public BiomeGenerator() {
        LandBiomesConfig = """
                      :GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGGGGGGGGGrrGGGGGGGGGGGGGGGGGGGGGGGG:
                      :GGGGGGGGGGGGGGGGrrGGGGGGGGGGGGGGGGGGGGGGGG:
                      :rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr:
                      :rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr:
                      :rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr:
                      :FFrrFFrrFFFFrrwwrr!!!!!!!!!!rrrrrrrrrrrrrr:
                      :FFrrFFrrFFFFrrwwrr!!!!!!!!!!rrrrrrrrrrrrrr:
                      :FFrrFFrrFFFFrrwwrr!!!!!!!!!!rrrrrrrrrrrrrr:
                      :FFFFFFFFFFFFFFwwww!!!!!!!!!!**************:
                      :FFFFFFFFFFFFFFwwww!!!!!!!!!!**************:
                      :FFFFFFFFFFFFFFwwww!!!!!!!!!!**************:
                      :FFFFFFFFFFFFFFwwww!!!!!!!!!!**************:
                      :FFFFFFFFFFFFFFwwww!!!!!!!!!!**************:
                      :FFFFFFFFFFFFFFw!ww!!!w!!!!!!**************:
                      :FFFFFFFFFFFFFFw!ww!!!w!!!!!!**************:
                      :FFFFFFFFFFFFFFw!ww!!!w!!!!!!**************:
                      :FFFFFFFFFFFFFFwwww!!!!!!!!!!******!!***!**:
                      :FFFFFFFFFFFFFFwwww!!!!!!!!!!******!!***!**:
                      :DDDDDDDDDDDDDDDDDD!!!!!!!!!!******!!******:
                      :DDDDDDDDDDDDwwwwww!!!!!!!!!!**************:
                      :DDDDDDDDDDDDwwwwww!!!!!!**!!**************:
                      :DDDDDDDDDDDDwbwwww!!!!!!**!!**************:
                      :bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb:
                      """;
        
        SeaBiomesConfig = """
                      :..........................................:
                      :..........................................:
                      :oooooooooooooooo..........................:
                      :oooooooooooooooooooooooooooo....oooooooooo:
                      :oooooooooooooooooooooooooooooo..oooooooooo:
                      :oooooooooo..oooooooooooooooooooooooooooooo:
                      """;
   
        new Biome<BiomeType>[] {
            new() { Char = 'G', Type = BiomeType.Glacier, Color = Colors.White },
            new() { Char = 'r', Type = BiomeType.Rock, Color = Color(112, 112, 110) }, // gris
            new() { Char = 'F', Type = BiomeType.FireDesert, Color = Color(186, 80, 43) }, // rojizo
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

        ConfigureLandBiomeMap(LandBiomesConfig);
        ConfigureSeaBiomeMap(SeaBiomesConfig);

        BiomeCells = new BiomeCell[height, width];

        MassLands = new DataGrid(width, height, 0f);
        MasslandBias = 0.35f;
        MasslandOffset = 0.99f;
        SeaLevel = 0.15f;
        RampFunc = (float h, float f) => (((h + 0.5f) / 2f) + 0.15f) * f;
        HumidityNormalizedGrid = new DataGrid(width, height, 0f);
        HeightFalloffGrid = new DataGrid(width, height, 0f);

        HeightNoise.NoiseTypeValue = FastNoiseLite.NoiseType.OpenSimplex2S;
        HeightNoise.Frequency = 0.012f;
        HeightNoise.FractalTypeValue = FastNoiseLite.FractalType.FBm;
        HeightNoise.FractalOctaves = 5;
        HeightNoise.FractalLacunarity = 2;
        HeightNoise.FractalGain = 0.5f;
        HeightNoise.FractalWeightedStrength = 0f;

        HumidityNoise.NoiseTypeValue = FastNoiseLite.NoiseType.OpenSimplex2S;
        HumidityNoise.Frequency = 0.003f;
        HumidityNoise.FractalTypeValue = FastNoiseLite.FractalType.FBm;
        HumidityNoise.FractalOctaves = 5;
        HumidityNoise.FractalLacunarity = 2;
        HumidityNoise.FractalGain = 0.5f;
        HumidityNoise.FractalWeightedStrength = 0f;
    }

    public void ConfigureLandBiomeMap(string biomeMap) {
        var charMapping = Biomes.ToDictionary(pair => pair.Value.Char, pair => pair.Value.Type);
        LandBiomesGrid = FloatGrid<BiomeType>.Parse(biomeMap, charMapping);
        LandBiomesConfig = biomeMap;
    }

    public void ConfigureSeaBiomeMap(string biomeMap) {
        var charMapping = Biomes.ToDictionary(pair => pair.Value.Char, pair => pair.Value.Type);
        SeaBiomesGrid = FloatGrid<BiomeType>.Parse(biomeMap, charMapping);
        SeaBiomesConfig = biomeMap;
    }

    public Biome<BiomeType> FindBiome(float humidity, float terrainHeight) {
        // terrainHeight is a value from 0 to 1f. If biomeSeaLevel is 0.15f, then:
        // 0.15 -    1f is land
        // 0    - 0.15f is ocean
        if (terrainHeight > SeaLevel) {
            // Land
            var landHeight = (terrainHeight - SeaLevel) / (1 - SeaLevel);
            var biomeType = LandBiomesGrid.Get(humidity, 1 - landHeight); // (1-height) because the highest values (like the glacier) are located in 0 pos
            return Biomes[biomeType];
        } else {
            // Sea
            var seaHeight = terrainHeight / SeaLevel; // transform the value from 0-0.15f in a 0..1f value
            var biomeType = SeaBiomesGrid.Get(humidity, 1 - seaHeight); // (1-height) because the highest values (like the glacier) are located in 0 pos
            return Biomes[biomeType];
        }
    }

    public enum OverlapType {
        Simple,
        MaxHeight,
    }

    public void Generate() {
        _random = new Random(HeightNoise.Seed);
        var sa = Stopwatch.StartNew();
        var s = Stopwatch.StartNew();
        MassLands.Fill(0);
        GenerateIslandsGrid(true, OverlapType.MaxHeight);
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
                var terrainHeight = HeightFalloffGrid.GetValue(x, y);
                var humidity = HumidityEnabled ? HumidityNormalizedGrid.GetValue(x, y) : 0f;
                var temp = CalculateTemperature(y, Height, terrainHeight);
                var biome = FindBiome(humidity, terrainHeight); // 1f - temp);
                BiomeCell.SeaLevel = SeaLevel;
                BiomeCell biomeCell = new BiomeCell {
                    Height = terrainHeight,
                    Humidity = humidity,
                    Temp = temp,
                    Biome = biome
                };
                // Console.Write(cell.Height.ToString("0.0")+ " | ");
                BiomeCells[x, y] = biomeCell;
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

    private int gridWidth = 3;
    private int gridHeight = 3;

    // Generate 9 islands in 3x3 cells
    public void GenerateIslandsGrid(bool up, OverlapType overlap) {
        var cellWidth = Width / gridWidth;
        var borderWidth = cellWidth / 4;
        var cellHeight = Height / gridHeight;
        var borderHeight = cellHeight / 4;

        for (var i = 0; i < gridWidth; i++) {
            for (var j = 0; j < gridHeight; j++) {
                // Generate a random position within the cell for the circle's center
                var cellXStart = i * cellWidth;
                var offset = 0;
                if (i == 0) {
                    offset = borderWidth;
                } else if (i == gridWidth - 1) {
                    offset = -borderWidth;
                }
                var cx = _random.Next(cellXStart + offset, cellXStart + cellWidth + offset);

                var cellYStart = j * cellHeight;
                if (j == 0) {
                    offset = borderHeight;
                } else if (j == gridHeight - 1) {
                    offset = -borderHeight;
                } else {
                    offset = 0;
                }
                var cy = _random.Next(cellYStart + offset, cellYStart + cellHeight + offset);

                // Calculate the maximum possible radius without going out of the grid
                var maxRadius = Math.Min(Math.Min(cx, Width - cx), Math.Min(cy, Height - cy));
                var rx = maxRadius;
                var ry = rx * _random.Range(0.6f, 0.9f);
                var rotation = _random.Range(0, Mathf.Pi * 2);
                AddIsland(MassLands, cx, cy, rx, (int)ry, rotation, overlap, new BiasGainInterpolation(MasslandBias, MasslandOffset), up);
            }
        }
    }

    public void AddIsland(DataGrid Data, int cx, int cy, int rx, int ry, float rotation, OverlapType overlap, IInterpolation? easing = null, bool up = true) {
        Draw.GradientEllipseRotated(cx, cy, rx, ry, rotation, (x, y, value) => {
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

    private float CalculateTemperature(int y, int height, float heightNormalized) {
        var equatorHeat = 0.2f; // más calor en el ecuador
        var positionFactor = 1 - Math.Abs(y - height / 2f) / (height / 2f); // de 0 en los polos a 1 en el ecuador
        // La temperatura disminuye con la altitud
        var heightFactor = 1 - heightNormalized;
        return positionFactor * equatorHeat + heightFactor * (1f - equatorHeat);
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

    public void FillTemperature(FastImage fastTexture) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var val = BiomeCells[x, y].Temp;
                fastTexture.SetPixel(x, y, new Color(val, val, val), false);
            }
        }
        fastTexture.Flush();
    }

    public void FillTerrain(FastImage fastTexture) {
        for (var y = 0; y < Height; y++) {
            for (var x = 0; x < Width; x++) {
                var biome = BiomeCells[x, y].Biome;
                fastTexture.SetPixel(x, y, biome.Color, false);
            }
        }
        fastTexture.Flush();
    }

    public void GraphFalloff(TextureRect textureRect) {
        var column = Height / 2;
        var f = new FastTexture(textureRect, Width, Height / 4);
        f.Fill(Colors.DarkBlue);
        var ratio = (float)Width / f.Width;
        for (var x = 0; x < f.Width; x++) {
            var y = 1f - MassLands.GetValue((int)(x * ratio), column);
            f.SetPixel(x, (int)(y * f.Height), Colors.White);
        }
        f.Flush();
    }

    public void GraphHeight(TextureRect textureRect) {
        var column = Height / 2;
        var f = new FastTexture(textureRect, Width, Height / 4);
        f.Fill(Colors.DarkBlue);
        var ratio = (float)Width / f.Width;
        for (var x = 0; x < f.Width; x++) {
            var height = 1f - HeightFalloffGrid.GetValue(Mathf.RoundToInt(x * ratio), column);
            f.SetPixel(x, (int)(height * f.Height), Colors.White);
        }
        f.Flush();
    }
}

public class RiverGenerator {
    public List<Vector2> FindRiverStartPoints(BiomeCell[,] heightMap, int numberOfPoints, float minDistance) {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        List<Vector2> startPoints = new List<Vector2>();

        // Lista para almacenar puntos con sus alturas
        List<KeyValuePair<Vector2, float>> pointsWithHeight = new List<KeyValuePair<Vector2, float>>();

        // Recorrer el mapa de alturas y almacenar cada punto con su altura
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                pointsWithHeight.Add(new KeyValuePair<Vector2, float>(new Vector2(x, y), heightMap[x, y].Height));
            }
        }

        // Ordenar los puntos por altura, de mayor a menor
        pointsWithHeight.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

        // Seleccionar puntos de inicio asegurando una distancia mínima entre ellos
        foreach (var point in pointsWithHeight) {
            if (startPoints.Count < numberOfPoints) {
                bool isFarEnough = true;

                foreach (var startPoint in startPoints) {
                    if (startPoint.DistanceTo(point.Key) < minDistance) {
                        isFarEnough = false;
                        break;
                    }
                }

                if (isFarEnough) {
                    startPoints.Add(point.Key);
                }
            } else {
                break;
            }
        }

        return startPoints;
    }
}