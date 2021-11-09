namespace Tools.Bus {
    public class GodotTopic<T> : Topic<GodotListener<T>, T>
        where T : IGodotEvent {
        public GodotTopic(string name) : base(name) {
        }

        public override void Subscribe(GodotListener<T> eventListener) {
            if (eventListener != null) eventListener.TopicName = Name;
            base.Subscribe(eventListener);
        }

        public int DisposeListeners() {
            return EventListeners.RemoveAll(listener => listener.IsDisposed());
        }

        public override void Publish(T @event) {
            int deleted = DisposeListeners();
            if (deleted > 0) {
                Debug.Topic($"Topic:{Name}",
                    $"Event published to {EventListeners.Count} listeners ({deleted} have been disposed)");
            } else {
                Debug.Topic($"Topic:{Name}", $"Event published to {EventListeners.Count} listeners");
            }

            EventListeners.ForEach(listener => listener.OnEvent(@event));
        }
    }

}