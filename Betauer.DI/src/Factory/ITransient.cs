namespace Betauer.DI.Factory;

public interface ITransient<out T> where T : class {
    public T Create();
}