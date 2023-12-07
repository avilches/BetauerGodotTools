using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            { Variant.Type.Nil,                      "Variant" },                    
            { Variant.Type.Bool,                     "bool" },                    
            { Variant.Type.Int,                      "long" },                      
            { Variant.Type.Float,                    "double" },                     
            { Variant.Type.String,                   "string" },                     
            { Variant.Type.Vector2,                  "Vector2" },                     
            { Variant.Type.Vector2I,                 "Vector2I" },                    
            { Variant.Type.Rect2,                    "Rect2" },                       
            { Variant.Type.Rect2I,                   "Rect2I" },                      
            { Variant.Type.Vector3,                  "Vector3" },                     
            { Variant.Type.Vector3I,                 "Vector3I" },                    
            { Variant.Type.Transform2D,              "Transform2D" },                 
            { Variant.Type.Vector4,                  "Vector4" },                     
            { Variant.Type.Vector4I,                 "Vector4I" },                    
            { Variant.Type.Plane,                    "Plane" },                       
            { Variant.Type.Quaternion,               "Quaternion" },                  
            { Variant.Type.Aabb,                     "Aabb" },                        
            { Variant.Type.Basis,                    "Basis" },                       
            { Variant.Type.Transform3D,              "Transform3D" },                 
            { Variant.Type.Projection,               "Projection" },                  
            { Variant.Type.Color,                    "Color" },                       
            { Variant.Type.StringName,               "StringName" },                  
            { Variant.Type.NodePath,                 "NodePath" },                    
            { Variant.Type.Rid,                      "Rid" },                         
            { Variant.Type.Object,                   "GodotObject" },                      
            { Variant.Type.Callable,                 "Callable" },                    
            { Variant.Type.Signal,                   "Godot.Signal" },                  
            { Variant.Type.Dictionary,               "Dictionary" },      
            { Variant.Type.Array,                    "Array" },           
            { Variant.Type.PackedByteArray,          "byte[]" },                     
            { Variant.Type.PackedInt32Array,         "int[]" },                    
            { Variant.Type.PackedInt64Array,         "long[]" },                    
            { Variant.Type.PackedFloat32Array,       "float[]" },                   
            { Variant.Type.PackedFloat64Array,       "double[]" },                   
            { Variant.Type.PackedStringArray,        "string[]" },                   
            { Variant.Type.PackedVector2Array,       "Vector2[]" },                   
            { Variant.Type.PackedVector3Array,       "Vector3[]" },                   
            { Variant.Type.PackedColorArray,         "Color[]" },                     
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
    public readonly string[] Notifications;

    public GodotClass(string className) {
        class_name = className;
        var camelCase = className.CamelCase();
        ClassName = ClassMap.ContainsKey(camelCase) ? ClassMap[camelCase] : camelCase;
        IsValid = ClassDB.IsClassEnabled(className);
        Notifications = System.Array.Empty<string>();
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
                if (IsNode) {
                    Notifications = ClassDB.ClassGetIntegerConstantList(ClassName, true)
                        .Where(n => n.StartsWith("NOTIFICATION"))
                        .Select(s => s.ToLower().CamelCase()
                            .Replace("Wm", "WM")
                            .Replace("2d", "2D")
                            .Replace("3d", "3D"))
                        .ToArray();
                }
            }
        }
        if (IsValid) {
            Signals = GetSignalsFromClass(true);
            AllSignals = GetSignalsFromClass(false);
        }
    }

    public long GetNotificationValue(string name) {
        return (long)Type!.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .First(fi => fi is { IsLiteral: true, IsInitOnly: false } && fi.Name == name).GetRawConstantValue()!;
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