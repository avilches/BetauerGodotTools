namespace Betauer.DI;

public interface IFactory<out T> {
    public T Get();
}