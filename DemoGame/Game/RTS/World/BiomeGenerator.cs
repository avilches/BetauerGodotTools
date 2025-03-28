using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Betauer.Core;
using Betauer.Core.DataMath;
using Betauer.Core.DataMath.Data;
using Betauer.Core.Easing;
using Betauer.Core.Image;
using Betauer.Core.PCG;
using Betauer.Core.PCG.Graph;
using Betauer.Core.PCG.GridMatching;
using Betauer.Core.PCG.PoissonDiskSampling;
using Godot;
using FastNoiseLite = Betauer.Core.PCG.FastNoiseLite;

namespace Veronenger.Game.RTS.World;

public enum BiomeType {
    None = -1,
    Glacier = 1,
    Rock = 2,

    FireDesert = 3,
    Desert = 4,
    Plains = 5,
    Forest = 6,
    Dirty = 7,

    Beach = 8,
    Sea = 9,
    Ocean = 10,
}

public class BiomeGenerator {
    public string LandBiomesConfig;
    public string SeaBiomesConfig;

    public readonly Dictionary<BiomeType, Biome> Biomes = new();

    public int Width { get; private set; }
    public int Height { get; private set; }

    // Massland is not normalized
    public int LandWidthCount { get; set; }
    public int LandHeightCount { get; set; }
    public float MasslandBias { get; set; }
    public float MasslandOffset { get; set; }
    // public float HeightBias { get; set; }
    public Func<float, float, float> RampFunc;
    public Array2D<float> MassLands { get; private set; }
    public float SeaLevel { get; set; }

    // This is the noise height, creating mountains and valleys
    public FastNoiseLite HeightNoise { get; } = new();

    // Final grid with the height + optionally applied the falloff
    public bool FalloffEnabled { get; set; } = true;
    public bool HumidityEnabled { get; set; } = true;
    public Array2D<float> HeightFalloffGrid { get; private set; }

    public FastNoiseLite HumidityNoise { get; } = new();
    public Array2D<float> HumidityNormalizedGrid { get; private set; }

    public FloatGrid<BiomeType> LandBiomesGrid { get; private set; }
    public FloatGrid<BiomeType> SeaBiomesGrid { get; private set; }
    public Array2D<BiomeCell> BiomeCells { get; private set; }
    // public Betauer.TileSet.TileMap.TileMap<BiomeType> TileMap { get; private set; }

    private Random _random;

    public int Seed {
        set {
            HeightNoise.Seed = value;
            HumidityNoise.Seed = value * 137712;
        }
    }

    public BiomeGenerator() {
        LandBiomesConfig = """
                           GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
                           GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
                           GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
                           GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
                           GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
                           GGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGGG
                           GGGGGGGGGGGGGGGGrrGGGGGGGGGGGGGGGGGGGGGGGG
                           GGGGGGGGGGGGGGGGrrGGGGGGGGGGGGGGGGGGGGGGGG
                           rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
                           rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
                           rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr
                           FFrrFFrrFFFFrrwwrr!!!!!!!!!!rrrrrrrrrrrrrr
                           FFrrFFrrFFFFrrwwrr!!!!!!!!!!rrrrrrrrrrrrrr
                           FFrrFFrrFFFFrrwwrr!!!!!!!!!!rrrrrrrrrrrrrr
                           FFFFFFFFFFFFFFwwww!!!!!!!!!!**************
                           FFFFFFFFFFFFFFwwww!!!!!!!!!!**************
                           FFFFFFFFFFFFFFwwww!!!!!!!!!!**************
                           FFFFFFFFFFFFFFwwww!!!!!!!!!!**************
                           FFFFFFFFFFFFFFwwww!!!!!!!!!!**************
                           FFFFFFFFFFFFFFw!ww!!!w!!!!!!**************
                           FFFFFFFFFFFFFFw!ww!!!w!!!!!!**************
                           FFFFFFFFFFFFFFw!ww!!!w!!!!!!**************
                           FFFFFFFFFFFFFFwwww!!!!!!!!!!******!!***!**
                           FFFFFFFFFFFFFFwwww!!!!!!!!!!******!!***!**
                           DDDDDDDDDDDDDDDDDD!!!!!!!!!!******!!******
                           DDDDDDDDDDDDwwwwww!!!!!!!!!!**************
                           DDDDDDDDDDDDwwwwww!!!!!!**!!**************
                           DDDDDDDDDDDDwbwwww!!!!!!**!!**************
                           bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb
                           """;

        SeaBiomesConfig = """
                          ..........................................
                          ..........................................
                          oooooooooooooooo..........................
                          oooooooooooooooooooooooooooo....oooooooooo
                          oooooooooooooooooooooooooooooo..oooooooooo
                          oooooooooo..oooooooooooooooooooooooooooooo
                          """;

        new Biome[] {
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

        BiomeCells = new Array2D<BiomeCell>(Width, Height);
        // TileMap = new TileMap<BiomeType>(0, width, height, BiomeType.None);

        MassLands = new Array2D<float>(width, height);
        MasslandBias = 0.35f;
        MasslandOffset = 0.99f;
        LandWidthCount = 8;
        LandHeightCount = 3;
        SeaLevel = 0.15f; // 0.06
        // HeightBias = 0.5f; // 0.3f
        RampFunc = (float h, float f) => ((h / 2f) + 0.5f) * f;
        HumidityNormalizedGrid = new Array2D<float>(width, height);
        HeightFalloffGrid = new Array2D<float>(width, height);

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

    public Biome FindBiome(float humidity, float terrainHeight) {
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

    public void Generate() {
        _random = new Random(HeightNoise.Seed);
        var sa = Stopwatch.StartNew();
        var s = Stopwatch.StartNew();
        MassLands.Fill(0);
        IslandGenerator.GenerateIslandsGrid(MassLands, LandWidthCount, LandHeightCount, _random, true, IslandGenerator.OverlapType.MaxHeight, Interpolation.Gain(MasslandBias, MasslandOffset));
        Console.WriteLine($"Generate1 masslands:{s.ElapsedMilliseconds}ms");
        s.Restart();

        HeightFalloffGrid.LoadNormalized((x, y) => {
            if (FalloffEnabled) {
                var height = HeightNoise.GetNoise(x, y);
                var r = MassLands[y, x]; // from 0 to 1
                return RampFunc(height, r);
            } else {
                return HeightNoise.GetNoise(x, y);
            }
        });
        // HeightFalloffGrid.Transform(val=> Functions.Bias(val, HeightBias));
        Console.WriteLine($"Generate2 normalize massland:{s.ElapsedMilliseconds}ms");
        s.Restart();

        HumidityNormalizedGrid.LoadNormalized((x, y) => HumidityNoise.GetNoise(x, y));
        Console.WriteLine($"Generate4:{s.ElapsedMilliseconds}ms");
        s.Restart();

        var list = new List<BiomeCell>();
        foreach (var (position, value) in BiomeCells.GetIndexedValues()) {
                var terrainHeight = HeightFalloffGrid[position];
                var humidity = HumidityEnabled ? HumidityNormalizedGrid[position] : 0f;
                var temp = CalculateTemperature(position.Y, Height, terrainHeight);
                var biome = FindBiome(humidity, terrainHeight); // 1f - temp);
                BiomeCell.SeaLevel = SeaLevel;
                BiomeCell biomeCell = new BiomeCell {
                    Position = position,
                    Height = terrainHeight,
                    Humidity = humidity,
                    Temp = temp,
                    Biome = biome
                };
                // Console.Write(cell.Height.ToString("0.0")+ " | ");
                BiomeCells[position] = biomeCell;
                // TileMap.SetTerrain(x, y, biome.Type);
                list.Add(biomeCell);
        }
        
        Console.WriteLine($"Generate5:{s.ElapsedMilliseconds}ms");
        s.Restart();
        // Draw the coast line
        var gridSize = 3;
        var centralPos = 1;
        var buffer = new BiomeCell[gridSize, gridSize]; 
        foreach (var ((x, y), cell) in BiomeCells.GetIndexedValues()) {
            BiomeCells.CopyChebyshevRegion(x, y, buffer);

            // If central pixel is not land, it can't be coast
            if (!buffer[centralPos, centralPos].Land) continue;
            
            foreach (var ss in buffer) {
                if (ss == null) continue;
                if (ss.Sea) {
                    cell.Coast = true;
                    break;
                }
            }
            for (var i = 0; i < gridSize; i++) {
                for (var j = 0; j < gridSize; j++) {
                    if (i == centralPos && j == centralPos) continue;
                    // If there is at least one neighbour which is water, then the central pixel is coast
                    if (buffer[i, j].Sea) {
                        cell.Coast = true;
                        break;
                    }
                }
            }
        }
        Console.WriteLine($"Generate5 (coast):{s.ElapsedMilliseconds}ms");
        s.Restart();
        GeneratePoissonPoints();
        // RiverGenerator.GenerateRivers(BiomeCells, 30, 100, _random);
        // RiverGeneratorPoint.GenerateRivers(PoissonPoints, BiomeCells, 30, 100, _random);

        var graph = new Graph<BiomeCell>();
        foreach(var e in delaunator.GetVoronoiEdges(Delaunator.VoronoiType.Centroid)) {
            var from = BiomeCells[(int)e.P.Y, (int)e.P.X];
            var to = BiomeCells[(int)e.Q.Y, (int)e.Q.X];
            graph.Connect(from, to);
            graph.Connect(to, from);
        }

        RiverGeneratorGraph.GenerateRivers(graph, BiomeCells, 30, 100, _random);

        Console.WriteLine($"Generate6:{s.ElapsedMilliseconds}ms");
        s.Restart();

        // var dict = list.Select(c => c.Biome.Type).GroupBy(x => x)
            // .ToDictionary(g => g.Key, g => g.Count());
        // dict.ForEach(pair => Console.WriteLine(pair.Key + ": " + pair.Value));
        Console.WriteLine($"Generate Total:{sa.ElapsedMilliseconds}ms");
    }

    public List<BiomeCell> PoissonPoints { get; private set; }

    int radius = 10;
    private Delaunator delaunator;
    private void GeneratePoissonPoints() {
        var uni = new UniformPoissonSampler2D(Width, Height);
        PoissonPoints = uni.Generate(radius, _random).Select(p => BiomeCells[(int)p.Y, (int)p.X]).ToList();
        delaunator = new Delaunator(PoissonPoints.Select(p => (Vector2)p.Position).ToArray());
    }

    private float CalculateTemperature(int y, int height, float heightNormalized) {
        var equatorHeat = 0.2f; // más calor en el ecuador
        var positionFactor = 1 - Math.Abs(y - height / 2f) / (height / 2f); // de 0 en los polos a 1 en el ecuador
        // La temperatura disminuye con la altitud
        var heightFactor = 1 - heightNormalized;
        return positionFactor * equatorHeat + heightFactor * (1f - equatorHeat);
    }

    public void FillMassland(FastImage fastTexture) {
        foreach (var ((x, y), val) in new Array2D<float>(Width, Height).LoadNormalized((x, y) => MassLands[y, x]).GetIndexedValues()) {
            fastTexture.SetPixel(x, y, new Color(val, val, val), false);
        }
        fastTexture.Flush();
    }

    public void FillPoisson(FastImage fastImage) {
        foreach(var v in PoissonPoints) {
            fastImage.SetPixel(v.Position.X, v.Position.Y, Colors.Black);
        }
        fastImage.Flush();
    }

    public void FillDelaunatorEdgesBasedOnCentroids(FastImage fastImage) {
        foreach(var e in delaunator.GetVoronoiEdges(Delaunator.VoronoiType.Centroid)) {
            fastImage.DrawLineAntialiasing((Vector2I)e.P, (Vector2I)e.Q, 1, Colors.Blue);
        }
        foreach(var v in PoissonPoints) {
            fastImage.SetPixel(v.Position.X, v.Position.Y, Colors.Black);
        }
        fastImage.Flush();
    }

    public void FillDelaunatorPointsBasedOnCentroids(FastImage fastImage) {
        foreach(var e in delaunator.GetVoronoiVertices(Delaunator.VoronoiType.Centroid)) {
            fastImage.SetPixel((Vector2I)e, Colors.Black);
        }
        fastImage.Flush();
    }

    public void FillDelaunatorEdgesBasedOnCircumCenter(FastImage fastImage) {
        foreach(var e in delaunator.GetVoronoiEdges(Delaunator.VoronoiType.Circumcenter)) {
            fastImage.DrawLineAntialiasing((Vector2I)e.P, (Vector2I)e.Q, 1, Colors.Blue);
        }
        foreach(var v in PoissonPoints) {
            fastImage.SetPixel(v.Position.X, v.Position.Y, Colors.Black);
        }
        fastImage.Flush();
    }

    public void FillDelaunatorTriangles(FastImage fastImage) {
        foreach(var e in delaunator.GetTriangles()) {
            fastImage.DrawLineAntialiasing((Vector2I)e.Points[0],(Vector2I)e.Points[1], 1, Colors.Red);
            fastImage.DrawLineAntialiasing((Vector2I)e.Points[1],(Vector2I)e.Points[2], 1, Colors.Red);
            fastImage.DrawLineAntialiasing((Vector2I)e.Points[2],(Vector2I)e.Points[0], 1, Colors.Red);
        }
        foreach(var v in PoissonPoints) {
            fastImage.SetPixel(v.Position.X, v.Position.Y, Colors.Black);
        }
        fastImage.Flush();
    }

    private int scale = 5;
    public void FillDelaunatorRandomVoronoiEdge(FastImage fastImage) {
        var point = delaunator.Points[_random.Next(delaunator.Points.GetLength(0))];
        fastImage.DrawCircle((Vector2I)point, 3, Colors.Black);

        var e = delaunator.FindVoronoiCell(point, Delaunator.VoronoiType.Centroid);
        if (e != null) {
            foreach(var p2 in Delaunator.CreateHull(e.Value.Points)) {
                fastImage.DrawLine((Vector2I)p2.P, (Vector2I)p2.Q, 1, Colors.Blue);
            }
        }
        foreach(var v in PoissonPoints) {
            fastImage.SetPixel(v.Position.X, v.Position.Y, Colors.Black);
        }
        fastImage.Flush();
    }

    public void FillDelaunatorVoronoiPath(FastImage fastImage) {
        var _random = new Random(1);
        var poissonPoint = delaunator.Points[_random.Next(delaunator.Points.GetLength(0))];
        fastImage.DrawCircle((Vector2I)poissonPoint, 3, Colors.Black);
        var edgePoint = (Vector2I)delaunator.FindVoronoiCell(poissonPoint, Delaunator.VoronoiType.Centroid).Value.Points[0];

        var set = new HashSet<Vector2I> { edgePoint };
        var graph = new Graph<Vector2I>();
        foreach(var e in delaunator.GetVoronoiEdges(Delaunator.VoronoiType.Centroid)) {
            graph.Connect((Vector2I)e.P, (Vector2I)e.Q);
            graph.Connect((Vector2I)e.Q, (Vector2I)e.P);
        }

        var candidates = graph.GetConnections(edgePoint);
        while (candidates.Count > 0) {
            var next = candidates[_random.Next(candidates.Count)];
            fastImage.DrawLineNoise((Vector2I)edgePoint, (Vector2I)next, HeightNoise, scale, Colors.Blue);
            candidates = graph.GetConnections(next);
            candidates.RemoveAll((p) => set.Contains(p));
            set.Add(next);
            edgePoint = next;
        }
        fastImage.Flush();
    }
    public void FillHeight(FastImage fastImage) {
        foreach (var ((x, y), val) in new Array2D<float>(Width, Height).LoadNormalized((x, y) => HeightNoise.GetNoise(x, y)).GetIndexedValues()) {
            fastImage.SetPixel(x, y, new Color(val, val, val), false);
        }
        fastImage.Flush();
    }

    public void FillFalloffGrid(FastImage fastImage) {
        var array2d = new Array2D<float>(Width, Height).LoadNormalized((x, y) => {
            var height = HeightNoise.GetNoise(x, y);
            var r = MassLands[y, x]; // from 0 to 1
            return RampFunc(height, r);
        });
        foreach (var ((x, y), val) in array2d.GetIndexedValues()) {
            fastImage.SetPixel(x, y, new Color(val, val, val), false);
        }
        fastImage.Flush();
    }

    public void FillHumidityNoise(FastImage fastImage) {
        foreach (var ((x, y), val) in HumidityNormalizedGrid.GetIndexedValues()) {
            fastImage.SetPixel(x, y, new Color(val, val, val), false);
        }
        fastImage.Flush();
    }

    public void FillTemperature(FastImage fastImage) {
        foreach (var ((x, y), val) in BiomeCells.GetIndexedValues()) {
            fastImage.SetPixel(x, y, new Color(val.Temp, val.Temp, val.Temp), false);
        }
        fastImage.Flush();
    }
    
    public void FindCoast(FastImage fastImage) {
        var landSeaRules = new Dictionary<string, Func<BiomeCell?, bool>> {
            { "s", cell => cell?.Sea ?? false },
            { "L", cell => cell?.Land ?? false },
        };
        var p = GridMatcher.Parse("""
                                  s s s
                                  L L L
                                  L L L
                                  """, landSeaRules);
        var buffer = new BiomeCell[3, 3];
        foreach (var ((x, y), val) in BiomeCells.GetIndexedValues()) {
            BiomeCells.CopyChebyshevRegion(x, y, buffer);
            if (p.Matches(buffer)) {
                fastImage.SetPixel(x, y, Colors.Blue, false);
            }
        }
    }

    public void FillTerrain(FastImage fastImage) {
        foreach (var ((x, y), cell) in BiomeCells.GetIndexedValues()) {
            fastImage.SetPixel(x, y, cell.Color, false);
        }
        fastImage.Flush();
    }

    public void GraphFalloff(TextureRect textureRect) {
        var column = Height / 2;
        var f = new FastTexture().Link(textureRect, Width, Height / 4);
        f.Fill(Colors.DarkBlue);
        var ratio = (float)Width / f.Width;
        for (var x = 0; x < f.Width; x++) {
            var y = 1f - MassLands[column, (int)(x * ratio)];
            f.SetPixel(x, (int)(y * f.Height), Colors.White);
        }
        f.Flush();
    }

    public void GraphHeight(TextureRect textureRect) {
        var column = Height / 2;
        var f = new FastTexture().Link(textureRect, Width, Height / 4);
        f.Fill(Colors.DarkBlue);
        var ratio = (float)Width / f.Width;
        for (var x = 0; x < f.Width; x++) {
            var height = 1f - HeightFalloffGrid[column, Mathf.RoundToInt(x * ratio)];
            f.SetPixel(x, (int)(height * f.Height), Colors.White);
        }
        f.Flush();
    }
}

public class IslandGenerator {
    public enum OverlapType {
        Simple,
        MaxHeight,
    }

    public static void GenerateIslandsGrid(Array2D<float> masslandGrid, int landWidthCount, int landHeightCount, Random random, bool up, OverlapType overlap, IInterpolation easing) {
        var width = masslandGrid.Width;
        var height = masslandGrid.Height;
        var cellWidth = width / landWidthCount;
        var borderWidth = cellWidth / 4;
        var cellHeight = height / landHeightCount;
        var borderHeight = cellHeight / 4;

        for (var i = 0; i < landWidthCount; i++) {
            for (var j = 0; j < landHeightCount; j++) {
                // Generate a random position within the cell for the circle's center
                var cellXStart = i * cellWidth;
                var offset = 0;
                if (i == 0) {
                    offset = borderWidth;
                } else if (i == landWidthCount - 1) {
                    offset = -borderWidth;
                }
                var cx = random.Next(cellXStart + offset, cellXStart + cellWidth + offset);

                var cellYStart = j * cellHeight;
                if (j == 0) {
                    offset = borderHeight;
                } else if (j == landHeightCount - 1) {
                    offset = -borderHeight;
                } else {
                    offset = 0;
                }
                var cy = random.Next(cellYStart + offset, cellYStart + cellHeight + offset);

                // Calculate the maximum possible radius without going out of the grid
                var rx = Math.Min(Math.Min(cx, width - cx), Math.Min(cy, height - cy));
                var ry = rx * random.Range(0.6f, 0.9f);
                var rotation = random.Range(0, Mathf.Pi * 2);
                AddIsland(masslandGrid, cx, cy, rx, (int)ry, rotation, overlap, easing, up);
            }
        }
    }

    public static void AddIsland(Array2D<float> normalizedData, int cx, int cy, int rx, int ry, float rotation, OverlapType overlap, IInterpolation? easing = null, bool up = true) {
        Draw.GradientEllipseRotated(cx, cy, rx, ry, rotation, (x, y, value) => {
            if (x < 0 || y < 0 || x >= normalizedData.Width || y >= normalizedData.Height) return;
            var heightValue = value;

            if (up) {
                if (overlap == OverlapType.Simple) {
                    normalizedData[y, x] += heightValue;
                } else if (overlap == OverlapType.MaxHeight) {
                    normalizedData[y, x] = Math.Max(normalizedData[y, x], heightValue);
                }
            } else {
                if (overlap == OverlapType.Simple) {
                    normalizedData[y, x] -= heightValue;
                } else if (overlap == OverlapType.MaxHeight) {
                    normalizedData[y, x] = Math.Min(normalizedData[y, x], -heightValue);
                }
            }
        }, easing);
    }
}