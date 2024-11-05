using System;
using Godot;

namespace Betauer.Core.Image;

public static partial class Draw {
    public static Random Random = new Random();

    public static void LineOneTurn(int x0, int y0, int x1, int y1, Action<int, int> onPixel, bool firstHorizontal = true) {
        // Move in one direction until aligned, then move in the other direction
        if (firstHorizontal) {
            /*
             ...........
             .########..
             ........#..
             ........#..
             ...........
            */
            // Move horizontally first until aligned, then move vertically to reach the target
            for (; x0 != x1; x0 += (x1 > x0) ? 1 : -1) onPixel(x0, y0);
            for (; y0 != y1; y0 += (y1 > y0) ? 1 : -1) onPixel(x0, y0);
        } else {
            /*
             ...........
             .#.........
             .#.........
             .########..
             ...........
            */
            // Move vertically first until aligned, then move horizontally to reach the target
            for (; y0 != y1; y0 += (y1 > y0) ? 1 : -1) onPixel(x0, y0);
            for (; x0 != x1; x0 += (x1 > x0) ? 1 : -1) onPixel(x0, y0);
        }
        onPixel(x0, y0);
    }

    /*
     ...........
     ...........
     .#.........
     .#.........
     .########..
     ........#..
     ........#..
     ........#..
     ...........
     ...........
    */
    public static void LineTwoTurns(int x0, int y0, int x1, int y1, Action<int, int> onPixel, bool firstHorizontal = true, float firstTurn = 0.5f, float secondTurn = 0.5f) {
        var midX = Mathf.RoundToInt(x0 + (x1 - x0) * firstTurn);
        var midY = Mathf.RoundToInt(y0 + (y1 - y0) * secondTurn);
        if (firstHorizontal) {
            for (; x0 != midX; x0 += (midX > x0) ? 1 : -1) onPixel(x0, y0);
            for (; y0 != midY; y0 += (midY > y0) ? 1 : -1) onPixel(x0, y0);
            for (; x0 != x1; x0 += (x1 > x0) ? 1 : -1) onPixel(x0, y0);
            for (; y0 != y1; y0 += (y1 > y0) ? 1 : -1) onPixel(x0, y0);
        } else {
            for (; y0 != midY; y0 += (midY > y0) ? 1 : -1) onPixel(x0, y0);
            for (; x0 != midX; x0 += (midX > x0) ? 1 : -1) onPixel(x0, y0);
            for (; y0 != y1; y0 += (y1 > y0) ? 1 : -1) onPixel(x0, y0);
            for (; x0 != x1; x0 += (x1 > x0) ? 1 : -1) onPixel(x0, y0);
        }
        onPixel(x0, y0);
    }

    public static void LineRandom(int x0, int y0, int x1, int y1, Action<int, int> onPixel, int maxStraightLength = 3, Random? random = null) {
        /*
        #.........
        #.........
        #####.....
        ....#.....
        ....###...
        ......#...
        ......#...
        ......###.
        ........##
        .........#
        */
        random ??= Random;
        if (maxStraightLength <= 0) maxStraightLength = int.MaxValue;
        var dx = Math.Abs(x1 - x0);
        var dy = Math.Abs(y1 - y0);
        var totalDistance = dx + dy;
        var offset = 0;
        while (x0 != x1 || y0 != y1) {
            onPixel(x0, y0);
            var horizontalProbability = (float)dx / totalDistance;
            if (random.NextDouble() < horizontalProbability) {
                offset++;
                if (offset > maxStraightLength) {
                    offset = 0;
                    if (y0 < y1) y0++;
                    else if (y0 > y1) y0--;
                } else {
                    if (x0 < x1) x0++;
                    else if (x0 > x1) x0--;
                }
            } else {
                offset--;
                if (offset < -maxStraightLength) {
                    offset = 0;
                    if (x0 < x1) x0++;
                    else if (x0 > x1) x0--;
                } else {
                    if (y0 < y1) y0++;
                    else if (y0 > y1) y0--;
                }
            }
        }
        onPixel(x0, y0);
    }
}