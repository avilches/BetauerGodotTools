using Godot;

namespace Tools.Bus {
    public interface IGodotNodeEvent {
        Node GetFilter();
    }

    public abstract class GodotListener<T> : EventListener<T> where T : IGodotNodeEvent {
        public Node Owner { get; set; }
        public Node Filter { get; }
        public string TopicName { get; set; }
        public string Name { get; }

        protected GodotListener(string name, Node owner, Node filter) {
            Name = name;
            Owner = owner;
            Filter = filter;
        }

        public bool IsDisposed() {
            if (GodotTools.IsDisposed(Owner)) {
                Debug.Event(TopicName, Name, $"Disposed. Owner: {GetNodeInfo(Owner)}");
                return true;
            }
            if (GodotTools.IsDisposed(Filter)) {
                Debug.Event(TopicName, Name, $"Disposed. Filter: {GetNodeInfo(Filter)}");
                return true;
            }
            return false;
        }

        private static string GetNodeInfo(Node node) {
            if (node == null) return "all";
            var nodeName = GodotTools.IsDisposed(node) ? "(no name: disposed)" : $"\"{node.Name}\"";
            return $"{node.GetType().Name} 0x{node.NativeInstance.ToString("x")} {nodeName}";
        }

        public void OnEvent(T @event) {
            if (Filter != null) {
                var filterNode = @event.GetFilter();
                var matches = filterNode == Filter;
                if (!matches) {
                    Debug.Event(TopicName, Name,
                        $"Filter: {GetNodeInfo(Filter)} | Rejected {GetNodeInfo(filterNode)}");
                    return;
                }
                Debug.Event(TopicName, Name,
                    $"Filter: {GetNodeInfo(Filter)} | -> Ok");
            } else {
                Debug.Event(TopicName, Name, "Ok");
            }
            Execute(@event);
        }

        public abstract void Execute(T @event);
    }

    public class GodotListenerDelegate<T> : GodotListener<T> where T : IGodotNodeEvent {
        public delegate void ExecuteMethod(T @event);

        private readonly ExecuteMethod _executeMethod;

        public GodotListenerDelegate(string name, Node owner, Node filter, ExecuteMethod executeMethod) :
            base(name, owner, filter) {
            _executeMethod = executeMethod;
        }

        public override void Execute(T @event) {
            _executeMethod.Invoke(@event);
        }
    }
}