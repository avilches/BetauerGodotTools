using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer.Nodes {
    public class DefaultNodeHandler {
        public static NodeHandler Instance = new();
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

        public class Event<T> : INodeEvent {
            public readonly Node? Node;
            public readonly T Delegate;
            public ProcessModeEnum ProcessMode;
            public bool IsEnabled(bool isTreePaused) {
                return _isEnabled &&
                       ShouldProcess(isTreePaused, ProcessMode) &&
                       (Node == null || Node.IsInsideTree());
            }

            // Node can be null, so the Event will last forever
            public bool IsDestroyed => _isDestroyed || (Node != null && !IsInstanceValid(Node));
            
            private bool _isEnabled = true;
            private bool _isDestroyed = false;
            
            public Event(Node? node, T @delegate, ProcessModeEnum pauseMode) {
                Node = node;
                Delegate = @delegate;
                ProcessMode = pauseMode;
            }

            public void Disable() => _isEnabled = false;
            public void Enable() => _isEnabled = true;
            public void Destroy() => _isDestroyed = true;
        }

        public readonly List<Event<Action<double>>> OnProcessList = new();
        public readonly List<Event<Action<double>>> OnPhysicsProcessList = new();
        public readonly List<Event<Action<InputEvent>>> OnInputList = new();
        public readonly List<Event<Action<InputEvent>>> OnUnhandledInputList = new();
        
        private SceneTree _sceneTree;

        public override void _EnterTree() {
            _sceneTree = GetTree();
            ProcessMode = ProcessModeEnum.Always;
        }

        public INodeEvent OnProcess(Node node, Action<double> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new Event<Action<double>>(node, action, pauseMode);
            OnProcessList.Add(nodeEvent);
            SetProcess(true);
            return nodeEvent;
        }

        public INodeEvent OnPhysicsProcess(Node node, Action<double> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new Event<Action<double>>(node, action, pauseMode);
            OnPhysicsProcessList.Add(nodeEvent);
            SetPhysicsProcess(true);
            return nodeEvent;
        }

        public INodeEvent OnInput(Node node, Action<InputEvent> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new Event<Action<InputEvent>>(node, action, pauseMode);
            OnInputList.Add(nodeEvent);
            SetProcessInput(true);
            return nodeEvent;
        }

        public INodeEvent OnUnhandledInput(Node node, Action<InputEvent> action, ProcessModeEnum pauseMode = ProcessModeEnum.Inherit) {
            var nodeEvent = new Event<Action<InputEvent>>(node, action, pauseMode);
            OnUnhandledInputList.Add(nodeEvent);
            SetProcessUnhandledInput(true);
            return nodeEvent;
        }
        
        public override void _Process(double delta) {
            if (OnProcessList.Count == 0) {
                SetProcess(false);
                return;
            }
            var isTreePaused = _sceneTree.Paused;
            OnProcessList.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled(isTreePaused)) nodeOnProcess.Delegate.Invoke(delta);
                return false;
            });
        }

        public override void _PhysicsProcess(double delta) {
            if (OnPhysicsProcessList.Count == 0) {
                SetPhysicsProcess(false);
                return;
            }
            var isTreePaused = _sceneTree.Paused;
            OnPhysicsProcessList.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled(isTreePaused)) nodeOnProcess.Delegate.Invoke(delta);
                return false;
            });
        }

        public override void _Input(InputEvent e) {
            if (OnInputList.Count == 0) {
                SetProcessInput(false);
                return;
            }
            var isInputHandled = false;
            var isTreePaused = _sceneTree.Paused;
            OnInputList.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled(isTreePaused)) {
                    isInputHandled = isInputHandled || _sceneTree.Root.IsInputHandled();
                    if (!isInputHandled) {
                        nodeOnProcess.Delegate.Invoke(e);
                    }
                }
                return false;
            });
        }

        public override void _UnhandledInput(InputEvent e) {
            if (OnUnhandledInputList.Count == 0) {
                SetProcessUnhandledInput(false);
                return;
            }
            var isInputHandled = false;
            var isTreePaused = _sceneTree.Paused;
            OnUnhandledInputList.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled(isTreePaused)) {
                    isInputHandled = isInputHandled || _sceneTree.Root.IsInputHandled();
                    if (!isInputHandled) {
                        nodeOnProcess.Delegate.Invoke(e);
                    }
                }
                return false;
            });
        }

        public string GetStateAsString() {
            string NodeName(Node? node) => node == null ? "forever" : IsInstanceValid(node) ? node.Name : "disposed";
            return 
$@"Process: {string.Join(", ", OnProcessList.Select(e => NodeName(e.Node)))}
PhysicsProcess: {string.Join(", ", OnPhysicsProcessList.Select(e => NodeName(e.Node)))}
Input: {string.Join(", ", OnInputList.Select(e => NodeName(e.Node)))}
UnhandledInput: {string.Join(", ", OnUnhandledInputList.Select(e => NodeName(e.Node)))}";
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

    }
 
}