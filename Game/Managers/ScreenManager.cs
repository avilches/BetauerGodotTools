using System.Collections.Generic;
using Betauer;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ScreenManager : Node, IScreenService /* needed to receive OnScreenResized signals */ {
        private IScreenService _service;

        public List<Vector2> WindowedResolutions = new List<Vector2>();
        private int _currentScreen = -1;

        public Vector2 BaseResolution = Resolution.FULLHD_DIV2;
        public override void _Ready() {
            _currentScreen = OS.CurrentScreen;
            ScreenConfiguration configuration = new ScreenConfiguration(
                BaseResolution,
                SceneTree.StretchMode.Viewport,
                SceneTree.StretchAspect.Keep);

            var sceneTree = GetTree();
            _service = new ScreenIntegerResolutionService(sceneTree);
            _service.Configure(configuration);

            if (!DiBootstrap.IsDevelopment) {
                SetFullscreen();
            } else {
                SetBorderless(false);
                SetWindowed( 1);
            }
            sceneTree.Connect(GodotConstants.GODOT_SIGNAL_screen_resized, this, nameof(OnScreenResized));
            CalculateResolutions();

        }

        public void OnChangeSettings() {
            CalculateResolutions();
        }

        private void CalculateResolutions() {
            WindowedResolutions.Clear();
            for (var n = 1; n <= _service.GetMaxScale(); n++) WindowedResolutions.Add(BaseResolution * n);
        }

        public void Configure(ScreenConfiguration configuration) {
            _service.Configure(configuration);
            OnChangeSettings();
        }

        public bool IsFullscreen() => _service.IsFullscreen();

        public void SetFullscreen(bool fs) {
            if (fs) {
                SetFullscreen();
            } else {
                SetWindowed();
            }
        }

        public void SetFullscreen() {
            _service.SetFullscreen();
            OnChangeSettings();
        }

        public int GetScale() => _service.GetScale();
        public int GetMaxScale() => _service.GetMaxScale();

        public void SetBorderless(bool borderless) {
            _service.SetBorderless(borderless);
            OnChangeSettings();
        }

        public void SetWindowed() {
            SetWindowed(GetScale());
        }

        public void SetWindowed(int newScale) {
            _service.SetWindowed(newScale);
            OnChangeSettings();
        }

        public void CenterWindow() => _service.CenterWindow();
        public void OnScreenResized() => _service.OnScreenResized();
    }
}