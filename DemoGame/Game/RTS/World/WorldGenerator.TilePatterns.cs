using System;
using Godot;

namespace Veronenger.Game.RTS.World; 

public enum TilePatterns {
    TextureGreenWithYellowStone,
    TextureGreen2,
    TextureGreen3,
    TextureGreen4,
	
    ModernDirt,
    ModernGreen,
    ModernWater,
    ModernDeepWater,
}



public partial class WorldGenerator {
    private enum TileSetsEnum {
        // The value must match the position in the array of tilesets in the tilemap
        GrasslandsTextures = 0,
        
        ModernTerrainAndFences = 0,
        ModernCamping = 1,
    }

    private static TileSetController<TilePatterns> CreateTileSetController(TileMap grassland, TileMap modern, Random random) {
        var ts = new TileSetController<TilePatterns>(Size, random);

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGreenWithYellowStone,
            TileMap = grassland,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(0, 0, 5, 5)
        });

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGreen2,
            TileMap = grassland,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(5, 0, 5, 5)
        });

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGreen3,
            TileMap = grassland,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(10, 0, 5, 5)
        });

        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.TextureGreen4,
            TileMap = grassland,
            SourceId = (int)TileSetsEnum.GrasslandsTextures,
            AtlasCoords = new Rect2I(15, 0, 5, 5)
        });
        
        
        
        
		
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernDirt,
            TileMap = modern,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(18,4,1, 1)
        });
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernGreen,
            TileMap = modern,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(1,2,1, 1)
        });
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernWater,
            TileMap = modern,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(5,7,1, 1)
        });
        ts.Add(new TilePattern<TilePatterns> {
            Key = TilePatterns.ModernWater,
            TileMap = modern,
            SourceId = (int)TileSetsEnum.ModernCamping,
            AtlasCoords = new Rect2I(5,7,1, 1)
        });
        return ts;
    }

    
}