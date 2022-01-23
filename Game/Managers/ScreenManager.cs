using System;
using System.Collections.Generic;
using Betauer;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ScreenSettings {
        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV3,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(),
            new List<AspectRatio> { AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5 },
            1);

        public bool PixelPerfect = true;
        public bool Fullscreen = true;
        public bool Borderless = false;
        public Resolution WindowedResolution = Configuration.BaseResolution;

        public ScreenSettings Load() {
            return this;
        }

        public void Save() {
            // Console.Write("PixelPerfect = " + PixelPerfect);
            // Console.Write("Fullscreen = " + Fullscreen);
            // Console.Write("Borderless = " + Borderless);
            // Console.Write("WindowedResolution = " + WindowedResolution);
        }
    }

    [Singleton]
    public class ScreenManager : Node /* needed to get the scene tree  GetTree() */ {
        private ScreenService _service;

        [Inject] ScreenSettings Settings;

        public override void _Ready() {
            _service = new ScreenService(GetTree(), ScreenSettings.Configuration,
                ScreenService.Strategy.PixelPerfectScale);

            Settings.Load();
            if (Settings.Fullscreen) {
                SetFullscreen(true, false);
            } else {
                SetPixelPerfect(Settings.PixelPerfect, false);
                SetWindowed(Settings.WindowedResolution, false);
                SetBorderless(Settings.Borderless, false);
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

        protected override void Dispose(bool disposing) {
            _service?.Dispose();
            base.Dispose(disposing);
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