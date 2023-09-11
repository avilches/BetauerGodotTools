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
    TransparentGrass,
    TransparentAsphalt,
    
    
	
    ModernDirt,
    ModernGreen,
    ModernWater,
    ModernDeepWater,
    
    
    TreesBigLight,
    TreesBigDark,
    TreesBigHangingLight,
    TreesBigHangingDark,
}



public partial class WorldGenerator {
    private enum TileSetsEnum {
        // The value must match the position in the array of tilesets in the tilemap
        GrasslandsTextures = 0,
        
        ModernTerrainAndFences = 6,
        ModernCamping = 7,
    }

    private static TileSetController<TilePatterns> CreateTileSetController(TileMap grassland, Random random) {
        var ts = new TileSetController<TilePatterns>(grassland, Layers, GridSize, random);

        ts.Add(TilePatterns.None, new TilePattern<TilePatterns> {
            SourceId = -1,
        });
        
        ts.Add(TilePatterns.TextureGreenWithYellowStone, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(0, 0, 5, 5)
        });

        ts.Add(TilePatterns.TextureGrassLight, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(5, 0, 5, 5)
        });

        ts.Add(TilePatterns.TextureGrass, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(10, 0, 5, 5)
        });

        ts.Add(TilePatterns.TextureGrassDark, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(15, 0, 5, 5)
        });
        
        ts.Add(TilePatterns.TextureDirt, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(20, 0, 5, 5)
        });

        ts.Add(TilePatterns.TextureTransparentPlant, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(10, 20, 4, 4)
        });
        
        
        ts.Add(TilePatterns.TextureBigMud, new TilePattern<TilePatterns> {
            SourceId = 1,
            AtlasCoords = new Rect2I(0, 0, 64, 64)
        });
        
        ts.Add(TilePatterns.TextureSoil, new TilePattern<TilePatterns> {
            SourceId = 2,
            AtlasCoords = new Rect2I(0, 0, 64, 64)
        });
        
        ts.Add(TilePatterns.TextureBigSand, new TilePattern<TilePatterns> {
            SourceId = 3,
            AtlasCoords = new Rect2I(0, 0, 7, 7)
        });
        
        ts.Add(TilePatterns.TransparentGrass, new TilePattern<TilePatterns> {
            SourceId = 4,
            AtlasCoords = new Rect2I(0, 0, 8, 8)
        });
        
        ts.Add(TilePatterns.TransparentAsphalt, new TilePattern<TilePatterns> {
            SourceId = 5,
            AtlasCoords = new Rect2I(0, 0, 29, 21)
        });
        
        
        
        
		
        ts.Add(TilePatterns.ModernDirt, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(18,4,1, 1)
        });
        ts.Add(TilePatterns.ModernGreen, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(1,2,1, 1)
        });
        ts.Add(TilePatterns.ModernWater, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(5,7,1, 1)
        });
        ts.Add(TilePatterns.ModernWater, new TilePattern<TilePatterns> {
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(5,7,1, 1)
        });

        return ts;
    }

    
}