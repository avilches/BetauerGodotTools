using Betauer.DI.Factory;

namespace Betauer.DI.ServiceProvider;

public abstract class Proxy {
    internal abstract Provider Provider { get; }

    public class Lazy<T> : Proxy, ILazy<T> where T : class {
        internal override SingletonProvider Provider { get; }

        public Lazy(SingletonProvider provider) {
            Provider = provider;
        }

        object ILazy.Get() => Get();

        public T Get() => (T)Provider.Get();
        
        public bool HasValue() => Provider.IsInstanceCreated;
    }

    public class Transient<T> : Proxy, ITransient<T> where T : class {
        internal override TransientProvider Provider { get; }

        public Transient(TransientProvider provider) {
            Provider = provider;
        }

        object ITransient.Create() => Create();

        public T Create() => (T)Provider.Get();
    }
}