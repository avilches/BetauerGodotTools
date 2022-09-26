using System;
using System.Collections.Generic;
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
        public class NodeEvent<T> : INodeEvent {
            internal readonly Node Node;
            internal readonly T Delegate;
            internal PauseModeEnum PauseMode;
            internal bool IsEnabled(bool isTreePaused) => _isEnabled && 
                                                          (!isTreePaused || PauseMode == PauseModeEnum.Process) &&
                                                          Node.IsInsideTree();
            internal bool IsDestroyed => _isDestroyed || !IsInstanceValid(Node);
            private bool _isEnabled = true;
            private bool _isDestroyed = false;
            
            public NodeEvent(Node node, T @delegate, PauseModeEnum pauseMode) {
                Node = node;
                Delegate = @delegate;
                PauseMode = pauseMode;
            }

            public void Disable() => _isEnabled = false;
            public void Enable() => _isEnabled = true;
            public void Destroy() => _isDestroyed = true;
        }

        private readonly List<NodeEvent<Action<float>>> _onProcesses = new List<NodeEvent<Action<float>>>();
        private readonly List<NodeEvent<Action<float>>> _onPhysicsProcesses = new List<NodeEvent<Action<float>>>();
        private readonly List<NodeEvent<Func<InputEvent, bool>>> _onInput = new List<NodeEvent<Func<InputEvent, bool>>>();
        private readonly List<NodeEvent<Func<InputEvent, bool>>> _onUnhandledInput = new List<NodeEvent<Func<InputEvent, bool>>>();
        private SceneTree _sceneTree;

        public override void _EnterTree() {
            _sceneTree = GetTree();
            PauseMode = PauseModeEnum.Process;
        }

        public INodeEvent OnProcess(Node node, Action<float> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new NodeEvent<Action<float>>(node, action, pauseMode);
            _onProcesses.Add(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnPhysicsProcess(Node node, Action<float> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new NodeEvent<Action<float>>(node, action, pauseMode);
            _onPhysicsProcesses.Add(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnInput(Node node, Action<InputEvent> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            return OnInput(node, input => {
                action.Invoke(input);
                return false;
            }, pauseMode);
        }

        public INodeEvent OnInput(Node node, Func<InputEvent, bool> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new NodeEvent<Func<InputEvent, bool>>(node, action, pauseMode);
            _onInput.Add(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnUnhandledInput(Node node, Action<InputEvent> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            return OnUnhandledInput(node, input => {
                action.Invoke(input);
                return false;
            }, pauseMode);
        }

        public INodeEvent OnUnhandledInput(Node node, Func<InputEvent, bool> action, PauseModeEnum pauseMode = PauseModeEnum.Inherit) {
            var nodeEvent = new NodeEvent<Func<InputEvent, bool>>(node, action, pauseMode);
            _onUnhandledInput.Add(nodeEvent);
            return nodeEvent;
        }
        
        public override void _Process(float delta) {
            var isTreePaused = _sceneTree.Paused;
            _onProcesses.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled(isTreePaused)) nodeOnProcess.Delegate.Invoke(delta);
                return false;
            });
        }

        public override void _PhysicsProcess(float delta) {
            var isTreePaused = _sceneTree.Paused;
            _onPhysicsProcesses.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled(isTreePaused)) nodeOnProcess.Delegate.Invoke(delta);
                return false;
            });
        }

        public override void _Input(InputEvent e) {
            var isInputHandled = false;
            var isTreePaused = _sceneTree.Paused;
            _onInput.RemoveAll(nodeOnProcess => {
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
            var isInputHandled = false;
            var isTreePaused = _sceneTree.Paused;
            _onUnhandledInput.RemoveAll(nodeOnProcess => {
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