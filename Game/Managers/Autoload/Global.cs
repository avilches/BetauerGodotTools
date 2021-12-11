using Godot;
using Tools;
using Tools.Animation;

namespace Veronenger.Game.Managers.Autoload {
    public class Global : Node /* needed because it's an autoload */ {

        [Inject] private GameManager GameManager;
        [Inject] private CharacterManager CharacterManager;

        public override void _Ready() {
            DiBootstrap.DefaultRepository.AutoWire(this);
            GodotTools.DisableAllNotifications(this);
        }

        public bool IsPlayer(KinematicBody2D player) {
            return CharacterManager.IsPlayer(player);
        }

        public void Animate(Node2D node, string animation, float duration) {
            GD.Print(node.GetType().Name+" "+node.Name+": "+animation+" "+duration+"s");

            TweenPlayer tweenPlayer = new TweenPlayer("").NewTween(this)
                .CreateSequence()
                .KeyframeFloat(node, "scale:y")
                .From(1)
                .Duration(duration)
                .KeyframeTo(0.20f, 1)
                .KeyframeTo(0.40f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeTo(0.43f, 1.1f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeTo(0.53f, 1)
                .KeyframeTo(0.70f, 1.05f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f))
                .KeyframeTo(0.80f, 0.95f)
                .KeyframeTo(0.90f, 1.02f)
                .KeyframeTo(1, 1f)
                .EndAnimate()
                .Parallel().KeyframeFloat(node, "position:y")
                .Duration(duration)
                .From(0f)
                .KeyframeOffset(0.20f, 0f)
                .KeyframeOffset(0.40f, -30f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeOffset(0.43f, 0f, BezierCurve.Create(0.7555f, 0.5f, 0.8555f, 0.06f))
                .KeyframeOffset(0.53f, +30f)
                .KeyframeOffset(0.70f, -15f, BezierCurve.Create(0.755f, 0.05f, 0.855f, 0.06f))
                .KeyframeOffset(0.80f, +15f)
                .KeyframeOffset(0.90f, -4f )
                .KeyframeOffset(1f   , +4f )
                .EndAnimate()
                .EndSequence()
                .Start();
            // .Parallel()

        }
    }
}