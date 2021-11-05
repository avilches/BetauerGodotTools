using Godot;

namespace Tools.Events {
    public class GodotNodeMulticastTopic<T> : MulticastTopic<GodotNodeListenerDelegate<T>, T>
        where T : IGodotNodeEvent {
        public GodotNodeMulticastTopic(string name) : base(name) {
        }

        public override void Publish(T @event) {
            int deleted = _eventListeners.RemoveAll(listener => listener.IsDisposed());
            if (deleted > 0) {
                Debug.Topic("GodotMulticast",Name,
                    $"Event published to {_eventListeners.Count} listeners ({deleted} have been disposed)");
            } else {
                Debug.Topic("GodotMulticast", Name, $"Event published to {_eventListeners.Count} listeners");
            }

            base.Publish(@event);
        }
    }

    public class GodotNodeUnicastTopic<T> : UnicastTopic<GodotNodeListenerDelegate<T>, T>
        where T : IGodotNodeEvent {
        public GodotNodeUnicastTopic(string name) : base(name) {
        }

        public override void Publish(T @event) {
            if (Listener == null) {
                Debug.Topic("GodotUnicast", Name, $"Event ignored: no listener");
            } else if (Listener.IsDisposed()) {
                Debug.Topic("GodotUnicast", Name, $"Event ignored: listener body is disposed (it will be deleted)");
                Listener = null;
            } else {
                Debug.Topic("GodotUnicast", Name, $"Event sent to {Listener.Name}");
                base.Publish(@event);
            }
        }
    }
}