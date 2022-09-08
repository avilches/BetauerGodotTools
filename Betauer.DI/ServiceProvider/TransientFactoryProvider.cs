using System;

namespace Betauer.DI.ServiceProvider {
    public class TransientFactoryProvider : FactoryProvider {
        public override Lifetime Lifetime => Lifetime.Transient;

        public TransientFactoryProvider(Type registerType, Type providerType, Func<object> factory, string? name = null, bool primary = false) : base(registerType, providerType, factory, name, primary) {
        }

        public override object Get(Container container) {
            var context = new ResolveContext(container);
            var instance = Get(context);
            context.End();
            return instance;
        }

        public override object Get(ResolveContext context) {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.TryGetFromCache(RegisterType, Name, out var transient)) {
                Logger.Debug("Get from context " + Lifetime + " " + transient.GetType().Name + " exposed as " +
                             RegisterType.Name + ": " + transient.GetHashCode().ToString("X"));
            } else {
                transient = CreateNewInstance(Lifetime, context);
                context.Container.InjectServices(transient, context);
            }
            return transient;
        }
    }
}