using System;
using Godot;
using Object = Godot.Object;

namespace Betauer.Bus {
    public interface IGodotEvent {
        public Node Origin { get; }
    }

    public abstract class GodotListener<T> : EventListener<T> where T : IGodotEvent {
        public Node Owner { get; }
        public string TopicName { get; private set; }
        public string Name { get; }
        protected Logger _logger;

        protected GodotListener(string name, Node owner) {
            Name = name;
            Owner = owner;
        }

        public void OnSubscribed(GodotTopic<T> godotTopic) {
            TopicName = godotTopic.Name;
            _logger = LoggerFactory.GetLogger(typeof(GodotListener<T>),
                TopicName);
        }

        public void Debug(string message) {
            _logger.Debug($"Received: \"{Name}\" | {message}");
        }

        public virtual bool IsDisposed() {
            if (Object.IsInstanceValid(Owner)) return false;
            Debug($"Disposed. Owner: {GetNodeInfo(Owner)}");
            return true;
        }

        protected static string GetNodeInfo(Node? node) {
            if (node == null) return "null";
            var nodeName = Object.IsInstanceValid(node) ? $"\"{node.Name}\"" : "(disposed)";
            return $"{node.GetType().Name} 0x{node.NativeInstance.ToString("x")} {nodeName}";
        }

        public abstract void OnEvent(T @event);
    }

    public interface IGodotFilterEvent : IGodotEvent {
        public Node Filter { get; }
    }

    public abstract class GodotFilterListener<T> : GodotListener<T> where T : IGodotFilterEvent {
        public Node? Filter { get; }

        protected GodotFilterListener(string name, Node owner, Node filter) : base(name, owner) {
            Filter = filter;
        }

        public override bool IsDisposed() {
            if (base.IsDisposed()) return true;
            // null filter is allowed, don't dispose it
            if (Filter == null || Object.IsInstanceValid(Filter)) return false;
            Debug($"Disposed. Filter: {GetNodeInfo(Filter)}");
            return true;
        }

        public override void OnEvent(T @event) {
            if (Filter == null) {
                Debug($"Origin: {GetNodeInfo(@event.Origin)} | -> Ok");
            } else {
                var matches = @event.Filter == Filter;
                if (!matches) {
                    Debug(
                        $"Origin: {GetNodeInfo(@event.Origin)} | Filter: {GetNodeInfo(Filter)} | Rejected {GetNodeInfo(@event.Filter)}");
                    return;
                }
                Debug($"Origin: {GetNodeInfo(@event.Origin)} | Filter: {GetNodeInfo(Filter)} | -> Ok");
            }
            Execute(@event);
        }

        public abstract void Execute(T @event);
    }

    public class GodotFilterListenerAction<T> : GodotFilterListener<T> where T : IGodotFilterEvent {
        private readonly Action<T>? _actionWithEvent;
        private readonly Action? _action;

        public GodotFilterListenerAction(string name, Node owner, Node filter, Action<T> actionWithEvent) :
            base(name, owner, filter) {
            _actionWithEvent = actionWithEvent;
        }

        public GodotFilterListenerAction(string name, Node owner, Node filter, Action action) :
            base(name, owner, filter) {
            _action = action;
        }

        public override void Execute(T @event) {
            if (_action != null) _action();
            else _actionWithEvent?.Invoke(@event);
        }
    }
}