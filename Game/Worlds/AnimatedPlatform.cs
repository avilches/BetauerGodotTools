using Veronenger.Game.Tools;
using Godot;
using static Godot.Mathf;

namespace Veronenger.Game.Worlds {
    public class AnimatedPlatform : KinematicBody2D {
        [Export] public bool IsFallingPlatform = false;
        [Export] public bool Enabled = true;

        private Vector2 _original;
        public Vector2 follow;

        public override void _EnterTree() {
            Configure();
        }

        public override void _PhysicsProcess(float delta) {
            if (Enabled) {
                UpdatePosition();
            }
        }

        public void Configure() {
            GameManager.Instance.PlatformManager.RegisterPlatform(this, IsFallingPlatform, true);

            _original = Position;

            // TODO: pasar a C#
            GDScript MyGDScript = (GDScript) GD.Load("res://Game/Tools/Effects/TweenSequence.gd");
            Object myGDScriptNode = (Object) MyGDScript.New(GetTree());
            Object tweener1 = (Object) myGDScriptNode.Call("append", this, nameof(follow), new Vector2(100, 0), 2);
            tweener1.Call("set_trans", Tween.TransitionType.Cubic);
            Object tweener2 = (Object) myGDScriptNode.Call("append", this, nameof(follow), new Vector2(0, 0), 2);
            tweener2.Call("set_trans", Tween.TransitionType.Cubic);
            myGDScriptNode.Call("set_loops");

            // var seq := TweenSequence.new(get_tree())
            // seq.append(self, "follow", Vector2(100, 0), 2).set_trans(Tween.TRANS_CUBIC)
            // seq.append(self, "follow", Vector2.ZERO, 2).set_trans(Tween.TRANS_CUBIC)
            // seq.set_loops()
        }

        public void UpdatePosition() {
            Position = _original + follow;
        }
    }
}