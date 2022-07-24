using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateGodotActionsExtensions {
        private const string GodotActionsExtensionsFile = "../Betauer.GodotAction/GodotActionExtensions.cs";
        private const string GodotActionClassesNamespace = "Betauer.GodotAction.Proxy";

        public static void WriteGodotActionExtensionsClass(List<GodotClass> classes) {
            var allMethods = classes
                .Where(godotClass => godotClass.IsNode &&
                                     !godotClass.IsEditor &&
                                     !godotClass.IsStatic &&
                                     !godotClass.IsAbstract)
                .Select(CreateGodotActionExtensionsMethod)
                .ToList();
            var bodySignalExtensionsClass = CreateGodotActionExtensionsClass(allMethods);
            Console.WriteLine($"Generated {Path.GetFullPath(GodotActionsExtensionsFile)}");
            File.WriteAllText(GodotActionsExtensionsFile, bodySignalExtensionsClass);
        }

        private static string CreateGodotActionExtensionsMethod(GodotClass godotClass) {
            return $@"
        public static {godotClass.GeneratedClassName} GetProxy(this {godotClass.FullClassName} target) => 
                GetOrCreateProxy<{godotClass.GeneratedClassName}>(target);";
        }
/*
        private static string CreateGodotActionExtensionsMethod(Signal signal) {
            return $@"
        public static {signal.GodotClass.FullClassName} On{signal.MethodName}(this {signal.GodotClass.FullClassName} target, Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) {{
            GetProxy<{signal.GodotClass.GeneratedClassName}>(target).On{signal.MethodName}(action, oneShot, deferred);
            return target;
        }}

        public static {signal.GodotClass.FullClassName} RemoveOn{signal.MethodName}(this {signal.GodotClass.FullClassName} target, Action{signal.Generics()} action) {{
            GetProxy<{signal.GodotClass.GeneratedClassName}>(target).RemoveOn{signal.MethodName}(action);
            return target;
        }}";
        }
*/
        private static string CreateGodotActionExtensionsClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;
using Environment = Godot.Environment;
using {GodotActionClassesNamespace};

namespace Betauer.GodotAction {{
    public static partial class GodotActionExtensions {{

        private const string ProxyName = ""__ProxyGodotAction__"";

        public static T GetOrCreateProxy<T>(Node owner) where T : Node {{
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