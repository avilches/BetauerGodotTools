using System;
using System.Collections.Generic;
using Betauer.Core;
using Betauer.DI.Attributes;
using Godot;
using NUnit.Framework;

namespace Veronenger.Game.RTS.World;

[Singleton]
public partial class WorldGenerator {
	[Inject] public Random Random { get; set; }
	private const int Layers = 2;
	private const int Size = 512;
	public TileSetController<TilePatterns> Controller { get; private set; }

	public void Generate(TileMap tileMap, NoiseTexture2D noiseTexture) {

		Controller = CreateTileSetController(tileMap, Random);
		Controller.Clear();

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
		
		var fastNoiseTextureController = new FastNoiseTextureController(noiseTexture);
		for (var y = 0; y < Size; y++) {
			for (var x = 0; x < Size; x++) {
				var tilePattern = tiles[fastNoiseTextureController.GetNoise(x, y)];
				Controller.Set(0, new Vector2I(x, y), tilePattern);
				// Controller.Set(1, pos, tilePattern);
			}
		}
		// Controller.Smooth(0);
		Controller.Fill();
	}

	public static void Main() {
		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0f), Is.EqualTo(0));
		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0f, false), Is.EqualTo(0));

		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.05f), Is.EqualTo(0));
		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.05f, false), Is.EqualTo(0));

		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.1f), Is.EqualTo(0));
		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.1f, false), Is.EqualTo(1));

		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.15f), Is.EqualTo(1));
		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.15f, false), Is.EqualTo(1));

		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.2f), Is.EqualTo(1));
		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.2f, false), Is.EqualTo(2));

		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.25f), Is.EqualTo(2));
		Assert.That(FindPosition(new[] { 0.1f, 0.2f}, 0.25f, false), Is.EqualTo(2));
	}


	/*
	 * Find the position of value in the offsets array (the offsets array must be sorted in ascending order).
	 * It takes every offset and check if the value is less than the offset (or less or equal if inclusive is true) and return the position of the offset.
	 * Example with  { 0.1f, 0.2f }
	 * 0.05f  => 0
	 * 
	 * 0.1f   => 0 if inclusive is true 
	 * 0.1f   => 1 if inclusive is false
	 * 
	 * 0.15f  => 1
	 * 
	 * 0.2f   => 1 if inclusive is true
	 * 0.2f   => 2 if inclusive is false
	 * 
	 * 0.25f  => 2
	 */
	static int FindPosition(IReadOnlyList<float> offsets, float value, bool inclusive = true) {
		var left = 0;
		var right = offsets.Count - 1;

		while (left <= right) {
			var mid = left + (right - left) / 2;
			if (inclusive ? offsets[mid] < value : offsets[mid] <= value) {
				left = mid + 1;
			} else {
				right = mid - 1;
			}
		}
		return right + 1;
	}

	public static int FloatToInt(float[] ranges, float val) {
		for (var i = 1; i < ranges.Length; i++) {
			var from = ranges[i - 1];
			var to = ranges[i];
			if (val >= from && val <= to) return i - 1;
		}
		return ranges.Length - 1;
	}

}

public class TilePattern<TTile> where TTile : Enum {
	public TTile Key { get; init; }
	public int SourceId { get; init; }
	public Rect2I AtlasCoords { get; init; }
	public int Layer;

	public Vector2I Position(Vector2I pos) {
		var x = AtlasCoords.Position.X + pos.X % AtlasCoords.Size.X;
		var y = AtlasCoords.Position.Y + pos.Y % AtlasCoords.Size.Y;
		return new Vector2I(x, y);
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

	private readonly Dictionary<TTile, TilePattern<TTile>> _tilePatterns = new();

	public Random Random { get; set; }

	public TileSetController(TileMap tileMap, int layers, int size, Random random) {
		TileMap = tileMap;
		Size = size;
		Random = random;
		Layers = layers;
		Data = new TileData[layers, size, size];
	}

	public void Add(TilePattern<TTile> tilePattern) {
		_tilePatterns[tilePattern.Key] = tilePattern;
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
							TileMap.SetCell(layer, pos, tilePattern.SourceId, tilePattern.Position(pos));
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
