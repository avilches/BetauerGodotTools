namespace Betauer.DI.Holder;

public interface IMutableHolder<out T> : IHolder<T> where T : class {
    public void Clear();
    public bool HasValue();
}