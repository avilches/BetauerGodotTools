using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using File = System.IO.File;

/**
     * Based on: https://github.com/semickolon/GodotRx/blob/6bf1a1ba9ffbe939e888d8d545c8d448a1f07bce/addons/godotrx/codegen.js
     * 
     * call with -s GeneratorScript.cs --no-window
     */
public class GeneratorScript : SceneTree {

    private const string SignalExtensionsFile = "../Betauer.Core/SignalExtensions.cs";
    private const string SignalConstantsFile = "../Betauer.Core/SignalConstants.cs";
    private const string ActionClassesPath = "../Betauer.GodotAction/GodotAction";
    
    private const string ActionClassNamespace = "Betauer.GodotAction";
    
    public override void _Initialize() {
        while (Root.GetChildCount() > 0) Root.RemoveChild(Root.GetChild(0));
        var classes = LoadGodotClasses();
        WriteAllActionClasses(classes);
        WriteSignalExtensionsClass(classes);
        WriteSignalConstantsClass(classes);
        Quit(0);
    }

    private void WriteSignalExtensionsClass(List<GodotClass> classes) {
        List<string> allMethods = classes
            .Where(godotClass => godotClass.Signals.Count > 0 &&
                                 !godotClass.IsEditor)
            .SelectMany(godotClass => godotClass.Signals)
            .Select(CreateSignalExtensionMethod)
            .ToList();
        var bodySignalExtensionsClass = CreateSignalExtensionsClass(allMethods);
        Console.WriteLine($"Generated {System.IO.Path.GetFullPath(SignalExtensionsFile)}");
        File.WriteAllText(SignalExtensionsFile, bodySignalExtensionsClass);
    }

    private void WriteSignalConstantsClass(List<GodotClass> classes) {
        List<string> allMethods = classes
            .Where(godotClass => godotClass.Signals.Count > 0 &&
                                 !godotClass.IsEditor)
            .SelectMany(godotClass => godotClass.Signals)
            .Select(CreateSignalConstantField)
            .ToList();
        var bodySignalConstantsClass = CreateSignalConstantsClass(allMethods);
        Console.WriteLine($"Generated {System.IO.Path.GetFullPath(SignalConstantsFile)}");
        File.WriteAllText(SignalConstantsFile, bodySignalConstantsClass);
    }

    private void WriteAllActionClasses(List<GodotClass> classes) {
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

    private static List<GodotClass> LoadGodotClasses() {
        var classes = ClassDB.GetClassList()
            .Select(className => new GodotClass(className))
            .Where(godotClass => godotClass.IsValid)
            .OrderBy(godotClass => godotClass.class_name)
            .ToList();
        return classes;
    }

    private void WriteClass(GodotClass godotClass) {
        var bodyClass = CreateActionClass(godotClass);
        File.WriteAllText(ActionClassesPath + "/" + godotClass.GeneratedClassName + ".cs", bodyClass);
    }

    private string CreateSignalExtensionMethod(Signal signal) {
        var targetParam = signal.GodotClass.IsStatic
            ? ""
            : $"this {signal.GodotClass.FullClassName} target, ";
        var target = signal.GodotClass.IsStatic ? $"{signal.GodotClass.ClassName}.Singleton" : "target";
        return $@"
        public static SignalHandler{signal.Generics()} On{signal.MethodName}({targetParam}Action{signal.Generics()} action) =>
            new SignalHandler{signal.Generics()}({target}, SignalConstants.{signal.SignalCsharpConstantName}, action);";
    }

    private string CreateSignalConstantField(Signal signal) {
        return $@"        public const string {signal.SignalCsharpConstantName} = ""{signal.signal_name}"";";
    }

    private string CreateActionMethod(Signal signal) {
        var actionVarName = $"_on{signal.MethodName}Action";
        var godotExecuteActionMethodName = $"Execute{signal.MethodName}";
        return $@"
        private List<Action{signal.Generics()}>? {actionVarName}; 
        public {signal.GodotClass.GeneratedClassName} On{signal.MethodName}(Action{signal.Generics()} action) {{
            if ({actionVarName} == null || {actionVarName}.Count == 0) {{
                {actionVarName} ??= new List<Action{signal.Generics()}>(); 
                Connect(""{signal.signal_name}"", this, nameof({godotExecuteActionMethodName}));
            }}
            {actionVarName}.Add(action);
            return this;
        }}
        public {signal.GodotClass.GeneratedClassName} RemoveOn{signal.MethodName}(Action{signal.Generics()} action) {{
            if ({actionVarName} == null || {actionVarName}.Count == 0) return this;
            {actionVarName}.Remove(action); 
            if ({actionVarName}.Count == 0) {{
                Disconnect(""{signal.signal_name}"", this, nameof({godotExecuteActionMethodName}));
            }}
            return this;
        }}
        private void {godotExecuteActionMethodName}({signal.GetParamNamesWithType()}) {{
            if ({actionVarName} == null || {actionVarName}.Count == 0) return;
            for (var i = 0; i < {actionVarName}.Count; i++) {actionVarName}[i].Invoke({signal.GetParamNames()});
        }}
        ";
    }

    private string CreateActionClass(GodotClass godotClass) {
        var methods = godotClass.AllSignals.Select(CreateActionMethod);
        var className = godotClass.GeneratedClassName;
        var extends = godotClass.FullClassName;
        var nodeMethods = godotClass.IsNode ? CreateNodeMethods(className) : "";
        return $@"using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace {ActionClassNamespace} {{
    public class {className} : {extends} {{
{nodeMethods}
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

    private string CreateSignalConstantsClass(IEnumerable<string> methods) {
        return $@"using System;
namespace Betauer {{
    public static partial class SignalConstants {{
{string.Join("\n", methods)}
    }}
}}";
    }

    private string CreateNodeMethods(string className) {
        return $@"
        private List<Action<float>>? _onProcessActions; 
        private List<Action<float>>? _onPhysicsProcessActions; 
        private List<Action<InputEvent>>? _onInputActions; 
        private List<Action<InputEvent>>? _onUnhandledInputActions; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInputActions;

        public {className} OnProcess(Action<float> action) {{
            _onProcessActions ??= new List<Action<float>>(1);
            _onProcessActions.Add(action);
            SetProcess(true);
            return this;
        }}
        public {className} OnPhysicsProcess(Action<float> action) {{
            _onPhysicsProcessActions ??= new List<Action<float>>(1);
            _onPhysicsProcessActions.Add(action);
            SetPhysicsProcess(true);
            return this;
        }}

        public {className} OnInput(Action<InputEvent> action) {{
            _onInputActions ??= new List<Action<InputEvent>>(1);
            _onInputActions.Add(action);
            SetProcessInput(true);
            return this;
        }}

        public {className} OnUnhandledInput(Action<InputEvent> action) {{
            _onUnhandledInputActions ??= new List<Action<InputEvent>>(1);
            _onUnhandledInputActions.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }}

        public {className} OnUnhandledKeyInput(Action<InputEventKey> action) {{
            _onUnhandledKeyInputActions ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInputActions.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }}

        public {className} RemoveOnProcess(Action<float> action) {{
            _onProcessActions?.Remove(action);
            return this;
        }}

        public {className} RemoveOnPhysicsProcess(Action<float> action) {{
            _onPhysicsProcessActions?.Remove(action);
            return this;
        }}

        public {className} RemoveOnInput(Action<InputEvent> action) {{
            _onInputActions?.Remove(action);
            return this;
        }}

        public {className} RemoveOnUnhandledInput(Action<InputEvent> action) {{
            _onUnhandledInputActions?.Remove(action);
            return this;
        }}

        public {className} RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {{
            _onUnhandledKeyInputActions?.Remove(action);
            return this;
        }}

        public override void _Process(float delta) {{
            if (_onProcessActions == null || _onProcessActions.Count == 0) {{
                SetProcess(false);
                return;
            }}
            for (var i = 0; i < _onProcessActions.Count; i++) _onProcessActions[i].Invoke(delta);
        }}

        public override void _PhysicsProcess(float delta) {{
            if (_onPhysicsProcessActions == null || _onPhysicsProcessActions.Count == 0) {{
                SetPhysicsProcess(false);
                return;
            }}
            for (var i = 0; i < _onPhysicsProcessActions.Count; i++) _onPhysicsProcessActions[i].Invoke(delta);
        }}

        public override void _Input(InputEvent @event) {{
            if (_onInputActions == null || _onInputActions?.Count == 0) {{
                SetProcessInput(false);
                return;
            }}
            for (var i = 0; i < _onInputActions.Count; i++) _onInputActions[i].Invoke(@event);
        }}

        public override void _UnhandledInput(InputEvent @event) {{
            if (_onUnhandledInputActions == null || _onUnhandledInputActions.Count == 0) {{
                SetProcessUnhandledInput(false);
                return;
            }}
            for (var i = 0; i < _onUnhandledInputActions.Count; i++) _onUnhandledInputActions[i].Invoke(@event);
        }}

        public override void _UnhandledKeyInput(InputEventKey @event) {{
            if (_onUnhandledKeyInputActions == null || _onUnhandledKeyInputActions.Count == 0) {{
                SetProcessUnhandledKeyInput(false);
                return;
            }}
            for (var i = 0; i < _onUnhandledKeyInputActions.Count; i++) _onUnhandledKeyInputActions[i].Invoke(@event);
        }}";
    }
}

public static class Tools {
    public static string CamelCase(string name) =>
        name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
            .Aggregate(string.Empty, (s1, s2) => s1 + s2);
}