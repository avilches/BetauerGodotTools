using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Core.Nodes; 

public static partial class Character2DExtensions {
    public static IEnumerable<T> GetFloorColliders<T>(this CharacterBody2D body2d, Vector2 upDirection, Func<T, bool>? predicate = null) where T : Node {
        return body2d.IsOnFloor() ? 
            GetColliders<T>(body2d, kinematicCollision2D => kinematicCollision2D.GetNormal().IsFloor(upDirection) && (predicate == null || predicate((T)kinematicCollision2D.GetCollider()))) : 
            Enumerable.Empty<T>();
    }

    public static IEnumerable<T> GetWallColliders<T>(this CharacterBody2D body2d, Vector2 upDirection, Func<T, bool>? predicate = null) where T : Node {
        return body2d.IsOnWall() ? 
            GetColliders<T>(body2d, kinematicCollision2D => kinematicCollision2D.GetNormal().IsWall(upDirection) && (predicate == null || predicate((T)kinematicCollision2D.GetCollider()))) : 
            Enumerable.Empty<T>();
    }

    public static IEnumerable<T> GetCeilingColliders<T>(this CharacterBody2D body2d, Vector2 upDirection, Func<T, bool>? predicate = null) where T : Node {
        return body2d.IsOnCeiling() ? 
            GetColliders<T>(body2d, kinematicCollision2D => kinematicCollision2D.GetNormal().IsCeiling(upDirection) && (predicate == null || predicate((T)kinematicCollision2D.GetCollider()))) : 
            Enumerable.Empty<T>();
    }

    public static IEnumerable<KinematicCollision2D> GetFloorCollisions(this CharacterBody2D body2d, Vector2 upDirection, Func<KinematicCollision2D, bool>? predicate = null) {
        return body2d.IsOnFloor() ? 
            GetCollisions(body2d, col => col.GetNormal().IsFloor(upDirection) && (predicate == null || predicate(col))) : 
            Enumerable.Empty<KinematicCollision2D>();
    }

    public static IEnumerable<KinematicCollision2D> GetWallCollisions(this CharacterBody2D body2d, Vector2 upDirection, Func<KinematicCollision2D, bool>? predicate = null) {
        return body2d.IsOnWall() ? 
            GetCollisions(body2d, col => col.GetNormal().IsWall(upDirection) && (predicate == null || predicate(col))) : 
            Enumerable.Empty<KinematicCollision2D>();
    }

    public static IEnumerable<KinematicCollision2D> GetCeilingCollisions(this CharacterBody2D body2d, Vector2 upDirection, Func<KinematicCollision2D, bool>? predicate = null) {
        return !body2d.IsOnCeiling() ? 
            Enumerable.Empty<KinematicCollision2D>() : 
            GetCollisions(body2d, col => col.GetNormal().IsCeiling(upDirection) && (predicate == null || predicate(col)));
    }

    public static IEnumerable<T> GetColliders<T>(this CharacterBody2D body2d, Func<T, bool> predicate) where T : Node {
        return GetColliders<T>(body2d, kinematicCollision2D => predicate((T)kinematicCollision2D.GetCollider()));
    }

    public static IEnumerable<T> GetColliders<T>(this CharacterBody2D body2d, Func<KinematicCollision2D, bool>? predicate = null) where T : Node {
        var slideCount = body2d.GetSlideCollisionCount();
        if (slideCount <= 0) yield break;
        for (var i = 0; i < slideCount; i++) {
            var kinematicCollision2D = body2d.GetSlideCollision(i);
            if (kinematicCollision2D.GetCollider() is T col && (predicate == null || predicate(kinematicCollision2D))) yield return col;
        }
    }

    public static IEnumerable<KinematicCollision2D> GetCollisions(this CharacterBody2D body2d, Func<KinematicCollision2D, bool>? predicate = null) {
        var slideCount = body2d.GetSlideCollisionCount();
        if (slideCount <= 0) yield break;
        for (var i = 0; i < slideCount; i++) {
            var kinematicCollision2D = body2d.GetSlideCollision(i);
            if (predicate == null || predicate(kinematicCollision2D)) yield return kinematicCollision2D;
        }
    }

}