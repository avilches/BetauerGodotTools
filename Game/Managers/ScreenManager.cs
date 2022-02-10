using System;
using System.Collections.Generic;
using Betauer.DI;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ScreenManager {
        private readonly UserScreenSettings _settings = new UserScreenSettings();
        private const bool DontSave = false;
        private ScreenService _service;

        [Inject] private Func<SceneTree> GetTree;

        public IUserScreenSettings Settings => _settings;

        public void ChangeScreenConfiguration(ScreenConfiguration screenConfiguration, ScreenService.Strategy? strategy) {
            _service.SetScreenConfiguration(screenConfiguration, strategy);
        }

        public void Load() {
            _settings.Load();
        }

        public void Start(ScreenConfiguration initialConfiguration) {
            _service = new ScreenService(GetTree(), initialConfiguration);
            SetPixelPerfect(_settings.PixelPerfect, DontSave);
            SetVSync(_settings.VSync, DontSave);
            if (_settings.Fullscreen) {
                SetFullscreen(true, DontSave);
            } else {
                SetWindowed(_settings.WindowedResolution, DontSave);
                SetBorderless(_settings.Borderless, DontSave);
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
                _settings.PixelPerfect = pixelPerfect;
                _settings.Save();
            }
        }

        public void SetBorderless(bool borderless, bool save = true) {
            _service.SetBorderless(borderless);
            if (save) {
                _settings.Borderless = borderless;
                _settings.Save();
            }
        }

        public void SetVSync(bool vsync, bool save = true) {
            OS.VsyncEnabled = vsync;
            if (save) {
                _settings.VSync = vsync;
                _settings.Save();
            }
        }

        public void SetFullscreen(bool fs, bool save = true) {
            if (fs) _service.SetFullscreen();
            else SetWindowed(_settings.WindowedResolution);
            if (save) {
                _settings.Fullscreen = fs;
                _settings.Save();
            }
        }

        public void SetWindowed(Resolution resolution, bool save = true) {
            _service.SetWindowed(resolution);
            if (save) {
                _settings.WindowedResolution = resolution;
                _settings.Save();
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