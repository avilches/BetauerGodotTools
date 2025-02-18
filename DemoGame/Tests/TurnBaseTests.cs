using NUnit.Framework;
using Veronenger.Game.Dungeon.World;

namespace Veronenger.Tests;

public class TurnBaseTests {
    [SetUp]
    public void Setup() {
        CellTypeConfig.DefaultConfig();
        ActionTypeConfig.RegisterAll(
            new ActionTypeConfig(ActionType.EndGame) { EnergyCost = 0 },

            new ActionTypeConfig(ActionType.Wait) { EnergyCost = 500 },
            new ActionTypeConfig(ActionType.Walk) { EnergyCost = 1000 },
            new ActionTypeConfig(ActionType.Attack) { EnergyCost = 1200 },
            new ActionTypeConfig(ActionType.Run) { EnergyCost = 2000 }
        );
    }
}