using System;
using Godot;

namespace Betauer.Core.Nodes;

public static class CollisionLayerExtensions {
    public static void OnAreaEntered(this Area2D area2D, int layer, Area2D.AreaEnteredEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaEntered += enteredArea2D => {
            if (enteredArea2D.CanBeDetectedBy(layer)) action(enteredArea2D);
        };
    }

    public static void OnAreaExited(this Area2D area2D, int layer, Area2D.AreaExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaExited += exitedArea2D => {
            if (exitedArea2D.CanBeDetectedBy(layer)) action(exitedArea2D);
        };
    }

    public static void OnAreaShapeEntered(this Area2D area2D, int layer, Area2D.AreaShapeEnteredEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaShapeEntered += (rid, enteredArea2D, areaShapeIndex, localShapeIndex) => {
            if (enteredArea2D.CanBeDetectedBy(layer)) action(rid, enteredArea2D, areaShapeIndex, localShapeIndex);
        };
    }

    public static void OnAreaShapeExited(this Area2D area2D, int layer, Area2D.AreaShapeExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaShapeExited += (rid, exitedArea2D, areaShapeIndex, localShapeIndex) => {
            if (exitedArea2D.CanBeDetectedBy(layer)) action(rid, exitedArea2D, areaShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyEntered(this Area2D area2D, int layer, Action<Node2D> action) {
        area2D.DetectLayer(layer);
        area2D.BodyEntered += (node2D) => {
            if (CanBeDetectedBy(node2D, layer)) action(node2D);
        };
    }

    public static void OnBodyEntered(this Area2D area2D, int layer, Action<TileMap> action) {
        area2D.DetectLayer(layer);
        area2D.BodyEntered += (node2D) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        };
    }

    public static void OnBodyEntered(this Area2D area2D, int layer, Action<PhysicsBody2D> action) {
        area2D.DetectLayer(layer);
        area2D.BodyEntered += (node2D) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        };
    }

    public static void OnBodyExited(this Area2D area2D, int layer, Area2D.BodyExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.BodyExited += (node2D) => {
            if (CanBeDetectedBy(node2D, layer)) action(node2D);
        };;
    }

    public static void OnBodyExited(this Area2D area2D, int layer, Action<TileMap> action) {
        area2D.DetectLayer(layer);
        area2D.BodyExited += (node2D) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        };
    }

    public static void OnBodyExited(this Area2D area2D, int layer, Action<PhysicsBody2D> action) {
        area2D.DetectLayer(layer);
        area2D.BodyExited += (node2D) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        };
    }

    public static void OnBodyShapeEntered(this Area2D area2D, int layer, Action<Rid, Node2D, long, long> action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeEntered += (rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node2D, layer)) action(rid, node2D, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeEntered(this Area2D area2D, int layer, Action<Rid, PhysicsBody2D, long, long> action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeEntered += (rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeEntered(this Area2D area2D, int layer, Action<Rid, TileMap, long, long> action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeEntered += (rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeExited(this Area2D area2D, int layer, Area2D.BodyShapeExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeExited += (rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node2D, layer)) action(rid, node2D, bodyShapeIndex, localShapeIndex);
        };;
    }

    public static void OnBodyShapeExited(this Area2D area2D, int layer, Action<Rid, PhysicsBody2D, long, long> action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeExited += (rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeExited(this Area2D area2D, int layer, Action<Rid, TileMap, long, long> action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeExited += (rid, node2D, bodyShapeIndex, localShapeIndex) => {
            if (node2D is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        };
    }
    
    public static void OnBodyEntered(this RigidBody2D rigidBody2D, int layer, Action<Node> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyEntered += (node) => {
            if (CanBeDetectedBy(node, layer)) action(node);
        };
    }

    public static void OnBodyEntered(this RigidBody2D rigidBody2D, int layer, Action<PhysicsBody2D> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyEntered += (node) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        };
    }

    public static void OnBodyEntered(this RigidBody2D rigidBody2D, int layer, Action<TileMap> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyEntered += (node) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        };
    }

    public static void OnBodyExited(this RigidBody2D rigidBody2D, int layer, RigidBody2D.BodyExitedEventHandler action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyExited += (node) => {
            if (CanBeDetectedBy(node, layer)) action(node);
        };
    }

    public static void OnBodyExited(this RigidBody2D rigidBody2D, int layer, Action<PhysicsBody2D> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyExited += (node) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(physicsBody2D);
        };
    }

    public static void OnBodyExited(this RigidBody2D rigidBody2D, int layer, Action<TileMap> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyExited += (node) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(tileMap);
        };
    }

    public static void OnBodyShapeEntered(this RigidBody2D rigidBody2D, int layer, Action<Rid, Node, long, long> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeEntered += (rid, node, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node, layer)) action(rid, node, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeEntered(this RigidBody2D rigidBody2D, int layer, Action<Rid, TileMap, long, long> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeEntered += (rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeEntered(this RigidBody2D rigidBody2D, int layer, Action<Rid, PhysicsBody2D, long, long> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeEntered += (rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeExited(this RigidBody2D rigidBody2D, int layer, Action<Rid, Node, long, long> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeExited += (rid, node, bodyShapeIndex, localShapeIndex) => {
            if (CanBeDetectedBy(node, layer)) action(rid, node, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeExited(this RigidBody2D rigidBody2D, int layer, Action<Rid, TileMap, long, long> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeExited += (rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is TileMap tileMap && tileMap.CanBeDetectedBy(layer)) action(rid, tileMap, bodyShapeIndex, localShapeIndex);
        };
    }

    public static void OnBodyShapeExited(this RigidBody2D rigidBody2D, int layer, Action<Rid, PhysicsBody2D, long, long> action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeExited += (rid, node, bodyShapeIndex, localShapeIndex) => {
            if (node is PhysicsBody2D physicsBody2D && physicsBody2D.CanBeDetectedBy(layer)) action(rid, physicsBody2D, bodyShapeIndex, localShapeIndex);
        };
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