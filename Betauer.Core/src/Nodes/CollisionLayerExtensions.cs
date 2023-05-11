using System;
using Godot;
using Betauer.Core.Signal;

namespace Betauer.Core.Nodes;

public static class CollisionLayerExtensions {
    /*
     * Areas y bodies can collide with multiple layers, so, signals don't distinguish between them.
     * This extension methods allow to detect collisions in a specific layer. So, the trick is check if the layer of collision objects matches the mask.
     */
     
    public static void OnAreaEntered(this Area2D area2D, int layer, Area2D.AreaEnteredEventHandler action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnAreaEntered((enteredArea2D) => {
            if (enteredArea2D.CanBeDetectedBy(layer)) action(enteredArea2D);
        }, oneShot, deferred);
    }

    public static void OnAreaExited(this Area2D area2D, int layer, Area2D.AreaExitedEventHandler action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnAreaExited(exitedArea2D => {
            if (exitedArea2D.CanBeDetectedBy(layer)) action(exitedArea2D);
        }, oneShot, deferred);
    }

    public static void OnAreaShapeEntered(this Area2D area2D, int layer, Area2D.AreaShapeEnteredEventHandler action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnAreaShapeEntered((rid, enteredArea2D, areaShapeIndex, localShapeIndex) => {
            if (enteredArea2D.CanBeDetectedBy(layer)) action(rid, enteredArea2D, areaShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnAreaShapeExited(this Area2D area2D, int layer, Area2D.AreaShapeExitedEventHandler action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnAreaShapeExited((rid, exitedArea2D, areaShapeIndex, localShapeIndex) => {
            if (exitedArea2D.CanBeDetectedBy(layer)) action(rid, exitedArea2D, areaShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyEntered(this Area2D area2D, int layer, Action<Node2D> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyEntered((node2D) => {
            if (CanBeDetectedBy(node2D, layer)) action(node2D);
        }, oneShot, deferred);
    }

    public static void OnBodyEntered(this Area2D area2D, int layer, Action<TileMap> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyEntered((node2D) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        }, oneShot, deferred);
    }

    public static void OnBodyEntered(this Area2D area2D, int layer, Action<PhysicsBody2D> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyEntered((node2D) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        }, oneShot, deferred);
    }

    public static void OnBodyExited(this Area2D area2D, int layer, Area2D.BodyExitedEventHandler action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyExited((node2D) => {
            if (CanBeDetectedBy(node2D, layer)) action(node2D);
        }, oneShot, deferred);;
    }

    public static void OnBodyExited(this Area2D area2D, int layer, Action<TileMap> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyExited((node2D) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        }, oneShot, deferred);
    }

    public static void OnBodyExited(this Area2D area2D, int layer, Action<PhysicsBody2D> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyExited((node2D) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeEntered(this Area2D area2D, int layer, Action<Rid, Node2D, long, long> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyShapeEntered((rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node2D, layer)) action(rid, node2D, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeEntered(this Area2D area2D, int layer, Action<Rid, PhysicsBody2D, long, long> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyShapeEntered((rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeEntered(this Area2D area2D, int layer, Action<Rid, TileMap, long, long> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyShapeEntered((rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeExited(this Area2D area2D, int layer, Area2D.BodyShapeExitedEventHandler action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyShapeExited((rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node2D, layer)) action(rid, node2D, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);;
    }

    public static void OnBodyShapeExited(this Area2D area2D, int layer, Action<Rid, PhysicsBody2D, long, long> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyShapeExited((rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeExited(this Area2D area2D, int layer, Action<Rid, TileMap, long, long> action, bool oneShot = false, bool deferred = false) {
        area2D.DetectLayer(layer);
        area2D.OnBodyShapeExited((rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }
    
    public static void OnBodyEntered(this RigidBody2D rigidBody2D, int layer, Action<Node> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyEntered((node) => {
            if (CanBeDetectedBy(node, layer)) action(node);
        }, oneShot, deferred);
    }

    public static void OnBodyEntered(this RigidBody2D rigidBody2D, int layer, Action<PhysicsBody2D> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyEntered((node) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        }, oneShot, deferred);
    }

    public static void OnBodyEntered(this RigidBody2D rigidBody2D, int layer, Action<TileMap> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyEntered((node) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        }, oneShot, deferred);
    }

    public static void OnBodyExited(this RigidBody2D rigidBody2D, int layer, RigidBody2D.BodyExitedEventHandler action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyExited((node) => {
            if (CanBeDetectedBy(node, layer)) action(node);
        }, oneShot, deferred);
    }

    public static void OnBodyExited(this RigidBody2D rigidBody2D, int layer, Action<PhysicsBody2D> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyExited((node) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        }, oneShot, deferred);
    }

    public static void OnBodyExited(this RigidBody2D rigidBody2D, int layer, Action<TileMap> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyExited((node) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeEntered(this RigidBody2D rigidBody2D, int layer, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node, layer)) action(rid, node, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeEntered(this RigidBody2D rigidBody2D, int layer, Action<Rid, TileMap, long, long> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeEntered(this RigidBody2D rigidBody2D, int layer, Action<Rid, PhysicsBody2D, long, long> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyShapeEntered((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeExited(this RigidBody2D rigidBody2D, int layer, Action<Rid, Node, long, long> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node, layer)) action(rid, node, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeExited(this RigidBody2D rigidBody2D, int layer, Action<Rid, TileMap, long, long> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        }, oneShot, deferred);
    }

    public static void OnBodyShapeExited(this RigidBody2D rigidBody2D, int layer, Action<Rid, PhysicsBody2D, long, long> action, bool oneShot = false, bool deferred = false) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.OnBodyShapeExited((rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
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
        var mask = EnableLayer(tileMap.TileSet.GetPhysicsLayerCollisionLayer(tileSetPhysicsLayer), layer);
        tileMap.TileSet.SetPhysicsLayerCollisionLayer(tileSetPhysicsLayer, mask);
    }

    public static void RemoveFromLayer(this CollisionObject2D o, int layer) =>
        o.SetCollisionLayerValue(layer, false);

    public static void RemoveFromLayer(this CollisionObject3D o, int layer) =>
        o.SetCollisionLayerValue(layer, false);

    public static void RemoveFromLayer(this GridMap o, int layer) =>
        o.SetCollisionLayerValue(layer, false);

    public static void RemoveFromLayer(this TileMap tileMap, int tileSetPhysicsLayer, int layer) {
        var mask = DisableLayer(tileMap.TileSet.GetPhysicsLayerCollisionLayer(tileSetPhysicsLayer), layer);
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

    public static void DetectLayer(this PhysicsPointQueryParameters2D o, int layer) {
        o.CollisionMask = EnableLayer(o.CollisionMask, layer);
    }

    public static void DetectLayer(this PhysicsRayQueryParameters2D o, int layer) {
        o.CollisionMask = EnableLayer(o.CollisionMask, layer);
    }

    public static void DetectLayer(this PhysicsShapeQueryParameters2D o, int layer) {
        o.CollisionMask = EnableLayer(o.CollisionMask, layer);
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
        var mask = EnableLayer(tileMap.TileSet.GetPhysicsLayerCollisionMask(tileSetPhysicsLayer), layer);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(tileSetPhysicsLayer, mask);
    }

    public static void IgnoreLayer(this CollisionObject2D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this CollisionObject3D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this RayCast2D o, int layer) =>
        o.SetCollisionMaskValue(layer, false);

    public static void IgnoreLayer(this PhysicsPointQueryParameters2D o, int layer) {
        o.CollisionMask = DisableLayer(o.CollisionMask, layer);
    }

    public static void IgnoreLayer(this PhysicsRayQueryParameters2D o, int layer) {
        o.CollisionMask = DisableLayer(o.CollisionMask, layer);
    }

    public static void IgnoreLayer(this PhysicsShapeQueryParameters2D o, int layer) {
        o.CollisionMask = DisableLayer(o.CollisionMask, layer);
    }

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
        var mask = DisableLayer(tileMap.TileSet.GetPhysicsLayerCollisionMask(tileSetPhysicsLayer), layer);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(tileSetPhysicsLayer, mask);
    }
    
    private static bool CanBeDetectedBy(Node node, int layer) {
        return node switch {
            PhysicsBody2D physicsBody2D => CanBeDetectedBy(physicsBody2D, layer),
            TileMap tileMap => CanBeDetectedBy(tileMap, layer),
            _ => throw new Exception("Documentation says only TileMap and PhysicsBody2D!!")
        };
    }

    public static bool CanBeDetectedBy(this CollisionObject2D physicsBody2D, int layer) {
        return HasLayerEnabled(physicsBody2D.CollisionLayer, layer);
    }
    
    public static bool CanBeDetectedBy(this TileMap tileMap, int layer) {
        var physicsLayersCount = tileMap.TileSet.GetPhysicsLayersCount();
        for (var physicsLayer = 0; physicsLayer < physicsLayersCount; physicsLayer++) {
            var mask = tileMap.TileSet.GetPhysicsLayerCollisionMask(physicsLayer);
            if (HasLayerEnabled(mask, layer)) return true;
        }
        return false;
    }

    private static bool HasLayerEnabled(uint mask, int layer) => BitTools.HasBit((int)mask, layer);
    private static uint EnableLayer(uint mask, int layer) => (uint)BitTools.EnableBit((int)mask, layer);
    private static uint DisableLayer(uint mask, int layer) => (uint)BitTools.DisableBit((int)mask, layer);
}