namespace Betauer.DI;

public interface IHolder<out T> where T : class {
    public T Get();
    public void Reset();
    public bool HasInstance();
}