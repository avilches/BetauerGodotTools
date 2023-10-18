using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Betauer.Application.Lifecycle;
using Betauer.Core.PoissonDiskSampling;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.RTS.Assets.Trees;
using Betauer.Core;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.Image;
using Betauer.TileSet.Image;
using Betauer.TileSet.Terrain;
using Betauer.TileSet.TileMap;
using Betauer.TileSet.TileMap.Handlers;
using Godot.Collections;

namespace Veronenger.Game.RTS.World;

[Singleton]
public partial class WorldGenerator {
	[Inject] public Random Random { get; set; }
	[Inject] public ITransient<Trees> TreesFactory { get; set; }
	[Inject] public SceneTree SceneTree { get; set; }
	[Inject] public ResourceHolder<Texture2D> Grasslands { get; set; }
	private Trees TreesInstance;
	private const int CellSize = 16;

	private const int Layers = 2;
	private const int GridSize = 200;
	private const int Size = GridSize * CellSize;
	public FastTextureNoiseWithGradient FastNoise { get; private set; }

	public void Generate(Godot.TileMap godotTileMap, NoiseTexture2D noiseTexture) {
		godotTileMap.Clear();
		TreesInstance = TreesFactory.Create();
		TreesInstance.Configure();

		FastNoise = new FastTextureNoiseWithGradient(noiseTexture);

		Measure("Place objects", () => { PlaceObjects(godotTileMap); });

		// var config = CreateTileHandlers();
		// var sources = config.ToDictionary(pair => pair.Key, pair => (ISource)pair.Value);

		var tiles = new[] {
			TilePatterns.TerrainGreen,
			TilePatterns.TerrainGreen,
			TilePatterns.ModernDirt,
			TilePatterns.ModernWater,
			TilePatterns.ModernDeepWater
			// TilePatterns.TextureGrass,
			// TilePatterns.TextureGrass,
			// TilePatterns.TextureGrassLight,
			// TilePatterns.TextureSoil,
			// TilePatterns.TextureStoneSquare
		};
		var tileMap = new TileMap<TilePatterns>(2, GridSize, GridSize, new System.Collections.Generic.Dictionary<TilePatterns, int> {
			{ TilePatterns.None, -1 }
		});
		tileMap.Apply((t, x, y) => {
			var tilePattern = tiles[FastNoise.GetNoiseGradient(x, y)];
			tileMap.SetTerrain(x, y, tilePattern);
			// if (tilePattern is TilePatterns.TerrainGreen && Random.NextBool(0.2f)) {
			// tileMap.SetType(1, x, y, TilePatterns.TransparentAsfalt);
			// }
		});

		tileMap.Apply(new TerrainTileHandler(1, (int)TilePatterns.ModernDirt, TerrainRuleSets.Blob47Rules, 8, TileSetLayouts.Blob47Godot));
		// tileMap.Apply(new TerrainTileHandler(1, TerrainRuleSets.Blob47Rules.ApplyTerrain((int)TilePatterns.TerrainGreen), 8, TileSetLayouts.Blob47Godot));
		// tileMap.Apply(new TerrainTileHandler(1, TerrainRuleSets.Blob47Rules.ApplyTerrain((int)TilePatterns.ModernDeepWater), 8, TileSetLayouts.Blob47Godot));
		
		godotTileMap.ZIndex = 1;
		tileMap.Smooth();
		tileMap.Flush(godotTileMap);
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

		var points = new VariablePoissonSampler2D(Size, Size)
			.Generate((float x, float y) => {
				var noise = 1f - FastNoise.GetNoise((int)x / 16, (int)y / 16);
				return Mathf.Lerp(minRadius, maxRadius, noise);
			}, minRadius, maxRadius);

		var grid = SpatialGrid.FromAverageDistance(minRadius, maxRadius);
		grid.AddPointsAsCircles(points);
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
		tree.Position = new Vector2(x , y);
		parent.AddChild(tree);
	}

	private void LocateSprite(Node parent, Trees.Id id, int x, int y) {
		var tree = TreesInstance.Duplicate(id);
		tree.ZIndex = 2;
		tree.Position = new Vector2(x , y);
		parent.AddChild(tree);
	}
}

