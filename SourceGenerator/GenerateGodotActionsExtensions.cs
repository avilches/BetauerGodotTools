using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateGodotActionsExtensions {
        private const string GodotActionsExtensionsFile = "../Betauer.GodotAction/GodotActionExtensions.cs";
        private const string GodotActionClassesNamespace = "Betauer.GodotAction";

        public static void WriteGodotActionExtensionsClass(List<GodotClass> classes) {
            List<string> allMethods = classes
                .Where(godotClass => godotClass.AllSignals.Count > 0 &&
                                     godotClass.IsNode &&
                                     !godotClass.IsEditor &&
                                     !godotClass.IsStatic &&
                                     !godotClass.IsAbstract)
                .SelectMany(godotClass => godotClass.AllSignals)
                .Select(CreateGodotActionExtensionsMethod)
                .ToList();
            var bodySignalExtensionsClass = CreateGodotActionExtensionsClass(allMethods);
            Console.WriteLine($"Generated {Path.GetFullPath(GodotActionsExtensionsFile)}");
            File.WriteAllText(GodotActionsExtensionsFile, bodySignalExtensionsClass);
        }

        private static string CreateGodotActionExtensionsMethod(Signal signal) {
            return $@"
        public static {signal.GodotClass.FullClassName} On{signal.MethodName}(this {signal.GodotClass.FullClassName} target, Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) {{
            GetProxy<{signal.GodotClass.GeneratedClassName}>(target).On{signal.MethodName}(action, oneShot, deferred);
            return target;
        }}";
        }

        private static string CreateGodotActionExtensionsClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using {GodotActionClassesNamespace};

namespace Betauer {{
    public static partial class GodotActionExtensions {{

        private const string ProxyName = ""__ProxyNodeAction__"";

        public static T GetProxy<T>(this Node owner) where T : Node {{
            T proxy = owner.GetNodeOrNull<T>(ProxyName);
            if (proxy == null) {{
                proxy = Activator.CreateInstance<T>();
                proxy.Name = ProxyName;
                owner.AddChild(proxy);
            }}
            return proxy;
        }}

{string.Join("\n", methods)}
    }}
}}";
        }
        
    }
}