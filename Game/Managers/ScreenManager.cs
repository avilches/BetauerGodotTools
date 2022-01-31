using System;
using System.Collections.Generic;
using Betauer;
using Betauer.DI;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    [Service]
    public class ScreenManager : DisposableObject {
        private const bool DontSave = false;
        private ScreenService _service;

        [Inject] private Func<SceneTree> GetTree;

        [Inject] public ScreenSettings Settings;

        public void LoadSettingsAndConfigure() {
            _service?.Dispose();
            _service = new ScreenService(GetTree(), ScreenSettings.Configuration,
                ScreenService.Strategy.PixelPerfectScale);

            Settings.Load();
            if (Settings.Fullscreen) {
                SetFullscreen(true, DontSave);
            } else {
                SetPixelPerfect(Settings.PixelPerfect, DontSave);
                SetWindowed(Settings.WindowedResolution, DontSave);
                SetBorderless(Settings.Borderless, DontSave);
                SetVSync(Settings.VSync, DontSave);
            }
        }

        public bool IsFullscreen() => _service.IsFullscreen();
        public List<ScaledResolution> GetResolutions() => _service.GetResolutions();

        public void SetPixelPerfect(bool pixelPerfect, bool save = true) {
            _service.SetStrategy(pixelPerfect
                ? ScreenService.Strategy.PixelPerfectScale
                : ScreenService.Strategy.FitToScreen);
            if (save) {
                Settings.PixelPerfect = pixelPerfect;
                Settings.Save();
            }
        }

        public void SetBorderless(bool borderless, bool save = true) {
            _service.SetBorderless(borderless);
            if (save) {
                Settings.Borderless = borderless;
                Settings.Save();
            }
        }

        public void SetVSync(bool vsync, bool save = true) {
            OS.VsyncEnabled = vsync;
            if (save) {
                Settings.VSync = vsync;
                Settings.Save();
            }
        }

        public void SetFullscreen(bool fs, bool save = true) {
            if (fs) _service.SetFullscreen();
            else SetWindowed(Settings.WindowedResolution);
            if (save) {
                Settings.Fullscreen = fs;
                Settings.Save();
            }
        }

        public void SetWindowed(Resolution resolution, bool save = true) {
            _service.SetWindowed(resolution);
            if (save) {
                Settings.WindowedResolution = resolution;
                Settings.Save();
            }
        }

        protected override void OnDispose(bool disposing) {
            // TODO: better handling
            _service?.Dispose();
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