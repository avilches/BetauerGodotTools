using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Betauer.SourceGenerators.NodeEvents;

public class NodeEventsClassGenerator(SourceProductionContext generatorExecutionContext, ClassDeclarationSyntax cds, INamedTypeSymbol symbol, string classGeneratedSuffix)
    : ClassGenerator(generatorExecutionContext, cds, symbol, classGeneratedSuffix) {

    private const string AttributeNamespace = "Betauer.Core.Nodes.Events";
    private const string ProcessAttributeName = "Process";
    private const string PhysicsProcessAttributeName = "PhysicsProcess";
    private const string NotificationsAttributeName = "Notifications";
    private const string InputEventsAttributeName = "InputEvents";

    private const string ProcessAttributeFullName = $"{AttributeNamespace}.{ProcessAttributeName}Attribute";
    private const string PhysicsProcessAttributeFullName = $"{AttributeNamespace}.{PhysicsProcessAttributeName}Attribute";
    private const string NotificationsAttributeFullName = $"{AttributeNamespace}.{NotificationsAttributeName}Attribute";
    private const string InputEventsAttributeFullName = $"{AttributeNamespace}.{InputEventsAttributeName}Attribute";

    public bool HasPartialNotificationMethod { get; private set; }
    public string PartialNotificationParameterName { get; private set; }

    public bool HasPartialProcessMethod { get; private set; }
    public bool HasPartialPhysicsProcessMethod { get; private set; }
    public string PartialProcessParameterName { get; private set; }
    public string PartialPhysicsProcessParameterName { get; private set; }

    public bool HasPartialInputMethod { get; private set; }
    public bool HasPartialUnhandledInputMethod { get; private set; }
    public bool HasPartialUnhandledKeyInputMethod { get; private set; }
    public bool HasPartialShortcutInputMethod { get; private set; }

    public string PartialInputParameterName { get; private set; }
    public string PartialUnhandledInputParameterName { get; private set; }
    public string PartialUnhandledKeyInputParameterName { get; private set; }
    public string PartialShortcutInputParameterName { get; private set; }

    public bool AddProcess { get; private set; }
    public bool AddPhysicsProcess { get; private set; }

    public bool AddNotification { get; private set; }

    public bool AddInput { get; private set; }
    public bool AddUnhandledInput { get; private set; }
    public bool AddUnhandledKeyInput { get; private set; }
    public bool AddShortcutInput { get; private set; }

    public bool HasAttribute(string attributeFullName, string disabler) {
        var attribute =  Symbol.GetAttribute(attributeFullName);
        if (attribute == null) return false;
        return !attribute.NamedArguments.Any(named => named.Key == disabler && !(bool)named.Value.Value!);
    }
    
    public bool Verify() {
        AddProcess = HasAttribute(NotificationsAttributeFullName, "Process") || Symbol.HasAttribute(ProcessAttributeFullName);
        AddPhysicsProcess = HasAttribute(NotificationsAttributeFullName, "PhysicsProcess") || Symbol.HasAttribute(PhysicsProcessAttributeFullName);
        AddNotification = Symbol.HasAttribute(NotificationsAttributeFullName);

        AddInput = HasAttribute(InputEventsAttributeFullName, "Handled");
        AddUnhandledInput = HasAttribute(InputEventsAttributeFullName, "Unhandled");
        AddUnhandledKeyInput = HasAttribute(InputEventsAttributeFullName, "UnhandledKey");
        AddShortcutInput = HasAttribute(InputEventsAttributeFullName, "Shortcut");
        if (!AddProcess &&
            !AddPhysicsProcess &&
            !AddNotification &&
            !AddInput &&
            !AddUnhandledInput &&
            !AddUnhandledKeyInput &&
            !AddShortcutInput) return false;
        
        (HasPartialProcessMethod, PartialProcessParameterName) = LoadProcessMethod("_Process");
        (HasPartialPhysicsProcessMethod, PartialPhysicsProcessParameterName) = LoadProcessMethod("_PhysicsProcess");

        (HasPartialInputMethod, PartialInputParameterName) = LoadInputMethod("_Input");
        (HasPartialUnhandledInputMethod, PartialUnhandledInputParameterName) = LoadInputMethod("_UnhandledInput");
        (HasPartialUnhandledKeyInputMethod, PartialUnhandledKeyInputParameterName) = LoadInputMethod("_UnhandledKeyInput");
        (HasPartialShortcutInputMethod, PartialShortcutInputParameterName) = LoadInputMethod("_ShortcutInput");

        (HasPartialNotificationMethod, PartialNotificationParameterName) = LoadNotificationMethod();

        if (AddProcess && !HasPartialProcessMethod) {
            ReportNonPartialMethod("B001", "_Process", "(double delta)", Symbol.HasAttribute(ProcessAttributeFullName) ? "Process" : NotificationsAttributeName);
        }

        if (AddPhysicsProcess && !HasPartialPhysicsProcessMethod) {
            ReportNonPartialMethod("B002", "_PhysicsProcess", "(double delta)", Symbol.HasAttribute(PhysicsProcessAttributeFullName) ? "Process" : NotificationsAttributeName);
        }

        if (AddNotification && !HasPartialNotificationMethod) {
            ReportNonPartialMethod("B003", "_Notification", "(int what)", NotificationsAttributeName);
        }

        if (AddInput && !HasPartialInputMethod) {
            ReportNonPartialMethod("B004", "_Input", "(InputEvent @event)", InputEventsAttributeName);
        }

        if (AddUnhandledInput && !HasPartialUnhandledInputMethod) {
            ReportNonPartialMethod("B005", "_UnhandledInput", "(InputEvent @event)", InputEventsAttributeName);
        }

        if (AddUnhandledKeyInput && !HasPartialUnhandledKeyInputMethod) {
            ReportNonPartialMethod("B006", "_UnhandledKeyInput", "(InputEvent @event)", InputEventsAttributeName);
        }

        if (AddShortcutInput && !HasPartialShortcutInputMethod) {
            ReportNonPartialMethod("B007", "_ShortcutInput", "(InputEvent @event)", InputEventsAttributeName);
        }
        return true;
    }

    private (bool, string?) LoadNotificationMethod() {
        var method = Cds.FindPartialMethod("_Notification", new[] { "int" });
        return (method != null, method?.ParameterList.Parameters.First().Identifier.ValueText);
    }

    private (bool, string?) LoadInputMethod(string inputName) {
        var method = Cds.FindPartialMethod(inputName, new[] { "InputEvent" });
        var param = method?.ParameterList.Parameters.First().Identifier.ValueText;
        return (method != null, param == "event" ? "@event" : param);
    }

    private (bool, string?) LoadProcessMethod(string processName) {
        var method = Cds.FindPartialMethod(processName, new[] { "double" });
        return (method != null, method?.ParameterList.Parameters.First().Identifier.ValueText);
    }

    public void GenerateSource() {
        AppendLine("#pragma warning disable CS8618");
        AppendLine("#nullable enable");
        AppendLine("using System;");
        AppendLine("using Godot;");
        AppendLine($"using {AttributeNamespace};");

        var interfaces = new List<string>(6);
        if (AddNotification) interfaces.Add("INotificationEvent");
        else {
            if (AddProcess) interfaces.Add("IProcessEvent");
            if (AddPhysicsProcess) interfaces.Add("IPhysicsProcessEvent");
        }
        if (AddInput) interfaces.Add("IInputEvent");
        if (AddUnhandledInput) interfaces.Add("IUnhandledInputEvent");
        if (AddUnhandledKeyInput) interfaces.Add("IUnhandledKeyInputEvent");
        if (AddShortcutInput) interfaces.Add("IShortcutInputEvent");

        AddNameSpace();
        AddParentInnerClasses();
        AppendLine($"partial class {Symbol.NameWithTypeParameters()} : {string.Join(", ", interfaces)} {{");
        BeginScope();

        if (AddProcess || AddNotification) OnProcess();

        if (AddPhysicsProcess || AddNotification) OnPhysicsProcess();
        
        if (AddProcess && HasPartialProcessMethod) _Process(PartialProcessParameterName);

        if (AddPhysicsProcess && HasPartialPhysicsProcessMethod) _PhysicsProcess(PartialPhysicsProcessParameterName);

        if (AddInput) {
            OnInput();
            if (HasPartialInputMethod) _InputEvent(PartialInputParameterName);
        }

        if (AddUnhandledInput) {
            OnUnhandledInput();
            if (HasPartialUnhandledInputMethod) _UnhandledInput(PartialUnhandledInputParameterName);
        }

        if (AddUnhandledKeyInput) {
            OnUnhandledKeyInput();
            if (HasPartialUnhandledKeyInputMethod) _UnhandledKeyInput(PartialUnhandledKeyInputParameterName);
        }

        if (AddShortcutInput) {
            OnShortcutInput();
            if (HasPartialShortcutInputMethod) _ShortcutInput(PartialShortcutInputParameterName);
        }

        if (AddNotification) {
            OnNotificationEvents();
            if (HasPartialNotificationMethod) _Notification(PartialNotificationParameterName, AddProcess, AddPhysicsProcess);
        }

        EndScope();
        AppendLine("}");
        EndParentInnerClasses();
        AppendLine("#nullable disable");
        AppendLine("#pragma warning restore CS8618");

        Context.AddSource(UniqueHint, Content.ToString());
    }
    
    private void OnProcess() =>
        AppendLine($$"""
            private Action<double>? __processAction;
            
            /// <para>Called during the processing step of the main loop. Processing happens at every frame and as fast as possible, so the <c>delta</c> time since the previous frame is not constant. <c>delta</c> is in seconds.</para>
            /// <para>It is only called if processing is enabled and can be toggled with <see cref="M:Godot.Node.SetProcess(System.Boolean)" />.</para>
            /// <para>Corresponds to the <see cref="F:Godot.Node.NotificationProcess" /> notification in <see cref="M:Godot.GodotObject._Notification(System.Int32)" />.</para>
            /// <para><b>Note:</b> This event is only called if the node is present in the scene tree (i.e. if it's not an orphan).</para>
            public event Action<double>? OnProcess {
            	add {
            		__processAction += value;
            		SetProcess(true);
            	}
            	remove {
            		__processAction -= value;
            		SetProcess(__processAction != null);
            	}
            }
            
            """);

    private void OnPhysicsProcess() =>
        AppendLine($$"""
            private Action<double>? __physicsProcessAction;
            
            /// <summary>
            /// <para>Called during the physics processing step of the main loop. Physics processing means that the frame rate is synced to the physics, i.e. the <c>delta</c> variable should be constant. <c>delta</c> is in seconds.</para>
            /// <para>It is only called if physics processing is enabled and can be toggled with <see cref="M:Godot.Node.SetPhysicsProcess(System.Boolean)" />.</para>
            /// <para>Corresponds to the <see cref="F:Godot.Node.NotificationPhysicsProcess" /> notification in <see cref="M:Godot.GodotObject._Notification(System.Int32)" />.</para>
            /// <para><b>Note:</b> This event is only called if the node is present in the scene tree (i.e. if it's not an orphan).</para>
            /// </summary>
            public event Action<double> OnPhysicsProcess {
                add {
                    __physicsProcessAction += value;
                    SetPhysicsProcess(true);
                }
                remove {
                    __physicsProcessAction -= value;
                    SetPhysicsProcess(__physicsProcessAction != null);
                }
            }
            
            """);

    private void OnInput() =>
        AppendLine($$"""
            private Action<InputEvent>? __inputAction;
            
            /// <summary>
            /// <para>Called when there is an input event. The input event propagates up through the node tree until a node consumes it.</para>
            /// <para>It is only called if input processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetProcessInput(System.Boolean)" />.</para>
            /// <para>To consume the input event and stop it propagating further to other nodes, <see cref="M:Godot.Viewport.SetInputAsHandled" /> can be called.</para>
            /// <para>For gameplay input, <see cref="M:Godot.Node._UnhandledInput(Godot.InputEvent)" /> and <see cref="M:Godot.Node._UnhandledKeyInput(Godot.InputEvent)" /> are usually a better fit as they allow the GUI to intercept the events first.</para>
            /// <para><b>Note:</b> This method is only called if the node is present in the scene tree (i.e. if it's not an orphan).</para>
            /// </summary>
            public event Action<InputEvent>? OnInput {
            	add {
            		__inputAction += value;
            		SetProcessInput(true);
            	}
            	remove {
            		__inputAction -= value;
            		SetProcessInput(__inputAction != null);
            	}
            }
            
            """);

    private void OnUnhandledInput() =>
        AppendLine($$"""
            private Action<InputEvent>? __unhandledInputAction;
            
            /// <summary>
            /// <para>Called when an <see cref="T:Godot.InputEvent" /> hasn't been consumed by <see cref="M:Godot.Node._Input(Godot.InputEvent)" /> or any GUI <see cref="T:Godot.Control" /> item. The input event propagates up through the node tree until a node consumes it.</para>
            /// <para>It is only called if unhandled input processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetProcessUnhandledInput(System.Boolean)" />.</para>
            /// <para>To consume the input event and stop it propagating further to other nodes, <see cref="M:Godot.Viewport.SetInputAsHandled" /> can be called.</para>
            /// <para>For gameplay input, this and <see cref="M:Godot.Node._UnhandledKeyInput(Godot.InputEvent)" /> are usually a better fit than <see cref="M:Godot.Node._Input(Godot.InputEvent)" /> as they allow the GUI to intercept the events first.</para>
            /// <para><b>Note:</b> This method is only called if the node is present in the scene tree (i.e. if it's not an orphan).</para>
            /// </summary>
            public event Action<InputEvent> OnUnhandledInput {
            	add {
            		__unhandledInputAction += value;
            		SetProcessUnhandledInput(true);
            	}
            	remove {
            		__unhandledInputAction -= value;
            		SetProcessUnhandledInput(__unhandledInputAction != null);
            	}
            }
            
            """);

    private void OnUnhandledKeyInput() =>
        AppendLine($$"""
            private Action<InputEvent>? __unhandledKeyInputAction;
            
            /// <summary>
            /// <para>Called when an <see cref="T:Godot.InputEventKey" /> hasn't been consumed by <see cref="M:Godot.Node._Input(Godot.InputEvent)" /> or any GUI <see cref="T:Godot.Control" /> item. The input event propagates up through the node tree until a node consumes it.</para>
            /// <para>It is only called if unhandled key input processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetProcessUnhandledKeyInput(System.Boolean)" />.</para>
            /// <para>To consume the input event and stop it propagating further to other nodes, <see cref="M:Godot.Viewport.SetInputAsHandled" /> can be called.</para>
            /// <para>This method can be used to handle Unicode character input with Alt, Alt + Ctrl, and Alt + Shift modifiers, after shortcuts were handled.</para>
            /// <para>For gameplay input, this and <see cref="M:Godot.Node._UnhandledInput(Godot.InputEvent)" /> are usually a better fit than <see cref="M:Godot.Node._Input(Godot.InputEvent)" /> as they allow the GUI to intercept the events first.</para>
            /// <para>This method also performs better than <see cref="M:Godot.Node._UnhandledInput(Godot.InputEvent)" />, since unrelated events such as <see cref="T:Godot.InputEventMouseMotion" /> are automatically filtered.</para>
            /// <para><b>Note:</b> This method is only called if the node is present in the scene tree (i.e. if it's not an orphan).</para>
            /// </summary>
            public event Action<InputEvent> OnUnhandledKeyInput {
            	add {
            		__unhandledKeyInputAction += value;
            		SetProcessUnhandledKeyInput(true);
            	}
            	remove {
            		__unhandledKeyInputAction -= value;
            		SetProcessUnhandledKeyInput(__unhandledKeyInputAction != null);
            	}
            }
            
            """);

    private void OnShortcutInput() =>
        AppendLine($$"""
            private Action<InputEvent>? __shortcutInputAction;
            
            /// <summary>
            /// <para>Called when an <see cref="T:Godot.InputEventKey" /> or <see cref="T:Godot.InputEventShortcut" /> hasn't been consumed by <see cref="M:Godot.Node._Input(Godot.InputEvent)" /> or any GUI <see cref="T:Godot.Control" /> item. The input event propagates up through the node tree until a node consumes it.</para>
            /// <para>It is only called if shortcut processing is enabled, which is done automatically if this method is overridden, and can be toggled with <see cref="M:Godot.Node.SetProcessShortcutInput(System.Boolean)" />.</para>
            /// <para>To consume the input event and stop it propagating further to other nodes, <see cref="M:Godot.Viewport.SetInputAsHandled" /> can be called.</para>
            /// <para>This method can be used to handle shortcuts.</para>
            /// <para><b>Note:</b> This method is only called if the node is present in the scene tree (i.e. if it's not orphan).</para>
            /// </summary>
            public event Action<InputEvent> OnShortcutInput {
            	add {
            		__shortcutInputAction += value;
            		SetProcessShortcutInput(true);
            	}
            	remove {
            		__shortcutInputAction -= value;
            		SetProcessShortcutInput(__shortcutInputAction != null);
            	}
            }
            
            """);

    private void _Process(string partialProcessParameterName) =>
        AppendLine($$"""
            public override partial void _Process(double {{partialProcessParameterName}}) {
                if (__processAction == null) SetProcess(false);
                else __processAction.Invoke({{partialProcessParameterName}});
            }
            
            """);

    private void _PhysicsProcess(string partialPhysicsProcessParameterName) =>
        AppendLine($$"""
            public override partial void _PhysicsProcess(double {{partialPhysicsProcessParameterName}}) {
                if (__physicsProcessAction == null) SetPhysicsProcess(false);
                else __physicsProcessAction.Invoke({{partialPhysicsProcessParameterName}});
            }
            
            """);

    private void _InputEvent(string partialInputParameterName) =>
        AppendLine($$"""
            public override partial void _Input(InputEvent {{partialInputParameterName}}) {
            	if (__inputAction == null) SetProcessInput(false);
            	else __inputAction.Invoke({{partialInputParameterName}});
            }
            
            """);

    private void _UnhandledInput(string partialUnhandledInputParameterName) =>
        AppendLine($$"""
            public override partial void _UnhandledInput(InputEvent {{partialUnhandledInputParameterName}}) {
            	if (__unhandledInputAction == null) SetProcessUnhandledInput(false);
            	else __unhandledInputAction.Invoke({{partialUnhandledInputParameterName}});
            }
            
            """);

    private void _UnhandledKeyInput(string partialUnhandledKeyInputParameterName) =>
        AppendLine($$"""
            public override partial void _UnhandledKeyInput(InputEvent {{partialUnhandledKeyInputParameterName}}) {
               if (__unhandledKeyInputAction == null) SetProcessUnhandledKeyInput(false);
               else __unhandledKeyInputAction.Invoke({{partialUnhandledKeyInputParameterName}});
            }
            
            """);

    private void _ShortcutInput(string partialShortcutInputParameterName) =>
        AppendLine($$"""
            public override partial void _ShortcutInput(InputEvent {{partialShortcutInputParameterName}}) {
            	if (__shortcutInputAction == null) SetProcessShortcutInput(false);
            	else __shortcutInputAction.Invoke({{partialShortcutInputParameterName}});
            }
            
            """);

    private void OnNotificationEvents() {
        var parentClass = Symbol.BaseType;
        while (parentClass!.ToDisplayString() != "Godot.Node") {
            AppendLine($"\n// Godot.{parentClass!.ToDisplayString()} notifications");
            if (Notifications.Names.TryGetValue(parentClass!.ToDisplayString(), out var names)) {
                foreach (var name in names) {
                    AppendLine(
                        $"""
                        /// <summary>
                        /// <para>Event called when the node receives the {name} notification.</para>
                        /// </summary>
                        public event Action On{name.Replace("Notification", "")};
                        """);
                }
            }
            parentClass = parentClass.BaseType;
        }
        AppendLine($$"""

            // Godot.Node notifications
            public event Action OnPaused;
            /// <summary>
            /// <para>Event called when the node is unpaused.</para>
            /// </summary>
            public event Action OnUnpaused;
            /// <summary>
            /// <para>Event called every frame when the physics process flag is set (see <see cref="M:Godot.Node.SetPhysicsProcess(System.Boolean)" />).</para>
            /// </summary>
            public event Action OnParented;
            /// <summary>
            /// <para>Event called when a node is unparented (parent removed it from the list of children).</para>
            /// </summary>
            public event Action OnUnparented;
            /// <summary>
            /// <para>Event called by scene owner when its scene is instantiated.</para>
            /// </summary>
            public event Action OnSceneInstantiated;
            /// <summary>
            /// <para>Event called when a drag operation begins. All nodes receive this notification, not only the dragged one.</para>
            /// <para>Can be triggered either by dragging a <see cref="T:Godot.Control" /> that provides drag data (see <see cref="M:Godot.Control._GetDragData(Godot.Vector2)" />) or using <see cref="M:Godot.Control.ForceDrag(Godot.Variant,Godot.Control)" />.</para>
            /// <para>Use <see cref="M:Godot.Viewport.GuiGetDragData" /> to get the dragged data.</para>
            /// </summary>
            public event Action OnDragBegin;
            /// <summary>
            /// <para>Event called when a drag operation ends.</para>
            /// <para>Use <see cref="M:Godot.Viewport.GuiIsDragSuccessful" /> to check if the drag succeeded.</para>
            /// </summary>
            public event Action OnDragEnd;
            /// <summary>
            /// <para>Event called when the node's name or one of its parents' name is changed. This notification is <i>not</i> received when the node is removed from the scene tree to be added to another parent later on.</para>
            /// </summary>
            public event Action OnPathRenamed;
            /// <summary>
            /// <para>Event called when the list of children is changed. This happens when child nodes are added, moved or removed.</para>
            /// </summary>
            public event Action OnChildOrderChanged;
            /// <summary>
            /// <para>Event called every frame when the internal process flag is set (see <see cref="M:Godot.Node.SetProcessInternal(System.Boolean)" />).</para>
            /// </summary>
            public event Action OnInternalProcess;
            /// <summary>
            /// <para>Event called every frame when the internal physics process flag is set (see <see cref="M:Godot.Node.SetPhysicsProcessInternal(System.Boolean)" />).</para>
            /// </summary>
            public event Action OnInternalPhysicsProcess;
            /// <summary>
            /// <para>Event called when the node is ready, just before <see cref="F:Godot.Node.NotificationReady" /> is received. Unlike the latter, it's sent every time the node enters tree, instead of only once.</para>
            /// </summary>
            public event Action OnPostEnterTree;
            /// <summary>
            /// <para>Event called when the node is disabled. See <see cref="F:Godot.Node.ProcessModeEnum.Disabled" />.</para>
            /// </summary>
            public event Action OnDisabled;
            /// <summary>
            /// <para>Event called when the node is enabled again after being disabled. See <see cref="F:Godot.Node.ProcessModeEnum.Disabled" />.</para>
            /// </summary>
            public event Action OnEnabled;
            /// <summary>
            /// <para>Event called right before the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.</para>
            /// </summary>
            public event Action OnEditorPreSave;
            /// <summary>
            /// <para>Event called right after the scene with the node is saved in the editor. This notification is only sent in the Godot editor and will not occur in exported projects.</para>
            /// </summary>
            public event Action OnEditorPostSave;
            /// <summary>
            /// <para>Event called from the OS when the mouse enters the game window.</para>
            /// <para>Implemented on desktop and web platforms.</para>
            /// </summary>
            public event Action OnWMMouseEnter;
            /// <summary>
            /// <para>Event called from the OS when the mouse leaves the game window.</para>
            /// <para>Implemented on desktop and web platforms.</para>
            /// </summary>
            public event Action OnWMMouseExit;
            /// <summary>
            /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is focused. This may be a change of focus between two windows of the same engine instance, or from the OS desktop or a third-party application to a window of the game (in which case <see cref="F:Godot.Node.NotificationApplicationFocusIn" /> is also emitted).</para>
            /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is focused.</para>
            /// </summary>
            public event Action OnWMWindowFocusIn;
            /// <summary>
            /// <para>Event called when the node's parent <see cref="T:Godot.Window" /> is defocused. This may be a change of focus between two windows of the same engine instance, or from a window of the game to the OS desktop or a third-party application (in which case <see cref="F:Godot.Node.NotificationApplicationFocusOut" /> is also emitted).</para>
            /// <para>A <see cref="T:Godot.Window" /> node receives this notification when it is defocused.</para>
            /// </summary>
            public event Action OnWMWindowFocusOut;
            /// <summary>
            /// <para>Event called from the OS when a close request is sent (e.g. closing the window with a "Close" button or Alt + F4).</para>
            /// <para>Implemented on desktop platforms.</para>
            /// </summary>
            public event Action OnWMCloseRequest;
            /// <summary>
            /// <para>Event called from the OS when a go back request is sent (e.g. pressing the "Back" button on Android).</para>
            /// <para>Specific to the Android platform.</para>
            /// </summary>
            public event Action OnWMGoBackRequest;
            /// <summary>
            /// <para>Event called from the OS when the window is resized.</para>
            /// </summary>
            public event Action OnWMSizeChanged;
            /// <summary>
            /// <para>Event called from the OS when the screen's DPI has been changed. Only implemented on macOS.</para>
            /// </summary>
            public event Action OnWMDpiChange;
            /// <summary>
            /// <para>Event called when the mouse enters the viewport.</para>
            /// </summary>
            public event Action OnVpMouseEnter;
            /// <summary>
            /// <para>Event called when the mouse leaves the viewport.</para>
            /// </summary>
            public event Action OnVpMouseExit;
            /// <summary>
            /// <para>Event called from the OS when the application is exceeding its allocated memory.</para>
            /// <para>Specific to the iOS platform.</para>
            /// </summary>
            public event Action OnOsMemoryWarning;
            /// <summary>
            /// <para>Event called when translations may have changed. Can be triggered by the user changing the locale. Can be used to respond to language changes, for example to change the UI strings on the fly. Useful when working with the built-in translation support, like <see cref="M:Godot.GodotObject.Tr(Godot.StringName,Godot.StringName)" />.</para>
            /// </summary>
            public event Action OnTranslationChanged;
            /// <summary>
            /// <para>Event called from the OS when a request for "About" information is sent.</para>
            /// <para>Specific to the macOS platform.</para>
            /// </summary>
            public event Action OnWMAbout;
            /// <summary>
            /// <para>Event called from Godot's crash handler when the engine is about to crash.</para>
            /// <para>Implemented on desktop platforms if the crash handler is enabled.</para>
            /// </summary>
            public event Action OnCrash;
            /// <summary>
            /// <para>Event called from the OS when an update of the Input Method Engine occurs (e.g. change of IME cursor position or composition string).</para>
            /// <para>Specific to the macOS platform.</para>
            /// </summary>
            public event Action OnOsImeUpdate;
            /// <summary>
            /// <para>Event called from the OS when the application is resumed.</para>
            /// <para>Specific to the Android platform.</para>
            /// </summary>
            public event Action OnApplicationResumed;
            /// <summary>
            /// <para>Event called from the OS when the application is paused.</para>
            /// <para>Specific to the Android platform.</para>
            /// </summary>
            public event Action OnApplicationPaused;
            /// <summary>
            /// <para>Event called from the OS when the application is focused, i.e. when changing the focus from the OS desktop or a thirdparty application to any open window of the Godot instance.</para>
            /// <para>Implemented on desktop platforms.</para>
            /// </summary>
            public event Action OnApplicationFocusIn;
            /// <summary>
            /// <para>Event called from the OS when the application is defocused, i.e. when changing the focus from any open window of the Godot instance to the OS desktop or a thirdparty application.</para>
            /// <para>Implemented on desktop platforms.</para>
            /// </summary>
            public event Action OnApplicationFocusOut;
            /// <summary>
            /// <para>Event called when text server is changed.</para>
            /// </summary>
            public event Action OnTextServerChanged;
            
            // Godot.GodotObject notifications
            /// <summary>
            /// <para>Event called when the object is initialized, before its script is attached. Used internally.</para>
            /// </summary>
            public event Action OnPostinitialize;
            /// <summary>
            /// <para>Event called when the object is about to be deleted. Can act as the deconstructor of some programming languages.</para>
            /// </summary>
            public event Action OnPredelete;
            /// <summary>
            /// <para>Event called when the node is paused.</para>
            /// </summary>
            
            """);
    }

    private void _Notification(string partialNotificationParameterName, bool addProcess, bool addPhysicsProcess) {
          AppendLine(
              $$"""
              public override partial void _Notification(int {{partialNotificationParameterName}}) {
                  switch ((long){{partialNotificationParameterName}}) {
              """);
          
          var parentClass = Symbol.BaseType;
          while (parentClass!.ToDisplayString() != "Godot.Node") {
              AppendLine($"\n        // Godot.{parentClass!.ToDisplayString()} notifications");
              if (Notifications.Names.TryGetValue(parentClass!.ToDisplayString(), out var names)) {
                  foreach (var name in names) {
                      AppendLine(
                          $"""
                                   case {name}:
                                       On{name.Replace("Notification", "")}?.Invoke();
                                       break;
                           """);
                  }
              }
              parentClass = parentClass.BaseType;
          }


          AppendLine(
              """
              
                      // Godot.Node notifications
                      case NotificationPaused:
                          OnPaused?.Invoke();
                          break;
                      case NotificationUnpaused:
                          OnUnpaused?.Invoke();
                          break;
                      case NotificationParented:
                          OnParented?.Invoke();
                          break;
                      case NotificationUnparented:
                          OnUnparented?.Invoke();
                          break;
                      case NotificationSceneInstantiated:
                          OnSceneInstantiated?.Invoke();
                          break;
                      case NotificationDragBegin:
                          OnDragBegin?.Invoke();
                          break;
                      case NotificationDragEnd:
                          OnDragEnd?.Invoke();
                          break;
                      case NotificationPathRenamed:
                          OnPathRenamed?.Invoke();
                          break;
                      case NotificationChildOrderChanged:
                          OnChildOrderChanged?.Invoke();
                          break;
                      case NotificationInternalProcess:
                          OnInternalProcess?.Invoke();
                          break;
                      case NotificationInternalPhysicsProcess:
                          OnInternalPhysicsProcess?.Invoke();
                          break;
                      case NotificationPostEnterTree:
                          OnPostEnterTree?.Invoke();
                          break;
                      case NotificationDisabled:
                          OnDisabled?.Invoke();
                          break;
                      case NotificationEnabled:
                          OnEnabled?.Invoke();
                          break;
                      case NotificationEditorPreSave:
                          OnEditorPreSave?.Invoke();
                          break;
                      case NotificationEditorPostSave:
                          OnEditorPostSave?.Invoke();
                          break;
                      case NotificationWMMouseEnter:
                          OnWMMouseEnter?.Invoke();
                          break;
                      case NotificationWMMouseExit:
                          OnWMMouseExit?.Invoke();
                          break;
                      case NotificationWMWindowFocusIn:
                          OnWMWindowFocusIn?.Invoke();
                          break;
                      case NotificationWMWindowFocusOut:
                          OnWMWindowFocusOut?.Invoke();
                          break;
                      case NotificationWMCloseRequest:
                          OnWMCloseRequest?.Invoke();
                          break;
                      case NotificationWMGoBackRequest:
                          OnWMGoBackRequest?.Invoke();
                          break;
                      case NotificationWMSizeChanged:
                          OnWMSizeChanged?.Invoke();
                          break;
                      case NotificationWMDpiChange:
                          OnWMDpiChange?.Invoke();
                          break;
                      case NotificationVpMouseEnter:
                          OnVpMouseEnter?.Invoke();
                          break;
                      case NotificationVpMouseExit:
                          OnVpMouseExit?.Invoke();
                          break;
                      case NotificationOsMemoryWarning:
                          OnOsMemoryWarning?.Invoke();
                          break;
                      case NotificationTranslationChanged:
                          OnTranslationChanged?.Invoke();
                          break;
                      case NotificationWMAbout:
                          OnWMAbout?.Invoke();
                          break;
                      case NotificationCrash:
                          OnCrash?.Invoke();
                          break;
                      case NotificationOsImeUpdate:
                          OnOsImeUpdate?.Invoke();
                          break;
                      case NotificationApplicationResumed:
                          OnApplicationResumed?.Invoke();
                          break;
                      case NotificationApplicationPaused:
                          OnApplicationPaused?.Invoke();
                          break;
                      case NotificationApplicationFocusIn:
                          OnApplicationFocusIn?.Invoke();
                          break;
                      case NotificationApplicationFocusOut:
                          OnApplicationFocusOut?.Invoke();
                          break;
                      case NotificationTextServerChanged:
                          OnTextServerChanged?.Invoke();
                          break;
              """);
          if (!addPhysicsProcess) {
              AppendLine(
                  """
                          case NotificationPhysicsProcess:
                              __physicsProcessAction?.Invoke(GetProcessDeltaTime());
                              break;
                  """);
          }
          if (!addProcess) {
              AppendLine(
                  """
                          case NotificationProcess:
                              __processAction?.Invoke(GetPhysicsProcessDeltaTime());
                              break;
                  """);
          }
          AppendLine(
              """
              
                      // Godot.GodotObject notifications
                      case NotificationPostinitialize:
                          OnPostinitialize?.Invoke();
                          break;
                      case NotificationPredelete:
                          OnPredelete?.Invoke();
                          break;
                          
                  }
              }
              """);
    }
}