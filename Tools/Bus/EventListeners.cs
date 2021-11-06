namespace Tools.Bus {
    public interface EventListener<T> {
        string Name { get; }
        void OnEvent(string topicName, T @event);
    }
}