namespace Betauer.Bus {
    public class GodotTopic<T> : Topic<GodotListener<T>, T>
        where T : IGodotEvent {

        private readonly Logger _logger;

        public GodotTopic(string name) : base(name) {
            _logger = LoggerFactory.GetLogger(typeof(GodotTopic<T>).GetNameWithoutGenerics(), name);
        }

        public override void Subscribe(GodotListener<T>? eventListener) {
            if (eventListener == null) return;
            eventListener.OnSubscribed(this);
            base.Subscribe(eventListener);
        }

        public int DisposeListeners() {
            return EventListeners.RemoveAll(listener => listener.IsDisposed());
        }

        public override void Publish(T @event) {
            int deleted = DisposeListeners();
            if (deleted > 0) {
                _logger.Debug($"Event published to {EventListeners.Count} listeners ({deleted} of {deleted+EventListeners.Count} have been disposed)");
            } else {
                _logger.Debug($"Event published to {EventListeners.Count} listeners");
            }

            EventListeners.ForEach(listener => listener.OnEvent(@event));
        }
    }

}