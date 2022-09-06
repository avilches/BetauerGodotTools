using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.Animation.Easing;
using Betauer.Animation.Tween;
using Betauer.DI;
using Betauer.OnReady;

namespace Veronenger.Game.Controller.Animation {
    public class TestPlatformController : Node2D {
        [OnReady("Body1")] private KinematicBody2D body1;
        [OnReady("Body2")] private KinematicBody2D body2;
        [OnReady("Body3")] private KinematicBody2D body3;

        public override void _Ready() {
            KeyframeAnimation.Create(body1)
                .SetDuration(1)
                .AnimateKeys(Property.Scale2Dy, Easings.SineInOut)
                .From(1f)
                .KeyframeTo(0.5f, 1.5f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .SetInfiniteLoops()
                .Play();

            BezierCurve curve = BezierCurve.Create(0.37f, 0.0f, 0.63f, 1f);
            // https://css-tricks.com/snippets/sass/easing-map-get-function/
            BezierCurve curveBourbon = BezierCurve.Create(0.445f, 0.050f, 0.550f, 0.950f);

            SequenceAnimation.Create(body2)
                .AnimateStepsBy(Property.PositionX, curve)
                .Offset(50f, 0.5f)
                .Offset(-50f, 0.5f)
                .EndAnimate()
                .SetInfiniteLoops()
                .Play();

            KeyframeAnimation.Create(body3)
                .SetDuration(2f)
                .AnimateKeys(Property.Scale2Dy)
                .From(1)
                .KeyframeTo(0.20f, 1)
                .KeyframeTo(0.40f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeTo(0.43f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeTo(0.53f, 1)
                .KeyframeTo(0.70f, 1.05f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f))
                .KeyframeTo(0.80f, 0.95f)
                .KeyframeTo(0.90f, 1.02f)
                .KeyframeTo(1, 1f)
                .EndAnimate()
                .SetInfiniteLoops()
                .Play();
        }
    }
}