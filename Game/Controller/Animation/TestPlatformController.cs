using Godot;
using Betauer;
using Betauer.Animation;

namespace Veronenger.Game.Controller.Animation {
    public class TestPlatformController : DiNode2D {
        [OnReady("Body1")] private KinematicBody2D body1;
        [OnReady("Body2")] private KinematicBody2D body2;
        [OnReady("Body3")] private KinematicBody2D body3;

        private SingleSequencePlayer _multipleSequencePlayer1;
        private SingleSequencePlayer _multipleSequencePlayer2;
        private MultipleSequencePlayer _multipleSequencePlayer3;

        public override void Ready() {
            _multipleSequencePlayer1 = new SingleSequencePlayer().CreateNewTween(this);
            _multipleSequencePlayer1
                .CreateSequence()
                .AnimateKeys<float>(body1, Property.Scale2DY, Easing.SineInOut)
                .From(1f)
                .Duration(1)
                .KeyframeTo(0.5f, 1.5f)
                .KeyframeTo(1f, 1f)
                .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops()
                .Start();

            BezierCurve curve = BezierCurve.Create(0.37f, 0.0f, 0.63f, 1f);
            // https://css-tricks.com/snippets/sass/easing-map-get-function/
            BezierCurve curveBourbon = BezierCurve.Create(0.445f, 0.050f, 0.550f, 0.950f);

            _multipleSequencePlayer2 = new SingleSequencePlayer().CreateNewTween(this)
                .CreateSequence()
                .AnimateStepsBy(body2, Property.PositionX, curve)
                .Offset(50f, 0.5f)
                .Offset(-50f, 0.5f)
                .EndAnimate()
                .EndSequence()
                .SetInfiniteLoops()
                .Start();

            _multipleSequencePlayer3 = new MultipleSequencePlayer().CreateNewTween(this)
                .CreateSequence()
                .AnimateKeys<float>(body3, Property.Scale2DY)
                .From(1)
                .Duration(2f)
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
        }

        public override void _Process(float delta) {
            // if (seq.G)
        }
        // public override void _PhysicsProcess(float delta) {
        // base._PhysicsProcess(delta);
        // }

        protected override void Dispose(bool disposing) {
            try {
                _multipleSequencePlayer1?.Dispose();
                _multipleSequencePlayer2?.Dispose();
                _multipleSequencePlayer3?.Dispose();
            } finally {
                base.Dispose(disposing);
            }
        }
    }
}