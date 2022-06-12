using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace Veronenger.SourceGenerator {
    public class Signal {
        public readonly string signal_name;
        public readonly string SignalName;
        public readonly GodotClass GodotClass;
        public List<SignalArg> Args;
        public string MethodName;


        public Signal(GodotClass godotClass, string name) {
            GodotClass = godotClass;
            signal_name = name; // "on_button_pressed"
            // Some signals include a "on_" prefix. The camel case version will not include it
            SignalName = Tools.CamelCase(name.StartsWith("on") ? name.Substring(2) : name); // "ButtonPressed"
            // IsStatic "VirtualServerButtonPressed"
            // !IsStatic "ButtonPressed"
            MethodName = GodotClass.IsStatic ? $"{GodotClass.ClassName}{SignalName}" : SignalName;
        }

        public string GetParamNames() {
            var p = 0;
            return string.Join(", ", Args.Select(arg => arg.ArgName).ToList());
        }

        public string GetParamNamesWithType() {
            var p = 0;
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

        private static readonly List<string> ClassStatic = new List<string> {
            "AudioServer",
            "ARVRServer",
            "CameraServer",
            "Input",
            "VisualServer",
            "VisualScriptEditor"
        };

        private static readonly List<string> EditorClasses = new List<string> {
            "EditorFileDialog",
            "EditorFileSystem",
            "EditorInspector",
            "EditorSelection",
            "EditorSettings",
            "EditorPlugin",
            "EditorProperty",
            "EditorResourcePreview",
            "EditorResourcePicker",
            "FileSystemDock",
            "ScriptEditor",
            "ScriptCreateDialog",
            "VisualScriptEditor"
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
        public readonly string FullClassName;
        public readonly string GeneratedClassName;
        public readonly bool IsStatic;
        public readonly bool IsEditor;
        public readonly bool IsAbstract;
        public readonly bool IsValid;
        public readonly List<Signal> Signals;
        public readonly List<Signal> AllSignals;

        public GodotClass(string className) {
            class_name = className;
            var camelCase = Tools.CamelCase(className);
            ClassName = ClassMap.ContainsKey(camelCase) ? ClassMap[camelCase] : camelCase;
            GeneratedClassName = ClassName + "Action";
            IsStatic = ClassStatic.Contains(ClassName);
            IsEditor = EditorClasses.Contains(ClassName);
            FullClassName = ClassName;
            IsValid = ClassDB.IsClassEnabled(className);
            if (IsValid && !IsEditor && !IsStatic) {
                var godotSharpAssemblyName = typeof(Node).Assembly.GetName().Name;
                var type = Type.GetType("Godot." + ClassName + ", " + godotSharpAssemblyName);
                if (type != null) {
                    IsAbstract = type.IsAbstract;
                } else {
                    IsValid = false;
                }
            }
            if (IsValid) {
                Signals = GetSignalsFromClass(ClassDB.ClassGetSignalList(class_name, true));
                AllSignals = GetSignalsFromClass(ClassDB.ClassGetSignalList(class_name, false));
            }
        }

        public List<Signal> GetSignalsFromClass(Array classDbSignals) {
            List<Signal> signals = new List<Signal>();
            foreach (Dictionary signalData in classDbSignals) {
                var signalName = (string)signalData["name"];
                var signalArgs = (Array)signalData["args"];
                var signal = new Signal(this, signalName);
                var args = new List<SignalArg>();
                foreach (Dictionary arg in signalArgs) {
                    string? argClassName = (string)arg["class_name"];
                    var argName = (string)arg["name"];
                    var argType = TypeMap[(int)arg["type"]];
                    argName = ReservedNames.ContainsKey(argName) ? ReservedNames[argName] : argName;
                    args.Add(new SignalArg(argClassName?.Length > 0 ? argClassName : argType, argName));
                }
                signal.Args = args.OrderBy(arg => arg.ArgName).ToList();
                signals.Add(signal);
            }
            return signals.OrderBy(signal => signal.SignalName).ToList();
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