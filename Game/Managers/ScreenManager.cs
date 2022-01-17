using System.Collections.Generic;
using Betauer;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ScreenManager : Node /* needed to get the scene tree on init with GetTree() */ {
        private IScreenService _service;

        private readonly ScreenConfiguration configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV2,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep, 1);

        public Resolution CurrentWindowedResolution;
        public bool CurrentPixelPerfect = false;
        public bool CurrentFullscreen = false;

        public override void _Ready() {
            SetPixelPerfect(true);

            var configuredWindowSize = configuration.BaseResolution; // TODO: load from settings
            CurrentWindowedResolution = configuredWindowSize;

            if (!DiBootstrap.IsDevelopment) {
                SetFullscreen();
            } else {
                SetBorderless(false);
                SetWindowed(configuredWindowSize);
            }
        }

        public void SetPixelPerfect(bool pixelPerfect) {
            _service?.Dispose();
            CurrentPixelPerfect = pixelPerfect;
            if (CurrentPixelPerfect) {
                _service = new IntegerScaleResolutionService(GetTree());
            } else {
                _service = new RegularResolutionService(GetTree());
            }
            _service.Configure(configuration);
        }

        protected override void Dispose(bool disposing) => _service?.Dispose();
        public void SetBorderless(bool borderless) => _service.SetBorderless(borderless);
        public bool IsFullscreen() => _service.IsFullscreen();

        public void SetFullscreen(bool fs) {
            if (fs) SetFullscreen();
            else SetWindowed(CurrentWindowedResolution);
        }

        public void SetFullscreen() => _service.SetFullscreen();

        public void SetWindowed(Resolution resolution) {
            _service.SetWindowed(resolution);
            CurrentWindowedResolution = resolution;
        }

        public List<ScaledResolution> GetResolutions() => _service.GetResolutions();

        /*
        private int _currentScreen = -1;
        private Vector2 _screenSize = Vector2.Zero;

        // Detect current screen change or monitor resolution (screen size) changed
        public override void _PhysicsProcess(float delta) {
            var screenSize = OS.GetScreenSize();
            if (_currentScreen != OS.CurrentScreen || screenSize != _screenSize) {
                _screenSize = screenSize;
                _currentScreen = OS.CurrentScreen;
                // GD.Print("New currentScreen: " + _currentScreen + " " + _screenSize);
                ScreenChanged();
            }
        }

        private void ScreenChanged() {
            var screenSize = OS.GetScreenSize();
            if (!OS.WindowFullscreen) {
                // Make the application window the smallest so it can fit the screen
                var windowSize = OS.WindowSize;
                if (windowSize.x >= screenSize.x || windowSize.y >= screenSize.y) {
                    var scaledResolution = GetResolutions()[0];
                    // GD.Print("OPPPPPS BIGGER WINDOW THAN SCREEN. Changing to " + scaledResolution);
                    SetWindowed(scaledResolution);
                }
            }
        }
        */
    }
}