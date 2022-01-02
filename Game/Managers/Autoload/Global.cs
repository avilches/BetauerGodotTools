using System;
using System.Threading.Tasks;
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

        public Task<MultipleSequencePlayer> AnimateGrid(Node node) {
            GD.Print(node.GetType());
            var tween = new Tween();
            AddChild(tween);
            var offsetAccumulated = 0;
            var offset = 0.5;
            SequencePlayer player = new SequencePlayer().CreateNewTween(this);

            int n = 0;
            foreach (var child in node.GetChildren()) {
                new SingleSequencePlayer()
                    .CreateNewTween(this)
                    .CreateSequence()
                    .Pause(0.2f * n)
                    .EndSequence()
                    .ImportTemplate(Template.FadeInUp, child as Node, 1f)
                    .SetLoops(3)
                    .EndSequence()
                    .CreateSequence()
                    .Pause(1f)
                    .ImportTemplate(Template.LightSpeedInLeft, child as Node, 1f)
                    .SetLoops(3)
                    .SetDuration(1f)
                    .EndSequence()
                    .Start();
                n++;
            }
            player.Start();
            return null;
        }

        public void Animate(Node node, string animation, float duration) {
            GD.Print(node.GetType().Name + " " + node.Name + ": " + animation + " " + duration + "s");

            try {
                animation = animation.ReplaceN("_", "");
                animation = animation.ReplaceN("bouncing", "bounce");

                // TODo: should a Template.Get() fail, or it's better to return an empty sequence?
                SingleSequencePlayer.Create(node, Template.Get(animation, 1000f), duration).Start();
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