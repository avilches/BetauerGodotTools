using Godot;

namespace Tools.Bus {
    public interface IGodotEvent {
        public Node Origin { get; }
    }

    public abstract class GodotListener<T> : EventListener<T> where T : IGodotEvent {
        public Node Owner { get; }
        public string TopicName { get; set; }
        public string Name { get; }

        protected GodotListener(string name, Node owner) {
            Name = name;
            Owner = owner;
        }

        public virtual bool IsDisposed() {
            if (!GodotTools.IsDisposed(Owner)) return false;
            Debug.Event(TopicName, Name, $"Disposed. Owner: {GetNodeInfo(Owner)}");
            return true;

        }
        protected static string GetNodeInfo(Node node) {
            if (node == null) return "all";
            var nodeName = GodotTools.IsDisposed(node) ? "(disposed)" : $"\"{node.Name}\"";
            return $"{node.GetType().Name} 0x{node.NativeInstance.ToString("x")} {nodeName}";
        }

        public abstract void OnEvent(T @event);
    }

    public interface IGodotFilterEvent: IGodotEvent {
        public Node Filter { get; }
    }

    public abstract class GodotFilterListener<T> : GodotListener<T> where T : IGodotFilterEvent {
        public Node Filter { get; }

        protected GodotFilterListener(string name, Node owner, Node filter) : base(name, owner) {
            Filter = filter;
        }

        public override bool IsDisposed() {
            if (base.IsDisposed()) return true;
            if (!GodotTools.IsDisposed(Filter)) return false;
            Debug.Event(TopicName, Name, $"Disposed. Filter: {GetNodeInfo(Filter)}");
            return true;
        }

        public override void OnEvent(T @event) {
            if (Filter == null) {
                Debug.Event(TopicName, Name, $"Origin: {GetNodeInfo(@event.Origin)} | -> Ok");
            } else {
                var matches = @event.Filter == Filter;
                if (!matches) {
                    Debug.Event(TopicName, Name,
                        $"Origin: {GetNodeInfo(@event.Origin)} | Filter: {GetNodeInfo(Filter)} | Rejected {GetNodeInfo(@event.Filter)}");
                    return;
                }
                Debug.Event(TopicName, Name,
                    $"Origin: {GetNodeInfo(@event.Origin)} | Filter: {GetNodeInfo(Filter)} | -> Ok");
            }
            Execute(@event);
        }

        public abstract void Execute(T @event);
    }

    public class GodotFilterListenerDelegate<T> : GodotFilterListener<T> where T : IGodotFilterEvent {
        public delegate void ExecuteMethod(T @event);

        private readonly ExecuteMethod _executeMethod;

        public GodotFilterListenerDelegate(string name, Node owner, Node filter, ExecuteMethod executeMethod) :
            base(name, owner, filter) {
            _executeMethod = executeMethod;
        }

        public override void Execute(T @event) {
            _executeMethod.Invoke(@event);
        }
    }
}