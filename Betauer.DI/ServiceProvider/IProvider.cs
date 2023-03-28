using System;

namespace Betauer.DI.ServiceProvider {
    public interface IProvider {
        public Container Container { get; set; }
        public string? Name { get; }
        public bool Primary { get; }
        public Type RegisterType { get; }
        public Type ProviderType { get; }
        public Lifetime Lifetime { get; }
        public object Get();
        public object Resolve(ResolveContext context);
    }
}