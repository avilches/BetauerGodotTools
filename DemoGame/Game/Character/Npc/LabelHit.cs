using Betauer.Core;
using Godot;

namespace Veronenger.Game.Character.Npc;

public class LabelHit : ILabelEffect {
    private Tween? _tweenHit;
    public readonly Label Label;

    public LabelHit(Label label) {
        Label = label;
        Label.Visible = false;
        Label.Text = "";
    }

    public void Show(string text) {
        Label.Text = text;
        Label.Visible = true;
        Label.Modulate = Colors.White;
        Label.Position = Vector2.Zero;
        _tweenHit?.Kill();
        _tweenHit = Label.GetTree().CreateTween();
        _tweenHit.Parallel().TweenProperty(Label, "position:y", -40, 0.8f).SetDelay(0.1);
        _tweenHit.Parallel().TweenProperty(Label, "modulate:a", 0, 0.8f).SetDelay(0.1);
    }

    public bool IsBusy() => _tweenHit?.IsRunning() ?? false;

    public bool IsInvalid() => !Label.IsInstanceValid();
}