namespace Betauer.DI.Factory;

public interface ILazy : IProxy {
    public bool HasValue();
    public object Get();
}

public interface ILazy<out T> : ILazy where T : class {
    public new T Get();
}