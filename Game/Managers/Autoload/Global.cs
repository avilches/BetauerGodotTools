using System;
using Godot;
using Betauer;
using Betauer.Animation;

namespace Veronenger.Game.Managers.Autoload {
    public class Global : Node /* needed because it's an autoload */ {

        // [Inject] private GameManager GameManager;
        [Inject] private CharacterManager CharacterManager;

        public override void _Ready() {
            DiBootstrap.DefaultRepository.AutoWire(this);
            this.DisableAllNotifications();
        }

        public bool IsPlayer(KinematicBody2D player) {
            return CharacterManager.IsPlayer(player);
        }

        public void Animate(Node node, string animation, float duration) {

            GD.Print(node.GetType().Name+" "+node.Name+": "+animation+" "+duration+"s");

            try {
                animation = animation.ReplaceN("_", "");
                animation = animation.ReplaceN("bouncing", "bounce");

                // TODo: should a Template.Get() fail, or it's better to return an empty sequence?
                TweenPlayer.With(node, Template.Get(animation, 1000f), duration).Start();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            // TweenPlayer tweenPlayer = new TweenPlayer("").NewTween(this)
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