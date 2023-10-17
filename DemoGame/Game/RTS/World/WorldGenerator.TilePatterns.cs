using System.Collections.Generic;
using Betauer.TileSet;
using Betauer.TileSet.Image;
using Betauer.TileSet.TileMap;
using Betauer.TileSet.TileMap.Handlers;
using Godot;

namespace Veronenger.Game.RTS.World;

public enum TilePatterns {
    None,
    TextureGreenWithYellowStone,
    TextureGrassLight,
    TextureGrass,
    TextureGrassDark,
    TextureDirt,
    TextureBrick1,
    TextureStoneSquare,

    TextureBigMud,
    TextureSoil,
    TextureBigSand,
    TransparentGrass,
    TransparentAsfalt,

    ModernDirt,
    ModernGreen,
    ModernWater,
    ModernDeepWater,


    TerrainGreen,
}

public partial class WorldGenerator {
    /*
    private static Dictionary<TilePatterns, ITileHandler<TilePatterns>> CreateTileHandlers() {
        var d = new Dictionary<TilePatterns, ITileHandler<TilePatterns>>();
        GrasslandTextures(d);
        BigTextures(d);
        ModernCamping(d);
        Terrain(d);
        return d;
    }

    private static void GrasslandTextures(IDictionary<TilePatterns, ITileHandler<TilePatterns>> d) {
        const int grasslandsTexturesSourceId = 0;

        d[TilePatterns.TextureGreenWithYellowStone] = new TileRegionHandler<TilePatterns> {
            SourceId = grasslandsTexturesSourceId,
            Region = new Rect2I(0, 0, 5, 5)
        };

        d[TilePatterns.TextureGrassLight] = new TileRegionHandler<TilePatterns> {
            SourceId = grasslandsTexturesSourceId,
            Region = new Rect2I(5, 0, 5, 5)
        };

        d[TilePatterns.TextureGrass] = new TileRegionHandler<TilePatterns> {
            SourceId = grasslandsTexturesSourceId,
            Region = new Rect2I(10, 0, 5, 5)
        };

        d[TilePatterns.TextureGrassDark] = new TileRegionHandler<TilePatterns> {
            SourceId = grasslandsTexturesSourceId,
            Region = new Rect2I(15, 0, 5, 5)
        };

        d[TilePatterns.TextureDirt] = new TileRegionHandler<TilePatterns> {
            SourceId = grasslandsTexturesSourceId,
            Region = new Rect2I(20, 0, 5, 5)
        };

        d[TilePatterns.TextureBrick1] = new TileRegionHandler<TilePatterns> {
            SourceId = grasslandsTexturesSourceId,
            Region = new Rect2I(15, 5, 5, 5)
        };

        d[TilePatterns.TextureStoneSquare] = new TileRegionHandler<TilePatterns> {
            SourceId = grasslandsTexturesSourceId,
            Region = new Rect2I(10, 10, 5, 5),
            Randomize = true
        };
    }

    private static void BigTextures(IDictionary<TilePatterns, ITileHandler<TilePatterns>> d) {
        d[TilePatterns.TextureBigMud] = new TileRegionHandler<TilePatterns> {
            SourceId = 1,
            Region = new Rect2I(0, 0, 64, 64)
        };

        d[TilePatterns.TextureSoil] = new TileRegionHandler<TilePatterns> {
            SourceId = 2,
            Region = new Rect2I(0, 0, 64, 64)
        };

        d[TilePatterns.TextureBigSand] = new TileRegionHandler<TilePatterns> {
            SourceId = 3,
            Region = new Rect2I(0, 0, 7, 7)
        };

        d[TilePatterns.TransparentGrass] = new TileRegionHandler<TilePatterns> {
            SourceId = 4,
            Region = new Rect2I(0, 0, 8, 8)
        };

        d[TilePatterns.TransparentAsfalt] = new TileRegionHandler<TilePatterns> {
            SourceId = 5,
            Region = new Rect2I(0, 0, 29, 21)
        };
    }

    private static void ModernCamping(IDictionary<TilePatterns, ITileHandler<TilePatterns>> d) {
        const int modernCampingSourceId = 7;
        d[TilePatterns.ModernDirt] = new TileTerrainHandler<TilePatterns> {
            SourceId = 8,
            TerrainId = (int)TilePatterns.ModernDirt,
            Rules = TileSetLayouts.Blob47Rules,
        };
        // d[TilePatterns.ModernDirt] = new TileRegionHandler<TilePatterns> {
            // SourceId = modernCampingSourceId,
            // Region = new Rect2I(18, 4, 1, 1)
        // };
        d[TilePatterns.ModernGreen] = new TileRegionHandler<TilePatterns> {
            SourceId = modernCampingSourceId,
            Region = new Rect2I(1, 2, 1, 1)
        };
        d[TilePatterns.ModernWater] = new TileRegionHandler<TilePatterns> {
            SourceId = modernCampingSourceId,
            Region = new Rect2I(5, 7, 1, 1)
        };
        d[TilePatterns.ModernWater] = new TileRegionHandler<TilePatterns> {
            SourceId = modernCampingSourceId,
            Region = new Rect2I(5, 7, 1, 1)
        };
        d[TilePatterns.ModernDeepWater] = new TileRegionHandler<TilePatterns> {
            SourceId = modernCampingSourceId,
            Region = new Rect2I(2, 8, 1, 1)
        };
    }

    private static void Terrain(IDictionary<TilePatterns, ITileHandler<TilePatterns>> d) {
        const int terrainSourceId = 10;
        d[TilePatterns.TerrainGreen] = new RandomTileHandler<TilePatterns> {
            SourceId = terrainSourceId,
            Region = new Rect2I(7, 4, 7, 4),
            Id = 0,
        };
    }
    */
}