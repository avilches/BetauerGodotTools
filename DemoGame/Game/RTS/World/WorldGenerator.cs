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
using Betauer.Nodes;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.Data;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap;
using Betauer.TileSet.TileMap.Handlers;
using TileMap = Godot.TileMap;

namespace Veronenger.Game.RTS.World;

[Singleton]
public partial class WorldGenerator {
    [Inject] public Random Random { get; set; }
    [Inject] public ITransient<Trees> TreesFactory { get; set; }
    [Inject] public SceneTree SceneTree { get; set; }
    [Inject] public ResourceHolder<Texture2D> Grasslands { get; set; }

    public BiomeGenerator BiomeGenerator { get; } = new();
    public TileMap<BiomeType> TileMap { get; private set; }
    public TileMap GodotTileMap { get; private set; }
    public Trees TreesInstance;
    public Drawer Drawer { get; private set; }
    private const int CellSize = 16;

    private const int Layers = 2;
    private const int Width = 300;
    private const int Height = 200;
    
    public enum ViewMode {
        Terrain,
        Height,
        HeightFalloff,
        Humidity,
        FalloffMap,
    }
    
    public ViewMode CurrentViewMode { get; set; } = ViewMode.Terrain;

    private int _seed = 0;
    public int Seed {
        get => _seed;
        set {
            _seed = value;
            BiomeGenerator.Seed = _seed;
        }
    }

    public void Configure(TileMap godotTileMap) {
        BiomeGenerator.Configure(Width, Height, Seed);
        TileMap = new TileMap<BiomeType>(Layers, Width, Height);
        GodotTileMap = godotTileMap;
        
        Drawer = GodotTileMap.AddDraw((canvas) => {
            // Hack needed to draw on top of the tilemap:
            // https://www.reddit.com/r/godot/comments/w3l48f/does_anyone_know_how_to_set_the_draw_order_when/
            // https://github.com/godotengine/godot/issues/35002 
            GodotTileMap.Visible = false;
            var subViewport = (SubViewport)canvas.GetViewport();
            subViewport.RenderTargetClearMode = SubViewport.ClearMode.Never;
            Draw(canvas);
            GodotTileMap.Visible = true;
            subViewport.RenderTargetClearMode = SubViewport.ClearMode.Always;
        });
        GodotTileMap.OnProcess(d => {
            // drawer.QueueRedraw();
        });
    }

    public void Generate() {
        var biomeGrid = BiomeGenerator.Generate();

        TileMap.Execute((t, x, y) => {
            var biomeType = biomeGrid[y, x].Biome.Type;
            TileMap.SetTerrain(x, y, biomeType);
        });

        TileMap.Smooth(Seed);
        // TileMap.IfTerrainEnum(BiomeType.Ocean).SetAtlasCoords(0, 6, new Vector2I(19, 13)); // Modern, deep water
        TileMap.IfTerrainEnum(BiomeType.Glacier).SetAtlasCoords(0, 6, new Vector2I(2, 2)); // Modern, snow
        TileMap.IfTerrainEnum(BiomeType.Mountain).SetAtlasCoords(0, 1, new Vector2I(0, 0)); // Mud
        
        TileMap.IfTerrainEnum(BiomeType.Desert).SetAtlasCoords(0, 0, new Vector2I(0, 15)); // Red
        TileMap.IfTerrainEnum(BiomeType.Savannah).SetAtlasCoords(0, 0, new Vector2I(0, 2)); // GreenYellow 
        TileMap.IfTerrainEnum(BiomeType.Forest).SetAtlasCoords(0, 0, new Vector2I(11, 1)); // 
        TileMap.IfTerrainEnum(BiomeType.Jungle).SetAtlasCoords(0, 0, new Vector2I(16, 2)); // 
        
        
        // TileMap.IfTerrainEnum(BiomeType.Beach).SetAtlasCoords(0, 3, new Vector2I(0, 0)); // Sand
        TileMap.IfTerrainEnum(BiomeType.Beach).SetAtlasCoords(0, 7, new Vector2I(18, 4)); // Sand
        TileMap.IfTerrainEnum(BiomeType.Sea).SetAtlasCoords(0, 6, new Vector2I(20, 13)); // Modern, water

        // TileMap.IfTerrain(BiomeType.ColdBeach).SetAtlasCoords(0, 3, new Vector2I(0, 0));
        // TileMap.IfTerrain(BiomeType.Glacier).SetAtlasCoords(0, 3, new Vector2I(0, 0));

        // TileMap.IfTerrain(Biomes.RockyBeach)
        //     .Do((t, x, y) => TileMap.SetAtlasCoords(0, 2, x, y, new Vector2I(0, 0)))
        //     .Apply();
        //
        // TileMap.IfTerrain(Biomes.Forest)
        //     .Do((t, x, y) => TileMap.SetAtlasCoords(0, 0, x, y, new Vector2I(5, 0)))
        //     .Apply();
        //
        // TileMap.IfTerrain(Biomes.Jungle)
        //     .Do((t, x, y) => TileMap.SetAtlasCoords(0, 0, x, y, new Vector2I(15, 0)))
        //     .Apply();
        //
        // TileMap.IfTerrain(Biomes.Snow)
        //     .Do((t, x, y) => TileMap.SetAtlasCoords(0, 0, x, y, new Vector2I(15, 5)))
        //     .Apply();
        //
        // TileMap.IfTerrain(Biomes.Rock)
        //     .Do((t, x, y) => TileMap.SetAtlasCoords(0, 0, x, y, new Vector2I(20, 0)))
        //     .Apply();

        GodotTileMap.Clear();
        TileMap.Flush();
        TileMap.DumpAtlasCoordsTo(GodotTileMap);

        /*
        TileMap.Execute((t, x, y) => {
            if (x > 100 || y > 100) return;
            if (TileMap.GetTerrainEnum(x, y) == BiomeType.Beach ||
                TileMap.GetTerrainEnum(x, y) == BiomeType.ColdBeach ||
                TileMap.GetTerrainEnum(x, y) == BiomeType.Glacier) {
                var l = new Label {
                    Text = biomeGrid[y, x].Height > 0 ? "+" : "-",
                    Position = godotTileMap.MapToLocal(new Vector2I(x, y))
                };
                godotTileMap.AddChild(l);
            }
        });
        */
        // GodotTileMap.ZIndex = 1;
        UpdateView();
    }
    
    public void UpdateView() {
        // GodotTileMap.Visible = CurrentViewMode == ViewMode.Terrain;
        Drawer.QueueRedraw();
    }

    private void Draw(CanvasItem canvas) {
        if (CurrentViewMode == ViewMode.Height) {
            DrawGrid(canvas, BiomeGenerator.HeightNormalizedGrid);
        } else if (CurrentViewMode == ViewMode.Humidity) {
            DrawGrid(canvas, BiomeGenerator.HumidityNormalizedGrid);
        } else if (CurrentViewMode == ViewMode.HeightFalloff) {
            DrawGrid(canvas, BiomeGenerator.HeightFalloffGrid);
        } else if (CurrentViewMode == ViewMode.FalloffMap) {
            DrawGrid(canvas, BiomeGenerator.FalloffMap);
        } else if (CurrentViewMode == ViewMode.Terrain) {
        }
    }
    
    private void DrawGrid(CanvasItem canvas, IDataGrid<float> normalizedGrid) {
        var offset = new Vector2(8, 8);
        TileMap.Execute((t, x, y) => {
            // canvas.TopLevel = true;
            var noise = normalizedGrid.GetValue(x, y);
            var color = new Color(noise, noise, noise);
            var position = (GodotTileMap.MapToLocal(new Vector2I(x, y))) - offset;
            canvas.DrawRect(new Rect2(position, new Vector2(16, 16)), color, true);
        });
    }

    public void GenerateOld(TileMap godotTileMap, NoiseTexture2D noiseHeight, NoiseTexture2D noiseMoisture) {
        godotTileMap.Clear();
        TreesInstance = TreesFactory.Create();
        TreesInstance.Configure();

        // FastNoiseHeight = new FastTextureNoiseWithGradient(noiseHeight);

        Measure("Place objects", () => { PlaceObjects(godotTileMap); });

        var tiles = new[] {
            TilePatterns.TerrainGreen,
            TilePatterns.TerrainGreen,
            TilePatterns.ModernDirt,
            TilePatterns.ModernWater,
            TilePatterns.ModernDeepWater
        };
        var tileMap = new TileMap<TilePatterns>(Layers, Width, Height, new Dictionary<TilePatterns, int> {
            { TilePatterns.None, -1 }
        });
        var sproutDarkerGrass = tileMap.CreateSource(8, TileSetLayouts.Blob47Godot);

        // tileMap.Execute((t, x, y) => tileMap.SetTerrain(x, y, tiles[FastNoiseHeight.GetNoiseGradient(x, y)]));
        tileMap.Smooth();
        tileMap.Execute(new TerrainTileHandler(1, TilePatternRuleSets.Blob47Rules.WithTerrain(tileMap.EnumToTerrain(TilePatterns.ModernDirt)), sproutDarkerGrass));
        tileMap.Execute(new TerrainTileHandler(1, TilePatternRuleSets.Blob47Rules.WithTerrain(tileMap.EnumToTerrain(TilePatterns.TerrainGreen)), sproutDarkerGrass));
        tileMap.IfTerrain((int)TilePatterns.ModernDirt)
            .Do((t, x, y) => sproutDarkerGrass.SetAtlasCoords(0, x, y, 255))
            .Apply();

        godotTileMap.ZIndex = 1;
        tileMap.DumpAtlasCoordsTo(godotTileMap);
    }

    private void PlaceObjects(Node parent) {
        var stumps = new[] {
            WeightValue.Create(Trees.Id.Trunk, 1f),
            WeightValue.Create(Trees.Id.Stump, 2f),
            WeightValue.Create(Trees.Id.MiniStump, 4f),
        };

        var bigTrees = new[] {
            WeightValue.Create(Trees.Id.BigTree1, 5f),
            WeightValue.Create(Trees.Id.BigTree2, 5f),
        };
        var smallTrees = new[] {
            WeightValue.Create(Trees.Id.SmallTree1, 5f),
            WeightValue.Create(Trees.Id.SmallTree2, 5f),
            WeightValue.Create(Trees.Id.SmallTree5, 3f),
            WeightValue.Create(Trees.Id.SmallTree6, 1f),
            WeightValue.Create(Trees.Id.SmallTree9, 5f),
            WeightValue.Create(Trees.Id.SmallTree10, 6f),
        };

        var miniForestRadius = CellSize;
        var minRadius = 16;
        var maxRadius = 120;
        //
        // var points = new VariablePoissonSampler2D(Size, Size)
        //     .Generate((float x, float y) => {
        //         var noise = 1f - FastNoiseHeight.GetNoise((int)x / 16, (int)y / 16);
        //         return Mathf.Lerp(minRadius, maxRadius, noise);
        //     }, minRadius, maxRadius);
        //
        var grid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
        // grid.AddPointsAsCircles(points);
        grid.ExpandAll(1);
        grid.RemoveAll(s => Random.NextBool(0.5));
        grid.FindShapes<Circle>().OrderBy(v => v.Y)
            .ThenBy(v => v.X)
            .ForEach(circle => {
                var width = (int)circle.Width;
                if (width <= 32) {
                    LocateSprite(parent, Random.Pick(smallTrees).Value, (int)circle.X, (int)circle.Y);
                } else {
                    LocateSprite(parent, Random.Pick(bigTrees).Value, (int)circle.X, (int)circle.Y);
                }
            });
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