using Godot;

namespace Tools.Events {
    public class GodotNodeMulticastTopic<T> : MulticastTopic<GodotNodeListenerDelegate<T>, T>
        where T : IGodotNodeEvent {
        public override void Publish(T @event) {
            Publish(@event, "");
        }

        public void Publish(T @event, string name) {
            int deleted = _eventListeners.RemoveAll(listener => listener.IsDisposed());
            if (deleted > 0) {
                Debug.Event("GodotMulticast",
                    $"Event \"{name}\" published to {_eventListeners.Count} listeners ({deleted} have been disposed)");
            } else {
                Debug.Event("GodotMulticast", $"Event \"{name}\" published to {_eventListeners.Count} listeners");
            }

            base.Publish(@event);
        }
    }

    public class GodotNodeUnicastTopic<T> : UnicastTopic<GodotNodeListenerDelegate<T>, T>
        where T : IGodotNodeEvent {
        public override void Publish(T @event) {
            Publish(@event, "");
        }

        public void Publish(T @event, string name) {
            if (Listener == null) {
                Debug.Event("GodotUnicast",
                    $"Event \"{name}\" ignored: no listener");
            } else if (Listener.IsDisposed()) {
                Debug.Event("GodotUnicast",
                    $"Event \"{name}\" ignored: listener body is disposed (it will be deleted)");
                Listener = null;
            } else {
                Debug.Event("GodotUnicast", $"Event \"{name}\" published");
                base.Publish(@event);
            }
        }
    }
}