using Betauer.Core;

namespace Betauer.TileSet.Terrain;

public static class TerrainTools {

    public static int CreateMask(int[] neighbours) {
        var bits = 0;
        for (var i = 0; i < neighbours.Length; i++) {
            if (neighbours[i] != (int)SingleTerrain.TileType.None) {
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
    public static int[,] CreateNeighboursGrid(int mask) {
        var neighbours = new int[3, 3];
        const int x = 1;
        const int y = 1;
        neighbours[y - 1, x    ] = BitTools.HasBit(mask, 1) ? 0 : -1; // TopSide
        neighbours[y - 1, x + 1] = BitTools.HasBit(mask, 2) ? 0 : -1; // TopRightCorner
        neighbours[y    , x + 1] = BitTools.HasBit(mask, 3) ? 0 : -1; // RightSide
        neighbours[y + 1, x + 1] = BitTools.HasBit(mask, 4) ? 0 : -1; // BottomRightCorner
        neighbours[y + 1, x    ] = BitTools.HasBit(mask, 5) ? 0 : -1; // BottomSide
        neighbours[y + 1, x - 1] = BitTools.HasBit(mask, 6) ? 0 : -1; // BottomLeftCorner
        neighbours[y    , x - 1] = BitTools.HasBit(mask, 7) ? 0 : -1; // LeftSide
        neighbours[y - 1, x - 1] = BitTools.HasBit(mask, 8) ? 0 : -1; // TopLeftCorner
        neighbours[y    , x    ] = 0; // Center
        return neighbours;
    }
}