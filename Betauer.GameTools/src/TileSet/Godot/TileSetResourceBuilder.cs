using System.IO;
using Betauer.TileSet.Image;
using Godot;

namespace Betauer.TileSet.Godot;

public class TileSetResourceBuilder {
    private readonly global::Godot.TileSet _godotTileSet;
    
    public TileSetResourceBuilder(Vector2I cellSize, int terrainSets = 1) {
        _godotTileSet = new global::Godot.TileSet {
            TileSize = cellSize,
        };
        for (var i = 0; i < terrainSets; i++) {
            _godotTileSet.AddTerrainSet();
            _godotTileSet.SetTerrainSetMode(i, global::Godot.TileSet.TerrainMode.CornersAndSides);
        }
    }

    public class TileSetTerrainBuilder {
        private readonly TileSetResourceBuilder _resourceBuilder;
        private readonly ITileSetLayout _layout;
        private readonly string _resourceName;
        private readonly global::Godot.Image.Format? _format;

        private TileSetImage? _tileSetImage;

        public TileSetTerrainBuilder(TileSetResourceBuilder resourceBuilder, ITileSetLayout layout, string resourceName, global::Godot.Image.Format? format = null) {
            _resourceBuilder = resourceBuilder;
            _layout = layout;
            _resourceName = resourceName;
            _format = format;
        }

        public TileSetTerrainBuilder From(ITileSetLayout sourceLayout, string sourceImagePath, global::Godot.Image.Format? format = null) {
            var tileSetImage = new TileSetImage(sourceImagePath, sourceLayout, format);
            _tileSetImage = tileSetImage.ExportAs(_layout, TileSetImage.Blob47Rules);
            _tileSetImage.SavePng(_resourceName);
            return this;
        }

        public void Add(Color color, string name, int terrainSet = 0) {
            var tileSetImage = _tileSetImage ?? new TileSetImage(_resourceName, _layout, _format);
            _resourceBuilder.AddTerrain(tileSetImage, _resourceName, color, name, terrainSet);
        }
    }
    
    public TileSetTerrainBuilder Terrain(ITileSetLayout layout, string resourceName, global::Godot.Image.Format? format = null) {
        return new TileSetTerrainBuilder(this, layout, resourceName, format);
    }


    public void AddTerrain(TileSetImage tileSetImage, string pngPath, Color color, string name, int terrainSet = 0) {
        _godotTileSet.AddTerrain(terrainSet);
        var terrainIndex = _godotTileSet.GetTerrainsCount(terrainSet) - 1;
        _godotTileSet.SetTerrainColor(terrainSet, terrainIndex, color);
        _godotTileSet.SetTerrainName(terrainSet, terrainIndex, name);

        var texture2D = tileSetImage.CreateTexture(pngPath);

        var tileSetAtlasSource = new TileSetAtlasSource {
            Texture = texture2D,
            Margins = Vector2I.Zero,
            TextureRegionSize = _godotTileSet.TileSize,
        };

        for (var y = 0; y < tileSetImage.Height; y++) {
            for (var x = 0; x < tileSetImage.Width; x++) {
                var tileId = tileSetImage.GetTileIdByPosition(x, y);
                if (tileId != -1) {
                    tileSetAtlasSource.CreateTile(new Vector2I(x, y));
                    var tileData = tileSetAtlasSource.GetTileData(new Vector2I(x, y), 0);
                    tileData.TerrainSet = terrainSet;
                    tileData.Terrain = terrainIndex;
                    tileData.SetTerrainMask(tileId, terrainIndex);
                }
            }
        }
        
        _godotTileSet.AddSource(tileSetAtlasSource);
        var sourceId = _godotTileSet.GetSourceCount() - 1;
    }

    public void Save(string resourcePath) {
        File.Delete(resourcePath);
        ResourceSaver.Save(_godotTileSet, "res://"+resourcePath);
    }
}