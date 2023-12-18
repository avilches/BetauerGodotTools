namespace Betauer.DI.Factory;

public interface ITransient : IProxy {
    public object Create();
}

public interface ITransient<out T> : ITransient where T : class {
    public new T Create();
}