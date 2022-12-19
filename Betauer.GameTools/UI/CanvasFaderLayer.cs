using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.UI;

public abstract partial class CanvasFaderLayer : CanvasLayer {

    public readonly ColorRect ColorRectBackground = new() {
        Name = "BackgroundFader",
        Color = Colors.Black,
    };
    
    public readonly ColorRect ColorRectForeground = new() {
        Name = "ForegroundFader",
        Color = Colors.Black,
    };

    private Tween? _tween;

    private bool _disabled = false;
    private float _opacity = 0.4f;
    private float _time = 0.3f;
    private bool _busy = false;

    public void Disable() {
        _disabled = true;
    }

    public void SetFadeToColor(Color black) {
        ColorRectBackground.Color = black;
        ColorRectForeground.Color = black;
    }

    public void BlockBackgroundInput() {
        if (_busy) return;
        ResetFade();
        ColorRectBackground.Visible = true;

    }

    public async Task FadeBackgroundOut(float opacity = 0.4f, float time = 0.3f) {
        if (_busy) return;
        _busy = true;
        try {
            CheckColorRectPosition();
            ResetFade();
            _busy = true;
            ColorRectBackground.Visible = true;
            _tween = KeyframeAnimation.Create()
                .SetDuration(time)
                .AnimateKeys(Properties.Opacity)
                .From(0).KeyframeTo(1f, opacity)
                .EndAnimate()
                .Play(ColorRectBackground);
            await _tween.AwaitFinished();
        } finally {
            _busy = false;
        }
    }

    public async Task FadeOut(float opacity, float time = 0.3f) {
        if (_busy) return;
        _busy = true;
        try {
            CheckColorRectPosition();
            ResetFade();
            _busy = true;
            ColorRectForeground.Visible = true;
            _tween = KeyframeAnimation.Create()
                .SetDuration(time)
                .AnimateKeys(Properties.Opacity)
                .From(0).KeyframeTo(1f, opacity)
                .EndAnimate()
                .Play(ColorRectForeground);
            await _tween.AwaitFinished();
        } finally {
            _busy = false;
        }
    }

    public Task FadeToBlack(float time = 0.3f) {
        return FadeOut(1, time);
    }

    public void ResetFade() {
        _tween?.Kill();
        ColorRectBackground.Modulate = Colors.Transparent;
        ColorRectForeground.Modulate = Colors.Transparent;
        ColorRectBackground.Visible = false;
        ColorRectForeground.Visible = false;
        _busy = false;
    }
    
    public override void _Notification(long what) {
        if (what == NotificationPredelete) {
            if (IsInstanceValid(ColorRectBackground) && ColorRectBackground.GetParent() == null) ColorRectBackground.Free();
            if (IsInstanceValid(ColorRectForeground) && ColorRectForeground.GetParent() == null) ColorRectForeground.Free();
        }
    }
    
    private void CheckColorRectPosition() {
        if (ColorRectBackground.GetParent() == null) {
            ColorRectBackground.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            ColorRectForeground.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            AddChild(ColorRectBackground); 
            AddChild(ColorRectForeground);
        }
        MoveChild(ColorRectBackground, 0);
        MoveChild(ColorRectForeground, -1);
    }


}