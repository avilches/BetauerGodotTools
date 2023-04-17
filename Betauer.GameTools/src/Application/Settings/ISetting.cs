namespace Betauer.Application.Settings;

public interface ISetting {
}

public interface ISetting<T> : ISetting {
    public T Value { get; set; }
}