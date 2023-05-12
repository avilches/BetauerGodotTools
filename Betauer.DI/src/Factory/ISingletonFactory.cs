namespace Betauer.DI.Factory;

public interface ISingletonFactory<out T> : IFactory<T> where T : class {
}