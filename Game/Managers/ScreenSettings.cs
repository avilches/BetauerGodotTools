using System.Collections.Generic;
using Betauer;
using Betauer.DI;
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
}