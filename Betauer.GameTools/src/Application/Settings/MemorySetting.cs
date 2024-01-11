using Godot;

namespace Betauer.Application.Settings;

public class MemorySetting<[MustBeVariant] T> : ISetting<T> {
    public T Value { get; set; }

    public MemorySetting(T value) {
        Value = value;
    }
}