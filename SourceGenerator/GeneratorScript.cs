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
        private List<Action<float>>? _onProcessAction; 
        private List<Action<float>>? _onPhysicsProcess; 
        private List<Action<InputEvent>>? _onInput; 
        private List<Action<InputEvent>>? _onUnhandledInput; 
        private List<Action<InputEventKey>>? _onUnhandledKeyInput;

        public {className} OnProcessAction(Action<float> action) {{
            _onProcessAction ??= new List<Action<float>>(1);
            _onProcessAction.Add(action);
            SetProcess(true);
            return this;
        }}
        public {className} OnPhysicsProcess(Action<float> action) {{
            _onPhysicsProcess ??= new List<Action<float>>(1);
            _onPhysicsProcess.Add(action);
            SetPhysicsProcess(true);
            return this;
        }}

        public {className} OnInput(Action<InputEvent> action) {{
            _onInput ??= new List<Action<InputEvent>>(1);
            _onInput.Add(action);
            SetProcessInput(true);
            return this;
        }}

        public {className} OnUnhandledInput(Action<InputEvent> action) {{
            _onUnhandledInput ??= new List<Action<InputEvent>>(1);
            _onUnhandledInput.Add(action);
            SetProcessUnhandledInput(true);
            return this;
        }}

        public {className} OnUnhandledKeyInput(Action<InputEventKey> action) {{
            _onUnhandledKeyInput ??= new List<Action<InputEventKey>>(1);
            _onUnhandledKeyInput.Add(action);
            SetProcessUnhandledKeyInput(true);
            return this;
        }}

        public {className} RemoveOnProcessAction(Action<float> action) {{
            _onProcessAction?.Remove(action);
            return this;
        }}

        public {className} RemoveOnPhysicsProcess(Action<float> action) {{
            _onPhysicsProcess?.Remove(action);
            return this;
        }}

        public {className} RemoveOnInput(Action<InputEvent> action) {{
            _onInput?.Remove(action);
            return this;
        }}

        public {className} RemoveOnUnhandledInput(Action<InputEvent> action) {{
            _onUnhandledInput?.Remove(action);
            return this;
        }}

        public {className} RemoveOnUnhandledKeyInput(Action<InputEventKey> action) {{
            _onUnhandledKeyInput?.Remove(action);
            return this;
        }}

        public override void _Process(float delta) {{
            if (_onProcessAction == null) {{
                SetProcess(false);
                return;
            }}
            for (var i = 0; i < _onProcessAction.Count; i++) _onProcessAction[i].Invoke(delta);
        }}

        public override void _PhysicsProcess(float delta) {{
            if (_onPhysicsProcess == null) {{
                SetPhysicsProcess(true);
                return;
            }}
            for (var i = 0; i < _onPhysicsProcess.Count; i++) _onPhysicsProcess[i].Invoke(delta);
        }}

        public override void _Input(InputEvent @event) {{
            if (_onInput == null) {{
                SetProcessInput(true);
                return;
            }}
            for (var i = 0; i < _onInput.Count; i++) _onInput[i].Invoke(@event);
        }}

        public override void _UnhandledInput(InputEvent @event) {{
            if (_onUnhandledInput == null) {{
                SetProcessUnhandledInput(true);
                return;
            }}
            for (var i = 0; i < _onUnhandledInput.Count; i++) _onUnhandledInput[i].Invoke(@event);
        }}

        public override void _UnhandledKeyInput(InputEventKey @event) {{
            if (_onUnhandledKeyInput == null) {{
                SetProcessUnhandledKeyInput(true);
                return;
            }}
            for (var i = 0; i < _onUnhandledKeyInput.Count; i++) _onUnhandledKeyInput[i].Invoke(@event);
        }}";
    }
}

public static class Tools {
    public static string CamelCase(string name) =>
        name.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
            .Aggregate(string.Empty, (s1, s2) => s1 + s2);
}