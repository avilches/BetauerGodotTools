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

        private TweenPlayer tweenPlayer1;
        private TweenPlayer tweenPlayer2;
        private TweenPlayer tweenPlayer3;
        public override void Ready() {

            tweenPlayer1 = new TweenPlayer("").NewTween(this);
            tweenPlayer1
                .CreateSequence()
                .AnimateKeys<float>(body2, Property.ScaleY, Easing.SineInOut)
                .From(1f)
                .Duration(1)
                .KeyframeTo(0.5f, 1.5f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops()
                .Start();

            BezierCurve curve = BezierCurve.Create(0.37f, 0.0f, 0.63f, 1f);
            https://css-tricks.com/snippets/sass/easing-map-get-function/
            BezierCurve curveBourbon = BezierCurve.Create(0.445f, 0.050f, 0.550f, 0.950f);

            tweenPlayer2 = new TweenPlayer("").NewTween(this);
            tweenPlayer2
                .CreateSequence()
                .AnimateStepsBy(body3, Property.PositionX, curve)
                .Offset(50f, 0.5f)
                .Offset(-50f, 0.5f)
                .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops();
                // .Start();


            tweenPlayer3 = new TweenPlayer("").NewTween(this)
                .CreateSequence()
                .AnimateKeys<float>(body3, Property.ScaleY)
                .From(1)
                .Duration(0.5f)
                .KeyframeTo(0.20f, 1)
                .KeyframeTo(0.40f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeTo(0.43f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeTo(0.53f, 1)
                .KeyframeTo(0.70f, 1.05f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f))
                .KeyframeTo(0.80f, 0.95f)
                .KeyframeTo(0.90f, 1.02f)
                .KeyframeTo(1, 1f)
                .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops()
            .Start();
            // .Parallel()


        }

        public override void _Process(float delta) {
            // if (seq.G)
        }
        // public override void _PhysicsProcess(float delta) {
            // base._PhysicsProcess(delta);
        // }
    }

}