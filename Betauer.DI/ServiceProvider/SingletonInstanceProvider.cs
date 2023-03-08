using System;

namespace Betauer.DI.ServiceProvider {
    public class SingletonInstanceProvider : Provider, ISingletonProvider {
        public override Lifetime Lifetime => Lifetime.Singleton;
        public bool IsInstanceCreated => true;
        public object Instance { get; }
        public bool Lazy => true;

        public SingletonInstanceProvider(Type registerType, Type providerType, object instance, string? name = null, bool primary = false) : base(registerType, providerType, name, primary) {
            Instance = instance ?? throw new ArgumentNullException(nameof(instance));
        }
        public override object Get(ResolveContext context) => Instance;
        public override object Get() => Instance;
    }
}