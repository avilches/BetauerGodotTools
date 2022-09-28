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

    public class NodeHandler : Node {
        public class Event<T> : INodeEvent {
            public readonly Node Node;
            public readonly T Delegate;
            public PauseModeEnum PauseMode;
            public bool IsEnabled(bool isTreePaused) => _isEnabled && 
                                                        (!isTreePaused || PauseMode == PauseModeEnum.Process) &&
                                                        Node.IsInsideTree();
            public bool IsDestroyed => _isDestroyed || !IsInstanceValid(Node);
            
            private bool _isEnabled = true;
            private bool _isDestroyed = false;
            
            public Event(Node node, T @delegate, PauseModeEnum pauseMode) {
                Node = node;
                Delegate = @delegate;
                PauseMode = pauseMode;
            }

            public void Disable() => _isEnabled = false;
            public void Enable() => _isEnabled = true;
            public void Destroy() => _isDestroyed = true;
        }

        public readonly List<Event<Action<float>>> OnProcessList = new();
        public readonly List<Event<Action<float>>> OnPhysicsProcessList = new();
        public readonly List<Event<Func<InputEvent, bool>>> OnInputList = new();
        public readonly List<Event<Func<InputEvent, bool>>> OnUnhandledInputList = new();
        
        private SceneTree _sceneTree;

        public override void _EnterTree() {
            _sceneTree = GetTree();
            PauseMode = PauseModeEnum.Process;
        }

        public INodeEvent OnProcess(Node node, Action<float> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new Event<Action<float>>(node, action, pauseMode);
            OnProcessList.Add(nodeEvent);
            SetProcess(true);
            return nodeEvent;
        }

        public INodeEvent OnPhysicsProcess(Node node, Action<float> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new Event<Action<float>>(node, action, pauseMode);
            OnPhysicsProcessList.Add(nodeEvent);
            SetPhysicsProcess(true);
            return nodeEvent;
        }

        public INodeEvent OnInput(Node node, Action<InputEvent> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            return OnInput(node, input => {
                action.Invoke(input);
                return false;
            }, pauseMode);
        }

        public INodeEvent OnInput(Node node, Func<InputEvent, bool> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new Event<Func<InputEvent, bool>>(node, action, pauseMode);
            OnInputList.Add(nodeEvent);
            SetProcessInput(true);
            return nodeEvent;
        }

        public INodeEvent OnUnhandledInput(Node node, Action<InputEvent> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            return OnUnhandledInput(node, input => {
                action.Invoke(input);
                return false;
            }, pauseMode);
        }

        public INodeEvent OnUnhandledInput(Node node, Func<InputEvent, bool> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new Event<Func<InputEvent, bool>>(node, action, pauseMode);
            OnUnhandledInputList.Add(nodeEvent);
            SetProcessUnhandledInput(true);
            return nodeEvent;
        }
        
        public override void _Process(float delta) {
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

        public override void _PhysicsProcess(float delta) {
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
                    isInputHandled = isInputHandled || _sceneTree.IsInputHandled();
                    if (!isInputHandled) {
                        if (nodeOnProcess.Delegate.Invoke(e)) _sceneTree.SetInputAsHandled();
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
                    isInputHandled = isInputHandled || _sceneTree.IsInputHandled();
                    if (!isInputHandled) {
                        if (nodeOnProcess.Delegate.Invoke(e)) _sceneTree.SetInputAsHandled();
                    }
                }
                return false;
            });
        }

        public string GetStateAsString() {
            string NodeName(Node node) => IsInstanceValid(node) ? node.Name : "disposed";
            return 
$@"Process: {string.Join(", ", OnProcessList.Select(e => NodeName(e.Node)))}
PhysicsProcess: {string.Join(", ", OnPhysicsProcessList.Select(e => NodeName(e.Node)))}
Input: {string.Join(", ", OnInputList.Select(e => NodeName(e.Node)))}
UnhandledInput: {string.Join(", ", OnUnhandledInputList.Select(e => NodeName(e.Node)))}";
        }
        
    }
    
    public static class NodeHandlerExtensions {
        public static INodeEvent OnProcess(this Node node, Action<float> action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnProcess(node, action, pauseMode);

        public static INodeEvent OnPhysicsProcess(this Node node, Action<float> action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnPhysicsProcess(node, action, pauseMode);

        public static INodeEvent OnInput(this Node node, Func<InputEvent, bool> action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnInput(node, action, pauseMode);

        public static INodeEvent OnInput(this Node node, Action<InputEvent> action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnInput(node, action, pauseMode);

        public static INodeEvent OnUnhandledInput(this Node node, Func<InputEvent, bool> action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnUnhandledInput(node, action, pauseMode);

        public static INodeEvent OnUnhandledInput(this Node node, Action<InputEvent> action, Node.PauseModeEnum pauseMode = Node.PauseModeEnum.Inherit) =>
            DefaultNodeHandler.Instance.OnUnhandledInput(node, action, pauseMode);

    }
 
}