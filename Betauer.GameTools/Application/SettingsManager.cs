using System.Collections.Generic;
using Betauer.Application.Screen;
using Godot;

namespace Betauer.Application {
    public class SettingsManager {
        private const bool DontSave = false;
        private SettingsFile _settingsFile;
        private ScreenService _service;

        public IUserSettings SettingsFile => _settingsFile;

        public void Load(SettingsFile settingsFile) {
            _settingsFile = settingsFile;
            _settingsFile.Load();
        }

        public void SaveControls() {
            _settingsFile.Save();
        }

        public void ChangeScreenConfiguration(ScreenConfiguration screenConfiguration, ScreenService.Strategy? strategy) {
            _service.SetScreenConfiguration(screenConfiguration, strategy);
        }

        public void Start(SceneTree tree, ScreenConfiguration initialConfiguration) {
            _service = new ScreenService(tree, initialConfiguration);
            SetPixelPerfect(_settingsFile.PixelPerfect, DontSave);
            SetVSync(_settingsFile.VSync, DontSave);
            if (_settingsFile.Fullscreen) {
                SetFullscreen(true, DontSave);
            } else {
                SetWindowed(_settingsFile.WindowedResolution, DontSave);
                SetBorderless(_settingsFile.Borderless, DontSave);
            }
        }

        public bool IsFullscreen() => _service.IsFullscreen();
        public List<ScaledResolution> GetResolutions() => _service.GetResolutions();

        public void SetPixelPerfect(bool pixelPerfect, bool save = true) {
            var strategy = pixelPerfect
                ? ScreenService.Strategy.PixelPerfectScale
                : ScreenService.Strategy.FitToScreen;
            _service.SetStrategy(strategy);
            if (save) {
                _settingsFile.PixelPerfect = pixelPerfect;
                _settingsFile.Save();
            }
        }

        public void SetBorderless(bool borderless, bool save = true) {
            _service.SetBorderless(borderless);
            if (save) {
                _settingsFile.Borderless = borderless;
                _settingsFile.Save();
            }
        }

        public void SetVSync(bool vsync, bool save = true) {
            OS.VsyncEnabled = vsync;
            if (save) {
                _settingsFile.VSync = vsync;
                _settingsFile.Save();
            }
        }

        public void SetFullscreen(bool fs, bool save = true) {
            if (fs) _service.SetFullscreen();
            else SetWindowed(_settingsFile.WindowedResolution);
            if (save) {
                _settingsFile.Fullscreen = fs;
                _settingsFile.Save();
            }
        }

        public void SetWindowed(Resolution resolution, bool save = true) {
            _service.SetWindowed(resolution);
            if (save) {
                _settingsFile.WindowedResolution = resolution;
                _settingsFile.Save();
            }
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