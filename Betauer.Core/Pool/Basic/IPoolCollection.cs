namespace Betauer.Core.Pool.Basic;

public interface IPoolCollection<T> {
    T Get();
    void Add(T ele);
    public int Count { get; }
}