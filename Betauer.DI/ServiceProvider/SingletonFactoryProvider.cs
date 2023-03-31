using System;
using Betauer.Tools.Logging;
using Betauer.Tools.Reflection;

namespace Betauer.DI.ServiceProvider {
    public class SingletonFactoryProvider : Provider, ISingletonProvider {
        private static readonly Logger Logger = LoggerFactory.GetLogger(typeof(Provider));
        private readonly Func<object> _factory;
        public override Lifetime Lifetime => Lifetime.Singleton;
        public bool IsInstanceCreated { get; private set; }
        public bool Lazy { get; }
        public object? Instance { get; private set; }

        public SingletonFactoryProvider(Type registerType, Type providerType, Func<object>? factory = null, string? name = null, bool primary = false, bool lazy = false) : base(registerType, providerType, name, primary) {
            _factory = factory ?? CreateDefaultFactory(providerType, Lifetime);
            Lazy = lazy;
        }

        public override object Resolve(ResolveContext context) {
            if (IsInstanceCreated) return Instance!;
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.TryGetSingletonFromCache(RegisterType, Name, out var singleton)) {
                Logger.Debug($"Get from context {Lifetime} {singleton.GetType().GetTypeName()} exposed as {RegisterType.GetTypeName()}: {singleton.GetHashCode():X}");
                return singleton;
            }
            lock (this) {
                // Just in case another thread was waiting for the lock
                if (IsInstanceCreated) return Instance!;
                Instance = _factory.Invoke();
                if (Instance == null) throw new NullReferenceException($"Singleton factory returned null for {RegisterType.GetTypeName()} {Name}");
                Logger.Debug($"Creating {Lifetime.Singleton}:{Instance.GetType().GetTypeName()}. Name: {Name}. HashCode: {Instance.GetHashCode():X}");
                context.AddSingleton(RegisterType, Name, Instance);
                IsInstanceCreated = true;
                context.Container.InjectServices(Lifetime, Instance, context);
            }
            return Instance;
        }
    }
}