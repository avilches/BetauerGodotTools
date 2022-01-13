using Betauer;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ScreenManager : Node, IScreenService /* needed to receive OnScreenResized signals */ {
        private IScreenService _service;

        public override void _Ready() {
            ScreenConfiguration configuration = new ScreenConfiguration(
                Resolution.FULLHD_DIV2,
                SceneTree.StretchMode.Viewport,
                SceneTree.StretchAspect.Keep);

            var rootViewport = GetNode<Viewport>("/root");
            var sceneTree = GetTree();
            _service = new ScreenIntegerResolutionService(sceneTree, rootViewport);
            _service.Configure(configuration);
            _service.Set(false, 3, false);
            sceneTree.Connect(GodotConstants.GODOT_SIGNAL_screen_resized, this, nameof(OnScreenResized));
        }

        public void Configure(ScreenConfiguration configuration) => _service.Configure(configuration);
        public void OnScreenResized() => _service.OnScreenResized();
        public bool IsFullscreen() => _service.IsFullscreen();
        public void SwapFullscreen() => _service.SwapFullscreen();
        public void SetFullscreen(bool fs) => _service.SetFullscreen(fs);
        public void SetScale(float scale) => _service.SetScale(scale);
        public void SetBorderless(bool borderless) => _service.SetBorderless(borderless);
        public void Set(bool fs, int scale, bool borderless) => _service.Set(fs, scale, borderless);
        public void CenterWindow() => _service.CenterWindow();
    }
}