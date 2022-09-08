using System;

namespace Betauer.DI.ServiceProvider {
    public class SingletonFactoryProvider : FactoryProvider, ISingletonProvider {
        public override Lifetime Lifetime => Lifetime.Singleton;
        public bool IsInstanceCreated { get; private set; }
        public bool Lazy { get; }
        public object? Instance { get; private set; }

        public SingletonFactoryProvider(Type registerType, Type providerType, Func<object> factory, string? name = null, bool primary = false, bool lazy = false) : base(registerType, providerType, factory, name, primary) {
            Lazy = lazy;
        }

        public override object Get(Container container) {
            if (IsInstanceCreated) return Instance!;
            var context = new ResolveContext(container);
            var instance = Get(context);
            context.End();
            return instance;
        }

        public override object Get(ResolveContext context) {
            if (IsInstanceCreated) return Instance!;
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.TryGetFromCache(RegisterType, Name, out var singleton)) {
                Logger.Debug("Get from context " + Lifetime + " " + singleton.GetType().Name + " exposed as " +
                             RegisterType.Name + ": " + singleton.GetHashCode().ToString("X"));
                return singleton;
            }
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (IsInstanceCreated) return Instance!;
                Instance = CreateNewInstance(Lifetime, context);
                IsInstanceCreated = true;
                context.Container.InjectServices(Instance, context);
            }
            return Instance;
        }
    }
}