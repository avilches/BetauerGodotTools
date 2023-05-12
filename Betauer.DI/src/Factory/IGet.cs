namespace Betauer.DI.Factory;

public interface IGet<out T> where T : class {
    public T Get();
}