using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Generator {
    public class GenerateGodotActionClasses {
        private const string ActionClassesPath = "../Betauer.GodotAction/GodotAction";
        private const string GodotActionClassesNamespace = "Betauer.GodotAction";

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
            var nodeMethods = godotClass.IsNode ? CreateNodeMethods(className) : "";
            return $@"using System;
using Godot;
using System.Collections.Generic;
using Environment = Godot.Environment;
using Animation = Godot.Animation;
using Object = Godot.Object;

namespace {GodotActionClassesNamespace} {{
    public class {className} : Node {{
        public {className}() {{
            SetProcess(false);
            SetPhysicsProcess(false);
            SetProcessInput(false);
            SetProcessUnhandledInput(false);
            SetProcessUnhandledKeyInput(false);
        }}
{nodeMethods}
{string.Join("\n", methods)}
    }}
}}";
        }

        private static string CreateSignalMethod(Signal signal) {
            var actionVarName = $"_on{signal.MethodName}Action";
            var godotExecuteActionMethodName = $"_GodotSignal{signal.MethodName}";
            return $@"
        private List<Action{signal.Generics()}>? {actionVarName}; 
        public {signal.GodotClass.GeneratedClassName} On{signal.MethodName}(Action{signal.Generics()} action, bool oneShot = false, bool deferred = false) {{
            if ({actionVarName} == null || {actionVarName}.Count == 0) {{
                {actionVarName} ??= new List<Action{signal.Generics()}>(); 
                GetParent().Connect(""{signal.signal_name}"", this, nameof({godotExecuteActionMethodName}));
            }}
            {actionVarName}.Add(action);
            return this;
        }}
        public {signal.GodotClass.GeneratedClassName} RemoveOn{signal.MethodName}(Action{signal.Generics()} action) {{
            if ({actionVarName} == null || {actionVarName}.Count == 0) return this;
            {actionVarName}.Remove(action); 
            if ({actionVarName}.Count == 0) {{
                GetParent().Disconnect(""{signal.signal_name}"", this, nameof({godotExecuteActionMethodName}));
            }}
            return this;
        }}
        private void {godotExecuteActionMethodName}({signal.GetParamNamesWithType()}) {{
            if ({actionVarName} == null || {actionVarName}.Count == 0) return;
            for (var i = 0; i < {actionVarName}.Count; i++) {actionVarName}[i].Invoke({signal.GetParamNames()});
        }}
        ";
        }

        private static string CreateNodeMethods(string className) {
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
}