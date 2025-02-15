using Godot;

namespace Betauer.Application.Lifecycle;

public partial class TextResource(string text) : Resource {
    public string Text { get; } = text;
}