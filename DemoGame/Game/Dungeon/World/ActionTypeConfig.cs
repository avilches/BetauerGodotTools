using System;
using System.Collections.Generic;

namespace Veronenger.Game.Dungeon.World;

public enum ActionType  : byte { // 1 byte = 256 values; short = 2 bytes = 65536 values
    Wait,
    Walk,
    Run,
    Attack,
}

public class ActionTypeConfig {
    private static readonly Dictionary<ActionType, ActionTypeConfig> Actions = new();

    public ActionType Type { get; init; }
    public required int EnergyCost { get; init;  }

    public ActionTypeConfig(ActionType type) {
        Type = type;
        if (Actions.ContainsKey(type)) {
            throw new Exception($"Action type {type} already registered!");
        }
        Actions[type] = this;
    }

    public static void Remove(ActionType type) {
        Actions.Remove(type);
    }

    public static void RemoveAll() {
        Actions.Clear();
    }

    public static void Verify() {
        foreach (var type in Enum.GetValues<ActionType>()) {
            if (!Actions.ContainsKey(type)) {
                throw new Exception($"Action type {type} not registered!");
            }
        }
    }

    public static ActionTypeConfig Get(ActionType type) {
        return Actions.TryGetValue(type, out var action)
            ? action
            : throw new Exception($"Action type {type} not registered!");
    }
}