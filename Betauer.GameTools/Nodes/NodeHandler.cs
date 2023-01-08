using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Nodes {
    public class DefaultNodeHandler {
        public static readonly NodeHandler Instance = new() {
            Name = "NodeHandler"
        };

        public static void Configure(Viewport node) {
            if (Instance.GetParent() != null) Instance.GetParent().RemoveChild(Instance);
            node.AddChild(Instance);
        }
    }

    public interface INodeEvent {
        public void Disable();
        public void Enable();
        public void Destroy();
    }

    public partial class NodeHandler : Node {
        public static bool ShouldProcess(bool pause, ProcessModeEnum processMode) {
            if (processMode == ProcessModeEnum.Inherit) return !pause;
            return processMode == ProcessModeEnum.Always ||
                   (pause && processMode == ProcessModeEnum.WhenPaused) ||
                   (!pause && processMode == ProcessModeEnum.Pausable);
        }

        public interface IEventHandler {
            string? Name { get; }
            bool IsDestroyed { get; }
            public bool IsEnabled(bool isTreePaused);
        }

        public interface IInputEventHandler : IEventHandler {
            public void Handle(InputEvent inputEvent);
        }

        public interface IProcessHandler : IEventHandler {
            public void Handle(double delta);
        }

        private abstract class NodeEvent : INodeEvent, IEventHandler {
            private readonly Node? Node;
            private ProcessModeEnum ProcessMode;
            // public T Delegate { get; }
            public bool IsEnabled(bool isTreePaused) {
                return _isEnabled &&
                       ShouldProcess(isTreePaused, ProcessMode) &&
                       (Node == null || Node.IsInsideTree());
            }

            // Node can be null, so the Event will last forever
            public bool IsDestroyed => _isDestroyed || (Node != null && !IsInstanceValid(Node));
            
            private bool _isEnabled = true;
            private bool _isDestroyed = false;
            
            internal NodeEvent(Node? node, ProcessModeEnum pauseMode) {
                Node = node;
                ProcessMode = pauseMode;
            }

            public string? Name => Node == null ? "forever" : IsInstanceValid(Node) ? Node.Name : "disposed";

            public void Disable() => _isEnabled = false;
            public void Enable() => _isEnabled = true;
            public void Destroy() => _isDestroyed = true;
        }

        private class ProcessNodeEvent : NodeEvent, IProcessHandler {
            private readonly Action<double> _delegate;
            internal ProcessNodeEvent(Node? node, Action<double> @delegate, ProcessModeEnum pauseMode) : base(node, pauseMode) {
                _delegate = @delegate;
            }

            public void Handle(double delta) {
                _delegate(delta);
            }
        }
        private class InputEventNodeEvent : NodeEvent, IInputEventHandler {
            private readonly Action<InputEvent> _delegate;
            internal InputEventNodeEvent(Node? node, Action<InputEvent> @delegate, ProcessModeEnum pauseMode) : base(node, pauseMode) {
                _delegate = @delegate;
            }

            public void Handle(InputEvent delta) {
                _delegate(delta);
            }
        }

        public readonly List<IProcessHandler> OnProcessList = new();
        public readonly List<IProcessHandler> OnPhysicsProcessList = new();
        public readonly List<IInputEventHandler> OnInputList = new();
        public readonly List<IInputEventHandler> OnShortcutInputList = new();
        public readonly List<IInputEventHandler> OnUnhandledInputList = new();
        public readonly List<IInputEventHandler> OnUnhandledKeyInputList = new();
        
        private SceneTree _sceneTree;

        public override void _EnterTree() {
            _sceneTree = GetTree();
            ProcessMode = ProcessModeEnum.Always;
        }

        public INodeEvent OnProcess(Node node, Action<double> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new ProcessNodeEvent(node, action, pauseMode);
            OnProcess(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnPhysicsProcess(Node node, Action<double> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new ProcessNodeEvent(node, action, pauseMode);
            OnPhysicsProcess(nodeEvent);
            return nodeEvent;
        }

        public void OnProcess(IProcessHandler inputEvent) {
            OnProcessList.Add(inputEvent);
            SetProcess(true);
        }

        public void OnPhysicsProcess(IProcessHandler inputEvent) {
            OnPhysicsProcessList.Add(inputEvent);
            SetPhysicsProcess(true);
        }

        public INodeEvent OnInput(Node node, Action<InputEvent> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new InputEventNodeEvent(node, action, pauseMode);
            OnInput(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnUnhandledInput(Node node, Action<InputEvent> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new InputEventNodeEvent(node, action, pauseMode);
            OnUnhandledInput(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnShortcutInput(Node node, Action<InputEvent> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new InputEventNodeEvent(node, action, pauseMode);
            OnShortcutInput(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnUnhandledKeyInput(Node node, Action<InputEvent> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new InputEventNodeEvent(node, action, pauseMode);
            OnUnhandledKeyInput(nodeEvent);
            return nodeEvent;
        }

        public void OnInput(IInputEventHandler inputEvent) {
            OnInputList.Add(inputEvent);
            SetProcessInput(true);
        }

        public void OnUnhandledInput(IInputEventHandler inputEvent) {
            OnUnhandledInputList.Add(inputEvent);
            SetProcessUnhandledInput(true);
        }

        public void OnShortcutInput(IInputEventHandler inputEvent) {
            OnShortcutInputList.Add(inputEvent);
            SetProcessShortcutInput(true);
        }

        public void OnUnhandledKeyInput(IInputEventHandler inputEvent) {
            OnUnhandledKeyInputList.Add(inputEvent);
            SetProcessUnhandledKeyInput(true);
        }

        public override void _Process(double delta) {
            ProcessNodeEvents(OnProcessList, delta, () => SetProcess(false));
        }

        public override void _PhysicsProcess(double delta) {
            ProcessNodeEvents(OnPhysicsProcessList, delta, () => SetPhysicsProcess(false));
        }

        private void ProcessNodeEvents(List<IProcessHandler> processHandlerList, double delta, Action disabler) {
            if (processHandlerList.Count == 0) {
                disabler();
                return;
            }
            var isTreePaused = _sceneTree.Paused;
            processHandlerList.RemoveAll(processHandler => {
                if (processHandler.IsDestroyed) return true;
                if (processHandler.IsEnabled(isTreePaused)) {
                    processHandler.Handle(delta);
                }
                return false;
            });
        }

        public override void _Input(InputEvent inputEvent) {
            ProcessInputEventList(OnInputList, inputEvent, () => SetProcessInput(false));
        }

        public override void _UnhandledInput(InputEvent inputEvent) {
            ProcessInputEventList(OnUnhandledInputList, inputEvent, () => SetProcessUnhandledInput(false));
        }

        public override void _ShortcutInput(InputEvent inputEvent) {
            ProcessInputEventList(OnShortcutInputList, inputEvent, () => SetProcessShortcutInput(false));
        }

        public override void _UnhandledKeyInput(InputEvent inputEvent) {
            ProcessInputEventList(OnUnhandledKeyInputList, inputEvent, () => SetProcessUnhandledKeyInput(false));
        }

        private void ProcessInputEventList(List<IInputEventHandler> inputEventHandlerList, InputEvent inputEvent, Action disabler) {
            if (inputEventHandlerList.Count == 0) {
                disabler();
                return;
            }
            var isInputHandled = _sceneTree.Root.IsInputHandled();
            var isTreePaused = _sceneTree.Paused;
            inputEventHandlerList.RemoveAll(inputEventHandler => {
                if (inputEventHandler.IsDestroyed) return true;
                if (!isInputHandled && inputEventHandler.IsEnabled(isTreePaused)) {
                    inputEventHandler.Handle(inputEvent);
                    isInputHandled = _sceneTree.Root.IsInputHandled();
                }
                return false;
            });
        }

        public string GetStateAsString() {
            return 
$@"{OnProcessList.Count} Process: {string.Join(", ", OnProcessList.Select(e => e.Name))}
{OnPhysicsProcessList.Count} PhysicsProcess: {string.Join(", ", OnPhysicsProcessList.Select(e => e.Name))}
{OnInputList.Count} Input: {string.Join(", ", OnInputList.Select(e => e.Name))}
{OnUnhandledInputList.Count} UnhandledInput: {string.Join(", ", OnUnhandledInputList.Select(e => e.Name))}
{OnShortcutInputList.Count} ShortcutInput: {string.Join(", ", OnShortcutInputList.Select(e => e.Name))}
{OnUnhandledKeyInputList.Count} UnhandledKeyInput: {string.Join(", ", OnUnhandledKeyInputList.Select(e => e.Name))}";
        }
    }
    
    public static class NodeHandlerExtensions {
        public static INodeEvent OnProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnProcess(node, action, pauseMode);

        public static INodeEvent OnPhysicsProcess(this Node node, Action<double> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnPhysicsProcess(node, action, pauseMode);

        public static INodeEvent OnInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnInput(node, action, pauseMode);

        public static INodeEvent OnUnhandledInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnUnhandledInput(node, action, pauseMode);

        public static INodeEvent OnShortcutInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnShortcutInput(node, action, pauseMode);

        public static INodeEvent OnUnhandledKeyInput(this Node node, Action<InputEvent> action, Node.ProcessModeEnum pauseMode = Node.ProcessModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnUnhandledKeyInput(node, action, pauseMode);

    }
 
}