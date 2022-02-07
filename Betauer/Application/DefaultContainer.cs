using Betauer.DI;

namespace Betauer.Application {
    public static class DefaultContainer {
        public static Container Container;

        public static void Set(Container container) {
            Container = container;
        }
    }
}