using System.Collections.Generic;
using Betauer.Application.Settings;
using Betauer.DI;
using Godot;
using Container = Betauer.DI.Container;

namespace Betauer.Application.Screen {
    public class ScreenSettingsManager {
        [Inject] protected Container Container;
        [Inject] protected SceneTree SceneTree;

        private const bool DontSave = false;
        private ScreenService _service;
        
        private ISetting<bool> _pixelPerfect;
        private ISetting<bool> _fullscreen;
        private ISetting<bool> _VSync;
        private ISetting<bool> _borderless;
        private ISetting<Resolution> _windowedResolution;

        public bool PixelPerfect => _pixelPerfect.Value;
        public bool Fullscreen => _fullscreen.Value;
        public bool VSync => _VSync.Value;
        public bool Borderless => _borderless.Value;
        public Resolution WindowedResolution => _windowedResolution.Value;

        public readonly ScreenConfiguration InitialScreenConfiguration;

        public ScreenSettingsManager(ScreenConfiguration initialScreenConfiguration) {
            InitialScreenConfiguration = initialScreenConfiguration;
        }

        [PostCreate]
        private void ConfigureSettings() {
            _service = new ScreenService(SceneTree, InitialScreenConfiguration);

            _pixelPerfect = Container.Resolve<Setting<bool>>("Settings.Screen.PixelPerfect");
            _fullscreen = Container.Resolve<Setting<bool>>("Settings.Screen.Fullscreen");
            _VSync = Container.Resolve<Setting<bool>>("Settings.Screen.VSync");
            _borderless = Container.Resolve<Setting<bool>>("Settings.Screen.Borderless");
            _windowedResolution = Container.Resolve<Setting<Resolution>>("Settings.Screen.WindowedResolution");
        }
        
        public void Setup() {
            SetPixelPerfect(PixelPerfect, DontSave);
            SetVSync(VSync, DontSave);
            if (Fullscreen) {
                SetFullscreen(true, DontSave);
            } else {
                SetWindowed(WindowedResolution, DontSave);
                SetBorderless(Borderless, DontSave);
            }
        }

        public void ChangeScreenConfiguration(ScreenConfiguration screenConfiguration,
            ScreenService.Strategy? strategy) {
            _service.SetScreenConfiguration(screenConfiguration, strategy);
        }

        public bool IsFullscreen() => _service.IsFullscreen();
        public List<ScaledResolution> GetResolutions() => _service.GetResolutions();

        public void SetPixelPerfect(bool pixelPerfect, bool save = true) {
            var strategy = pixelPerfect
                ? ScreenService.Strategy.PixelPerfectScale
                : ScreenService.Strategy.FitToScreen;
            _service.SetStrategy(strategy);
            if (save) _pixelPerfect.Value = pixelPerfect;
        }

        public void SetBorderless(bool borderless, bool save = true) {
            _service.SetBorderless(borderless);
            if (save) _borderless.Value = borderless;
        }

        public void SetVSync(bool vsync, bool save = true) {
            OS.VsyncEnabled = vsync;
            if (save) _VSync.Value = vsync;
        }

        public void SetFullscreen(bool fs, bool save = true) {
            if (fs) _service.SetFullscreen();
            else SetWindowed(WindowedResolution, false); // Never save resolution because it has not changed
            if (save) _fullscreen.Value = fs;
        }

        public void SetWindowed(Resolution resolution, bool save = true) {
            _service.SetWindowed(resolution);
            if (save) _windowedResolution.Value = resolution;
        }

        public void CenterWindow() {
            _service.CenterWindow();
        }

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