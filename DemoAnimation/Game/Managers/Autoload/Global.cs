using System;
using System.Threading.Tasks;
using Godot;
using Betauer;
using Betauer.Animation;
using Betauer.DI;

namespace Veronenger.Game.Managers.Autoload {
    public class Global : Node /* needed because it's an autoload */ {
        // [Inject] private GameManager GameManager;
        private readonly Launcher _launcher = new Launcher();
        public override void _Ready() {
            this.DisableAllNotifications();
            _launcher.WithParent(this);
        }

        public Task<MultipleSequencePlayer> AnimateGrid(Node node) {
            GD.Print(node.GetType());
            var offsetAccumulated = 0;
            var offset = 0.5;

            int n = 0;
            foreach (var child in node.GetChildren()) {
                _launcher.Play(1, Template.FadeIn, child as Node, 0.1f * n);
                n++;
            }
            return null;
        }

        public void Animate(Node node, string animation, float duration) {
            GD.Print(node.GetType().Name + " " + node.Name + ": " + animation + " " + duration + "s");

            try {
                animation = animation.ReplaceN("_", "");
                animation = animation.ReplaceN("bouncing", "bounce");

                // TODo: should a Template.Get() fail, or it's better to return an empty sequence?
                _launcher.Play(Template.Get(animation, 1000f), node, 0, duration);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            // TweenPlayer tweenPlayer = new TweenPlayer().NewTween(this)
            //     .ImportTemplate(Template.bounce, node)
            //     .Parallel()
            //     .AnimateSteps<Color>(node, "modulate")
            //     .From(Colors.Aqua)
            //     .To(Colors.Brown, 0.5f)
            //     .To(Colors.White, 0.5f)
            //     .EndAnimate()
            //     .SetDuration(duration)
            //     .EndSequence()
            //     .Start();
            //
        }
    }
}