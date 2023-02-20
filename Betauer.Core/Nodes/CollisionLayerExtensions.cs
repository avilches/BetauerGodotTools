using System;
using Godot;
using Godot.Collections;

namespace Betauer.Core.Nodes;

public static class CollisionLayerExtensions {
    public static void OnAreaEntered(this Area2D area2D, int layer, Area2D.AreaEnteredEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaEntered += action;
    }

    public static void OnAreaExited(this Area2D area2D, int layer, Area2D.AreaExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaExited += action;
    }

    public static void OnAreaShapeEntered(this Area2D area2D, int layer, Area2D.AreaShapeEnteredEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaShapeEntered += action;
    }

    public static void OnAreaShapeExited(this Area2D area2D, int layer, Area2D.AreaShapeExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.AreaShapeExited += action;
    }

    public static void OnBodyEntered(this Area2D area2D, int layer, Area2D.BodyEnteredEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.BodyEntered += action;
    }

    public static void OnBodyExited(this Area2D area2D, int layer, Area2D.BodyExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.BodyExited += action;
    }

    public static void OnBodyShapeEntered(this Area2D area2D, int layer, Area2D.BodyShapeEnteredEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeEntered += action;
    }

    public static void OnBodyShapeExited(this Area2D area2D, int layer, Area2D.BodyShapeExitedEventHandler action) {
        area2D.DetectLayer(layer);
        area2D.BodyShapeExited += action;
    }

    public static void OnBodyEntered(this RigidBody2D rigidBody2D, int layer, RigidBody2D.BodyEnteredEventHandler action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyEntered += action;
    }

    public static void OnBodyExited(this RigidBody2D rigidBody2D, int layer, RigidBody2D.BodyExitedEventHandler action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyExited += action;
    }

    public static void OnBodyShapeEntered(this RigidBody2D rigidBody2D, int layer, RigidBody2D.BodyShapeEnteredEventHandler action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeEntered += action;
    }

    public static void OnBodyShapeExited(this RigidBody2D rigidBody2D, int layer, RigidBody2D.BodyShapeExitedEventHandler action) {
        rigidBody2D.DetectLayer(layer);
        rigidBody2D.BodyShapeExited += action;
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

    public static void DetectLayer(this PhysicsPointQueryParameters2D o, int layer) {
        o.CollisionMask = BitTools.EnableBit(o.CollisionMask, (uint)layer);
    }

    public static void DetectLayer(this PhysicsRayQueryParameters2D o, int layer) {
        o.CollisionMask = BitTools.EnableBit(o.CollisionMask, (uint)layer);
    }

    public static void DetectLayer(this PhysicsShapeQueryParameters2D o, int layer) {
        o.CollisionMask = BitTools.EnableBit(o.CollisionMask, (uint)layer);
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

    public static void IgnoreLayer(this PhysicsPointQueryParameters2D o, int layer) {
        o.CollisionMask = BitTools.DisableBit(o.CollisionMask, (uint)layer);
    }

    public static void IgnoreLayer(this PhysicsRayQueryParameters2D o, int layer) {
        o.CollisionMask = BitTools.DisableBit(o.CollisionMask, (uint)layer);
    }

    public static void IgnoreLayer(this PhysicsShapeQueryParameters2D o, int layer) {
        o.CollisionMask = BitTools.DisableBit(o.CollisionMask, (uint)layer);
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
        var mask = BitTools.DisableBit(tileMap.TileSet.GetPhysicsLayerCollisionMask(tileSetPhysicsLayer), (uint)layer);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(tileSetPhysicsLayer, mask);
    }
}