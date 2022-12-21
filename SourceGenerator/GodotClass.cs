using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using Array = Godot.Collections.Array;

namespace Generator {
    public class Signal {
        private static readonly System.Collections.Generic.Dictionary<string, string> SignalMap =
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

    public class GodotClass {
        private static readonly System.Collections.Generic.Dictionary<string, string> ClassMap =
            new() {
                // { "_VisualScriptEditor", "VisualScriptEditor" },
                // { "TCPServer", "TCP_Server" },
            };

        private static readonly System.Collections.Generic.Dictionary<string, string> ReservedNames =
            new() {
                { "event", "@event" },
                { "object", "@object" },
            };

        private static readonly System.Collections.Generic.Dictionary<Variant.Type, string> TypeMap =
            new() {
                  { Variant.Type.Nil,                      typeof(Godot.Variant).FullName },                    
                  { Variant.Type.Bool,                     typeof(System.Boolean).FullName },                    
                  { Variant.Type.Int,                      typeof(System.Int32).FullName },                      
                  { Variant.Type.Float,                    typeof(System.Single).FullName },                     
                  { Variant.Type.String,                   typeof(System.String).FullName },                     
                  { Variant.Type.Vector2,                  typeof(Godot.Vector2).FullName },                     
                  { Variant.Type.Vector2i,                 typeof(Godot.Vector2i).FullName },                    
                  { Variant.Type.Rect2,                    typeof(Godot.Rect2).FullName },                       
                  { Variant.Type.Rect2i,                   typeof(Godot.Rect2i).FullName },                      
                  { Variant.Type.Vector3,                  typeof(Godot.Vector3).FullName },                     
                  { Variant.Type.Vector3i,                 typeof(Godot.Vector3i).FullName },                    
                  { Variant.Type.Transform2d,              typeof(Godot.Transform2D).FullName },                 
                  { Variant.Type.Vector4,                  typeof(Godot.Vector4).FullName },                     
                  { Variant.Type.Vector4i,                 typeof(Godot.Vector4i).FullName },                    
                  { Variant.Type.Plane,                    typeof(Godot.Plane).FullName },                       
                  { Variant.Type.Quaternion,               typeof(Godot.Quaternion).FullName },                  
                  { Variant.Type.Aabb,                     typeof(Godot.AABB).FullName },                        
                  { Variant.Type.Basis,                    typeof(Godot.Basis).FullName },                       
                  { Variant.Type.Transform3d,              typeof(Godot.Transform3D).FullName },                 
                  { Variant.Type.Projection,               typeof(Godot.Projection).FullName },                  
                  { Variant.Type.Color,                    typeof(Godot.Color).FullName },                       
                  { Variant.Type.StringName,               typeof(Godot.StringName).FullName },                  
                  { Variant.Type.NodePath,                 typeof(Godot.NodePath).FullName },                    
                  { Variant.Type.Rid,                      typeof(Godot.RID).FullName },                         
                  { Variant.Type.Object,                   typeof(Godot.Object).FullName },                      
                  { Variant.Type.Callable,                 typeof(Godot.Callable).FullName },                    
                  { Variant.Type.Signal,                   typeof(Godot.SignalInfo).FullName },                  
                  { Variant.Type.Dictionary,               typeof(Godot.Collections.Dictionary).FullName },      
                  { Variant.Type.Array,                    typeof(Godot.Collections.Array).FullName },           
                  { Variant.Type.PackedByteArray,          typeof(System.Byte[]).FullName },                     
                  { Variant.Type.PackedInt32Array,         typeof(System.Int32[]).FullName },                    
                  { Variant.Type.PackedInt64Array,         typeof(System.Int64[]).FullName },                    
                  { Variant.Type.PackedFloat32Array,       typeof(System.Single[]).FullName },                   
                  { Variant.Type.PackedFloat64Array,       typeof(System.Double[]).FullName },                   
                  { Variant.Type.PackedStringArray,        typeof(System.String[]).FullName },                   
                  { Variant.Type.PackedVector2Array,       typeof(Godot.Vector2[]).FullName },                   
                  { Variant.Type.PackedVector3Array,       typeof(Godot.Vector3[]).FullName },                   
                  { Variant.Type.PackedColorArray,         typeof(Godot.Color[]).FullName },                     
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
                Variant.Type t = (Variant.Type)(long)arg["type"];
                var argType = TypeMap[t];
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