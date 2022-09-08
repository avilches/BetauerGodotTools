using System;

namespace Betauer.DI.ServiceProvider {
    public interface IProvider {
        public string? Name { get; }
        public bool Primary { get; }
        public Type RegisterType { get; }
        public Type ProviderType { get; }
        public Lifetime Lifetime { get; }
        public object Get(ResolveContext context);
        public object Get(Container container);
    }
}