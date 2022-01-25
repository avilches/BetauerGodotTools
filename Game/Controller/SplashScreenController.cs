using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Betauer;
using Betauer.Animation;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : DiControl {
        [Inject] GameManager GameManager;
        [Inject] public ScreenManager ScreenManager;

        [OnReady("ColorRect")] private ColorRect ColorRect;
        [OnReady("ColorRect/CenterContainer")] private CenterContainer CenterContainer;

        [OnReady("ColorRect/CenterContainer/TextureRect")]
        private TextureRect _sprite;

        private Launcher _launcher = new Launcher();

        private Vector2 baseResolutionSize;

        public override void _EnterTree() {
            ScreenManager.Settings.Load();
            if (ScreenManager.Settings.Fullscreen) {
                OS.WindowFullscreen = true;
            }
            baseResolutionSize = new Vector2(OS.WindowSize.x, ScreenManager.Settings.WindowedResolution.Size.y);
            baseResolutionSize = OS.WindowSize;
            GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.KeepHeight,
                baseResolutionSize, 1);
        }

        public override async void Ready() {
            CenterContainer.RectSize = baseResolutionSize;
            ColorRect.RectSize = baseResolutionSize;
            ColorRect.Color = Colors.Aqua.Darkened(0.9f);
            _launcher.CreateNewTween(this)
                .Play(SequenceBuilder
                        .Create(_sprite)
                        .SetInfiniteLoops()
                        .AnimateSteps(null, Property.Modulate)
                        .From(Colors.White)
                        .To(Colors.Red, 0.5f)
                        .EndAnimate()
                    , _sprite);

            await LoadMuchasCosas();
            GameManager.Start(this);
            ScreenManager.LoadSettingsAndConfigure();
        }


        private async Task LoadMuchasCosas() {
            string[] cosas = {
                "res://Scenes/SplashScreen.tscn",
                "res://Worlds/World1.tscn",
                "res://Worlds/Environment/GVaniaBridgeTileMap.tscn",
                "res://Worlds/Environment/gvania-bridge-tileset.png",
                "res://Worlds/Environment/GVania/street-lamp.png",
                "res://Worlds/Environment/GVania/dragon-bones-head.png",
                "res://Worlds/Environment/GVania/dragon-bones-fang.png",
                "res://Worlds/Environment/GVania/block.png",
                "res://Worlds/Environment/GVania/dragon-bones-ribs.png",
                "res://Worlds/Environment/GVania/statue.png",
                "res://Worlds/Environment/GVania-town-tileset.png",
                "res://Worlds/Environment/GVania-church-tileset.png",
                "res://Worlds/Environment/GVania/big-block.png",
                "res://Worlds/Environment/GVania-patreon-horror.png",
                "res://Scenes/WorldComplete.tscn",
                "res://icon.png",
                "res://Worlds/Environment/gvania-bridge-background.png",
                "res://Worlds/Environment/gvania-bridge-middleground.png",
                "res://Worlds/OnEnterStart.gd",
                "res://Worlds/OnEnterStop.gd",
                "res://Worlds/World2.tscn",
                "res://Scenes/SlopeStairs.tscn",
                "res://Worlds/Environment/anokalisa/village.png",
                "res://Worlds/Environment/anokalisa/gallery.png",
                "res://Worlds/Environment/anokalisa/LegacyVaniaMock.tscn",
                "res://Worlds/Environment/anokalisa/LegacyVania.tscn",
                "res://Scenes/Zombie.tscn",
                "res://Scenes/Enemy.tscn",
                "res://Characters/Zombie/zombie-sheet.png",
                "res://Scenes/MainMenu.tscn",
                "res://Assets/Lato-Bold-m.tres",
                "res://Assets/Lato-Bold.ttf",
                "res://Assets/Lato-Bold-XL.tres",
                "res://Assets/SimpleBox/SimpleBox.tres",
                "res://Assets/SimpleBox/Montserrat-Bold.ttf",
                "res://Assets/SimpleBox/icons.svg",
                "res://Scenes/Player.tscn",
                "res://Characters/Player-heroine/run.png",
                "res://Characters/Player-heroine/idle.png",
                "res://Characters/Player-heroine/jump.png",
                "res://Game/Controller/Stage/StageCameraController.cs"
            };
            foreach (var cosa in cosas) {
                await LoadAsync2(cosa);
            }
        }

        public async Task LoadAsync2(string path) {
            var polls = new List<long>();
            var idle = new List<long>();
            Stopwatch total = Stopwatch.StartNew();
            using (var loader = ResourceLoader.LoadInteractive(path)) {
                Resource resource = null;
                while (resource == null) {
                    Stopwatch x = Stopwatch.StartNew();
                    var err = loader.Poll();
                    polls.Add(x.ElapsedMilliseconds);
                    if (err == Error.FileEof) {
                        resource = loader.GetResource();
                        GD.Print(resource.GetType().Name + " " + resource.ResourceName + " " + resource.ResourcePath +
                                 " ");
                        // var s = resource as PackedScene;
                        // GetTree().GetRoot().AddChild(s.Instance());
                        break;
                    }

                    Stopwatch x2 = Stopwatch.StartNew();
                    await GetTree().AwaitPhysicsFrame();
                    idle.Add(x2.ElapsedMilliseconds);
                }

                GD.Print("* " + path + ". Total: " + total.ElapsedMilliseconds + "ms polls:" +
                         string.Join(", ", polls) + " idle:" + string.Join(", ", idle));
            }
        }
    }
}