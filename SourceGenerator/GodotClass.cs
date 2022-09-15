using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace Generator {
    public class Signal {
        public readonly string signal_name;
        public readonly string SignalName;
        public readonly string SignalCsharpConstantName;
        public readonly GodotClass GodotClass;
        public readonly string MethodName;
        public readonly List<SignalArg> Args;

        public Signal(GodotClass godotClass, string name, List<SignalArg> args) {
            GodotClass = godotClass;
            signal_name = name; // "on_button_pressed"
            // Some signals include a "on_" prefix. The camel case version will not include it
            SignalName = (name.StartsWith("on") ? name.Substring(2) : name).CamelCase(); // "ButtonPressed"
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

    public class GodotClass {
        private static readonly System.Collections.Generic.Dictionary<string, string> ClassMap =
            new System.Collections.Generic.Dictionary<string, string> {
                { "_VisualScriptEditor", "VisualScriptEditor" },
                { "TCPServer", "TCP_Server" },
            };

        private static readonly System.Collections.Generic.Dictionary<string, string> ReservedNames =
            new System.Collections.Generic.Dictionary<string, string> {
                { "event", "@event" },
                { "object", "@object" },
            };

        private static readonly System.Collections.Generic.Dictionary<int, string> TypeMap =
            new System.Collections.Generic.Dictionary<int, string> {
                { 0, "object" },
                { 1, "bool" },
                { 2, "int" },
                { 3, "float" },
                { 4, "string" },
                { 5, "Vector2" },
                { 6, "Rect2" },
                { 7, "Vector3" },
                { 8, "Transform2D" },
                { 9, "Plane" },
                { 10, "Quat" },
                { 11, "AABB" },
                { 12, "Basis" },
                { 13, "Transform" },
                { 14, "Color" },
                { 15, "NodePath" },
                { 16, "RID" },
                { 17, "Object" },
                { 18, "Godot.Collections.Dictionary" },
                { 19, "Godot.Collections.Array" },
                { 20, "byte[]" },
                { 21, "int[]" },
                { 22, "float[]" },
                { 23, "string[]" },
                { 24, "Vector2[]" },
                { 25, "Vector3[]" },
                { 26, "Color[]" }
            };


        public readonly string class_name;
        public readonly string ClassName;
        public readonly bool IsStatic;
        public readonly bool IsAbstract;
        public readonly bool IsValid;
        public readonly bool IsNode;
        public readonly List<Signal> Signals;
        public readonly List<Signal> AllSignals;
        public readonly Type? Type;

        public GodotClass(string className) {
            class_name = className;
            var camelCase = className.CamelCase();
            ClassName = ClassMap.ContainsKey(camelCase) ? ClassMap[camelCase] : camelCase;
            IsValid = ClassDB.IsClassEnabled(className);
            if (IsValid) {
                var godotSharpAssemblyName = typeof(Node).Assembly.GetName().Name;
                var fullQualifiedName = $"Godot.{ClassName}, {godotSharpAssemblyName}";
                Type = Type.GetType(fullQualifiedName);
                if (Type == null) {
                    IsValid = false;
                } else {
                    IsStatic = Type.IsAbstract && Type.IsSealed;
                    IsAbstract = Type.IsAbstract;
                    IsNode = Type.IsSubclassOf(typeof(Node));
                }
            }
            if (IsValid) {
                Signals = GetSignalsFromClass(true);
                AllSignals = GetSignalsFromClass(false);
            }
        }

        public List<Signal> GetSignalsFromClass(bool noInheritance) {
            var classDbSignals = ClassDB.ClassGetSignalList(class_name, noInheritance);
            var signals = new List<Signal>();
            foreach (Dictionary signalData in classDbSignals) {
                var signalName = (string)signalData["name"];
                var signalArgs = CreateSignalArgs((Array)signalData["args"]);
                signals.Add(new Signal(this, signalName, signalArgs));
            }
            return signals.OrderBy(signal => signal.SignalName).ToList();
        }

        private static List<SignalArg> CreateSignalArgs(Array signalGodotArgs) {
            var args = new List<SignalArg>();
            foreach (Dictionary arg in signalGodotArgs) {
                string? argClassName = (string)arg["class_name"];
                var argName = (string)arg["name"];
                var argType = TypeMap[(int)arg["type"]];
                argName = ReservedNames.ContainsKey(argName) ? ReservedNames[argName] : argName;
                args.Add(new SignalArg(argClassName?.Length > 0 ? argClassName : argType, argName));
            }
            return args;
        }
    }

    public class SignalArg {
        public string Type;
        public string ArgName;

        public SignalArg(string type, string argName) {
            Type = type;
            ArgName = argName;
        }
    }
}