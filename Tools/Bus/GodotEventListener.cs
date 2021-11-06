using Godot;

namespace Tools.Bus {
    public interface IGodotNodeEvent {
        Node GetFrom();
    }

    public abstract class GodotNodeListener<T> : EventListener<T> where T : IGodotNodeEvent {
        public Node Body { get; }
        public string Name { get; }

        protected GodotNodeListener(string name, Node body) {
            Name = name;
            Body = body;
        }

        public bool IsDisposed() => GodotTools.IsDisposed(Body);

        private static string GetNodeInfo(Node node) {
            var nodeName = GodotTools.IsDisposed(node) ? "(no name: disposed)" : $"\"{node.Name}\"";
            return $"{node.GetType().Name} 0x{node.NativeInstance.ToString("x")} {nodeName}";
        }

        public void OnEvent(string topicName, T @event) {
            var nodeFrom = @event.GetFrom();
            var matches = nodeFrom == Body;

            if (matches) {
                Debug.Event(topicName, "GodotNodeListener." + Name,
                    $"Listening {GetNodeInfo(Body)} events. Received ok");
                Execute(@event);
            } else {
                Debug.Event(topicName, "GodotNodeListener." + Name,
                    $"Listening {GetNodeInfo(Body)} events. Rejected {GetNodeInfo(nodeFrom)}");
            }
        }

        public abstract void Execute(T @event);
    }


    public class GodotNodeListenerDelegate<T> : GodotNodeListener<T> where T : IGodotNodeEvent {
        public delegate void ExecuteMethod(T @event);

        private readonly ExecuteMethod _executeMethod;

        public GodotNodeListenerDelegate(string name, Node body, ExecuteMethod executeMethod) : base(name, body) {
            _executeMethod = executeMethod;
        }

        public override void Execute(T @event) {
            _executeMethod.Invoke(@event);
        }
    }
}