using System;
using System.Collections.Generic;
using Godot;

namespace Betauer {
    public class DefaultNodeHandler {
        public static NodeHandler Instance = new NodeHandler();
    }

    public class NodeHandler : Node {
        private class NodeEvent<T> {
            internal readonly Node Node;
            internal readonly Action<T> Action;

            public NodeEvent(Node node, Action<T> action) {
                Node = node;
                Action = action;
            }

            public override bool Equals(object? obj) => 
                obj is NodeEvent<T> other && Equals(other);
            public bool Equals(NodeEvent<T>? obj) => 
                ReferenceEquals(this, obj) || 
                (obj is { } && Node.Equals(obj.Node) && Action.Equals(obj.Action));

            public override int GetHashCode() {
                unchecked {
                    return (Node.GetHashCode() * 397) ^ Action.GetHashCode();
                }
            }
        }

        private readonly List<NodeEvent<float>> _onProcesses = new List<NodeEvent<float>>();
        private readonly List<NodeEvent<float>> _onPhysicsProcesses = new List<NodeEvent<float>>();
        private readonly List<NodeEvent<InputEvent>> _onInput = new List<NodeEvent<InputEvent>>();
        private readonly List<NodeEvent<InputEvent>> _onUnhandledInput = new List<NodeEvent<InputEvent>>();

        public void OnProcess(Node node, Action<float> action) {
            _onProcesses.Add(new NodeEvent<float>(node, action));
        }

        public void OnPhysicsProcess(Node node, Action<float> action) {
            _onPhysicsProcesses.Add(new NodeEvent<float>(node, action));
        }

        public void OnInput(Node node, Action<InputEvent> action) {
            _onInput.Add(new NodeEvent<InputEvent>(node, action));
        }

        public void OnUnhandledInput(Node node, Action<InputEvent> action) {
            _onUnhandledInput.Add(new NodeEvent<InputEvent>(node, action));
        }

        public void RemoveOnProcess(Node node, Action<float> action) {
            _onProcesses.Remove(new NodeEvent<float>(node, action));
        }

        public void RemoveOnPhysicsProcess(Node node, Action<float> action) {
            _onPhysicsProcesses.Remove(new NodeEvent<float>(node, action));
        }

        public void RemoveOnInput(Node node, Action<InputEvent> action) {
            _onInput.Remove(new NodeEvent<InputEvent>(node, action));
        }

        public void RemoveOnUnhandledInput(Node node, Action<InputEvent> action) {
            _onUnhandledInput.Remove(new NodeEvent<InputEvent>(node, action));
        }

        public override void _Process(float delta) {
            _onProcesses.RemoveAll(nodeOnProcess => {
                if (!IsInstanceValid(nodeOnProcess.Node)) return true;
                if (nodeOnProcess.Node.IsInsideTree()) nodeOnProcess.Action.Invoke(delta);
                return false;
            });
        }

        public override void _PhysicsProcess(float delta) {
            _onPhysicsProcesses.RemoveAll(nodeOnProcess => {
                if (!IsInstanceValid(nodeOnProcess.Node)) return true;
                if (nodeOnProcess.Node.IsInsideTree()) nodeOnProcess.Action.Invoke(delta);
                return false;
            });
        }

        public override void _Input(InputEvent @event) {
            _onInput.RemoveAll(nodeOnProcess => {
                if (!IsInstanceValid(nodeOnProcess.Node)) return true;
                if (nodeOnProcess.Node.IsInsideTree()) nodeOnProcess.Action.Invoke(@event);
                return false;
            });
        }

        public override void _UnhandledInput(InputEvent @event) {
            _onUnhandledInput.RemoveAll(nodeOnProcess => {
                if (!IsInstanceValid(nodeOnProcess.Node)) return true;
                if (nodeOnProcess.Node.IsInsideTree()) nodeOnProcess.Action.Invoke(@event);
                return false;
            });
        }
    }
    
    public static class NodeActionExtension {
        public static void OnProcess(this Node node, Action<float> action) =>
            DefaultNodeHandler.Instance.OnProcess(node, action);

        public static void OnPhysicsProcess(this Node node, Action<float> action) =>
            DefaultNodeHandler.Instance.OnPhysicsProcess(node, action);

        public static void OnInput(this Node node, Action<InputEvent> action) =>
            DefaultNodeHandler.Instance.OnInput(node, action);

        public static void OnUnhandledInput(this Node node, Action<InputEvent> action) =>
            DefaultNodeHandler.Instance.OnUnhandledInput(node, action);

        public static void RemoveOnProcess(this Node node, Action<float> action) =>
            DefaultNodeHandler.Instance.RemoveOnProcess(node, action);

        public static void RemoveOnPhysicsProcess(this Node node, Action<float> action) =>
            DefaultNodeHandler.Instance.RemoveOnPhysicsProcess(node, action);

        public static void RemoveOnInput(this Node node, Action<InputEvent> action) =>
            DefaultNodeHandler.Instance.RemoveOnInput(node, action);

        public static void RemoveOnUnhandledInput(this Node node, Action<InputEvent> action) =>
            DefaultNodeHandler.Instance.RemoveOnUnhandledInput(node, action);
    }
 
}