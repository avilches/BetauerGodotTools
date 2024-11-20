using System;
using System.Collections.Generic;
using Betauer.Core.PCG.Examples.Metazelda.util;
using Godot;

namespace Betauer.Core.PCG.Examples.Metazelda.constraints;

public class ColorMap {
    
    protected int xsum, ysum, xmin, xmax, ymin, ymax;
    protected Dictionary<Vector2I, int> map;

    public ColorMap() {
        map = new Dictionary<Vector2I, int>();
        ymin = xmin = Int32.MaxValue;
        ymax = xmax = Int32.MinValue;
    }
    
    public void Set(int x, int y, int color) {
        Vector2I xy = new Vector2I(x,y);

        if (map.ContainsKey(xy) == false) {
            xsum += x;
            ysum += y;
        }
        map[xy] = color;
        
        if (x < xmin) xmin = x;
        if (x > xmax) xmax = x;
        if (y < ymin) ymin = y;
        if (y > ymax) ymax = y;
    }
    
    public int Get(int x, int y) {
        return map[new Vector2I(x,y)];
    }
    
    public Vector2I GetCenter() {
        return new Vector2I(xsum/map.Count, ysum/map.Count);
    }
    
    public int GetWidth() {
        return xmax-xmin+1;
    }
    
    public int GetHeight() {
        return ymax-ymin+1;
    }
    
    public int GetLeft() {
        return xmin;
    }
    
    public int GetTop() {
        return ymin;
    }
    
    public int GetRight() {
        return xmax;
    }
    
    public int GetBottom() {
        return ymax;
    }
    
    protected bool IsConnected() {
        if (map.Count == 0) return false;

        // Do a breadth first search starting at the top left to check if
        // every position is reachable.
        List<Vector2I> world = new List<Vector2I>(map.Keys);
        Queue<Vector2I> queue = new Queue<Vector2I>();

        Vector2I first = world[0];
        world.RemoveAt(0);
        queue.Enqueue(first);
        Vector2I[] directions = [ Vector2I.Up, Vector2I.Right, Vector2I.Down, Vector2I.Left ];
        
        while (queue.Count > 0) {
            Vector2I pos = queue.Dequeue();
            
            foreach (Vector2I d in directions) {
                Vector2I neighbor = pos + d;
                
                if (world.Contains(neighbor)) {
                    world.Remove(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return world.Count == 0;
    }
    
    public void CheckConnected() {
        if (!IsConnected()) {
            // Parts of the map are unreachable!
            throw new MZGenerationFailureException("ColorMap is not fully connected");
        }
    }
    
}