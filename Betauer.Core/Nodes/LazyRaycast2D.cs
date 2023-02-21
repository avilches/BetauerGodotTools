using System;
using Godot;

namespace Betauer.Core.Nodes;

public class LazyRaycast2D {
    public RaycastCollision Collision = new(null);
    public PhysicsRayQueryParameters2D Query = new();
    public PhysicsDirectSpaceState2D DirectSpaceState;

    public LazyRaycast2D(PhysicsDirectSpaceState2D directSpaceState) {
        DirectSpaceState = directSpaceState;
    }
    
    public LazyRaycast2D(CanvasItem canvasItem) {
        DirectSpaceState = canvasItem.GetWorld2D().DirectSpaceState;
    }

    public LazyRaycast2D From(Vector2 from) {
        Query.From = from;
        return this;
    }
    
    public LazyRaycast2D From(Node2D from) {
        Query.From = from.GlobalPosition;
        return this;
    }
    
    public LazyRaycast2D To(Vector2 from) {
        Query.To = from;
        return this;
    }
    
    public LazyRaycast2D To(Node2D from) {
        Query.To = from.GlobalPosition;
        return this;
    }
    
    public LazyRaycast2D Config(Action<PhysicsRayQueryParameters2D> config) {
        config(Query);
        return this;
    }
    
    public LazyRaycast2D Cast() {
        Collision.Recycle(DirectSpaceState.IntersectRay(Query));
        return this;
    }
    
}