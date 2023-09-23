using System.Collections.Generic;
using Betauer.Core;
using Godot;

namespace Betauer.TileSet.Godot;

public static class GodotTileDataExtensions {
    public static Dictionary<global::Godot.TileSet.CellNeighbor, int> MaskValues = new()  {
        { global::Godot.TileSet.CellNeighbor.TopSide, 1 },
        { global::Godot.TileSet.CellNeighbor.TopRightCorner, 2 },
        { global::Godot.TileSet.CellNeighbor.RightSide, 3 },
        { global::Godot.TileSet.CellNeighbor.BottomRightCorner, 4 },
        { global::Godot.TileSet.CellNeighbor.BottomSide, 5 },
        { global::Godot.TileSet.CellNeighbor.BottomLeftCorner, 6 },
        { global::Godot.TileSet.CellNeighbor.LeftSide, 7 },
        { global::Godot.TileSet.CellNeighbor.TopLeftCorner, 8 },
    };
    
    public static int GetTerrainMask(this TileData tileData) {
        var bits = 0;
        foreach (var maskValue in MaskValues) {
            if (tileData.GetTerrainPeeringBit(maskValue.Key) == 0) {
                bits = BitTools.EnableBit(bits, maskValue.Value);
            }
        }
        return bits;
    }

    public static int SetTerrainMask(this TileData tileData, int bits, int value) {
        foreach (var maskValue in MaskValues) {
            if (BitTools.HasBit(bits, maskValue.Value)) {
                tileData.SetTerrainPeeringBit(maskValue.Key, value);
            }
        }
        return bits;
    }
}