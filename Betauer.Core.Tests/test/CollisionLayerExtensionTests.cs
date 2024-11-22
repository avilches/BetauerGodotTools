using Betauer.Core.Nodes;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestFixture]
public class CollisionLayerExtensionTests {

    [Test]
    public void TileMaskTests() {

        var tilemap = new TileMap();
        var tile = new TileSet();
        tile.AddPhysicsLayer();
        tilemap.TileSet = tile;
        var area = new Area2D();

        area.CollisionMask = 0;
        tile.SetPhysicsLayerCollisionMask(0, 0);
        Assert.That(tile.GetPhysicsLayerCollisionMask(0), Is.EqualTo(area.CollisionMask));
        
        area.DetectLayer(2);                
        tilemap.DetectLayer(0, 2);                
        Assert.That(tile.GetPhysicsLayerCollisionMask(0), Is.EqualTo(area.CollisionMask));

        area.DetectLayer(7);                
        tilemap.DetectLayer(0, 7);                
        Assert.That(tile.GetPhysicsLayerCollisionMask(0), Is.EqualTo(area.CollisionMask));

        area.IgnoreLayer(7);                
        tilemap.IgnoreLayer(0, 7);                
        Assert.That(tile.GetPhysicsLayerCollisionMask(0), Is.EqualTo(area.CollisionMask));

        area.IgnoreLayer(5);                
        tilemap.IgnoreLayer(0, 5);                
        Assert.That(tile.GetPhysicsLayerCollisionMask(0), Is.EqualTo(area.CollisionMask));

    }
    
    [Test]
    public void TileLayerTests() {

        var tilemap = new TileMap();
        var tile = new TileSet();
        tile.AddPhysicsLayer();
        tilemap.TileSet = tile;
        var area = new Area2D();

        area.CollisionLayer = 0;
        tile.SetPhysicsLayerCollisionLayer(0, 0);
        Assert.That(tile.GetPhysicsLayerCollisionLayer(0), Is.EqualTo(area.CollisionLayer));
        
        area.AddToLayer(2);                
        tilemap.AddToLayer(0, 2);                
        Assert.That(tile.GetPhysicsLayerCollisionLayer(0), Is.EqualTo(area.CollisionLayer));

        area.AddToLayer(7);                
        tilemap.AddToLayer(0, 7);                
        Assert.That(tile.GetPhysicsLayerCollisionLayer(0), Is.EqualTo(area.CollisionLayer));

        area.RemoveFromLayer(7);                
        tilemap.RemoveFromLayer(0, 7);                
        Assert.That(tile.GetPhysicsLayerCollisionLayer(0), Is.EqualTo(area.CollisionLayer));

        area.RemoveFromLayer(5);                
        tilemap.RemoveFromLayer(0, 5);                
        Assert.That(tile.GetPhysicsLayerCollisionLayer(0), Is.EqualTo(area.CollisionLayer));

    }
    
    
}