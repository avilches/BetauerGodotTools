using System;

namespace Betauer.DI.ServiceProvider {
    public abstract class FactoryProvider: BaseProvider {
        protected readonly Logger Logger = LoggerFactory.GetLogger(typeof(FactoryProvider));
        private readonly Func<object> _factory;

        protected FactoryProvider(Type registerType, Type providerType, Func<object> factory, string? name, bool primary) : base(registerType, providerType, name, primary) {
            _factory = factory;
        }

        protected object CreateNewInstance(Lifetime lifetime, ResolveContext context) {
            var instance = _factory();
            if (Logger.IsEnabled(TraceLevel.Debug)) {
                Logger.Debug("Creating " + lifetime + " " + instance.GetType().Name + " exposed as " +
                             RegisterType.Name + ": " + instance.GetHashCode().ToString("X"));
            }
            context.AddInstanceToCache(RegisterType, lifetime, instance, Name);
            return instance;
        }
    }
}