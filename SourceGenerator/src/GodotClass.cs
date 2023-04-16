using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using Array = Godot.Collections.Array;

namespace Generator;

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
            { Variant.Type.Vector2I,                 typeof(Godot.Vector2I).FullName },                    
            { Variant.Type.Rect2,                    typeof(Godot.Rect2).FullName },                       
            { Variant.Type.Rect2I,                   typeof(Godot.Rect2I).FullName },                      
            { Variant.Type.Vector3,                  typeof(Godot.Vector3).FullName },                     
            { Variant.Type.Vector3I,                 typeof(Godot.Vector3I).FullName },                    
            { Variant.Type.Transform2D,              typeof(Godot.Transform2D).FullName },                 
            { Variant.Type.Vector4,                  typeof(Godot.Vector4).FullName },                     
            { Variant.Type.Vector4I,                 typeof(Godot.Vector4I).FullName },                    
            { Variant.Type.Plane,                    typeof(Godot.Plane).FullName },                       
            { Variant.Type.Quaternion,               typeof(Godot.Quaternion).FullName },                  
            { Variant.Type.Aabb,                     typeof(Godot.Aabb).FullName },                        
            { Variant.Type.Basis,                    typeof(Godot.Basis).FullName },                       
            { Variant.Type.Transform3D,              typeof(Godot.Transform3D).FullName },                 
            { Variant.Type.Projection,               typeof(Godot.Projection).FullName },                  
            { Variant.Type.Color,                    typeof(Godot.Color).FullName },                       
            { Variant.Type.StringName,               typeof(Godot.StringName).FullName },                  
            { Variant.Type.NodePath,                 typeof(Godot.NodePath).FullName },                    
            { Variant.Type.Rid,                      typeof(Godot.Rid).FullName },                         
            { Variant.Type.Object,                   typeof(Godot.GodotObject).FullName },                      
            { Variant.Type.Callable,                 typeof(Godot.Callable).FullName },                    
            { Variant.Type.Signal,                   typeof(Godot.Signal).FullName },                  
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