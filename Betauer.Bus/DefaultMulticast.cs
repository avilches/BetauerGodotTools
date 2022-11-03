namespace Betauer.Bus {
    public static class DefaultMulticast {
        public static Multicast<TArgs> Get<TArgs>() =>
            DefaultMulticast<TArgs>.Instance;

        public static Multicast<TPublisher, TArgs> Get<TPublisher, TArgs>() =>
            DefaultMulticast<TPublisher, TArgs>.Instance;
    }

    public static class DefaultMulticast<TPublisher, TArgs> {
        public static Multicast<TPublisher, TArgs> Instance = new();
    }

    public static class DefaultMulticast<TArgs> {
        public static Multicast<TArgs> Instance = new();
    }
}