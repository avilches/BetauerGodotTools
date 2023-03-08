using System;

namespace Betauer.DI.ServiceProvider {
    public abstract class Provider : IProvider {

        public static IProvider Create(Type registeredType, Type type, Func<object> factory, Lifetime lifetime = Lifetime.Singleton, string? name = null, bool primary = false, bool lazy = false) {
            return lifetime == Lifetime.Singleton
                ? new SingletonFactoryProvider(registeredType, type, factory, name, primary, lazy)
                : new TransientFactoryProvider(registeredType, type, factory, name, primary);
        }

        public Container Container { get; set; }
        public Type RegisterType { get; }
        public Type ProviderType { get; }
        public string? Name { get; }
        public bool Primary { get; } // If name is not null, only the (last) Primary provider is used as fallback
        public abstract Lifetime Lifetime { get; }

        protected Provider(Type registerType, Type providerType, string? name, bool primary) {
            if (!registerType.IsAssignableFrom(providerType)) {
                throw new InvalidCastException(
                    $"Can't create a provider of {providerType.Name} and register with {registerType}");
            } 
            RegisterType = registerType;
            ProviderType = providerType;
            Name = name;
            Primary = name != null && primary; // primary is needed only when the provider has name
        }

        public virtual object Get() {
            var context = Container.NewResolveContext();
            var instance = Get(context);
            context.End();
            return instance;
        }

        public abstract object Get(ResolveContext context);
    }
}