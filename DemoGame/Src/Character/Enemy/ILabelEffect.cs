using Godot;

namespace Veronenger.Character.Enemy;

public interface ILabelEffect {
    public void Show(string text);
    public bool Busy { get; }
    public Object Owner { get; }
}