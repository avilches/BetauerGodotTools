using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.RTS.Assets.Trees;
using Betauer.Core;
using Betauer.Core.DataMath.Collision.Spatial2D;
using Betauer.Core.Image;
using Betauer.Core.PCG.GridMatching;
using Betauer.Core.PCG.PoissonDiskSampling;
using Betauer.TileSet.Image;
using FastNoiseLite = Betauer.Core.PCG.FastNoiseLite;
using TileMap = Godot.TileMap;

namespace Veronenger.Game.RTS.World;

[Singleton]
public partial class WorldGenerator {
    [Inject] public Random Random { get; set; }
    [Inject] public ITransient<Trees> TreesFactory { get; set; }
    [Inject] public SceneTree SceneTree { get; set; }

    public BiomeGenerator BiomeGenerator { get; }

    // public TileMap<BiomeType> TileMap { get; private set; }
    public TileMap GodotTileMap { get; private set; }
    public FastTexture FastFinalMap { get; private set; }
    public Trees TreesInstance;
    private const int CellSize = 16;

    private const int Width = 800;
    private const int Height = 500;

    public enum ViewMode {
        Massland,
        Height,
        HeightFalloff,
        Temperature,
        Humidity,
        Terrain,
    }

    public ViewMode CurrentViewMode { get; set; } = ViewMode.Terrain;

    public int Seed { get; set; } = 0;

    public WorldGenerator() {
        BiomeGenerator = new BiomeGenerator();
        BiomeGenerator.Configure(Width, Height, Seed);
    }

    public void Configure(TileMap godotTileMap, FastTexture fastTexture) {
        GodotTileMap = godotTileMap;

        // TileMap = new TileMap<BiomeType>(Layers, Width, Height);
        FastFinalMap = fastTexture;

        GodotTileMap.Draw += () => {
            // Hack needed to draw on top of the tilemap:
            // https://www.reddit.com/r/godot/comments/w3l48f/does_anyone_know_how_to_set_the_draw_order_when/
            // https://github.com/godotengine/godot/issues/35002 
            GodotTileMap.Visible = false;
            var subViewport = (SubViewport)GodotTileMap.GetViewport();
            subViewport.RenderTargetClearMode = SubViewport.ClearMode.Never;
            // put here your Draw calls, like DrawRect, DrawCircle, etc.
            GodotTileMap.Visible = true;
            subViewport.RenderTargetClearMode = SubViewport.ClearMode.Always;
        };
    }

    public void Generate() {
        BiomeGenerator.Seed = Seed;
        BiomeGenerator.Generate();

        // return;
        GenerateOld(GodotTileMap, BiomeGenerator.HeightNoise, BiomeGenerator.HumidityNoise);
        foreach (var (pos, cell) in BiomeGenerator.BiomeCells.GetIndexedValues()) {
            switch (cell.Biome.Type) {
                case BiomeType.Beach:
                    GodotTileMap.SetCell(0, pos, 7, new Vector2I(18, 4)); // Sand
                    break;
                case BiomeType.None:
                    break;
                case BiomeType.Glacier:
                    GodotTileMap.SetCell(0, pos, 6, new Vector2I(2, 2)); // Modern, snow
                    break;
                case BiomeType.Rock:
                    GodotTileMap.SetCell(0, pos, 1, new Vector2I(0, 0)); // Mud
                    break;
                case BiomeType.FireDesert:
                    break;
                case BiomeType.Desert:
                    GodotTileMap.SetCell(0, pos, 0, new Vector2I(0, 15)); // Red
                    break;
                case BiomeType.Plains:
                    GodotTileMap.SetCell(0, pos, 0, new Vector2I(0, 2)); // GreenYellow 
                    break;
                case BiomeType.Forest:
                    GodotTileMap.SetCell(0, pos, 0, new Vector2I(11, 1)); // 
                    break;
                case BiomeType.Dirty:
                    GodotTileMap.SetCell(0, pos, 0, new Vector2I(16, 2)); // 
                    break;
                case BiomeType.Sea:
                    GodotTileMap.SetCell(0, pos, 6, new Vector2I(20, 13)); // Modern, water
                    break;
                case BiomeType.Ocean:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void Draw(ViewMode viewMode) {
        CurrentViewMode = viewMode;
        ReDraw();
    }

    public void ReDraw() {
        if (CurrentViewMode == ViewMode.Massland) {
            BiomeGenerator.FillMassland(FastFinalMap);
        } else if (CurrentViewMode == ViewMode.Height) {
            BiomeGenerator.FillHeight(FastFinalMap);
        } else if (CurrentViewMode == ViewMode.HeightFalloff) {
            BiomeGenerator.FillFalloffGrid(FastFinalMap);
        } else if (CurrentViewMode == ViewMode.Humidity) {
            BiomeGenerator.FillHumidityNoise(FastFinalMap);
        } else if (CurrentViewMode == ViewMode.Temperature) {
            BiomeGenerator.FillTemperature(FastFinalMap);
        } else if (CurrentViewMode == ViewMode.Terrain) {
            BiomeGenerator.FillTerrain(FastFinalMap);
            /*
            TileMap.Execute((t, x, y) => {
                // canvas.TopLevel = true;
                var biome = BiomeGenerator.BiomeCells[y, x];
                FastFinalMap.SetPixel(x, y, biome.Biome.Color, false);
            });
            */
        }
    }

    public class TileMapSource {
        public int SourceId { get; }
        public ITileSetLayout TileSetLayout { get; }

        public TileMapSource(int sourceId, ITileSetLayout tileSetLayout) {
            SourceId = sourceId;
            TileSetLayout = tileSetLayout;
        }

        public void SetCell(Godot.TileMap godotTileMap, int layer, int x, int y, int tileId) {
            var coords = TileSetLayouts.Blob47Godot.GetAtlasCoordsByTileId(tileId);
            godotTileMap.SetCell(0, new Vector2I(x, y), SourceId, coords);
        }
    }

    public void GenerateOld(TileMap godotTileMap, FastNoiseLite noiseHeight, FastNoiseLite noiseMoisture) {
        godotTileMap.Clear();
        TreesInstance = TreesFactory.Create();
        TreesInstance.Configure();

        Measure("Place objects", () => { PlaceObjects(godotTileMap, noiseMoisture); });

        var tiles = new[] {
            TilePatterns.TerrainGreen,
            TilePatterns.TerrainGreen,
            TilePatterns.ModernDirt,
            TilePatterns.ModernWater,
            TilePatterns.ModernDeepWater
        };

        var sproutDarkerGrass = new TileMapSource(8, TileSetLayouts.Blob47Godot);

        var buffer = new int[3, 3];
        foreach (var ((x, y), cell) in BiomeGenerator.BiomeCells.GetIndexedValues()) {
            BiomeGenerator.BiomeCells.CopyTo(x, y, buffer, (c) => c.Biome.Type == BiomeType.Plains ? 0 : -1, -1);
            var tileId = Blob47.Rules.FindTilePatternId(buffer, -1);
            sproutDarkerGrass.SetCell(godotTileMap, 0, x, y, tileId);
        }
        godotTileMap.ZIndex = 1;
    }

    private void PlaceObjects(Node parent, FastNoiseLite FastNoiseHeight) {
        (Trees.Id, float)[] stumps = [
            (Trees.Id.Trunk, 1f),
            (Trees.Id.Stump, 2f),
            (Trees.Id.MiniStump, 4f),
        ];

        (Trees.Id, float)[] bigTrees = [
            (Trees.Id.BigTree1, 5f),
            (Trees.Id.BigTree2, 5f),
        ];
        (Trees.Id, float)[] smallTrees = [
            (Trees.Id.SmallTree1, 5f),
            (Trees.Id.SmallTree2, 5f),
            (Trees.Id.SmallTree5, 3f),
            (Trees.Id.SmallTree6, 1f),
            (Trees.Id.SmallTree9, 5f),
            (Trees.Id.SmallTree10, 6f),
        ];

        var miniForestRadius = CellSize;
        var minRadius = 16;
        var maxRadius = 120;
        //
        var points = new VariablePoissonSampler2D(Width, Height).Generate((float x, float y) => {
            var noise = 1f - FastNoiseHeight.GetNoise((int)x / 16, (int)y / 16);
            return Mathf.Lerp(minRadius, maxRadius, noise);
        }, minRadius, maxRadius);

        var grid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        grid.AddPointsAsCircles(points);
        grid.ExpandAll(1);
        grid.RemoveAll(s => Random.NextBool(0.5));
        foreach (var circle in grid.FindShapes<Circle>().OrderBy(v => v.Y).ThenBy(v => v.X)) {
            var width = (int)circle.Width;
            if (width <= 32) {
                LocateSprite(parent, Random.Pick(smallTrees), (int)circle.X, (int)circle.Y);
            } else {
                LocateSprite(parent, Random.Pick(bigTrees), (int)circle.X, (int)circle.Y);
            }
        }
        Console.WriteLine(grid.FindShapes().Select(s => s.Width).Min());
        Console.WriteLine(grid.FindShapes().Select(s => s.Width).Max());
    }

    private void Measure(string name, Action action, int times = 1) {
        var x = Stopwatch.StartNew();
        for (var i = 0; i < times; i++) {
            action();
        }
        Console.WriteLine(name + " " + x.ElapsedMilliseconds + "ms (" + times + " times)");
    }

    private async Task Measure(string name, Func<Task> action, int times = 1) {
        var x = Stopwatch.StartNew();
        for (var i = 0; i < times; i++) {
            await action();
        }
        Console.WriteLine(name + " " + x.ElapsedMilliseconds + "ms (" + times + " times)");
    }

    private void LocateSprite(Node parent, string name, int x, int y) {
        var tree = Random.Next(TreesInstance.List(name)).Duplicate() as StaticBody2D;
        tree.Position = new Vector2(x, y);
        parent.AddChild(tree);
    }

    private void LocateSprite(Node parent, Trees.Id id, int x, int y) {
        var tree = TreesInstance.Duplicate(id);
        tree.ZIndex = 2;
        tree.Position = new Vector2(x, y);
        parent.AddChild(tree);
    }
}