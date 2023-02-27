using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Betauer.Core.Nodes;

public class LazyRaycast2D {
    private PhysicsDirectSpaceState2D? _directSpaceState;

    public RaycastCollision Collision = new(null);
    public PhysicsRayQueryParameters2D Query = new();
    public CanvasItem? DirectSpaceStateAccessor;

    public LazyRaycast2D UseDirectSpace(PhysicsDirectSpaceState2D directSpaceState) {
        _directSpaceState = directSpaceState;
        return this;
    }

    public LazyRaycast2D GetDirectSpaceFrom(CanvasItem canvasItem) {
        DirectSpaceStateAccessor = canvasItem;
        return this;
    }
    
    public PhysicsDirectSpaceState2D DirectSpaceState => _directSpaceState ??= DirectSpaceStateAccessor!.GetWorld2D().DirectSpaceState; 

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
    
    /**
     * Not tested!!!!
     */
    private readonly Array<Rid> _excludeList = new();
    public readonly List<RaycastCollision> _collisions = new();
    public LazyRaycast2D ___MultipleCast(int max = 10) {
        _collisions.Clear();
        Collision.Recycle(DirectSpaceState.IntersectRay(Query));
        if (!Collision.IsColliding) return this;
        
        _collisions.Add(Collision);
        _excludeList.Clear();
        var backup = Query.Exclude;
        if (Query.Exclude.Count > 0) _excludeList.AddRange(Query.Exclude);
        Query.Exclude = _excludeList;
        var lastCollision = Collision;
        while (_collisions.Count < max) {
            _excludeList.Add(lastCollision.Rid);
            var data = DirectSpaceState.IntersectRay(Query);
            if (data.Count == 0 || _excludeList.Contains(data["rid"].AsRid())) break;
            _collisions.Add(new RaycastCollision(data));
        }
        Query.Exclude = backup;
        return this;
    }
}