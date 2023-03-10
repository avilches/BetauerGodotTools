namespace Betauer.DI.Factory;

public interface IFactory<out T> {
    public T Get();
}