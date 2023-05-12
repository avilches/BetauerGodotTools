namespace Betauer.DI.Factory;

public interface ITransientFactory<out T> : IFactory<T> where T : class {
}