using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateGodotActionClasses {
        private const string ActionClassesPath = "../Betauer.GodotAction/Proxy";
        private const string GodotActionClassesNamespace = "Betauer.GodotAction.Proxy";

        public static void WriteAllActionClasses(List<GodotClass> classes) {
            var n = 0;
            classes.Where(godotClass => godotClass.AllSignals.Count > 0 &&
                                        !godotClass.IsEditor &&
                                        !godotClass.IsStatic &&
                                        !godotClass.IsAbstract)
                .ToList()
                .ForEach(godotClass=> {
                    WriteClass(godotClass);
                    n++;
                });
            Console.WriteLine($"Generated {n} Action classes in {System.IO.Path.GetFullPath(ActionClassesPath)}");
        }

        private static void WriteClass(GodotClass godotClass) {
            var bodyClass = CreateActionClass(godotClass);
            File.WriteAllText(ActionClassesPath + "/" + godotClass.GeneratedClassName + ".cs", bodyClass);
        }
        
        private static string CreateActionClass(GodotClass godotClass) {
            var methods = godotClass.AllSignals.Where(signal=>!signal.GodotClass.IsAbstract).Select(CreateSignalMethod);
            var className = godotClass.GeneratedClassName;
            var extends = godotClass.FullClassName;
            return $@"using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace {GodotActionClassesNamespace} {{
    public class {className} : ProxyNode {{
{string.Join("\n", methods)}
    }}
}}";
        }

        private static string CreateSignalMethod(Signal signal) {
            var actionVarName = $"_on{signal.MethodName}Action";
            var godotExecuteActionMethodName = $"_GodotSignal{signal.MethodName}";
            var parameters = actionVarName;
            if (signal.GetParamNames().Length > 0) {
                parameters = parameters + ", " + signal.GetParamNames();
            }
            return $@"
        private List<Action{signal.Generics()}>? {actionVarName}; 
        public {signal.GodotClass.GeneratedClassName} On{signal.MethodName}(Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) {{
            AddSignal(ref {actionVarName}, ""{signal.signal_name}"", nameof({godotExecuteActionMethodName}), action, oneShot, deferred);
            return this;
        }}

        public {signal.GodotClass.GeneratedClassName} RemoveOn{signal.MethodName}(Action{signal.Generics()} action) {{
            RemoveSignal({actionVarName}, ""{signal.signal_name}"", nameof({godotExecuteActionMethodName}), action);
            return this;
        }}

        private {signal.GodotClass.GeneratedClassName} {godotExecuteActionMethodName}({signal.GetParamNamesWithType()}) {{
            ExecuteSignal({parameters});
            return this;
        }}";
        }
    }
}