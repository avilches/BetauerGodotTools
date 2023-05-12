namespace Betauer.DI.Factory;

public interface ILazy<out T> : IGet<T> where T : class {
}