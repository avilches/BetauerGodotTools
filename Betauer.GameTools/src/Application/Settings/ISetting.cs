namespace Betauer.Application.Settings;

public interface ISetting<T> {
    public T Value { get; set; }
}