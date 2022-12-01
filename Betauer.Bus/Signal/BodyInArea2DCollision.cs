using System;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Bus.Signal;

public class BodyInArea2DCollision {
    private readonly int _layer;

    public BodyInArea2DCollision(int layer) {
        _layer = layer;
    }

    public void OnEnteredIn(Area2D area2D, Action<Node> onEnter) {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnBodyEntered(onEnter);
    }

    public void OnTileMapEnteredIn<T>(Area2D area2D, Action<T> onEnter) where T : TileMap {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnBodyEntered(node => {
            if (node is T t) onEnter(t);
        });
    }

    public void OnBodyEnteredIn<T>(Area2D area2D, Action<T> onEnter) where T : PhysicsBody2D {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnBodyEntered(node => {
            if (node is T t) onEnter(t);
        });
    }

    public void OnExitedIn(Area2D area2D, Action<Node> onEnter) {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnBodyExited(onEnter);
    }

    public void OnTileMapExitedIn<T>(Area2D area2D, Action<T> onEnter) where T : TileMap {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnBodyExited(node => {
            if (node is T t) onEnter(t);
        });
    }

    public void OnBodyExitedIn<T>(Area2D area2D, Action<T> onEnter) where T : PhysicsBody2D {
        area2D.SetCollisionMaskValue(_layer, true);
        area2D.OnBodyExited(node => {
            if (node is T t) onEnter(t);
        });
    }

    public void Detect(PhysicsBody2D body) {
        body.SetCollisionLayerValue(_layer, true);
    }

    public void Detect(TileMap tileMap, int tileSetPhysicsLayer) {
        // TODO: test
        var mask = tileMap.TileSet.GetPhysicsLayerCollisionLayer(tileSetPhysicsLayer);
        mask &= (uint)1 << _layer;
        tileMap.TileSet.SetPhysicsLayerCollisionLayer(tileSetPhysicsLayer, mask);
    }
}