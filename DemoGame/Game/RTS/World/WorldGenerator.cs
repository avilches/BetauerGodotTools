using System;
using System.Diagnostics;
using System.Linq;
using Betauer.Core.PoissonDiskSampling;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.RTS.Assets.Trees;
using Betauer.Core;
using Betauer.Core.Collision.Spatial2D;
using Betauer.Core.Image;

namespace Veronenger.Game.RTS.World;

[Singleton]
public partial class WorldGenerator {
	[Inject] public Random Random { get; set; }
	[Inject] public ITransient<Trees> TreesFactory { get; set; }
	private Trees TreesInstance;
	private const int CellSize = 16;

	private const int Layers = 2;
	private const int GridSize = 256;
	private const int Size = GridSize * CellSize;
	public TileSetController<TilePatterns> Controller { get; private set; }
	public FastTextureNoiseWithGradient FastNoise { get; private set; }

	public void Generate(TileMap tileMap, NoiseTexture2D noiseTexture) {

		Controller = CreateTileSetController(tileMap, Random);
		Controller.Clear();
		TreesInstance = TreesFactory.Create();
		TreesInstance.Configure();
		
		FastNoise = new FastTextureNoiseWithGradient(noiseTexture);

		CreateBackground(FastNoise);
		PlaceObjects(tileMap);

		Controller.Flush();
	}

	private void CreateBackground(FastTextureNoiseWithGradient fastNoiseTextureController) {
		var tiles0 = new[] { 
			TilePatterns.TextureGrass, 
			TilePatterns.TextureGrass, 
			TilePatterns.TextureGrassLight, 
			TilePatterns.TextureSoil,
			TilePatterns.TextureBigMud };
		var tiles1 = new[] { 
			TilePatterns.TextureBigMud, 
			TilePatterns.TextureBigMud, 
			TilePatterns.TextureBigMud, 
			TilePatterns.TextureBigMud,
			TilePatterns.TextureBigMud };
		var tiles2 = new[] { 
			TilePatterns.None, 
			TilePatterns.None, 
			TilePatterns.TransparentAsphalt, 
			TilePatterns.None,
			TilePatterns.None };

		var tiles = tiles0;
		for (var y = 0; y < GridSize; y++) {
			for (var x = 0; x < GridSize; x++) {
				var pos = new Vector2I(x, y);
				var tilePattern = tiles[fastNoiseTextureController.GetNoiseGradient(x, y)];
				Controller.Set(0, pos, tilePattern);
				Controller.Set(0, pos, TilePatterns.TextureSoil);
				if (tilePattern is TilePatterns.TextureGrass or TilePatterns.TextureGrassLight or TilePatterns.TextureGrassDark) {
					// LocateSprite(tileMap, Trees.Id.BigTree1, x * 16, y * 16);
					Controller.Set(1, pos, TilePatterns.TransparentGrass);
				}

				// Controller.Set(1, pos, tilePattern);
			}
		}
	}

	private void PlaceObjects(TileMap tileMap) {
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
				Console.WriteLine(width);
				if (width <= 32) {
					LocateSprite(tileMap, Random.Pick(smallTrees).Value, (int)circle.X, (int)circle.Y);
				} else {
					LocateSprite(tileMap, Random.Pick(bigTrees).Value, (int)circle.X, (int)circle.Y);
				}
			});
		Console.WriteLine(grid.FindShapes().Select(s => s.Width).Min());
		Console.WriteLine(grid.FindShapes().Select(s => s.Width).Max());
	}

	private void Measure(string name, Action action, int times = 10000) {
		var x = Stopwatch.StartNew();
		for (var i = 0; i < times; i++) {
			action();
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
		tree.Position = new Vector2(x , y);
		parent.AddChild(tree);
	}
}

