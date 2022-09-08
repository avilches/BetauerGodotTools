using System;

namespace Betauer.DI.ServiceProvider {
    public abstract class BaseProvider : IProvider {
        public bool Primary { get; }
        public Type RegisterType { get; }
        public Type ProviderType { get; }
        public string? Name { get; }
        public abstract Lifetime Lifetime { get; }

        protected BaseProvider(Type registerType, Type providerType, string? name, bool primary) {
            if (!registerType.IsAssignableFrom(providerType)) {
                throw new InvalidCastException(
                    $"Can't create a provider of {providerType.Name} and register with {registerType}");
            } 
            RegisterType = registerType;
            ProviderType = providerType;
            Name = name;
            Primary = primary;
        }

        public abstract object Get(ResolveContext context);
        public abstract object Get(Container context);
    }
}