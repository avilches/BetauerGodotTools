using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
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
            var classes = LoadGodotClasses();
            WriteAllActionClasses(classes);
            WriteSignalExtensionsClass(classes);
            Quit(0);
        }

        private void WriteSignalExtensionsClass(List<GodotClass> classes) {
            List<string> allMethods = classes
                .Where(godotClass => godotClass.Signals.Count > 0 &&
                                     !godotClass.IsEditor)
                .SelectMany(godotClass => godotClass.Signals)
                .Select(CreateSignalExtensionMethod)
                .ToList();
            var bodyClass = CreateSignalExtensionsClass(allMethods);
            File.WriteAllText("./Betauer.Core/SignalExtensions.cs", bodyClass);
        }

        private void WriteAllActionClasses(List<GodotClass> classes) {
            classes.Where(godotClass => godotClass.AllSignals.Count > 0 &&
                                        !godotClass.IsEditor &&
                                        !godotClass.IsStatic &&
                                        !godotClass.IsAbstract)
                .ToList()
                .ForEach(WriteClass);
        }

        private static List<GodotClass> LoadGodotClasses() {
            var classes = ClassDB.GetClassList()
                .Select(className => new GodotClass(className))
                .Where(godotClass => godotClass.IsValid)
                .OrderBy(godotClass => godotClass.class_name)
                .ToList();
            return classes;
        }

        private void WriteClass(GodotClass godotClass) {
            var classMethods = godotClass.AllSignals.Select(CreateActionMethod);
            var bodyClass = CreateActionClass(godotClass.GeneratedClassName, godotClass.FullClassName, classMethods);
            File.WriteAllText("./Betauer.Core/Classes/" + godotClass.GeneratedClassName + ".cs", bodyClass);
        }

        private string CreateSignalExtensionMethod(Signal signal) {
            var targetParam = signal.GodotClass.IsStatic
                ? ""
                : $"this {signal.GodotClass.FullClassName} target, ";
            var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
            var signalConst = $"{signal.GodotClass.ClassName}_{signal.MethodName}Signal";
            return $@"
        public const string {signalConst} = ""{signal.signal_name}""; 
        public static SignalHandler{signal.Generics()} On{signal.MethodName}({targetParam}Action{signal.Generics()} action) {{
            return new SignalHandler{signal.Generics()}({target}, {signalConst}, action);
        }}";
        }

        private string CreateActionMethod(Signal signal) {
            var actionVarName = $"_on{signal.MethodName}Action";
            var godotExecuteActionMethodName = $"Execute{signal.MethodName}";
            return $@"
        private Action{signal.Generics()}? {actionVarName}; 
        public {signal.GodotClass.GeneratedClassName} On{signal.MethodName}(Action{signal.Generics()} action) {{
            if ({actionVarName} == null) 
                Connect(""{signal.signal_name}"", this, nameof({godotExecuteActionMethodName}));
            {actionVarName} = action;
            return this;
        }}
        public {signal.GodotClass.GeneratedClassName} RemoveOn{signal.MethodName}() {{
            if ({actionVarName} == null) return this; 
            Disconnect(""{signal.signal_name}"", this, nameof({godotExecuteActionMethodName}));
            {actionVarName} = null;
            return this;
        }}
        private void {godotExecuteActionMethodName}({signal.GetParamNamesWithType()}) =>
            {actionVarName}?.Invoke({signal.GetParamNames()});
        ";
        }

        private string CreateActionClass(string className, string extends, IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace Betauer.Classes {{
    public class {className} : {extends} {{
{string.Join("\n", methods)}
    }}
}}";
        }

        private string CreateSignalExtensionsClass(IEnumerable<string> methods) {
            return $@"using System;
using Godot;
using Object = Godot.Object;
using Animation = Godot.Animation;

namespace Betauer {{
    public static partial class SignalExtensions {{
{string.Join("\n", methods)}
    }}
}}";
        }
    }

    public static class Tools {
        public static string CamelCase(string name) =>
            name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                .Aggregate(string.Empty, (s1, s2) => s1 + s2);
    }
}