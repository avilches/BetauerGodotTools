using System;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.Core.Nodes;

public static class CollisionLayerExtensions {
                
    public static SignalHandler OnAreaEntered(this Area2D area2D, int layer, Action<Area2D> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnAreaEntered(action, oneShot, deferred);
    }

    public static SignalHandler OnAreaExited(this Area2D area2D, int layer, Action<Area2D> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnAreaExited(action, oneShot, deferred);
    }

    public static SignalHandler OnAreaShapeEntered(this Area2D area2D, int layer, Action<RID, Area2D, int, int> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnAreaShapeEntered(action, oneShot, deferred);
    }

    public static SignalHandler OnAreaShapeExited(this Area2D area2D, int layer, Action<RID, Area2D, int, int> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnAreaShapeExited(action, oneShot, deferred);
    }

    public static SignalHandler OnNode2DEntered(this Area2D area2D, int layer, Action<Node2D> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnBodyEntered(action, oneShot, deferred);
    }

    public static SignalHandler OnNode2DExited(this Area2D area2D, int layer, Action<Node2D> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnBodyExited(action, oneShot, deferred);
    }

    public static SignalHandler OnNode2DShapeEntered(this Area2D area2D, int layer, Action<RID, Node2D, int, int> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnBodyShapeEntered(action, oneShot, deferred);
    }

    public static SignalHandler OnNode2DShapeExited(this Area2D area2D, int layer, Action<RID, Node2D, int, int> action, bool oneShot = false,
        bool deferred = false) {
        area2D.DetectLayer(layer);
        return area2D.OnBodyShapeExited(action, oneShot, deferred);
    }

    public static SignalHandler OnBodyEntered<T>(this Area2D area2D, int layer, Action<T> action, bool oneShot = false, bool deferred = false)
        where T : PhysicsBody2D {
        area2D.DetectLayer(layer);
        return area2D.OnBodyEntered(node => {
            if (node is T t) action(t);
        }, oneShot, deferred);
    }

    public static SignalHandler OnBodyExited<T>(this Area2D area2D, int layer, Action<T> action, bool oneShot = false, bool deferred = false)
        where T : PhysicsBody2D {
        area2D.DetectLayer(layer);
        return area2D.OnBodyExited(node => {
            if (node is T t) action(t);
        }, oneShot, deferred);
    }

    public static SignalHandler OnBodyShapeEntered<T>(this Area2D area2D, int layer, Action<RID, T, int, int> action, bool oneShot = false,
        bool deferred = false) where T : PhysicsBody2D {
        area2D.DetectLayer(layer);
        return area2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is T t) action(rid, t, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static SignalHandler OnBodyShapeExited<T>(this Area2D area2D, int layer, Action<RID, T, int, int> action, bool oneShot = false,
        bool deferred = false) where T : PhysicsBody2D {
        area2D.DetectLayer(layer);
        return area2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is T t) action(rid, t, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }


    public static SignalHandler OnTileMapEntered<T>(this Area2D area2D, int layer, Action<T> action, bool oneShot = false,
        bool deferred = false) where T : TileMap {
        area2D.DetectLayer(layer);
        return area2D.OnBodyEntered(node => {
            if (node is T t) action(t);
        }, oneShot, deferred);
    }

    public static SignalHandler OnTileMapExited<T>(this Area2D area2D, int layer, Action<T> action, bool oneShot = false,
        bool deferred = false) where T : TileMap {
        area2D.DetectLayer(layer);
        return area2D.OnBodyExited(node => {
            if (node is T t) action(t);
        }, oneShot, deferred);
    }

    public static SignalHandler OnTileMapShapeEntered<T>(this Area2D area2D, int layer, Action<RID, T, int, int> action, bool oneShot = false,
        bool deferred = false) where T : TileMap {
        area2D.DetectLayer(layer);
        return area2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is T t) action(rid, t, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static SignalHandler OnTileMapShapeExited<T>(this Area2D area2D, int layer, Action<RID, T, int, int> action, bool oneShot = false,
        bool deferred = false) where T : TileMap {
        area2D.DetectLayer(layer);
        return area2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is T t) action(rid, t, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void AddToLayer(this CollisionObject2D o, int layer) {
        o.SetCollisionLayerValue(layer, true);
    }

    public static void AddToLayer(this CollisionObject3D o, int layer) {
        o.SetCollisionLayerValue(layer, true);
    }

    public static void AddToLayer(this GridMap o, int layer) {
        o.SetCollisionLayerValue(layer, true);
    }

    public static void AddToLayer(this TileMap tileMap, int tileSetPhysicsLayer, int layer) {
        var mask = BitTools.EnableBit(tileMap.TileSet.GetPhysicsLayerCollisionLayer(tileSetPhysicsLayer), (uint)layer);
        tileMap.TileSet.SetPhysicsLayerCollisionLayer(tileSetPhysicsLayer, mask);
    }

    public static void RemoveFromLayer(this CollisionObject2D o, int layer) =>
        o.SetCollisionLayerValue(layer, false);

    public static void RemoveFromLayer(this CollisionObject3D o, int layer) =>
        o.SetCollisionLayerValue(layer, false);

    public static void RemoveFromLayer(this GridMap o, int layer) =>
        o.SetCollisionLayerValue(layer, false);

    public static void RemoveFromLayer(this TileMap tileMap, int tileSetPhysicsLayer, int layer) {
        var mask = BitTools.DisableBit(tileMap.TileSet.GetPhysicsLayerCollisionLayer(tileSetPhysicsLayer), (uint)layer);
        tileMap.TileSet.SetPhysicsLayerCollisionLayer(tileSetPhysicsLayer, mask);
    }

    public static void DetectLayer(this CollisionObject2D o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this CollisionObject3D o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this RayCast2D o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this RayCast3D o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this ShapeCast2D o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this ShapeCast3D o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this GridMap o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this NavigationMesh o, int layer) {
        o.SetCollisionMaskValue(layer, true);
    }

    public static void DetectLayer(this TileMap tileMap, int tileSetPhysicsLayer, int layer) {
        var mask = BitTools.EnableBit(tileMap.TileSet.GetPhysicsLayerCollisionMask(tileSetPhysicsLayer), (uint)layer);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(tileSetPhysicsLayer, mask);
    }

    public static void IgnoreLayer(this CollisionObject2D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this CollisionObject3D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this RayCast2D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this RayCast3D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this ShapeCast2D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this ShapeCast3D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this GridMap o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this NavigationMesh o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this TileMap tileMap, int tileSetPhysicsLayer, int layer) {
        var mask = BitTools.DisableBit(tileMap.TileSet.GetPhysicsLayerCollisionMask(tileSetPhysicsLayer), (uint)layer);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(tileSetPhysicsLayer, mask);
    }
    
}