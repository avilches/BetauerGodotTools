using System;
using System.Collections.Generic;

namespace Veronenger.Game.Dungeon.World;

public enum ActionType {
    Wait,
    Walk,
    Run,
    Attack,
}

public class ActionConfig {
    private static readonly Dictionary<ActionType, ActionConfig> Actions = new();

    public ActionType Type { get; }
    public int EnergyCost { get; }

    private ActionConfig(ActionType type, int energyCost) {
        Type = type;
        EnergyCost = energyCost;
    }

    public static void RegisterAction(ActionType type, int energyCost) {
        Actions[type] = new ActionConfig(type, energyCost);
    }

    public static ActionConfig Get(ActionType type) {
        return Actions.TryGetValue(type, out var action)
            ? action
            : throw new Exception($"Action type {type} not registered!");
    }
}