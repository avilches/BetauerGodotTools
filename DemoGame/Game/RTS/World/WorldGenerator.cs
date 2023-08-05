using System;
using System.Collections.Generic;
using System.Linq;
using Betauer.DI.Attributes;
using Godot;

namespace Veronenger.Game.RTS.World;

public enum TilePatterns {
	GreenWithYellowStone,
	Green2,
	Green3,
	Green4,
}
	
[Singleton]
public class WorldGenerator {

	[Inject] public Random Random { get; set; }

	private Sprite2D _sprite2D;
	private NoiseTexture2D _noiseTexture;
	private Noise noise;

	public void Generate(TileMap grassland, TileMap modern, Sprite2D sprite2D) {
		_sprite2D = sprite2D;
		_noiseTexture = (NoiseTexture2D)sprite2D.Texture;
		noise = _noiseTexture.Noise;

		var controller = CreateTileSetController(grassland, modern);

		var normalizedOffsetsArray = CreateNormalizedOffsetsArray(_noiseTexture.ColorRamp);
		var tiles = new[] { 
			TilePatterns.Green2, 
			TilePatterns.GreenWithYellowStone, 
			TilePatterns.Green3, 
			TilePatterns.Green4 };
		if (normalizedOffsetsArray.Length - 1 != tiles.Length) throw new Exception("Tiles length should be offsets - 1");
		for (var y = 0; y < 512; y += 1) {
			for (var x = 0; x < 512; x += 1) {
				var pos = new Vector2I(x, y);
				var noise2Dv = noise.GetNoise2Dv(pos); // -0.77..0.77
				var normalNoise = (noise2Dv + 1f) / 2f;  // 0..1 (normalized val) 
				var tile = FloatToInt(normalNoise, normalizedOffsetsArray);
				var tilePatterns = tiles[tile];
				controller.Set(tilePatterns, 0, pos);
			}
		}
	}

	private enum TileSetsEnum {
		// The value must match the position in the array of tilesets in the tilemap
		GrasslandsTextures = 0,
	}

	private static TileSetController<TilePatterns> CreateTileSetController(TileMap grassland, TileMap modern) {
		var ts = new TileSetController<TilePatterns>();

		ts.Add(new TilePattern<TilePatterns> {
			Key = TilePatterns.GreenWithYellowStone,
			TileMap = grassland,
			SourceId = (int)TileSetsEnum.GrasslandsTextures,
			AtlasCoords = new Rect2I(0, 0, 5, 5)
		});

		ts.Add(new TilePattern<TilePatterns> {
			Key = TilePatterns.Green2,
			TileMap = grassland,
			SourceId = (int)TileSetsEnum.GrasslandsTextures,
			AtlasCoords = new Rect2I(5, 0, 5, 5)
		});

		ts.Add(new TilePattern<TilePatterns> {
			Key = TilePatterns.Green3,
			TileMap = grassland,
			SourceId = (int)TileSetsEnum.GrasslandsTextures,
			AtlasCoords = new Rect2I(10, 0, 5, 5)
		});

		ts.Add(new TilePattern<TilePatterns> {
			Key = TilePatterns.Green4,
			TileMap = grassland,
			SourceId = (int)TileSetsEnum.GrasslandsTextures,
			AtlasCoords = new Rect2I(15, 0, 5, 5)
		});
		return ts;
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
	private readonly Dictionary<TTile, TilePattern<TTile>> _tilePatterns = new();

	public void Add(TilePattern<TTile> tilePattern) {
		_tilePatterns[tilePattern.Key] = tilePattern;
	}

	public void Set(TTile tile, int layer, Vector2I pos) {
		var tilePattern = _tilePatterns[tile];
		var tileMap = tilePattern.TileMap;
		tileMap.SetCell(layer, pos, tilePattern.SourceId, tilePattern.Position(pos));
	}
}
