using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.Core;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.RTS.World;

[Singleton]
public partial class WorldGenerator {
	[Inject] public Random Random { get; set; }

	private Sprite2D _sprite2D;
	private NoiseTexture2D _noiseTexture;
	private Noise noise;

	private static int Size = 512;
	
	public TileSetController<TilePatterns> Controller { get; private set; }

	public void Generate(TileMap grassland, TileMap modern, Sprite2D sprite2D) {
		_sprite2D = sprite2D;
		_noiseTexture = (NoiseTexture2D)sprite2D.Texture;
		noise = _noiseTexture.Noise;

		Controller = CreateTileSetController(grassland, modern, Random);

		var normalizedOffsetsArray = CreateNormalizedOffsetsArray(_noiseTexture.ColorRamp);
		var tiles = new[] { 
			TilePatterns.ModernDirt, 
			TilePatterns.ModernDirt, 
			TilePatterns.ModernGreen, 
			TilePatterns.ModernWater };
		
		if (normalizedOffsetsArray.Length - 1 != tiles.Length) throw new Exception("Tiles length should be offsets - 1");
		for (var y = 0; y < Size; y += 1) {
			for (var x = 0; x < Size; x += 1) {
				var pos = new Vector2I(x, y);
				var noise2Dv = noise.GetNoise2Dv(pos); // -0.77..0.77
				var normalNoise = (noise2Dv + 1f) / 2f;  // 0..1 (normalized val) 
				var tile = FloatToInt(normalNoise, normalizedOffsetsArray);
				var tilePatterns = tiles[tile];
				Controller.Set(tilePatterns, 0, pos);
			}
		}
		Controller.Smooth();
		Controller.Fill();
	}

	// Returns an array from 0.0f to 0.1f
	private float[] CreateNormalizedOffsetsArray(Gradient noiseTextureColorRamp) {
		var offsets = noiseTextureColorRamp.Offsets;
		var normalizedOffsets = new List<float>();
		if (offsets.First() > 0.0f) {
			normalizedOffsets.Add(0f);
		}
		normalizedOffsets.AddRange(offsets);
		if (normalizedOffsets.Last() < 1f) {
			normalizedOffsets.Add(1f);
		}
		var normalizedOffsetsArray = normalizedOffsets.ToArray();
		return normalizedOffsetsArray;
	}

	// Returns a value from 0 to max
	public float[] CreateRanges(int amount, float start = 0f, float end = 1f) {
		var ranges = new float[amount];
		var step = 1f / amount;
		for (var i = 0; i < amount; i++) {
			ranges[i] = Mathf.Lerp(start, end, step * i);
		}
		return ranges;
	}

	public int FloatToInt(float val, float[] ranges) {
		for (var i = 1; i < ranges.Length; i++) {
			var from = ranges[i - 1];
			var to = ranges[i];
			if (val > from && val <= to) return i - 1;
		}
		throw new IndexOutOfRangeException();
	}
}

public class TilePattern<TTile> where TTile : Enum {
	public TTile Key { get; init; }
	public TileMap TileMap { get; init; }
	public int SourceId { get; init; }
	public Rect2I AtlasCoords { get; init; }

	public Vector2I Position(Vector2I pos) {
		var x = AtlasCoords.Position.X + pos.X % AtlasCoords.Size.X;
		var y = AtlasCoords.Position.Y + pos.Y % AtlasCoords.Size.Y;
		return new Vector2I(x, y);
	}
}

public class TileSetController<TTile> where TTile : Enum {
	internal struct TileData {
		internal readonly TTile Tile;
		internal readonly int Layer;
		internal readonly bool Set;

		public TileData(TTile tile, int layer) {
			Tile = tile;
			Layer = layer;
			Set = true;
		}
	}

	internal readonly TileData[,] Data;
	internal readonly int Size;

	private readonly Dictionary<TTile, TilePattern<TTile>> _tilePatterns = new();

	public Random Random { get; set; }

	public TileSetController(int size, Random random) {
		Size = size;
		Random = random;
		Data = new TileData[size, size];
	}

	public void Add(TilePattern<TTile> tilePattern) {
		_tilePatterns[tilePattern.Key] = tilePattern;
	}
	
	public void Set(TTile tile, int layer, Vector2I pos) {
		Data[pos.X, pos.Y] = new TileData(tile, layer);
	}

	public void Smooth() {
		var steps = 0;
		while (SmoothStep() && steps ++ < 15) {
		}
	}

	public bool SmoothStep() {
		var worked = false;
		void SetData(int x, int y, TileData data) {
			if (Data[x, y].Tile.ToInt() == data.Tile.ToInt()) return;
			Data[x, y] = data;
			worked = true;
		}

		TileData GetData(int x, int y, TileData def) {
			if (x < 0 || x >= Size ||
				y < 0 || y >= Size) return def;
			return Data[x, y];
		}
		
		for (var y = 0; y < Size; y += 1) {
			for (var x = 0; x < Size; x += 1) {
				var data = Data[x, y];
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
		for (var y = 0; y < Size; y += 1) {
			for (var x = 0; x < Size; x += 1) {
				var data = Data[x, y];
				var tilePattern = _tilePatterns[data.Tile];
				var tileMap = tilePattern.TileMap;
				var pos = new Vector2I(x, y);
				tileMap.SetCell(data.Layer, pos, tilePattern.SourceId, tilePattern.Position(pos));
			}
		}
	}
}
