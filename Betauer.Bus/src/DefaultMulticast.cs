namespace Betauer.Bus {
    public static class DefaultMulticast {
        public static Multicast<TEvent> Get<TEvent>() =>
            DefaultMulticast<TEvent>.Instance;

        public static Multicast<TPublisher, TEvent> Get<TPublisher, TEvent>() =>
            DefaultMulticast<TPublisher, TEvent>.Instance;
    }

    public static class DefaultMulticast<TPublisher, TEvent> {
        public static Multicast<TPublisher, TEvent> Instance = new();
    }

    public static class DefaultMulticast<TEvent> {
        public static Multicast<TEvent> Instance = new();
    }
}