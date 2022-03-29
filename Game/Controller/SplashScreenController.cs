using Betauer;
using Betauer.Animation;
using Betauer.DI;
using Godot;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller {
    public class SplashScreenController : DiControl {
        [Inject] private GameManager _gameManager;
        [Inject] private ScreenManager _screenManager;
        [Inject] private ResourceManager _resourceManager;

        [OnReady("ColorRect")] private ColorRect ColorRect;
        [OnReady("ColorRect/CenterContainer")] private CenterContainer CenterContainer;

        [OnReady("ColorRect/CenterContainer/TextureRect")]
        private TextureRect _sprite;

        private readonly Launcher _launcher = new Launcher();

        private Vector2 _baseResolutionSize;

        public override void _EnterTree() {
            _screenManager.Load();
            if (_screenManager.Settings.Fullscreen) {
                OS.WindowFullscreen = true;
            }
            _baseResolutionSize = new Vector2(OS.WindowSize.x, _screenManager.Settings.WindowedResolution.Size.y);
            _baseResolutionSize = OS.WindowSize;
            GetTree().SetScreenStretch(SceneTree.StretchMode.Mode2d, SceneTree.StretchAspect.KeepHeight,
                _baseResolutionSize, 1);
        }

        public override async void Ready() {
            CenterContainer.RectSize = _baseResolutionSize;
            ColorRect.RectSize = _baseResolutionSize;
            ColorRect.Color = Colors.Aqua.Darkened(0.9f);
            _launcher.WithParent(this)
                .Play(SequenceBuilder
                        .Create(_sprite)
                        .SetInfiniteLoops()
                        .AnimateSteps(null, Property.Modulate)
                        .From(Colors.White)
                        .To(Colors.Red, 0.2f)
                        .EndAnimate()
                    , _sprite);

            await _resourceManager.Load(context => {
                // GD.Print(context.TotalLoadedPercent.ToString("P") + " = " + context.TotalLoadedSize + " / " +
                // context.TotalSize + " resource " + context.ResourceLoadedPercent.ToString("P") + " = " +
                // context.ResourceLoadedSize + " / " + context.ResourceSize + " " + context.ResourcePath);
            }, async () => {
                await GetTree().AwaitIdleFrame();
            });

            _screenManager.Start(ApplicationConfig.Configuration);
            await GetTree().AwaitIdleFrame();
            _gameManager.OnFinishLoad(this);
        }
    }
}