using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using File = System.IO.File;

namespace Veronenger.SourceGenerator {
    /**
     * Based on: https://github.com/semickolon/GodotRx/blob/6bf1a1ba9ffbe939e888d8d545c8d448a1f07bce/addons/godotrx/codegen.js
     * 
     * call with -s SourceGenerator/GenerateSignalScript.cs --no-window
     */
    public class GenerateSignalScript : SceneTree {
        public override void _Initialize() {
            while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
            List<string> allMethods = new List<string>();
            ClassDB.GetClassList().ToList()
                .ForEach(s => {
                    var classMethods = GetSignalsFromClass(s).Select(CreateSignalMethod);
                    allMethods.AddRange(classMethods);
                });
            var bodyClass = CreateBodyClass(allMethods);
            File.WriteAllText("./Betauer.Core/SignalExtensions.cs", bodyClass);
            Quit(0);
        }

        private static readonly System.Collections.Generic.Dictionary<string, string> ClassMap =
            new System.Collections.Generic.Dictionary<string, string> {
                { "_VisualScriptEditor", "VisualScriptEditor" },
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
                { 17, "Godot.Object" },
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

        public class SignalArg {
            public readonly string Type;
            public readonly string Name;
            public readonly string ClassName;

            public SignalArg(string type, string name, string? argClassName) {
                Type = type;
                Name = name;
                ClassName = argClassName;
            }
        }

        public class Signal {
            public readonly string ClassName;
            public readonly string Name;
            public readonly bool IsStatic;
            public List<SignalArg> Args = new List<SignalArg>();

            public string MethodName => IsStatic ? $"{ClassName}{CamelCase(Name)}" : $"{CamelCase(Name)}";

            public string Generics() {
                return Args.Count == 0
                    ? ""
                    : $@"<{string.Join(", ", Args.Select(arg => arg.ClassName?.Length > 0 ? arg.ClassName : arg.Type))}>";
            }

            public Signal(string className, string name, bool isStatic) {
                ClassName = className;
                Name = name;
                IsStatic = isStatic;
            }

            public void AddArgs(string argName, string type, string argClassName) {
                Args.Add(new SignalArg(type, argName, argClassName));
            }
        }

        public List<Signal> GetSignalsFromClass(string className) {
            List<Signal> signals = new List<Signal>();
            foreach (Dictionary signalData in ClassDB.ClassGetSignalList(className, true)) {
                var signalName = (string)signalData["name"];
                var signalArgs = (Array)signalData["args"];
                className = ClassMap.ContainsKey(className) ? ClassMap[className] : className;
                if (EditorClasses.Contains(className)) continue;
                var isStatic = ClassStatic.Contains(className);
                if (signalName.StartsWith("on")) {
                    signalName = signalName.Substring(2);
                }
                var signal = new Signal(className, signalName, isStatic);
                foreach (Dictionary arg in signalArgs) {
                    var argClassName = (string)arg["class_name"];
                    var argName = (string)arg["name"];
                    var argType = TypeMap[(int)arg["type"]];
                    signal.AddArgs(argName, argType, argClassName);
                }
                signals.Add(signal);
            }
            return signals;
        }

        private string CreateSignalMethod(Signal signal) {
            var targetParam = signal.IsStatic
                ? ""
                : $"this {(signal.ClassName == "Animation" ? "Godot.Animation" : signal.ClassName)} target, ";
            var target = signal.IsStatic ? $"{signal.ClassName}.Singleton" : "target";
            var signalConst = $"{signal.ClassName}_{signal.MethodName}Signal";
            return $@"
        public const string {signalConst} = ""{signal.Name}""; 
        public static SignalHandler{signal.Generics()} On{signal.MethodName}({targetParam}Action{signal.Generics()} action) {{
            return new SignalHandler{signal.Generics()}({target}, {signalConst}, action);
        }}";
        }


        private string CreateBodyClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;

namespace Betauer {{
    public static partial class SignalExtensions {{
{string.Join("\n", methods)}
    }}
}}";
        }

        public static string CamelCase(string name) =>
            name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);
    }
}