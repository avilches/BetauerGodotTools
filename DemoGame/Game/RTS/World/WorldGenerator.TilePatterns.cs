using System;
using Godot;

namespace Veronenger.Game.RTS.World; 

public enum TilePatterns {
    None,
    TextureGreenWithYellowStone,
    TextureGrassLight,
    TextureGrass,
    TextureGrassDark,
    TextureDirt,
    TextureTransparentPlant,
    
    
    TextureBigMud,
    TextureSoil,
    TextureBigSand,
    TextureAsphaltLight,
    
    
	
    ModernDirt,
    ModernGreen,
    ModernWater,
    ModernDeepWater,
}



public partial class WorldGenerator {
    private enum TileSetsEnum {
        // The value must match the position in the array of tilesets in the tilemap
        GrasslandsTextures = 0,
        
        ModernTerrainAndFences = 6,
        ModernCamping = 7,
    }

    private static TileSetController<TilePatterns> CreateTileSetController(TileMap grassland, Random random) {
        var ts = new TileSetController<TilePatterns>(grassland, Layers, Size, random);

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.None,
            SourceId = -1,
        });
        
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGreenWithYellowStone,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(0, 0, 5, 5)
        });

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGrassLight,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(5, 0, 5, 5)
        });

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGrass,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(10, 0, 5, 5)
        });

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGrassDark,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(15, 0, 5, 5)
        });
        
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureDirt,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(20, 0, 5, 5)
        });

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureTransparentPlant,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(10, 20, 4, 4)
        });
        
        
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureBigMud,
            SourceId = 1,
            AtlasCoords = new Rect2I(0, 0, 64, 64)
        });
        
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureSoil,
            SourceId = 2,
            AtlasCoords = new Rect2I(0, 0, 64, 64)
        });
        
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureBigSand,
            SourceId = 3,
            AtlasCoords = new Rect2I(0, 0, 7, 7)
        });
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureAsphaltLight,
            SourceId = 5,
            AtlasCoords = new Rect2I(0, 0, 29, 21)
        });
        
        
        
        
		
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernDirt,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(18,4,1, 1)
        });
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernGreen,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(1,2,1, 1)
        });
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernWater,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(5,7,1, 1)
        });
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernWater,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(5,7,1, 1)
        });
        return ts;
    }

    
}