using Betauer.DI.Factory;

namespace Betauer.DI.Holder;

public interface IMutableHolder<out T> : ILazy<T> where T : class {
    public void Clear();
    public bool HasValue();
}