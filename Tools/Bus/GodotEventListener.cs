using Godot;

namespace Tools.Bus {
    public interface IGodotNodeEvent {
        Node GetFilter();
    }

    public abstract class GodotListener<T> : EventListener<T> where T : IGodotNodeEvent {
        public Node Filter { get; }
        public string Name { get; }

        protected GodotListener(string name, Node filter) {
            Name = name;
            Filter = filter;
        }

        public bool IsDisposed() => Filter != null && GodotTools.IsDisposed(Filter);

        private static string GetNodeInfo(Node node) {
            if (node == null) return "all";
            var nodeName = GodotTools.IsDisposed(node) ? "(no name: disposed)" : $"\"{node.Name}\"";
            return $"{node.GetType().Name} 0x{node.NativeInstance.ToString("x")} {nodeName}";
        }

        public void OnEvent(string topicName, T @event) {
            var nodeFrom = @event.GetFilter();
            var matches = Filter == null || nodeFrom == Filter;

            if (matches) {
                Debug.Event(topicName, "GodotNodeListener." + Name,
                    $"Listening {GetNodeInfo(Filter)} events. Received ok");
                Execute(@event);
            } else {
                Debug.Event(topicName, "GodotNodeListener." + Name,
                    $"Listening {GetNodeInfo(Filter)} events. Rejected {GetNodeInfo(nodeFrom)}");
            }
        }

        public abstract void Execute(T @event);
    }

    public class GodotListenerDelegate<T> : GodotListener<T> where T : IGodotNodeEvent {
        public delegate void ExecuteMethod(T @event);

        private readonly ExecuteMethod _executeMethod;

        public GodotListenerDelegate(string name, Node filter, ExecuteMethod executeMethod) : base(name, filter) {
            _executeMethod = executeMethod;
        }

        public override void Execute(T @event) {
            _executeMethod.Invoke(@event);
        }
    }
}