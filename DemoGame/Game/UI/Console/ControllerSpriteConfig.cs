using System.Collections.Generic;
using Godot;

namespace Veronenger.Game.UI.Console;

public abstract class ControllerSpriteConfig : IControllerSpriteConfig {
    private const int AxisOffset = 1000;

    private readonly Dictionary<int, ConsoleButtonView> _mapping = new();

    public abstract Texture2D Texture2D { get; }

    public ConsoleButtonView? Get(JoyButton button) =>
        _mapping.TryGetValue((int)button, out var o) ? o : null;

    public ConsoleButtonView? Get(JoyAxis axis) =>
        _mapping.TryGetValue((int)axis + AxisOffset, out var o) ? o : null;


    protected void Add(JoyButton joyButton, string animation, int frame, int framePressed) =>
        Add((int)joyButton, animation, frame, framePressed);
		
    protected void Add(JoyAxis joyAxis, string animation, int frame, int framePressed) =>
        Add((int)joyAxis + AxisOffset, animation, frame, framePressed);
		
    private void Add(int x, string animation, int frame, int framePressed) =>
        _mapping.Add(x, new ConsoleButtonView(animation, frame, framePressed));
}