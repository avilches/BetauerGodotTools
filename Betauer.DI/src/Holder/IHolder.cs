namespace Betauer.DI.Holder;

public interface IHolder<out T> where T : class {
    public T Get();
}