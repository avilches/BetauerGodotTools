using System.Diagnostics;
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
        private TweenPlayer seq2;
        public override void Ready() {

            tweenPlayer = new TweenPlayer("").NewTween(this);
            tweenPlayer
                .CreateSequence()
                    .AnimateFloat(body2, "position:x", Easing.SineInOut)
                        .Offset(50f, 0.5f)
                        .Offset(-50f, 0.5f)
                    .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops()
                .Start();

            BezierCurve curve = BezierCurve.Create(0.37f, 0.0f, 0.63f, 1f);
            https://css-tricks.com/snippets/sass/easing-map-get-function/
            BezierCurve curveBourbon = BezierCurve.Create(0.445f, 0.050f, 0.550f, 0.950f);

            tweenPlayer = new TweenPlayer("").NewTween(this);
            tweenPlayer
                .CreateSequence()
                    .AnimateFloat(body3, "position:x", curve)
                        .Offset(50f, 0.5f)
                        .Offset(-50f, 0.5f)
                    .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops()
                .Start();

        }

        public override void _Process(float delta) {
            // if (seq.G)
        }
        // public override void _PhysicsProcess(float delta) {
            // base._PhysicsProcess(delta);
        // }
    }

}