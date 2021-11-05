using System;
using Godot;

namespace Tools.Events {
    public interface IGodotNodeEvent {
        Node GetFrom();
    }

    public abstract class GodotNodeListener<T> : ConditionalEventListener<T> where T : IGodotNodeEvent {
        public Node Body { get; }

        public GodotNodeListener(Node body) {
            Body = body;
        }

        public bool IsDisposed() => Body.NativeInstance == IntPtr.Zero;

        private static string GetNodeInfo(Node node) {
            string nodeName = node.NativeInstance == IntPtr.Zero ? "(no name: disposed)" : $"\"{node.Name}\"";
            return $"{node.GetType().Name} {nodeName} [{node.NativeInstance}/0x{node.GetHashCode():X}]";
        }

        public override bool CanBeExecuted(T @event) {
            var nodeFrom = @event.GetFrom();
            var matches = nodeFrom == Body;

            if (matches) {
                Debug.Event("GodotNodeListener", $"Listening {GetNodeInfo(Body)} events. Received ok");
            } else {
                Debug.Event("GodotNodeListener",
                    $"Listening {GetNodeInfo(Body)} events. Rejected {GetNodeInfo(nodeFrom)}");
            }
            return matches;
        }
    }


    public class GodotNodeListenerDelegate<T> : GodotNodeListener<T> where T : IGodotNodeEvent {
        public delegate void ExecuteMethod(T @event);

        private readonly ExecuteMethod _executeMethod;

        public GodotNodeListenerDelegate(Node body, ExecuteMethod executeMethod) : base(body) {
            _executeMethod = executeMethod;
        }

        public override void Execute(T @event) {
            _executeMethod.Invoke(@event);
        }
    }
}