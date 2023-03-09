namespace Betauer.Core.Pool.Basic;

public interface IBasicPool<T> {
    public T Create();
    public T Get();
    public void Return(T element);
    public void OnGet(T element);
    public void OnReturn(T element);
}