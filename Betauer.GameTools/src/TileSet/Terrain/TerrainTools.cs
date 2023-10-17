using Betauer.Core;

namespace Betauer.TileSet.Terrain;

public static class TerrainTools {

    public static int CreateMask(int[] neighbours) {
        var bits = 0;
        for (var i = 0; i < neighbours.Length; i++) {
            if (neighbours[i] != -1) {
                bits = BitTools.EnableBit(bits, i + 1);
            }
        }
        return bits;
    }

    public static int CreateMask(bool[] neighbours) {
        var bits = 0;
        for (var i = 0; i < neighbours.Length; i++) {
            if (neighbours[i]) {
                bits = BitTools.EnableBit(bits, i + 1);
            }
        }
        return bits;
    }
    
    /// <summary>
    /// From mask 20, which is
    /// |   |
    /// | **|
    /// | * |
    /// it will return { { -1, -1, -1 }, { -1, 0, 0 }, { -1, 0,-1 } }
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static T[,] CreateNeighboursGrid<T>(int mask, T value, T empty) {
        var neighbours = new T[3, 3];
        const int x = 1;
        const int y = 1;
        neighbours[y - 1, x    ] = BitTools.HasBit(mask, 1) ? value : empty; // TopSide
        neighbours[y - 1, x + 1] = BitTools.HasBit(mask, 2) ? value : empty; // TopRightCorner
        neighbours[y    , x + 1] = BitTools.HasBit(mask, 3) ? value : empty; // RightSide
        neighbours[y + 1, x + 1] = BitTools.HasBit(mask, 4) ? value : empty; // BottomRightCorner
        neighbours[y + 1, x    ] = BitTools.HasBit(mask, 5) ? value : empty; // BottomSide
        neighbours[y + 1, x - 1] = BitTools.HasBit(mask, 6) ? value : empty; // BottomLeftCorner
        neighbours[y    , x - 1] = BitTools.HasBit(mask, 7) ? value : empty; // LeftSide
        neighbours[y - 1, x - 1] = BitTools.HasBit(mask, 8) ? value : empty; // TopLeftCorner
        neighbours[y    , x    ] = value; // Center
        return neighbours;
    }
}