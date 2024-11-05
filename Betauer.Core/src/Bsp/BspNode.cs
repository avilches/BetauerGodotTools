using System;
using Godot;

namespace Betauer.Core.Bsp;

public class BspNode {
    public readonly int X, Y, Width, Height;
    public BspNode? A { get; internal set; }
    public BspNode? B { get; internal set; }
    public Rect2I? Rect2I { get; internal set; }

    public BspNode(int x, int y, int width, int height) {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public bool Split(int tries, Func<BspNode, int, (bool horizontal, float splitBy)> splitter, int depth, int minRoomHeight, int minRoomWidth, float maxRatio) {
        Console.WriteLine("[" + depth + "] Splitting node " + X + "/" + Y + " Size:" + Width + "/" + Height);
        while (--tries >= 0) {
            var (splitHorizontally, splitBy) = splitter(this, depth);
            if (splitHorizontally) {
                var newHeight = Mathf.RoundToInt(Height * splitBy);
                Console.WriteLine("- Splitting height " + Height + " horizontally by " + splitBy + ". New height: " + newHeight);
                A = new BspNode(X, Y, Width, newHeight);
                B = new BspNode(X, Y + newHeight, Width, Height - newHeight);
            } else {
                var newWidth = Mathf.RoundToInt(Width * splitBy);
                Console.WriteLine("- Splitting width " + Width + " vertically by " + splitBy + ". New width: " + newWidth);
                A = new BspNode(X, Y, newWidth, Height);
                B = new BspNode(X + newWidth, Y, Width - newWidth, Height);
            }

            // If the space for a potential room in both nodes is valid, we can stop the loop and continue the split
            if (A.Width - 1 >= minRoomWidth &&
                B.Width - 1 >= minRoomWidth &&
                A.Height - 1 >= minRoomHeight &&
                B.Height - 1 >= minRoomHeight) {
                var ratioA = (float)Math.Max(A.Width - 1, A.Height - 1) / Math.Min(A.Width - 1, A.Height - 1);
                var ratioB = (float)Math.Max(B.Width - 1, B.Height - 1) / Math.Min(B.Width - 1, B.Height - 1);
                if (ratioA <= maxRatio && ratioB <= maxRatio) {
                    return true;
                } else {
                    Console.WriteLine("!!! Invalid ratio A: " + ratioA + " B: " + ratioB);
                }
            } else {
                Console.WriteLine("!!! Invalid room size less than "+minRoomWidth+"/"+minRoomHeight+". A: " + (A.Width - 1) + "/" + (A.Height - 1) + " B: " + (B.Width - 1) + "/" + (B.Height - 1));
            }
        }
        Console.WriteLine("!!! Impossible to split node " + X + "/" + Y + " Size: " + Width + "/" + Height);
        A = null;
        B = null;
        return false;
    }

    public void CreateRoom(Func<int,int,int,int,Rect2I> createRoom) {
        Rect2I = createRoom(X + 1, Y + 1, Width - 1, Height - 1);
        A = null;
        B = null;
    }
}