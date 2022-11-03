using System;
using System.Threading.Tasks;
using Betauer.Animation;
using Betauer.Animation.Tween;
using Betauer.Nodes;
using Godot;

namespace DemoAnimation.Managers.Autoload {
    public class Global : Node /* needed because it's an autoload */ {
        public override void _Ready() {
            this.DisableAllNotifications();
        }

        public Task AnimateGrid(Node node) {
            GD.Print(node.GetType());
            var offsetAccumulated = 0;
            var offset = 0.5;

            int n = 0;
            foreach (var child in node.GetChildren()) {
                Templates.FadeIn.Play(child as Node, 0.1f * n);
                n++;
            }
            return null;
        }

        public void Animate(Node node, string animation, float duration) {
            GD.Print(node.GetType().Name + " " + node.Name + ": " + animation + " " + duration + "s");

            try {
                animation = animation.ReplaceN("_", "");
                animation = animation.ReplaceN("bouncing", "bounce");

                // TODo: should a Templates.Get() fail, or it's better to return an empty sequence?
                Templates.Get<float, KeyframeAnimation>(animation, 1000f)!.Play(node, 0, duration);
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            // TweenPlayer tweenPlayer = new TweenPlayer().NewTween(this)
            //     .ImportTemplate(Templates.bounce, node)
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