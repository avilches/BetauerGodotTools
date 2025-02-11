namespace Veronenger.Game.Dungeon.World;

public enum CellType : byte { // 1 byte = 256 cell types; short = 2 bytes = 65536 values
    Wall,
    Floor,
    Door,
}

public record CellTypeConfig(CellType Type) : EnumConfig<CellType, CellTypeConfig>(Type);


