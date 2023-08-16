using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.DI.Attributes;
using Betauer.DI.Factory;
using Godot;
using Veronenger.RTS.Assets;

namespace Veronenger.Game.RTS.World;

[Singleton]
public partial class WorldGenerator {
	[Inject] public Random Random { get; set; }
	[Inject] public ITransient<Trees> TreesFactory { get; set; }
	private Trees TreesInstance;
	private const int Layers = 2;
	private const int Size = 512;
	public TileSetController<TilePatterns> Controller { get; private set; }

	public void Generate(TileMap tileMap, NoiseTexture2D noiseTexture) {

		Controller = CreateTileSetController(tileMap, Random);
		Controller.Clear();
		TreesInstance = TreesFactory.Create();
		TreesInstance.Configure();

		var tiles0 = new[] { 
			TilePatterns.TextureGrassDark, 
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
			TilePatterns.TextureAsphaltLight, 
			TilePatterns.None,
			TilePatterns.None };

		var tiles = tiles0;
		var fastNoiseTextureController = new FastNoiseTextureGradientController(noiseTexture);
		var treeWeights = new[] {
			WeightValue.Create(Trees.Id.Trunk, 1f),
			WeightValue.Create(Trees.Id.Stump, 2f),
			WeightValue.Create(Trees.Id.MiniStump, 4f),
		};
		for (var y = 0; y < Size; y++) {
			for (var x = 0; x < Size; x++) {
				var tilePattern = tiles[fastNoiseTextureController.GetNoise(x, y)];
				Controller.Set(0, new Vector2I(x, y), tilePattern);
				if (tilePattern == TilePatterns.TextureGrassDark) {
					if (Random.Next(0, 10) < 1) {
						var val = Random.Pick(treeWeights).Value;
						LocateSprite(tileMap, val, x, y);
					}
				}
				// Controller.Set(1, pos, tilePattern);
			}
		}
		// Controller.Smooth(0);
		Controller.Fill();
	}

	private void LocateSprite(Node parent, Trees.Id id, int x, int y) {
		var tree = TreesInstance.Duplicate(id);
		tree.Position = new Vector2(x * 16, y * 16);
		parent.AddChild(tree);
	}
}

public interface ITilePattern<TTile> where TTile : Enum {
	public TTile Key { get; init; }
	public int SourceId { get; init; }
	public void Apply(TileMap tileMap, int layer, Vector2I pos);
}

public class TilePattern<TTile> : ITilePattern<TTile> 
	where TTile : Enum {
	public TTile Key { get; init; }
	public int SourceId { get; init; }
	public Rect2I AtlasCoords { get; init; }

	public void Apply(TileMap tileMap, int layer, Vector2I pos) {
		var x = AtlasCoords.Position.X + pos.X % AtlasCoords.Size.X;
		var y = AtlasCoords.Position.Y + pos.Y % AtlasCoords.Size.Y;
		tileMap.SetCell(layer, pos, SourceId, new Vector2I(x, y));
	}
}

public class TileSetController<TTile> where TTile : Enum {
	internal class TileData {
		internal readonly TTile Tile;

		public TileData(TTile tile) {
			Tile = tile;
		}
	}

	internal readonly TileData[,,] Data;
	internal readonly int Layers;
	internal readonly int Size;
	internal readonly TileMap TileMap;

	private readonly Dictionary<TTile, ITilePattern<TTile>> _tilePatterns = new();

	public Random Random { get; set; }

	public TileSetController(TileMap tileMap, int layers, int size, Random random) {
		TileMap = tileMap;
		Size = size;
		Random = random;
		Layers = layers;
		Data = new TileData[layers, size, size];
	}

	public void Add(TTile key, ITilePattern<TTile> tilePattern) {
		_tilePatterns[key] = tilePattern;
	}
	
	public void Set(int layer, Vector2I pos, TTile tile) {
		Data[layer, pos.X, pos.Y] = new TileData(tile);
	}

	public void Smooth(int layer) {
		var steps = 0;
		while (SmoothStep(layer) && steps ++ < 15) {
		}
	}

	public bool SmoothStep(int layer) {
		var worked = false;
		void SetData(int x, int y, TileData data) {
			if (Data[layer, x, y].Tile.ToInt() == data.Tile.ToInt()) return;
			Data[layer, x, y] = new TileData(data.Tile);
			worked = true;
		}

		TileData GetData(int x, int y, TileData def) {
			if (x < 0 || x >= Size ||
				y < 0 || y >= Size) return def;
			return Data[layer, x, y];
		}
		
		for (var y = 0; y < Size; y++) {
			for (var x = 0; x < Size; x++) {
				var data = Data[layer, x, y];
				var left = GetData(x - 1, y, data);
				var right = GetData(x + 1, y, data);
				var up = GetData(x, y - 1, data);
				var down = GetData(x, y + 1, data);
				bool equalLat = left.Tile.ToInt() == right.Tile.ToInt();
				bool equalVer = up.Tile.ToInt() == down.Tile.ToInt();
				if (equalLat && equalVer) {
					SetData(x, y, Random.NextBool() ? left : up);
				} else if (equalLat) {
					SetData(x, y, left);
				} else if (equalVer) {
					SetData(x, y, up);
				}
			}
		}
		return worked;
	}

	public void Fill() {
		for (var layer = 0; layer < Layers; layer += 1) {
			for (var y = 0; y < Size; y++) {
				for (var x = 0; x < Size; x++) {
					var data = Data[layer, x, y];
					if (data != null) {
						var tilePattern = _tilePatterns[data.Tile];
						if (tilePattern.SourceId >= 0) {
							var pos = new Vector2I(x, y);
							tilePattern.Apply(TileMap, layer, pos);
						}
					}
				}
			}
		}
	}

	public void Clear() {
		TileMap.Clear();
	}
}
