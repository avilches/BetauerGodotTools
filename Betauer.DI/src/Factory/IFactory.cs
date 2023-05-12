namespace Betauer.DI.Factory;

public interface IFactory<out T> : IGet<T> where T : class {
}