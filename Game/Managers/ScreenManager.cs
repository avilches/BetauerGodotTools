using System;
using System.Collections.Generic;
using Betauer;
using Betauer.Screen;
using Godot;

namespace Veronenger.Game.Managers {
    [Singleton]
    public class ScreenSettings {
        private const string Filename = "video.cfg";

        private const string VideoSection = "Video";
        private const string PixelPerfectProperty = "PixelPerfect";
        private const string FullscreenProperty = "Fullscreen";
        private const string VSyncProperty = "VSync";
        private const string BorderlessProperty = "Borderless";
        private const string WindowedResolutionProperty = "WindowedResolution";

        public static readonly ScreenConfiguration Configuration = new ScreenConfiguration(
            Resolutions.FULLHD_DIV3,
            SceneTree.StretchMode.Viewport,
            SceneTree.StretchAspect.Keep,
            Resolutions.All(),
            new List<AspectRatio> { AspectRatios.Ratio16_9, AspectRatios.Ratio21_9, AspectRatios.Ratio12_5 },
            1);

        public bool PixelPerfect = true;
        public bool Fullscreen = true;
        public bool VSync = true;
        public bool Borderless = false;
        public Resolution WindowedResolution = Configuration.BaseResolution;

        private readonly ConfigFile _cf = new ConfigFile();
        private readonly string _resourceName;

        public ScreenSettings() {
            _resourceName = System.IO.Path.Combine(OS.GetUserDataDir(), System.IO.Path.GetFileName(Filename));
        }

        public ScreenSettings Load() {
            var error = _cf.Load(_resourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(ScreenSettings)).Error($"Load \"{_resourceName}\" error: {error}");
                return this;
            }
            PixelPerfect = (bool)_cf.GetValue(VideoSection, PixelPerfectProperty, true);
            Fullscreen = (bool)_cf.GetValue(VideoSection, FullscreenProperty, true);
            VSync = (bool)_cf.GetValue(VideoSection, VSyncProperty, true);
            Borderless = (bool)_cf.GetValue(VideoSection, BorderlessProperty, true);
            var sn = (Vector2)_cf.GetValue(VideoSection, WindowedResolutionProperty, Configuration.BaseResolution.Size);
            WindowedResolution = new Resolution(sn);
            return this;
        }

        public ScreenSettings Save() {
            _cf.SetValue(VideoSection, PixelPerfectProperty, PixelPerfect);
            _cf.SetValue(VideoSection, FullscreenProperty, Fullscreen);
            _cf.SetValue(VideoSection, VSyncProperty, VSync);
            _cf.SetValue(VideoSection, BorderlessProperty, Borderless);
            _cf.SetValue(VideoSection, WindowedResolutionProperty, WindowedResolution.Size);
            var error = _cf.Save(_resourceName);
            if (error != Error.Ok) {
                LoggerFactory.GetLogger(typeof(ScreenSettings)).Error($"Save \"{_resourceName}\" error: {error}");
            }
            return this;
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
            const bool dontSave = false;
            if (Settings.Fullscreen) {
                SetFullscreen(true, dontSave);
            } else {
                SetPixelPerfect(Settings.PixelPerfect, dontSave);
                SetWindowed(Settings.WindowedResolution, dontSave);
                SetBorderless(Settings.Borderless, dontSave);
                SetVSync(Settings.VSync, dontSave);
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