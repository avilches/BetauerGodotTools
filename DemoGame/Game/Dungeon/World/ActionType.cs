
using Betauer.Core;

namespace Veronenger.Game.Dungeon.World;

public enum ActionType  : byte { // 1 byte = 256 values; short = 2 bytes = 65536 values
    EndGame,

    Wait,
    Walk,
    Run,
    Attack,


}

public record ActionTypeConfig(ActionType Type) : EnumConfig<ActionType, ActionTypeConfig>(Type) {
    public required int EnergyCost { get; init; }
}