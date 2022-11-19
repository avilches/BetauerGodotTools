using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Signal;
using Godot;

namespace Betauer.UI;

public abstract partial class CanvasFaderLayer : CanvasLayer {

    public readonly ColorRect ColorRectBackground = new() {
        Modulate = Colors.White
    };
    public readonly ColorRect ColorRectForeground = new() {
        Modulate = Colors.White
    };

    private Tween? _tween;

    private bool _disabled = false;
    private float _opacity = 0.4f;
    private float _time = 0.3f;
    private bool _busy = false;

    public void Disable() {
        _disabled = true;
    }
    
    private void CheckColorRectPosition() {
        if (ColorRectBackground.GetParent() == null) {
            ColorRectBackground.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            ColorRectForeground.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            ColorRectBackground.Color = Colors.Black;
            ColorRectForeground.Color = Colors.Black;
            ColorRectBackground.Modulate = Colors.Transparent;
            ColorRectForeground.Modulate = Colors.Transparent;
            AddChild(ColorRectBackground);
            AddChild(ColorRectForeground);
        }
        MoveChild(ColorRectBackground, 0);
        MoveChild(ColorRectForeground, -1);
    }

    public void SetFadeToColor(Color black) {
        CheckColorRectPosition();
        ColorRectBackground.Color = black;
        ColorRectForeground.Color = black;
    }

    public async Task FadeBackgroundOut(float opacity = 0.4f, float time = 0.3f) {
        if (_busy) return;
        _busy = true;
        try {
            CheckColorRectPosition();
            ResetFade();
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
    }
}