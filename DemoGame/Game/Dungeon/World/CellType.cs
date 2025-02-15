using Betauer.Core;

namespace Veronenger.Game.Dungeon.World;

public enum CellType : byte { // 1 byte = 256 cell types; short = 2 bytes = 65536 values
    Wall,
    Floor,
    Door,
}

public record CellTypeConfig(CellType Type, bool IsBlocked) : EnumConfig<CellType, CellTypeConfig>(Type) {
    public static void DefaultConfig() {
        RegisterAll(
            new CellTypeConfig(CellType.Floor, false),
            new CellTypeConfig(CellType.Wall, true),
            new CellTypeConfig(CellType.Door, false)
        );
    }
}