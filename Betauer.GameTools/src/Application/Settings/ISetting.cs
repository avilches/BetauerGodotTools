using Godot;

namespace Betauer.Application.Settings;

public interface ISetting {
}

public interface ISetting<[MustBeVariant] T> : ISetting {
    public T Value { get; set; }
}