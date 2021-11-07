namespace Tools.Bus {
    public class GodotMulticastTopic<T> : MulticastTopic<GodotListener<T>, T>
        where T : IGodotNodeEvent {
        public GodotMulticastTopic(string name) : base(name) {
        }

        public override void Subscribe(GodotListener<T> eventListener) {
            if (eventListener != null) eventListener.TopicName = Name;
            base.Subscribe(eventListener);
        }

        public override void Publish(T @event) {
            int deleted = EventListeners.RemoveAll(listener => listener.IsDisposed());
            if (deleted > 0) {
                Debug.Topic($"GodotMulticast.{Name}",
                    $"Event published to {EventListeners.Count} listeners ({deleted} have been disposed)");
            } else {
                Debug.Topic($"GodotMulticast.{Name}", $"Event published to {EventListeners.Count} listeners");
            }

            EventListeners.ForEach(listener => listener.OnEvent(@event));
        }
    }

    public class GodotUnicastTopic<T> : UnicastTopic<GodotListener<T>, T>
        where T : IGodotNodeEvent {
        public GodotUnicastTopic(string name) : base(name) {
        }

        public override void Subscribe(GodotListener<T> eventListener) {
            if (eventListener != null) eventListener.TopicName = Name;
            base.Subscribe(eventListener);
        }

        public override void Publish(T @event) {
            if (Listener == null) {
                Debug.Topic($"GodotUnicast.{Name}", $"Event ignored: no listener");
            } else if (Listener.IsDisposed()) {
                Debug.Topic($"GodotUnicast.{Name}", $"Event ignored: listener body is disposed (it will be deleted)");
                Listener = null;
            } else {
                Debug.Topic($"GodotUnicast.{Name}", $"Event sent to {Listener.Name}");
                Listener.OnEvent(@event);
            }
        }
    }


}