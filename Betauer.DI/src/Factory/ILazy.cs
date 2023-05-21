namespace Betauer.DI.Factory;

public interface ILazy<out T> where T : class {
    public T Get();
}