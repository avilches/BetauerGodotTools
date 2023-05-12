namespace Betauer.DI.Factory;

public interface IFactory<out T> where T : class {
    public T Get();
}