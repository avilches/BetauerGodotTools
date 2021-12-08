using Godot;
using Tools;
using Tools.Animation;
using Tools.Effects.Deprecated;
using TweenPlayer = Tools.Animation.TweenPlayer;

namespace Veronenger.Game.Controller.Animation {
    public class TestPlatformController : DiNode2D {

        [OnReady("Body1")] private KinematicBody2D body1;
        [OnReady("Body2")] private KinematicBody2D body2;
        [OnReady("Body3")] private KinematicBody2D body3;

        private TweenPlayer tweenPlayer;
        public override void Ready() {

            TinyTweenSequence seq = new TinyTweenSequence(true);
            seq.Add(0, 100, 5, ScaleFuncs.SineEaseInOut);
            seq.AddReverseAll();
            seq.AutoUpdate(this, value => body1.Position = new Vector2(value, body1.Position.y));

            BezierCurve curve = BezierCurve.Create(0.37f, 0.0f, 0.63f, 1f);

            https://css-tricks.com/snippets/sass/easing-map-get-function/
            BezierCurve curveBourbon = BezierCurve.Create(0.445f, 0.050f, 0.550f, 0.950f);
            TinyTweenSequence seq2 = new TinyTweenSequence(true);
            seq2.Add(0, 100, 5, curve.GetY);
            seq2.AddReverseAll();
            seq2.AutoUpdate(this, value => body2.Position = new Vector2(value, body2.Position.y));


            tweenPlayer = new TweenPlayer("").NewTween(this);
            tweenPlayer
                .CreateSequence()
                    .AnimateFloat(body3, "position:x", Tween.TransitionType.Sine)
                        .AddOffset(100f, 5)
                        .AddOffset(-100f, 5)
                    .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops()
                .Start();

        }

        public override void _PhysicsProcess(float delta) {
            base._PhysicsProcess(delta);
        }
    }

}