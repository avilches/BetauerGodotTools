using System.Collections.Generic;

namespace Betauer.Application; 

public class Arguments {
    public readonly Godot.Collections.Dictionary<string, string> Options;
    public readonly List<string> Commands;

    public Arguments(Godot.Collections.Dictionary<string, string> options, List<string> commands) {
        Options = options;
        Commands = commands;
    }
            
    public static Arguments ParseArgs(IEnumerable<string> getCmdlineArgs) {
        var options = new Godot.Collections.Dictionary<string, string>();
        var commands = new List<string>();
        string option = null;
        foreach (var arg in getCmdlineArgs) {
            if (arg.StartsWith("-")) {
                if (option != null) {
                    options[option] = "";
                    option = null;
                }
                var startParam = arg.StartsWith("--") ? 2 : 1;
                var x = arg.IndexOf("=");
                if (x > 0) {
                    var param = arg.Substring(startParam, x - startParam);
                    var value = arg.Substring(x + 1);
                    options[param] = value;
                } else {
                    option = arg.Substring(startParam);
                }
            } else {
                if (option != null) {
                    options[option] = arg;
                    option = null;
                } else {
                    commands.Add(arg);
                }
            }
        }
        return new Arguments(options, commands);
    }
}