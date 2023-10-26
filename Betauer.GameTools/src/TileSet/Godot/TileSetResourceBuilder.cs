using System;
using System.IO;
using Betauer.TileSet.Image;
using Godot;

namespace Betauer.TileSet.Godot;

public class TileSetResourceBuilder {
    private readonly global::Godot.TileSet _godotTileSet;
    
    public TileSetResourceBuilder(Vector2I cellSize) {
        _godotTileSet = new global::Godot.TileSet {
            TileSize = cellSize,
        };
    }

    public abstract class TileSetTerrainBuilder {
        protected readonly TileSetResourceBuilder ResourceBuilder;
        protected readonly ITileSetLayout Layout;
        protected readonly string ResourceName;
        protected readonly global::Godot.Image.Format? Format;

        protected TileSetImage? TileSetImage;
        protected int SourceIdVal = -1;

        protected TileSetTerrainBuilder(TileSetResourceBuilder resourceBuilder, ITileSetLayout layout, string resourceName, global::Godot.Image.Format? format = null) {
            ResourceBuilder = resourceBuilder;
            Layout = layout;
            ResourceName = resourceName;
            Format = format;
        }

        public virtual void Add(Color color, string name, int terrainSet = 0) {
            var terrainIndex = ResourceBuilder.AddTerrain(color, name, terrainSet);
            ResourceBuilder.AddTileSetAtlasSource(TileSetImage, ResourceName, SourceIdVal, terrainSet, terrainIndex);
        }
    }

    public class UseTileSetTerrainBuilder : TileSetTerrainBuilder {
        public UseTileSetTerrainBuilder(TileSetResourceBuilder resourceBuilder, ITileSetLayout layout, string resourceName, global::Godot.Image.Format? format = null) :
            base(resourceBuilder, layout, resourceName, format) {
        }
        
        public UseTileSetTerrainBuilder CopyFrom(string sourceImagePath) {
            File.Copy(sourceImagePath, ResourceName, true);
            return this;
        }

        public UseTileSetTerrainBuilder SourceId(int sourceId) {
            SourceIdVal = sourceId;
            return this;
        }

        public override void Add(Color color, string name, int terrainSet = 0) {
            TileSetImage = new TileSetImage(ResourceName, Layout, Format);
            base.Add(color, name, terrainSet);
        }
    }

    public class CreateTileSetTerrainBuilder : TileSetTerrainBuilder {
        public CreateTileSetTerrainBuilder(TileSetResourceBuilder resourceBuilder, ITileSetLayout layout, string resourceName, global::Godot.Image.Format? format = null) :
            base(resourceBuilder, layout, resourceName, format) {
        }

        public CreateTileSetTerrainBuilder From(ITileSetLayout sourceLayout, string sourceImagePath, global::Godot.Image.Format? format = null) {
            var tileSetImage = new TileSetImage(sourceImagePath, sourceLayout, format);
            TileSetImage = tileSetImage.ExportAs(Layout, TileSetImage.Blob47Rules);
            TileSetImage.SavePng(ResourceName);
            return this;
        }

        public CreateTileSetTerrainBuilder SourceId(int sourceId) {
            SourceIdVal = sourceId;
            return this;
        }
        public override void Add(Color color, string name, int terrainSet = 0) {
            if (TileSetImage == null) {
                throw new Exception("TileSetImage is null. Call to From() in order to generate and save the TileSet image.");
            }
            base.Add(color, name, terrainSet);
        }
    }

    public CreateTileSetTerrainBuilder CreateTerrain(ITileSetLayout layout, string resourceName, global::Godot.Image.Format? format = null) {
        return new CreateTileSetTerrainBuilder(this, layout, resourceName, format);
    }

    public UseTileSetTerrainBuilder UseTerrain(ITileSetLayout layout, string resourceName, global::Godot.Image.Format? format = null) {
        return new UseTileSetTerrainBuilder(this, layout, resourceName, format);
    }

    private int AddTerrain(Color color, string name, int terrainSet) {
        while (_godotTileSet.GetTerrainSetsCount() < terrainSet + 1) {
            _godotTileSet.AddTerrainSet();
            _godotTileSet.SetTerrainSetMode(_godotTileSet.GetTerrainSetsCount() - 1, global::Godot.TileSet.TerrainMode.CornersAndSides);
        }
        
        _godotTileSet.AddTerrain(terrainSet);
        var terrainIndex = _godotTileSet.GetTerrainsCount(terrainSet) - 1;
        _godotTileSet.SetTerrainColor(terrainSet, terrainIndex, color);
        _godotTileSet.SetTerrainName(terrainSet, terrainIndex, name);
        return terrainIndex;
    }

    private void AddTileSetAtlasSource(TileSetImage tileSetImage, string pngPath, int sourceId, int terrainSet, int terrainIndex) {
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
    
        if (sourceId != -1 && _godotTileSet.HasSource(sourceId)) {
            throw new Exception($"SourceId {sourceId} already exists");
        }
        
        var createdSourceId = _godotTileSet.AddSource(tileSetAtlasSource);
        if (sourceId != -1 && createdSourceId != sourceId) {
            _godotTileSet.SetSourceId(createdSourceId, sourceId);
        }
    }

    public void Save(string resourcePath) {
        File.Delete(resourcePath);
        ResourceSaver.Save(_godotTileSet, "res://"+resourcePath);
    }
}