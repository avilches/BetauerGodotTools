using System;
using System.Collections.Generic;
using Godot;

namespace Betauer.Nodes {
    public class DefaultNodeHandler {
        public static NodeHandler Instance = new NodeHandler();
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
            internal bool IsEnabled  => _isEnabled && Node.IsInsideTree();
            internal bool IsDestroyed => _isDestroyed || IsInstanceValid(Node);
            private bool _isEnabled = true;
            private bool _isDestroyed = false;
            public NodeEvent(Node node, T @delegate) {
                Node = node;
                Delegate = @delegate;
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
        }

        public INodeEvent OnProcess(Node node, Action<float> action) {
            var nodeEvent = new NodeEvent<Action<float>>(node, action);
            _onProcesses.Add(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnPhysicsProcess(Node node, Action<float> action) {
            var nodeEvent = new NodeEvent<Action<float>>(node, action);
            _onPhysicsProcesses.Add(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnInput(Node node, Action<InputEvent> action) {
            return OnInput(node, input => {
                action.Invoke(input);
                return false;
            });
        }

        public INodeEvent OnInput(Node node, Func<InputEvent, bool> action) {
            var nodeEvent = new NodeEvent<Func<InputEvent, bool>>(node, action);
            _onInput.Add(nodeEvent);
            return nodeEvent;
        }

        public INodeEvent OnUnhandledInput(Node node, Action<InputEvent> action) {
            return OnUnhandledInput(node, input => {
                action.Invoke(input);
                return false;
            });
        }

        public INodeEvent OnUnhandledInput(Node node, Func<InputEvent, bool> action) {
            var nodeEvent = new NodeEvent<Func<InputEvent, bool>>(node, action);
            _onUnhandledInput.Add(nodeEvent);
            return nodeEvent;
        }
        
        public override void _Process(float delta) {
            _onProcesses.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled) nodeOnProcess.Delegate.Invoke(delta);
                return false;
            });
        }

        public override void _PhysicsProcess(float delta) {
            _onPhysicsProcesses.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled) nodeOnProcess.Delegate.Invoke(delta);
                return false;
            });
        }

        public override void _Input(InputEvent e) {
            var isInputHandled = false;
            _onInput.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled) {
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
            _onUnhandledInput.RemoveAll(nodeOnProcess => {
                if (nodeOnProcess.IsDestroyed) return true;
                if (nodeOnProcess.IsEnabled) {
                    isInputHandled = isInputHandled || _sceneTree.IsInputHandled();
                    if (!isInputHandled) {
                        if (nodeOnProcess.Delegate.Invoke(e)) _sceneTree.SetInputAsHandled();
                    }
                }
                return false;
            });
        }
    }
    
    public static class NodeActionExtension {
        public static INodeEvent OnProcess(this Node node, Action<float> action) =>
            DefaultNodeHandler.Instance.OnProcess(node, action);

        public static INodeEvent OnPhysicsProcess(this Node node, Action<float> action) =>
            DefaultNodeHandler.Instance.OnPhysicsProcess(node, action);

        public static INodeEvent OnInput(this Node node, Func<InputEvent, bool> action) =>
            DefaultNodeHandler.Instance.OnInput(node, action);

        public static INodeEvent OnInput(this Node node, Action<InputEvent> action) =>
            DefaultNodeHandler.Instance.OnInput(node, action);

        public static INodeEvent OnUnhandledInput(this Node node, Func<InputEvent, bool> action) =>
            DefaultNodeHandler.Instance.OnUnhandledInput(node, action);

        public static INodeEvent OnUnhandledInput(this Node node, Action<InputEvent> action) =>
            DefaultNodeHandler.Instance.OnUnhandledInput(node, action);

    }
 
}