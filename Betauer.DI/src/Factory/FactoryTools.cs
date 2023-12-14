using System;
using Betauer.Core;

namespace Betauer.DI.Factory;

public class FactoryTools {
    /// <summary>
    /// Extracts the Create() method from the IFactory<T> object received as parameter.
    /// It's useful because SingletonProvider and TransientProvider don't receive a IFactory<T> as parameter, instead
    /// they need a Func<T> or Func<object>
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Func<T> From<T>(IFactory<T> factory) where T : class {
        return factory.Create;
    }
    
    /// <summary>
    /// Extracts the Create() method from the IFactory<T> object received as parameter.
    /// It's useful because SingletonProvider and TransientProvider don't receive a IFactory<T> as parameter, instead
    /// they need a Func<T> or Func<object>
    /// </summary>
    ///
    /// object factory = new NodeFactory(); // implements IFactory<Node>
    /// var create = FactoryWrapper.Create(factory);
    /// It is equivalent to:
    ///
    /// Func<Node> create = () => {
    ///     IFactory<Node> typedFactory = (IFactory<Node>)factory;
    ///     Node node = typedFactory.Create();
    ///     return (object)
    /// }
    /// </summary>
    /// <param name="factory">A IFactory<T> instance</param>
    /// <returns>A Func<object> that returns T as result of Create()</returns>
    public static Func<object> From(object factory) {
        var type = factory.GetType().FindGenericsFromInterfaceDefinition(typeof(IFactory<>))[0];
        var factoryType = typeof(FactoryWrapper.Typed<>).MakeGenericType(type);
        var factoryWrapper = (FactoryWrapper)Activator.CreateInstance(factoryType, factory)!;
        return factoryWrapper.Create;
    }

    internal abstract class FactoryWrapper {
        public abstract object Create();
        public class Typed<T> : FactoryWrapper, IFactory<T> where T : class {
            private readonly IFactory<T> _factoryProvider;
            public Typed(IFactory<T> factoryProvider) {
                _factoryProvider = factoryProvider;
            }
            public override T Create() {
                return _factoryProvider.Create();
            }
        }
    }
}
