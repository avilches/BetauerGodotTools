using Godot;

namespace Veronenger.Game.Platform.Character;

public class LabelHit {
    private readonly Label _label;

    public LabelHit(Label label) {
        _label = label;
        _label.Visible = false;
        _label.Text = "";
    }

    public Tween Show(string text) {
        _label.Text = text;
        _label.Visible = true;
        _label.Modulate = Colors.White;
        _label.Position = Vector2.Zero;
        var tweenHit = _label.GetTree().CreateTween();
        tweenHit.Parallel().TweenProperty(_label, "position:y", -40, 0.8f).SetDelay(0.1);
        tweenHit.Parallel().TweenProperty(_label, "modulate:a", 0, 0.8f).SetDelay(0.1);
        return tweenHit;
    }
}