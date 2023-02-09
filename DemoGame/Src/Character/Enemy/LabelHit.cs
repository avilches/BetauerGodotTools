using Godot;

namespace Veronenger.Character.Enemy;

public class LabelHit : ILabelEffect {
    private Tween? _tweenHit;
    public readonly Label Label;

    public LabelHit(Label label) {
        Label = label;
        label.Visible = false;
        label.Text = "";
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

    public bool Busy => _tweenHit?.IsRunning() ?? false;

    public GodotObject Owner => Label;
}