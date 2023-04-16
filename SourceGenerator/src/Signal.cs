using System.Collections.Generic;
using System.Linq;

namespace Generator;

public class Signal {
    private static readonly Dictionary<string, string> SignalMap =
        new() {
            { "GraphNode:Selected", "SelectedSignal" },
        };

    public readonly string signal_name;
    public readonly string SignalName;
    public readonly string SignalCsharpConstantName;
    public readonly GodotClass GodotClass;
    public readonly string MethodName;
    public readonly List<SignalArg> Args;

    public Signal(GodotClass godotClass, string name, List<SignalArg> args) {
        GodotClass = godotClass;
        signal_name = name; // "on_button_pressed"
        var camelCase = signal_name.CamelCase();
        SignalName = SignalMap.ContainsKey($"{GodotClass.ClassName}:{camelCase}") ? SignalMap[$"{GodotClass.ClassName}:{camelCase}"] : camelCase; // "ButtonPressed"
        // IsStatic "VirtualServerButtonPressed"
        // !IsStatic "ButtonPressed"
        MethodName = GodotClass.IsStatic ? $"{GodotClass.ClassName}{SignalName}" : SignalName;
        // const VirtualServer_ButtonPressedSignal = "pressed_signal"
        SignalCsharpConstantName = $"{GodotClass.ClassName}_{MethodName}Signal";
        Args = args;
    }

    public string GetParamNames() {
        return string.Join(", ", Args.Select(arg => arg.ArgName).ToList());
    }

    public string GetParamNamesWithType() {
        return string.Join(", ", Args.Select(arg => arg.Type + " " + arg.ArgName).ToList());
    }

    public string Generics() {
        return Args.Count == 0 ? "" : $@"<{string.Join(", ", Args.Select(arg => arg.Type))}>";
    }
}